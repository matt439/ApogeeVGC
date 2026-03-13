# PrepareHit Events Not Short-Circuiting When Try Event Fails

**Commit:** `61866047a15883d95fc05742deb082168d22dbdb`
**Date:** 2026-03-12

## Problem
The Try/PrepareHit event chain was evaluating all three events unconditionally, causing abilities like Protean to incorrectly activate on failed moves (e.g. when Sucker Punch fails because the target is not attacking).

## Root Cause
Showdown evaluates these events with JS `&&` chaining: `hitResult = singleEvent('Try') && singleEvent('PrepareHit') && runEvent('PrepareHit')`. If `Try` returns false, the `&&` short-circuits and `PrepareHit` never fires. The C# code was calling all three events unconditionally and then combining the results at the end. This meant `PrepareHit` (which triggers abilities like Protean) would fire even when `Try` had already returned false. Additionally, the NOT_FAIL return check was incorrectly checking short-circuit-null values instead of identifying which event actually failed.

## Fix
Added explicit short-circuit evaluation: `PrepareHit` events are only called if the preceding event passed. The NOT_FAIL return now correctly identifies which specific event in the chain failed.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveHit.cs` -- Added short-circuit logic with `tryPassed`/`preparePassed` flags and corrected NOT_FAIL identification

## Pattern
Missing short-circuit evaluation semantics when translating JS `&&` chains to sequential C# method calls.
