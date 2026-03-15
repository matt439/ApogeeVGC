# NOT_FAIL (Empty) from OnHit Returning True Instead of EmptyRelayVar

**Commit:** `bd15d2cae2f3a40c8ce88059a4ed8bfcc51a893e`
**Date:** 2026-03-13

## Problem
When an `onHit` handler returned NOT_FAIL (Showdown's empty string `''`), the C# code mapped it to `null`, which caused `SingleEvent` to return the default `relayVar` (`true`). This made `RunMoveEffects` treat NOT_FAIL as success, preventing `hitStepMoveHitLoop` from breaking when it should (e.g. Baton Pass with no allies to switch to).

## Root Cause
`OnHitEventInfo` mapped `EmptyBoolEmptyVoidUnion` (NOT_FAIL) to C# `null`. In the event system, a `null` handler return means "no opinion / passthrough," so `SingleEvent` returned the default relay variable (`true`). NOT_FAIL has JS type priority 1 (`'string'`), between `undefined` (0) and `null` (2), and should be overridden by boolean `false` in `combineResults` (e.g. Baton Pass fail combined with selfSwitch). By promoting NOT_FAIL to `true`, the `combineResults` logic could never produce the `false` needed to break the hit loop.

## Fix
Changed `OnHit`, `OnHitField`, and `OnHitSide` EventInfo handlers to return `EmptyRelayVar` for NOT_FAIL instead of `null`. Added detection in `RunMoveEffects` to skip the "didn't try -> true" promotion when `onHit` returned NOT_FAIL, preserving the falsy semantics needed for `combineResults`.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveEffects.cs` -- Added `onHitReturnedNotFail` flag and EmptyRelayVar detection across all onHit/onHitSide/onHitField paths
- `ApogeeVGC/Sim/Events/Handlers/MoveEventMethods/OnHitEventInfo.cs` -- Changed NOT_FAIL mapping from `null` to `new EmptyRelayVar()`
- `ApogeeVGC/Sim/Events/Handlers/MoveEventMethods/OnHitFieldEventInfo.cs` -- Changed NOT_FAIL mapping from `NullRelayVar` to `EmptyRelayVar`
- `ApogeeVGC/Sim/Events/Handlers/MoveEventMethods/OnHitSideEventInfo.cs` -- Changed NOT_FAIL mapping from `NullRelayVar` to `EmptyRelayVar`

## Pattern
Incorrect mapping of Showdown's NOT_FAIL sentinel value losing its distinct semantics in the C# type system.
