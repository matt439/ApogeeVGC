# Volt Switch Damage Type Bug Fix

## Problem Summary

Volt Switch (and other self-switching moves) were not triggering the switch prompt after dealing damage. The move would deal damage successfully, but the game would immediately request another move instead of prompting the player to switch Pokémon.

## Root Cause

The bug was caused by **type priority in the result combination logic** overwriting integer damage values with boolean success indicators.

### The Type Priority System

The `CombineResults` function in `BattleActions.ResultCombining.cs` uses a priority system to determine which type takes precedence when combining move results:

1. **Undefined** (priority 0) - highest priority
2. **Empty** (priority 1)
3. **null** (priority 2)
4. **Boolean** (priority 3)
5. **Integer** (priority 4) - **lowest priority**

When combining two values of different types, the type with the **lower priority number** (higher precedence) wins.

### The Bug Flow

1. **Damage Calculation**: `GetSpreadDamage` and `SpreadDamage` correctly calculated and applied damage, storing **integer values** (e.g., 27) in the `damage` array.

2. **RunMoveEffects Entry**: The `damage` array entered `RunMoveEffects` with integer values representing the damage dealt.

3. **Per-Target Processing**: For each target, `RunMoveEffects` created a local `didSomething` variable (initially `undefined`) to track whether any effects were applied (boosts, status, etc.).

4. **No Effects Applied**: Since Volt Switch doesn't apply boosts or status effects, `didSomething` remained `undefined` throughout the loop.

5. **Default Success**: At line 241-245, if `didSomething` was still `undefined`, it was set to `true` (boolean):
   ```csharp
   if (didSomething is UndefinedBoolIntUndefinedUnion)
   {
       didSomething = BoolIntUndefinedUnion.FromBool(true);
   }
   ```

6. **The Critical Mistake**: The code then combined this boolean with the integer damage:
   ```csharp
   damage[i] = CombineResults(damage[i], didSomething);  // integer + boolean
   didAnything = CombineResults(didAnything, didSomething);
   ```

7. **Type Priority Overwrites Integer**: Because **boolean has higher priority than integer**, `CombineResults` returned the boolean (`true`), **discarding the damage integer** (27).

8. **Wrong Branch**: The failure check looked for an integer to determine success:
   ```csharp
   if (didAnything is not (IntBoolIntUndefinedUnion { Value: 0 } or IntBoolIntUndefinedUnion) && ...)
   ```
   Since `didAnything` was now a boolean instead of an integer, this condition evaluated to `true`, causing the move to be marked as "failed" despite dealing damage.

9. **No Switch Prompt**: The `else if (move.SelfSwitch != null ...)` branch was never reached, so `SwitchFlag` was never set.

## The Fix

The fix involves **preserving integer damage values** and not overwriting them with boolean success indicators:

```csharp
// Only combine didSomething into damage if damage isn't already an integer (actual damage dealt)
// If damage was dealt, preserve that integer value instead of replacing it with a boolean
if (damage[i] is not IntBoolIntUndefinedUnion)
{
    damage[i] = CombineResults(damage[i], didSomething);
    didAnything = CombineResults(didAnything, didSomething);
}
```

### Why This Works

1. **When damage is dealt**: `damage[i]` is an `IntBoolIntUndefinedUnion` (integer), so we skip the entire block, preserving both:
   - The integer in `damage[i]` (e.g., 27)
   - The integer in `didAnything` (sum of all damage, aggregated from the `damage` array)

2. **When no damage is dealt**: `damage[i]` is not an integer (undefined or boolean), so we combine `didSomething` to track whether any other effects succeeded.

3. **Result**: `didAnything` remains an integer when damage is dealt, allowing the success check to pass and the `SwitchFlag` to be set correctly.

## Key Lessons

### 1. Type Priority Can Hide Bugs

The priority system made sense for combining multiple result types, but it had an unintuitive consequence: **higher-priority types silently discard lower-priority data**, even when that data is more important (actual damage vs. generic success).

### 2. Damage is More Important Than Generic Success

When a move deals damage, that integer value is **more meaningful** than a boolean "something happened" flag. The fix recognizes this by treating integer damage as the final word on success.

### 3. Aggregation Can Mask Issues

The initial `didAnything = damage.Aggregate(...)` correctly started with integer values, but subsequent `CombineResults` calls could corrupt this by mixing in booleans. The fix prevents this corruption.

### 4. Debug Logging Was Essential

The extensive debug logging added during troubleshooting revealed:
- Damage was being dealt (27, 13 damage messages)
- `didAnything` was the wrong type (`BoolBoolIntUndefinedUnion` instead of `IntBoolIntUndefinedUnion`)
- The failure branch was being entered despite successful damage

Without these logs, it would have been much harder to identify that the issue was **type coercion** rather than a logic error.

## Testing

After the fix, Volt Switch correctly:
1. Deals damage (integer stored in `damage` array)
2. Sets `didAnything` to an integer
3. Passes the success check
4. Sets `SwitchFlag` on the source Pokémon
5. Triggers a switch request instead of a move request

## Related Code Locations

- **Bug Location**: `ApogeeVGC\Sim\BattleClasses\BattleActions.MoveEffects.cs` (lines 240-252)
- **Type Priority**: `ApogeeVGC\Sim\BattleClasses\BattleActions.Core.cs` (`GetBattleActionsPriority`)
- **Result Combining**: `ApogeeVGC\Sim\BattleClasses\BattleActions.ResultCombining.cs`
- **Success Check**: `ApogeeVGC\Sim\BattleClasses\BattleActions.MoveEffects.cs` (lines 254-280)

## Similar Bugs to Watch For

Any code that:
1. Combines results of different types using `CombineResults`
2. Expects integer values to be preserved
3. Mixes damage calculation with effect application

Should be reviewed to ensure integer damage values aren't being accidentally overwritten by booleans or other higher-priority types.
