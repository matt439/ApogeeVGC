# Contributing to ApogeeVGC

Thank you for contributing to ApogeeVGC! This guide will help you understand the project architecture and coding conventions.

## Architecture Overview

### Battle Simulation Flow

1. **Initialization** (`Driver.cs`): Sets up battle, teams, and initial state
2. **Turn Loop** (`BattleAsync`): Processes player choices and executes actions
3. **Action Resolution**: Moves, switches, abilities trigger in priority order
4. **State Updates**: Pokemon, Side, and Field states are updated
5. **Event Callbacks**: Abilities and effects respond to battle events

### Project Organization

#### Partial Class Pattern
Large classes are split into multiple files for maintainability:

- **`Pokemon.Core.cs`**: Core properties, constructor, basic methods
- **`Pokemon.Transformation.cs`**: Type changes, form changes, stat modifications
- **`Pokemon.Helpers.cs`**: Utility methods and calculations
- **`Side.Core.cs`**: Side initialization and core properties
- **`Side.Conditions.cs`**: Side condition management (Reflect, Light Screen, Tailwind, etc.)
- **`BattleAsync.Players.cs`**: Player-related logic
- **`BattleAsync.Delegates.cs`**: Event delegates and callbacks

**When to use**: If a class exceeds ~300-400 lines or has distinct logical sections, split it into partial classes with descriptive suffixes.

#### State Management Pattern
State classes in `Utils/State.*.cs` provide immutable snapshots:

- **`State.Core.cs`**: Base state infrastructure
- **`State.Battle.cs`**: Battle state snapshots for replay/undo

**When to use**: For any mutable game state that needs history tracking or serialization.

### Coding Patterns & Conventions

#### 1. Showdown ID Compatibility
All Pokemon, moves, abilities, and items use **Showdown IDs** (lowercase, hyphenated):
```csharp
// GOOD
string speciesId = "pikachu";
string abilityId = "lightning-rod";
string moveId = "thunderbolt";

// BAD
string speciesId = "Pikachu";
string abilityId = "LightningRod";
```

Use `ShowdownIdTools` extension methods for ID manipulation:
```csharp
string displayName = showdownId.ShowdownIdToDisplayName(); // "pikachu" -> "Pikachu"
```

#### 2. Union Types for Type Safety
Use `SpecificUnion` and similar discriminated unions instead of base classes:
```csharp
// GOOD - Type-safe, exhaustive pattern matching
SpecificUnion<Pokemon, PokemonSlot> target = GetTarget();
var result = target.Match(
    pokemon => HandlePokemon(pokemon),
    slot => HandleSlot(slot)
);

// BAD - Loses type information
object target = GetTarget();
if (target is Pokemon p) { ... }
```

#### 3. Async Methods
Battle simulation uses async/await extensively:
```csharp
// GOOD - Async all the way
public async Task<BattleResult> ExecuteTurnAsync(TurnActions actions)
{
    await ProcessMovesAsync(actions);
    await ProcessEndOfTurnAsync();
    return GetResult();
}

// BAD - Blocking async code
public BattleResult ExecuteTurn(TurnActions actions)
{
    ProcessMovesAsync(actions).Wait(); // Deadlock risk
}
```

#### 4. Nullable Reference Types
The project uses nullable reference types (enabled in .csproj):
```csharp
// GOOD - Explicit nullability
public Pokemon? GetPokemonBySlot(int slot) { ... }
public Pokemon GetActivePokemon() { ... } // Never null

// Use null-forgiving operator only when you're certain
Pokemon pokemon = maybePokemon!;
```

#### 5. Ability Implementation Pattern
Abilities inherit from `Ability` base class and override event hooks:
```csharp
public class IntimidateAbility : Ability
{
    public override async Task OnSwitchInAsync(Pokemon pokemon, Battle battle)
    {
   // Lower opposing Pokemon's Attack stat
     var opponents = battle.GetOpponents(pokemon);
        foreach (var opponent in opponents.Where(p => p.IsActive))
        {
   await opponent.ModifyStatAsync(StatType.Attack, -1);
        }
    }
}
```

**Common event hooks**:
- `OnSwitchInAsync`: When Pokemon enters battle
- `OnBeforeMove`: Before Pokemon uses a move
- `OnAfterMove`: After Pokemon uses a move
- `OnDamageReceived`: When Pokemon takes damage
- `OnEndOfTurn`: At end of each turn

#### 6. Immutable Data Structures
Prefer records for immutable data:
```csharp
// GOOD - Immutable stat snapshot
public record PokemonStats(int HP, int Attack, int Defense, int SpAtk, int SpDef, int Speed);

// BAD - Mutable struct
public struct PokemonStats
{
    public int HP { get; set; }
    // ...
}
```

#### 7. Error Handling
Use exceptions for truly exceptional cases, validation for user input:
```csharp
// GOOD
if (moveIndex < 0 || moveIndex >= pokemon.Moves.Count)
{
  throw new ArgumentOutOfRangeException(nameof(moveIndex), 
   "Move index must be between 0 and move count");
}

// GOOD - Domain-specific exceptions
if (pokemon.HP <= 0)
{
    throw new InvalidBattleStateException("Cannot use move with fainted Pokemon");
}
```

### Doubles Battle Mechanics

#### Target Selection
Moves can target different positions:
- **Adjacent opponents**: Most attacks (Thunderbolt, etc.)
- **All adjacent**: Spread moves (Earthquake, Surf, etc.)
- **Self**: Stat boosts, protection (Protect, Swords Dance)
- **Ally**: Support moves (Helping Hand)
- **All Pokemon**: Weather/terrain moves

#### Speed and Priority
Action order is determined by:
1. **Priority**: Higher priority moves go first (Quick Attack = +1)
2. **Speed stat**: Higher speed goes first
3. **Trick Room**: Reverses speed order
4. **Tailwind**: Doubles team's speed

### Testing Considerations
When implementing new features:
- Test both single and multi-target scenarios
- Verify ability interactions (multiple abilities triggering)
- Check speed calculation edge cases (paralysis, Trick Room, etc.)
- Validate with Pokémon Showdown behavior as reference

### Common Gotchas
- **Showdown IDs must be lowercase**: Always use lowercase, hyphenated IDs
- **Async all the way**: Don't block on async methods; use `await`
- **Null checks**: Use nullable types and check for null appropriately
- **Type effectiveness**: Remember VGC uses Gen 9 type chart
- **Doubles targeting**: Always consider multi-target scenarios

## Pokemon Showdown Reference
The project references Pokémon Showdown's TypeScript code for mechanics and data structures. All relevant Showdown files are present in the solution in the 'pokemon-showdown' solution folder.

## Questions?
For complex battle mechanics, refer to:
- Official VGC rules and mechanics documentation
- Smogon University doubles resources
