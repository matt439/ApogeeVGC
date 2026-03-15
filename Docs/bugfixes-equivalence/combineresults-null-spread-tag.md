# CombineResults Not Matching JS Null Semantics Causing [spread] Tag Issues

**Commit:** `57b1fe23`
**Date:** 2026-03-12

## Problem
Blocked targets in spread moves retained their `[spread]` damage tag when they should have been removed from the spread list. For example, when a boost was blocked by Clear Amulet, the target was still treated as if the move "didn't try to do anything" rather than "tried and failed".

## Root Cause
In JavaScript, `null` is semantically distinct from `undefined`: `undefined` means "nothing was attempted" while `null` means "something was attempted but produced no result". When `CombineResults(undefined, null)` was called (e.g., a boost blocked by Clear Amulet returning null), C# had no representation for JS null in the `BoolIntUndefinedUnion` type and returned `undefined`. This caused the downstream "didn't try" check to evaluate to `true`, keeping the target in the spread list. Additionally, the `HitStepMoveHitLoop` break check used pattern matching on `false` instead of Showdown's `!x.some(v => v || v === 0)` condition.

## Fix
Added `NullBoolIntUndefinedUnion` as a new union case representing JS null (falsy like undefined, but semantically distinct). Updated `CombineResults` to handle null inputs with proper JS null propagation rules. Fixed `HitStepMoveHitLoop` to use `IsTruthy()` and `IsZero()` methods matching Showdown's truthiness check.

## Files Changed
- `ApogeeVGC/Sim/Utils/Unions/BoolIntUndefinedUnion.cs` -- Added `NullBoolIntUndefinedUnion` record representing JS null
- `ApogeeVGC/Sim/BattleClasses/BattleActions.ResultCombining.cs` -- Rewrote `CombineResults` with JS null handling and propagation rules
- `ApogeeVGC/Sim/BattleClasses/BattleActions.HitSteps.cs` -- Fixed move hit loop break condition to use `IsTruthy()`/`IsZero()`
- `ApogeeVGC/Sim/Utils/Unions/BoolIntEmptyUndefinedUnion.cs` -- Handle `NullBoolIntUndefinedUnion` in conversion method

## Pattern
JS null vs. undefined distinction: C# lacked a representation for JS null as a distinct falsy value from undefined, causing incorrect "didn't try" detection in spread move result combining.
