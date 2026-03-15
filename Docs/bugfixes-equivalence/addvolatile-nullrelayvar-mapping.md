# AddVolatile NullRelayVar Mapped to Undefined Instead of Null

**Commit:** `cce112436ae8ca474b4ea085be0d8f2004975fdd`
**Date:** 2026-03-13

## Problem
When Aroma Veil blocked a volatile (e.g. Encore), `AddVolatile` returned `NullRelayVar`, but the switch expression in `RunMoveEffects` mapped it to `FromUndefined()` via the wildcard case instead of `null`, causing `didSomething` to stay undefined and get promoted to `true`.

## Root Cause
The switch expression converting `AddVolatile` results to `BoolIntUndefinedUnion` had no explicit case for `NullRelayVar`. It fell through to the wildcard `_` case, which returned `FromUndefined()`. In Showdown, a `null` return has JS `typeof "object"` (priority 2), not `"undefined"` (priority 0). This meant `didSomething` retained its undefined state and was later promoted to `true`, making `damage[i] = true` instead of `false`, which prevented the hit loop from breaking and fired 2 extra `eachEvent(Update)` calls.

## Fix
Added an explicit `NullRelayVar => null` case to the switch expression in `RunMoveEffects`, before the wildcard fallthrough.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveEffects.cs` -- Added NullRelayVar case to AddVolatile result switch expression

## Pattern
Missing discriminated union case in pattern match causing incorrect JS null/undefined semantics mapping.
