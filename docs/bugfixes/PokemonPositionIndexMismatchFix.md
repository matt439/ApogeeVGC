# Pokemon Position Index Mismatch Fix

## Problem Summary
`ArgumentOutOfRangeException` when attempting to switch with a trapped Pokemon, caused by index mismatches between `Active` list and `moveRequest.Active` list.

## Errors

### Initial Error
```
System.ArgumentOutOfRangeException
  Message=Index was out of range. Must be non-negative and less than the size of the collection. (Parameter 'index')
  at ApogeeVGC.Sim.SideClasses.Side.UpdateRequestForPokemon(Pokemon pokemon...)
```

### Follow-up Error (after partial fix)
```
System.InvalidOperationException
  Message=Pokemon not found in active list or index out of range (activeIndex=1, moveRequest.Active.Count=1)
```

## Root Cause

### Issue 1: Pokemon.Position Semantics
In the TypeScript Pokemon Showdown source, `pokemon.position` is documented as:
> "Index of `pokemon.side.pokemon` and `pokemon.side.active`, which are guaranteed to be the same for active pokemon."

However, in the C# implementation, this invariant was not always maintained.

### Issue 2: MoveRequest.Active Index Mismatch (Critical)
The C# code was building `moveRequest.Active` using a filter that removed fainted Pokemon:
```csharp
// WRONG - changes indices by filtering
var activeData = side.Active
    .Where(pokemon => pokemon is { Fainted: false })
    .Select(pokemon => pokemon!.GetMoveRequestData())
    .ToList();
```

The TypeScript code preserves indices using map:
```typescript
// CORRECT - preserves indices
const activeData = side.active.map(pokemon => pokemon?.getMoveRequestData());
```

**Example of the bug:**
- `side.Active[0]` = fainted Pokemon
- `side.Active[1]` = alive Pokemon (at index 1)
- C# (old): `moveRequest.Active` = [alive_pokemon_data] (size 1, index 0)
- TypeScript: `moveRequest.Active` = [null, alive_pokemon_data] (size 2, index 1 preserved)

When `UpdateRequestForPokemon` tries to look up the alive Pokemon using `Active.IndexOf(pokemon)` which returns 1, but `moveRequest.Active.Count` is only 1, we get an index out of range error.

## Solution

### Part 1: Fix MoveRequest.Active Generation
Changed `Battle.Requests.cs` to preserve indices like TypeScript:
```csharp
// AFTER - preserves indices, null for fainted/empty slots
var activeData = side.Active
    .Select(pokemon => pokemon is { Fainted: false } ? pokemon.GetMoveRequestData() : null)
    .ToList();
```

### Part 2: Update MoveRequest Type
Changed `MoveRequest.Active` to allow nullable entries:
```csharp
// BEFORE
public required IReadOnlyList<PokemonMoveRequestData> Active { get; init; }

// AFTER  
public required IReadOnlyList<PokemonMoveRequestData?> Active { get; init; }
```

### Part 3: Fix SlotConditions Access
Changed all `SlotConditions[pokemon.Position]` references to use validated indices.

### Part 4: Update All Consumers of MoveRequest.Active
All code that accesses `moveRequest.Active[index]` must handle null entries (representing fainted Pokemon):
- `PlayerRandom.cs` - Skip null entries and add pass actions
- `PlayerConsole.cs` - Skip null entries and add pass actions
- `ChoiceInputManager.MainBattle.cs` - Pattern match with null checks before accessing
- `ChoiceInputManager.TeamPreview.cs` - Pattern match with null checks

## Files Changed
- `ApogeeVGC\Sim\Choices\MoveRequest.cs` - Made Active property nullable
- `ApogeeVGC\Sim\BattleClasses\Battle.Requests.cs` - Preserve indices in request generation  
- `ApogeeVGC\Sim\SideClasses\Side.Choices.cs` - Use Active.IndexOf instead of pokemon.Position
- `ApogeeVGC\Sim\Player\PlayerRandom.cs` - Handle null entries with pass actions
- `ApogeeVGC\Sim\Player\PlayerConsole.cs` - Handle null entries with pass actions
- `ApogeeVGC\Gui\ChoiceUI\ChoiceInputManager.MainBattle.cs` - Add null checks for Active access
- `ApogeeVGC\Gui\ChoiceUI\ChoiceInputManager.TeamPreview.cs` - Add null checks for Active access

## TypeScript Reference
- `pokemon-showdown/sim/battle.ts` line 1400: `side.active.map(pokemon => pokemon?.getMoveRequestData())`
- `pokemon-showdown/sim/side.ts` line 858: `(this.activeRequest as MoveRequest).active[pokemon.position]`

## Lessons Learned
1. When porting TypeScript `.filter().map()` patterns, check if index preservation matters
2. The `map()` function preserves indices with undefined entries; `Where().Select()` does not
3. `pokemon.Position` should equal the Active list index for active Pokemon, but verify this invariant
4. Add defensive checks when accessing arrays by index from external sources

## Keywords
`Position`, `Active`, `SlotConditions`, `ArgumentOutOfRangeException`, `InvalidOperationException`, `trapped`, `ChooseSwitch`, `UpdateRequestForPokemon`, `index mismatch`, `MoveRequest`, `fainted Pokemon`, `index preservation`
2. The `index` variable from `GetChoiceIndex()` is the reliable active slot index within choice processing methods
3. Add defensive bounds checking when accessing arrays with Pokemon position values

## Keywords
`Position`, `Active`, `SlotConditions`, `ArgumentOutOfRangeException`, `trapped`, `ChooseSwitch`, `UpdateRequestForPokemon`, `index mismatch`
