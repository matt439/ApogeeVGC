# SparseBoostsTableVoidUnion Return Conversion Fix

**Date**: 2025
**Severity**: High  
**Systems Affected**: Event handlers that return `SparseBoostsTableVoidUnion` (ModifyBoost events, stat boost modifications)

## Problem

The Unaware ability (and potentially other handlers returning `SparseBoostsTableVoidUnion`) caused a crash when handling the `ModifyBoost` event. The error was:

```
System.InvalidOperationException: Event ModifyBoost adapted handler failed on effect Unaware (Ability)
Inner exception: InvalidOperationException: Event ModifyBoost: Unable to convert return value of type 'VoidSparseBoostsTableVoidUnion' to RelayVar
```

### Error Stack Trace

The exception occurred when:
1. A Pokémon with Unaware was switching in
2. `Pokemon.UpdateSpeed()` called `GetStat()` which triggered a `ModifyBoost` event
3. The Unaware ability's `OnAnyModifyBoost` handler executed and returned `VoidReturn()`
4. `EventHandlerAdapter.ConvertReturnValue` tried to convert the return value
5. The method didn't recognize `VoidSparseBoostsTableVoidUnion` and threw an exception

### Detailed Call Stack
```
ApogeeVGC.Sim.Events.EventHandlerAdapter.ConvertReturnValue()
  ?
ApogeeVGC.Sim.BattleClasses.Battle.InvokeEventHandlerInfo()
  ?
ApogeeVGC.Sim.BattleClasses.Battle.RunEvent(EventId.ModifyBoost, ...)
  ?
ApogeeVGC.Sim.PokemonClasses.Pokemon.GetStat()
  ?
ApogeeVGC.Sim.PokemonClasses.Pokemon.UpdateSpeed()
  ?
ApogeeVGC.Sim.BattleClasses.BattleQueue.InsertChoice()
  ?
ApogeeVGC.Sim.BattleClasses.BattleActions.SwitchIn()
```

## Root Cause

The `EventHandlerAdapter.ConvertReturnValue` method was missing conversion cases for the `SparseBoostsTableVoidUnion` type and its concrete implementations:
- `SparseBoostsTableSparseBoostsTableVoidUnion` (when the union holds a `SparseBoostsTable` value)
- `VoidSparseBoostsTableVoidUnion` (when the union holds a `VoidReturn` value)

This was a gap in the union type conversion system. Many other union types were already handled (e.g., `IntBoolUnion`, `IntBoolVoidUnion`, `BoolIntUndefinedUnion`), but `SparseBoostsTableVoidUnion` was overlooked.

## Affected Code

**Unaware Ability** (`ApogeeVGC\Data\Abilities\AbilitiesSTU.cs`):
```csharp
[AbilityId.Unaware] = new()
{
    Id = AbilityId.Unaware,
    Name = "Unaware",
    Num = 109,
    Rating = 4.0,
    Flags = new AbilityFlags { Breakable = true },
    OnAnyModifyBoost = new OnAnyModifyBoostEventInfo((battle, boosts, pokemon) =>
    {
        if (battle.EffectState.Target is not PokemonEffectStateTarget { Pokemon: var unawareUser })
            return new VoidReturn();  // ? Returns VoidSparseBoostsTableVoidUnion
        if (unawareUser == pokemon) return new VoidReturn();  // ? Returns VoidSparseBoostsTableVoidUnion

        // When Unaware user is attacking: ignore target's Def, SpD, and Evasion boosts
        if (unawareUser == battle.ActivePokemon && pokemon == battle.ActiveTarget)
        {
            boosts.Def = 0;
            boosts.SpD = 0;
            boosts.Evasion = 0;
        }

        // When Unaware user is being attacked: ignore attacker's Atk, Def (for Body Press), SpA, and Accuracy boosts
        if (pokemon == battle.ActivePokemon && unawareUser == battle.ActiveTarget)
        {
            boosts.Atk = 0;
            boosts.Def = 0;
            boosts.SpA = 0;
            boosts.Accuracy = 0;
        }

        return new VoidReturn();  // ? Returns VoidSparseBoostsTableVoidUnion
    }),
},
```

**Pokemon.GetStat** (`ApogeeVGC\Sim\PokemonClasses\Pokemon.Stats.cs`):
```csharp
public int GetStat(StatIdExceptHp statName, bool unboosted = false, bool unmodified = false)
{
    // ...
    if (!unboosted && Battle is not null)
    {
        // Get boosts
        var boostsTable = new SparseBoostsTable { [statBoost] = boostValue };
        var boosts = Battle.RunEvent(EventId.ModifyBoost, this, null, null,
            new SparseBoostsTableRelayVar(boostsTable));  // ? Triggers Unaware's OnAnyModifyBoost
        // ...
    }
    // ...
}
```

## Solution

Added conversion cases for `SparseBoostsTableVoidUnion` in `EventHandlerAdapter.ConvertReturnValue`:

```csharp
// SparseBoostsTableVoidUnion -> SparseBoostsTable or VoidReturn
SparseBoostsTableSparseBoostsTableVoidUnion sparseBoostsVoid => new SparseBoostsTableRelayVar(sparseBoostsVoid.Table),
VoidSparseBoostsTableVoidUnion => new VoidReturnRelayVar(),
```

### Placement

The new cases were added after the `VoidFalseUnion` conversion cases and before the primitive types, following the established pattern of handling union types before primitive types.

## Type System Details

**SparseBoostsTableVoidUnion** is defined as:
```csharp
public abstract record SparseBoostsTableVoidUnion
{
    public static implicit operator SparseBoostsTableVoidUnion(SparseBoostsTable table) =>
        new SparseBoostsTableSparseBoostsTableVoidUnion(table);
    public static implicit operator SparseBoostsTableVoidUnion(VoidReturn value) =>
        new VoidSparseBoostsTableVoidUnion(value);
    public static SparseBoostsTableVoidUnion FromVoid() => new VoidSparseBoostsTableVoidUnion(new VoidReturn());
}

public record SparseBoostsTableSparseBoostsTableVoidUnion(SparseBoostsTable Table) :
    SparseBoostsTableVoidUnion;
public record VoidSparseBoostsTableVoidUnion(VoidReturn Value) : SparseBoostsTableVoidUnion;
```

**Conversion Logic**:
- `SparseBoostsTableSparseBoostsTableVoidUnion` ? `SparseBoostsTableRelayVar` (preserves the SparseBoostsTable value)
- `VoidSparseBoostsTableVoidUnion` ? `VoidReturnRelayVar` (indicates no modification)

**SparseBoostsTableRelayVar** already existed in `RelayVar.cs` (line 66), so no new RelayVar type needed to be created.

## TypeScript Reference

In pokemon-showdown, the Unaware ability's `onAnyModifyBoost` handler returns implicitly void:

```typescript
unaware: {
    onAnyModifyBoost(boosts, pokemon) {
        const unawareUser = this.effectState.target;
        if (unawareUser === pokemon) return;  // ? implicit void
        if (unawareUser === this.activePokemon && pokemon === this.activeTarget) {
            boosts['def'] = 0;
            boosts['spd'] = 0;
            boosts['evasion'] = 0;
        }
        if (pokemon === this.activePokemon && unawareUser === this.activeTarget) {
            boosts['atk'] = 0;
            boosts['def'] = 0;
            boosts['spa'] = 0;
            boosts['accuracy'] = 0;
        }
    },  // ? no return statement = implicit void
    // ...
},
```

## Related Patterns

This fix is similar to previous union type conversion fixes:
- [IntBoolUnion Return Conversion Fix](IntBoolUnionReturnConversionFix.md) - Added support for `IntBoolUnion`
- [MoveIdVoidUnion Return Conversion Fix](MoveIdVoidUnionReturnConversionFix.md) - Added support for `MoveIdVoidUnion`
- [VoidFalseUnion Return Conversion Fix](VoidFalseUnionReturnConversionFix.md) - Added support for `VoidFalseUnion`

## Testing

The fix was verified by:
1. Building the solution successfully
2. Further testing would involve running battles with Pokémon that have the Unaware ability

## Impact

This fix enables:
- **Unaware ability** to function correctly when ignoring stat boosts
- Any other event handlers that return `SparseBoostsTableVoidUnion` to work properly
- Future event handlers using `SparseBoostsTableVoidUnion` return types
- All ModifyBoost events to handle both boost modifications and void returns

## Keywords

`Unaware`, `SparseBoostsTableVoidUnion`, `SparseBoostsTableSparseBoostsTableVoidUnion`, `VoidSparseBoostsTableVoidUnion`, `ModifyBoost`, `OnAnyModifyBoost`, `event handler`, `union type conversion`, `stat boosts`, `EventHandlerAdapter`, `ConvertReturnValue`, `SparseBoostsTableRelayVar`
