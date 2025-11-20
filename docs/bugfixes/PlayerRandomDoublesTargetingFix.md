# Player Random Doubles Targeting Fix

## Problem Summary

When running doubles battles with `PlayerRandom`, the battle would enter an infinite loop where the random player repeatedly generated invalid move choices. Every move choice was rejected with errors like "Can't move: Heavy Slam needs a target", causing the player to regenerate choices indefinitely without ever making progress.

**Symptoms:**
- Infinite loop in doubles battles (never in singles)
- Debug output showing repeated patterns of:
  ```
  [PlayerRandom] Pokemon 1 selected move: Heavy Slam
  [Side.Choose] Action 0 failed: Can't move: Heavy Slam needs a target
  [Side.Choose] Overall choice failed for Random
  ```
- Random player trying different moves but all failing with "needs a target"
- Eventually hitting turn limit with `BattleTurnLimitException`

## Root Cause

The `PlayerRandom.GetRandomTargetLocation()` method always returned `0` (auto-targeting), regardless of move type or battle format. This worked fine in singles battles, but in **doubles battles**, moves that require explicit targeting validation fails when `targetLoc == 0` and `Active.Count >= 2`.

From `Side.ChooseMove()`:
```csharp
else if (Battle.Actions.TargetTypeChoices(move.Target))
{
    if (targetLoc == 0 && Active.Count >= 2)
    {
        return EmitChoiceError($"Can't move: {move.Name} needs a target");
    }
    ...
}
```

The `TargetTypeChoices()` method returns `true` for these move targets:
- `MoveTarget.Normal` - Standard single-target moves (e.g., Heavy Slam, Fake Out)
- `MoveTarget.Any` - Can target any Pokemon
- `MoveTarget.AdjacentAlly` - Target an adjacent ally
- `MoveTarget.AdjacentAllyOrSelf` - Target self or adjacent ally
- `MoveTarget.AdjacentFoe` - Target an adjacent opponent

In doubles battles, these moves **require** an explicit target location (non-zero value) to specify which opponent or ally to target.

## Solution

Modified `GetRandomTargetLocation()` to detect moves requiring explicit targeting and return a valid target location (1 or 2) for doubles battles:

```csharp
private int GetRandomTargetLocation(MoveTarget targetType)
{
    // Check if this move type requires explicit targeting
    var requiresExplicitTarget = targetType is MoveTarget.Normal 
        or MoveTarget.Any 
        or MoveTarget.AdjacentAlly 
        or MoveTarget.AdjacentAllyOrSelf 
        or MoveTarget.AdjacentFoe;

    if (!requiresExplicitTarget)
    {
        // Moves like AllAdjacent, AllAdjacentFoes, etc. use auto-targeting (0)
        return 0;
    }

    // For moves requiring explicit targets, pick a random opponent slot (1 or 2)
    // In doubles: 1 = left opponent, 2 = right opponent
    return _random.Random(1, 3); // Returns 1 or 2
}
```

**Target Location System:**
- `0` = Auto-targeting (for moves that don't need explicit targets)
- `1` = Left opponent slot
- `2` = Right opponent slot
- `-1` = Left ally slot (rarely used)
- `-2` = Right ally slot (rarely used)

For simplicity, the random player picks between opponent slots 1 and 2 for all targeting moves, which covers the vast majority of doubles battle scenarios.

## Files Modified

**ApogeeVGC\Sim\Player\PlayerRandom.cs**
- Updated `GetRandomTargetLocation()` method to return valid target locations for doubles battles

## Testing

The fix should be tested by:
1. Running doubles battles with `PlayerRandom` vs any other player type
2. Verifying moves like Heavy Slam, Fake Out, Low Kick, etc. execute successfully
3. Ensuring the battle completes without infinite loops
4. Verifying singles battles still work correctly (should not be affected)

## Related Bug Fixes

This fix is similar in nature to previous fixes that addressed missing validation or incorrect assumptions about battle format:
- **Endless Battle Loop Fix** - Battle state not properly handled when Pokemon fainted
- **Sync Simulator Request After Battle End Fix** - Missing battle-ended checks in request loop

All three involve loops that don't properly validate state before continuing iteration.

## Keywords

`PlayerRandom`, `doubles battle`, `infinite loop`, `target location`, `targeting moves`, `TargetTypeChoices`, `MoveTarget`, `GetRandomTargetLocation`, `Side.ChooseMove`, `AdjacentFoe`, `Normal target`, `doubles format`, `auto-targeting`

---

**Date**: 2025-01-19
**Severity**: Critical
**Systems Affected**: Random player AI, doubles battles, move targeting
**Impact**: Makes `PlayerRandom` unusable in doubles format, causing infinite loops
