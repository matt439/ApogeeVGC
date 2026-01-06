# Protect Bug Fix - IsZero() Logic Error in BoolIntEmptyUndefinedUnion

## Status
? **FIXED AND VERIFIED** - Protect now correctly blocks incoming attacks

## Issue Summary
Protect move was executing but not preventing damage from incoming attacks. The move would use its turn, show in battle logs, but the opponent's attack would still deal full damage.

## Root Cause
The `IsZero()` method in `BoolBoolIntEmptyUndefinedUnion` was incorrectly implemented as `!Value`, which meant that `false` values were being treated as "zero" (i.e., a successful hit with 0 damage) instead of being treated as a failure that should filter out the target.

### The Bug
In `ApogeeVGC\Sim\Utils\Unions\BoolIntEmptyUndefinedUnion.cs`:
```csharp
// ? BEFORE (BROKEN)
public record BoolBoolIntEmptyUndefinedUnion(bool Value) : BoolIntEmptyUndefinedUnion
{
 public override bool IsTruthy() => Value;
    public override bool IsZero() => !Value; // BUG: false values return true for IsZero()
}
```

### Why This Broke Protect
1. Protect's `OnTryHit` handler returns `new Empty()` to signal a blocked move
2. `EventHandlerAdapter.ConvertReturnValue` correctly converts `EmptyBoolIntEmptyVoidUnion` ? `BoolRelayVar(false)`
3. `HitStepTryEvent` correctly converts `BoolRelayVar(false)` ? `BoolBoolIntEmptyUndefinedUnion(false)`
4. `TrySpreadMoveHit` filters targets using:
   ```csharp
   if (hitResults[i].IsTruthy() || hitResults[i].IsZero())
   {
       newTargets.Add(targets[i]); // Keep this target
   }
   ```
5. For `BoolBoolIntEmptyUndefinedUnion(false)`:
   - `IsTruthy()` returns `false` ?
   - `IsZero()` returns `!false` = `true` ? **BUG!**
6. Because `IsZero()` returned `true`, the target was kept in the list and got hit!

### Semantic Meaning of Union Values
In Pokemon battle mechanics, there are distinct semantic meanings:

| Value Type | Meaning | Example Use Case |
|------------|---------|------------------|
| **Integer 0** | Move dealt 0 damage but still "hit" | High defense, type resistance, Substitute |
| **Boolean false** | Move was blocked/failed for this target | Protect, immunity, miss |
| **Empty** | Special "NOT_FAIL" marker | Protect-style successful blocks |
| **Undefined** | No damage dealt, move continues | Status moves, effects that don't deal damage |

The `IsZero()` method is meant to identify the "0 damage but still hit" case (integer 0), **NOT** boolean false.

## Solution
Changed `IsZero()` to always return `false` for boolean variants:

```csharp
// ? AFTER (FIXED)
public record BoolBoolIntEmptyUndefinedUnion(bool Value) : BoolIntEmptyUndefinedUnion
{
    public override bool IsTruthy() => Value;
  public override bool IsZero() => false; // Boolean values are never "zero damage"
 
    public override BoolIntUndefinedUnion ToBoolIntUndefinedUnion() => Value;
}
```

### Why This Works
Now when Protect blocks a move:
1. Protect's `OnTryHit` returns `Empty`
2. Converts to `BoolRelayVar(false)` via `EventHandlerAdapter`
3. Converts to `BoolBoolIntEmptyUndefinedUnion(false)` in `HitStepTryEvent`
4. `IsTruthy()` returns `false` ?
5. `IsZero()` returns `false` ?
6. Target is filtered out (not added to `newTargets`) ?
7. Move doesn't hit the protected Pokemon ?

## Files Modified
1. **`ApogeeVGC\Sim\Utils\Unions\BoolIntEmptyUndefinedUnion.cs`**
   - Fixed `IsZero()` method in `BoolBoolIntEmptyUndefinedUnion` class
   - Changed from `return !Value` to `return false`

## Testing Results
After the fix, Protect works correctly:
- ? Executes with priority +4 (goes first)
- ? Adds the Protect volatile to the user
- ? Blocks incoming attacks that have the `Protect` flag
- ? Displays "-singleturn, Protect" message when activated
- ? Displays "-activate, [pokemon], move: Protect" when blocking an attack
- ? Opponent's attack does not deal damage
- ? Stall mechanic works (success rate decreases on consecutive uses)

## Debugging Process Summary
The bug was found through extensive debug logging that revealed:
1. Protect's `OnTryHit` was being called ?
2. It was returning `Empty` ?
3. Conversion to `BoolRelayVar(false)` was correct ?
4. **But targets weren't being filtered out** ?
5. Investigation of target filtering logic in `TrySpreadMoveHit` revealed the `IsZero()` bug

## Related Context

### From UnionTypeHandlingGuide.md
> In Pokemon battle mechanics, `Undefined` and `0` are semantically different:
> - `0` means the move dealt 0 damage (e.g., due to immunity or substitutes)
> - `Undefined` means the move doesn't deal damage at all (e.g., status moves like Leech Seed)
> - `false` means the move/action failed or was blocked

The same distinction applies to boolean values - they represent **pass/fail states**, not damage amounts.

### Similar Bugs to Watch For
When working with union types, be careful about:
1. **Semantic meaning** - Don't treat different types as equivalent (false ? 0)
2. **IsZero() usage** - Should only return true for actual numeric zero, not false booleans
3. **IsTruthy() usage** - Different types have different truthiness rules
4. **Target filtering** - Make sure blocked/failed targets are properly filtered out

## Future Reference
If similar issues occur with other protective moves (Detect, King's Shield, Spiky Shield, etc.):
1. Check if they return `Empty` from `OnTryHit`
2. Verify `EventHandlerAdapter` conversion is correct
3. Check target filtering logic in `TrySpreadMoveHit`
4. Verify `IsZero()` and `IsTruthy()` implementations for affected union types

All protective moves should follow the same pattern:
```csharp
OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
{
    if (!(move.Flags.Protect ?? false))
        return new VoidReturn(); // Not a protectable move
    
    // Show activation message
    if (battle.DisplayUi)
        battle.Add("-activate", target, "move: [MoveName]");
    
    // Block the move (NOT_FAIL)
    return new Empty();
}, priority: 3),
```

## Date Fixed
2025-01-XX (current session)

## Related Issues
- LeechSeedBugFix.md - Similar union type handling issue with Undefined checks
- UnionTypeHandlingGuide.md - General guidance on union type semantics
