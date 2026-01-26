# VoidFalseUnion Return Conversion Fix

**Date**: 2025-01-20  
**Severity**: High  
**Systems Affected**: Move-blocking conditions (Throat Chop, Taunt, etc.), ModifyMove event handlers

---

## Problem

When a Pokemon with the Throat Chop condition tried to use a sound-based move, the battle crashed with the following exception:

```
System.InvalidOperationException: Event ModifyMove adapted handler failed on effect Throat Chop (Condition)

Inner Exception:
InvalidOperationException: Event ModifyMove: Unable to convert return value of type 'VoidVoidFalseUnion' to RelayVar
```

**Symptoms**:
- Battle crashes when a Pokemon affected by Throat Chop tries to use a sound move
- Stack trace shows failure in `EventHandlerAdapter.ConvertReturnValue`
- Error indicates the return value type `VoidVoidFalseUnion` cannot be converted to `RelayVar`

**Affected Code Flow**:
1. Pokemon is hit by Throat Chop move, gaining the Throat Chop condition
2. On subsequent turns, when the Pokemon tries to use a move, the `ModifyMove` event is triggered
3. The `OnModifyMove` handler for Throat Chop condition checks if the move has the sound flag
4. Handler returns either `VoidFalseUnion.FromVoid()` (for non-sound moves) or `VoidFalseUnion.FromFalse()` (for sound moves)
5. `EventHandlerAdapter.ConvertReturnValue` fails to convert the `VoidFalseUnion` variants to `RelayVar`

---

## Root Cause

The `EventHandlerAdapter.ConvertReturnValue` method didn't know how to convert `VoidFalseUnion` union types into `RelayVar` types.

**Code Context**:

In `ConditionsSTU.cs` lines 1194-1208, the `ThroatChop` condition defines an `OnModifyMove` handler:
```csharp
OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
{
    // Gen 9 has no Z-moves or Max moves, so we just check sound flag
    if (move.Flags.Sound == true)
    {
        if (battle.DisplayUi)
        {
            battle.Add("cant", pokemon, "move: Throat Chop");
        }

        return VoidFalseUnion.FromFalse();  // Returns FalseVoidFalseUnion
    }

    return VoidFalseUnion.FromVoid();  // Returns VoidVoidFalseUnion
}),
```

The `VoidFalseUnion` is a discriminated union that can be either:
- `VoidVoidFalseUnion(VoidReturn)` - when it returns void (no effect, allow move)
- `FalseVoidFalseUnion` - when it returns false (block/prevent the move)

**The Gap**: The `EventHandlerAdapter.ConvertReturnValue` method had cases for many union types (`BoolVoidUnion`, `IntVoidUnion`, `MoveIdVoidUnion`, etc.) but was missing the case for `VoidFalseUnion`.

---

## Solution

Added conversion logic to `EventHandlerAdapter.ConvertReturnValue` to handle both variants of `VoidFalseUnion`.

**Modified File**: `ApogeeVGC\Sim\Events\EventHandlerAdapter.cs`

**Changes**:
Added a new case in the pattern match expression after the `MoveIdVoidUnion` case (around line 443):

```csharp
// VoidFalseUnion -> VoidReturn or false
VoidVoidFalseUnion => new VoidReturnRelayVar(),
FalseVoidFalseUnion => new BoolRelayVar(false),
```

---

## How It Works

1. When the `OnModifyMove` handler returns a `VoidFalseUnion`, it's either:
   - `VoidVoidFalseUnion` containing a `VoidReturn` (allow the move to proceed normally)
   - `FalseVoidFalseUnion` representing false (block/prevent the move)
2. `EventHandlerAdapter.ConvertReturnValue` pattern matches on the return value type
3. For `VoidVoidFalseUnion`, it returns `VoidReturnRelayVar` (no effect)
4. For `FalseVoidFalseUnion`, it returns `BoolRelayVar(false)` (block the move)
5. The event system can now properly handle the return value

---

## Semantic Meaning

Following the copilot instructions for this codebase:
- `VoidReturn` = "no return needed" / "continue normally" = no effect on event flow
- `False` = "return false" = explicit signal to block/prevent the action

For Throat Chop specifically:
- Non-sound moves return `VoidReturn` ? move proceeds normally
- Sound moves return `false` ? move is blocked with "cant" message

---

## Testing

**Reproduces Issue**: 
- Create a Pokemon with Throat Chop condition
- Have it attempt to use a sound-based move (e.g., Boomburst, Hyper Voice, Snarl)
- Exception thrown when ModifyMove event is triggered

**Verifies Fix**:
- Pokemon with Throat Chop can use non-sound moves normally
- Pokemon with Throat Chop is blocked from using sound moves with "cant" message
- No exceptions thrown during ModifyMove event handling

---

## Related Fixes

This follows the same pattern as other union type conversion fixes:
- [MoveIdVoidUnion Return Conversion Fix](./MoveIdVoidUnionReturnConversionFix.md) - VoidMoveIdVoidUnion cannot be converted to RelayVar
- [Tailwind OnModifySpe Fix](./TailwindOnModifySpeFix.md) - VoidReturn causing IntRelayVar type mismatch
- [Stat Modification Handler VoidReturn Fix](./StatModificationHandlerVoidReturnFix.md) - Multiple stat handlers returning VoidReturn instead of int
- [Union Type Handling Guide](./UnionTypeHandlingGuide.md) - Comprehensive guide for preventing union type issues

**Pattern**: Event handlers that return union types (e.g., `IntVoidUnion`, `BoolVoidUnion`, `VoidFalseUnion`) need explicit conversion cases in `EventHandlerAdapter.ConvertReturnValue` to unwrap the variant and convert it to the appropriate `RelayVar` type.

---

## Potentially Affected Conditions

Other conditions that may use `VoidFalseUnion` for move-blocking mechanics:
- Taunt (blocks status moves)
- Torment (blocks repeated moves)
- Disable (blocks specific move)
- Gravity (blocks certain moves)
- Heal Block (blocks healing moves)

All of these should now work correctly with the fix in place.

---

## Keywords

`Throat Chop`, `sound move`, `move blocking`, `VoidFalseUnion`, `ModifyMove event`, `event handler return`, `union type conversion`, `Taunt`, `Torment`, `Disable`
