# MoveIdVoidUnion Return Conversion Fix

**Date**: 2025-01-20
**Severity**: High  
**Systems Affected**: Two-turn moves (Fly, Dig, Dive, etc.), LockMove event handlers

---

## Problem

When a Pokemon used a two-turn move (e.g., Fly, Dig, Dive, Skull Bash, Solar Beam, Sky Drop), the battle crashed with the following exception:

```
System.InvalidOperationException: Event LockMove adapted handler failed on effect Two Turn Move (Condition)

Inner Exception:
InvalidOperationException: Event LockMove: Unable to convert return value of type 'VoidMoveIdVoidUnion' to RelayVar
```

**Symptoms**:
- Battle crashes when two-turn moves are used
- Stack trace shows failure in `EventHandlerAdapter.ConvertReturnValue`
- Error indicates the return value type `VoidMoveIdVoidUnion` cannot be converted to `RelayVar`

**Affected Code Flow**:
1. Pokemon uses a two-turn move (e.g., Fly)
2. `TwoTurnMove` condition is applied with `OnLockMove` handler
3. During `RunMove`, the `LockMove` event is triggered
4. The `OnLockMove` handler returns `MoveIdVoidUnion.FromVoid()` (which creates a `VoidMoveIdVoidUnion`)
5. `EventHandlerAdapter.ConvertReturnValue` fails to convert `VoidMoveIdVoidUnion` to `RelayVar`

---

## Root Cause

The `EventHandlerAdapter.ConvertReturnValue` method didn't know how to convert `MoveIdVoidUnion` union types into `RelayVar` types.

**Code Context**:

In `ConditionsSTU.cs` lines 1313-1322, the `TwoTurnMove` condition defines an `OnLockMove` handler:
```csharp
OnLockMove = new OnLockMoveEventInfo(
    (Func<Battle, Pokemon, MoveIdVoidUnion>)((battle, _) =>
    {
        if (battle.EffectState.Move.HasValue)
        {
            return battle.EffectState.Move.Value; // Returns MoveIdMoveIdVoidUnion
        }

        return MoveIdVoidUnion.FromVoid(); // Returns VoidMoveIdVoidUnion
    })),
```

The `MoveIdVoidUnion` is a discriminated union that can be either:
- `MoveIdMoveIdVoidUnion(MoveId)` - when it returns a move ID to lock into
- `VoidMoveIdVoidUnion(VoidReturn)` - when it returns void (no locked move)

**The Gap**: The `EventHandlerAdapter.ConvertReturnValue` method had cases for many union types (`BoolVoidUnion`, `IntVoidUnion`, `DoubleVoidUnion`, `PokemonVoidUnion`, etc.) but was missing the case for `MoveIdVoidUnion`.

---

## Solution

Added conversion logic to `EventHandlerAdapter.ConvertReturnValue` to handle both variants of `MoveIdVoidUnion`.

**Modified File**: `ApogeeVGC\Sim\Events\EventHandlerAdapter.cs`

**Changes**:
Added a new case in the pattern match expression after the `PokemonVoidUnion` case (around line 427):

```csharp
// MoveIdVoidUnion -> MoveId or VoidReturn
MoveIdMoveIdVoidUnion moveIdVoid => new MoveIdRelayVar(moveIdVoid.MoveId),
VoidMoveIdVoidUnion => new VoidReturnRelayVar(),
```

---

## How It Works

1. When the `OnLockMove` handler returns a `MoveIdVoidUnion`, it's either:
   - `MoveIdMoveIdVoidUnion` containing a `MoveId` value
   - `VoidMoveIdVoidUnion` containing a `VoidReturn`
2. `EventHandlerAdapter.ConvertReturnValue` pattern matches on the return value type
3. For `MoveIdMoveIdVoidUnion`, it extracts the `MoveId` and wraps it in `MoveIdRelayVar`
4. For `VoidMoveIdVoidUnion`, it returns `VoidReturnRelayVar`
5. The event system can now properly handle the return value

---

## Testing

**Reproduces Issue**: 
- Run battles with random teams until a two-turn move (Fly, Dig, Dive, etc.) is used
- Exception thrown when LockMove event is triggered

**Verifies Fix**:
- Two-turn moves now execute correctly
- First turn: Pokemon charges/prepares (invulnerable for some moves)
- Second turn: Pokemon attacks with the locked move
- No exceptions thrown during LockMove event handling

---

## Related Fixes

This follows the same pattern as other union type conversion fixes:
- [Tailwind OnModifySpe Fix](./TailwindOnModifySpeFix.md) - VoidReturn causing IntRelayVar type mismatch
- [Stat Modification Handler VoidReturn Fix](./StatModificationHandlerVoidReturnFix.md) - Multiple stat handlers returning VoidReturn instead of int
- [Union Type Handling Guide](./UnionTypeHandlingGuide.md) - Comprehensive guide for preventing union type issues

**Pattern**: Event handlers that return union types (e.g., `IntVoidUnion`, `BoolVoidUnion`, `MoveIdVoidUnion`) need explicit conversion cases in `EventHandlerAdapter.ConvertReturnValue` to unwrap the variant and convert it to the appropriate `RelayVar` type.

---

## Keywords

`two-turn move`, `LockMove event`, `MoveIdVoidUnion`, `event handler return`, `union type conversion`, `Fly`, `Dig`, `Dive`, `Skull Bash`, `Solar Beam`, `Sky Drop`
