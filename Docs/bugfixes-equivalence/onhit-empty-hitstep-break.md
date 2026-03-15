# OnHit NOT_FAIL (Empty) Mapping Causing hitStepMoveHitLoop Premature Break

**Commit:** `a7c1a2bb1ceb608a267c47f25913b2e6057faf70`
**Date:** 2026-03-12

## Problem
When an `onHit` handler returned NOT_FAIL (Showdown's empty string `''`), the result was mapped to `NullRelayVar`, which got converted to `false` in `RunMoveEffects`. This made the damage array contain `[false]`, triggering the break check in `hitStepMoveHitLoop` and skipping 2 `eachEvent('Update')` PRNG calls.

## Root Cause
`OnHitEventInfo` mapped `EmptyBoolEmptyVoidUnion` (NOT_FAIL) to `NullRelayVar`. In `RunMoveEffects`, `NullRelayVar` was converted to `null` via the `null->false` path, producing `damage[i] = false`. The `hitStepMoveHitLoop` break check then saw all damage entries as false and exited the loop early, skipping subsequent `eachEvent(Update)` calls. The correct behavior is that NOT_FAIL should keep the damage entry non-false, since the move did not truly fail -- it simply had no effect (e.g. Shore Up or Heal Pulse at full HP).

## Fix
Changed the `EmptyBoolEmptyVoidUnion` mapping in `OnHitEventInfo` from `NullRelayVar` to C# `null`, so `SingleEvent` returns the default relay variable (`true`), keeping the damage entry non-false.

## Files Changed
- `ApogeeVGC/Sim/Events/Handlers/MoveEventMethods/OnHitEventInfo.cs` -- Changed NOT_FAIL mapping from `new NullRelayVar()` to `null` (passthrough to default)

## Pattern
Incorrect mapping of NOT_FAIL to NullRelayVar causing false-negative damage tracking and premature loop termination.
