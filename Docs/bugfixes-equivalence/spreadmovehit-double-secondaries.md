# SpreadMoveHit Processing Secondaries Twice When Called from selfDrops

**Commit:** `5d2f1096`
**Date:** 2026-03-12

## Problem
Moves with both `Self` (self-targeting stat drops) and `Secondaries` (target-affecting secondary effects) were processing their secondary effects twice -- once during the inner `SpreadMoveHit` call from selfDrops and once during the outer call -- resulting in extra PRNG consumption that shifted the random stream for the rest of the battle.

## Root Cause
In Showdown, `spreadMoveHit` checks `moveData.secondaries` where `moveData` is the `hitEffect` parameter passed to the function. When called from the selfDrops code path (`isSelf=true`), the hitEffect is the move's `Self` HitEffect object, which has no `secondaries` property, so Showdown naturally skips secondary processing. In C#, the secondary processing check used `move.Secondaries` (the main move object) rather than the hitEffect parameter. Since the main move always has its secondaries, the check passed for the selfDrops call, causing secondaries to fire from both the inner (self) and outer (normal) invocations.

## Fix
Added `&& !isSelf` to the secondaries processing guard, so secondary effects are only processed during the primary (non-self) `SpreadMoveHit` call and during non-secondary calls, matching Showdown's behavior.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveHit.cs` -- Added !isSelf guard to secondaries processing condition

## Pattern
Checking move-level data instead of hitEffect-level data, causing duplicate effect processing on self-targeting code paths.
