# IntBoolUnion Return Conversion Fix

**Date**: 2025
**Severity**: High  
**Systems Affected**: Event handlers that return `IntBoolUnion` (healing events, berry interactions)

## Problem

The Ripen ability (and potentially other handlers returning `IntBoolUnion`) caused a crash when handling the `TryHeal` event. The error was:

```
System.InvalidOperationException: Event TryHeal adapted handler failed on effect Ripen (Ability)
Inner exception: InvalidOperationException: Event TryHeal: Unable to convert return value of type 'IntIntBoolUnion' to RelayVar
```

### Error Stack Trace

The exception occurred when:
1. Berry Juice item triggered a `TryHeal` event
2. The Ripen ability's `OnTryHeal` handler executed and returned `IntBoolUnion.FromInt(damage)`
3. `EventHandlerAdapter.ConvertReturnValue` tried to convert the return value
4. The method didn't recognize `IntIntBoolUnion` (concrete type of `IntBoolUnion`) and threw an exception

## Root Cause

The `EventHandlerAdapter.ConvertReturnValue` method was missing conversion cases for the `IntBoolUnion` type and its concrete implementations:
- `IntIntBoolUnion` (when the union holds an int value)
- `BoolIntBoolUnion` (when the union holds a bool value)

This was a gap in the union type conversion system. Many other union types were already handled (e.g., `IntBoolVoidUnion`, `BoolIntUndefinedUnion`), but `IntBoolUnion` was overlooked.

## Affected Code

**Ripen Ability** (`ApogeeVGC\Data\Abilities\AbilitiesPQR.cs`):
```csharp
OnTryHeal = new OnTryHealEventInfo(
    (Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>)((battle, damage,
        target, _, effect) =>
    {
        // ... effect checking logic ...
        
        if (effect is Item { IsBerry: true })
        {
            battle.ChainModify(2);
            return IntBoolUnion.FromInt(battle.FinalModify(damage));  // ? Returns IntIntBoolUnion
        }

        return IntBoolUnion.FromInt(damage);  // ? Returns IntIntBoolUnion
    })),
```

**Berry Juice Item** (`ApogeeVGC\Data\Items\ItemsABC.cs`):
```csharp
OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
{
    if (pokemon.Hp <= pokemon.MaxHp / 2)
    {
        // Check TryHeal event first, then use item if allowed
        RelayVar? canHeal = battle.RunEvent(EventId.TryHeal, pokemon, null,
            battle.Effect, 20);  // ? Triggers Ripen's OnTryHeal
        if (canHeal is not BoolRelayVar { Value: false } && pokemon.UseItem())
        {
            battle.Heal(20);
        }
    }
}),
```

## Solution

Added conversion cases for `IntBoolUnion` in `EventHandlerAdapter.ConvertReturnValue`:

```csharp
// IntBoolUnion -> int or bool
IntIntBoolUnion intBool => new IntRelayVar(intBool.Value),
BoolIntBoolUnion boolInt => new BoolRelayVar(boolInt.Value),
```

### Placement

The new cases were added before the `IntVoidUnion` conversion cases, following the established pattern of handling simpler union types before more complex ones.

## Type System Details

**IntBoolUnion** is defined as:
```csharp
public abstract record IntBoolUnion
{
    public abstract int ToInt();
    
    public static IntBoolUnion FromInt(int value) => new IntIntBoolUnion(value);
    public static IntBoolUnion FromBool(bool value) => new BoolIntBoolUnion(value);
}

public record IntIntBoolUnion(int Value) : IntBoolUnion;
public record BoolIntBoolUnion(bool Value) : IntBoolUnion;
```

**Conversion Logic**:
- `IntIntBoolUnion` ? `IntRelayVar` (preserves the integer value)
- `BoolIntBoolUnion` ? `BoolRelayVar` (preserves the boolean value)

This follows the same pattern as other union type conversions in the system.

## Related Patterns

This fix is similar to previous union type conversion fixes:
- [MoveIdVoidUnion Return Conversion Fix](MoveIdVoidUnionReturnConversionFix.md) - Added support for `MoveIdVoidUnion`
- [VoidFalseUnion Return Conversion Fix](VoidFalseUnionReturnConversionFix.md) - Added support for `VoidFalseUnion`

## Testing

The fix was verified by:
1. Building the solution successfully
2. Testing would involve running battles with Pokémon that have the Ripen ability and hold berries

## Impact

This fix enables:
- **Ripen ability** to function correctly when doubling healing from berries
- Any other event handlers that return `IntBoolUnion` to work properly
- Future event handlers using `IntBoolUnion` return types

## Keywords

`Ripen`, `IntBoolUnion`, `IntIntBoolUnion`, `BoolIntBoolUnion`, `TryHeal`, `event handler`, `union type conversion`, `berry`, `healing`, `EventHandlerAdapter`, `ConvertReturnValue`
