# Substitute Target Propagation Missing in Hit Loop

**Date:** 2026-03-14

## Problem
When a multi-hit move (e.g., Rage Fist) struck a Substitute, the `timesAttacked` counter was incorrectly incremented. This caused Rage Fist's base power to diverge from Showdown, producing different damage values and PRNG desync.

## Root Cause
In Showdown's `hitStepMoveHitLoop`, `spreadMoveHit` returns modified targets:
```javascript
[moveDamageThisHit, targetsCopy] = this.spreadMoveHit(targetsCopy, ...)
```

When a Substitute absorbs a hit, `spreadMoveHit` sets `targets[i] = null` (the `HIT_SUBSTITUTE` path). The post-loop `timesAttacked` increment then skips null targets:
```javascript
if (target && pokemon !== target) {
    target.timesAttacked += move.smartTarget ? 1 : hit - 1;
}
```

In C#, the modified targets were discarded:
```csharp
(SpreadMoveDamage moveDamageThisHit, SpreadMoveTargets _) = SpreadMoveHit(...)
```

The original `targetsCopy` was preserved unchanged, so the target was never nullified. The `timesAttacked` loop had no null check and always incremented, even when Substitute absorbed the hit.

## Fix
1. Capture modified targets from `SpreadMoveHit` and update `targetsCopy`:
```csharp
(SpreadMoveDamage moveDamageThisHit, SpreadMoveTargets modifiedTargets) = SpreadMoveHit(...);
targetsCopy.Clear();
for (int ti = 0; ti < modifiedTargets.Count; ti++)
    targetsCopy.Add(modifiedTargets[ti] is PokemonPokemonUnion p ? p.Pokemon : null!);
```

2. Changed `targetsCopy` type from `List<Pokemon>` to `List<Pokemon?>` to allow null entries.

3. Added null check in the `timesAttacked` loop to match Showdown's `if (target && ...)` guard.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.HitSteps.cs` — Propagate modified targets from SpreadMoveHit, allow nullable targets, add null guard

## Impact
Affects any battle where a move hits a Substitute. The target nullification also affects `gotAttacked` tracking, which feeds into mechanics like Rage Fist's base power calculation (50 + 50 * timesAttacked).

## Pattern
When Showdown destructures return values to update local state (`[a, b] = fn(...)`), C# must capture and apply those updates rather than discarding them with `_`.
