# CombineResults Missing JS Type Priority and moveSucceeded Treating 0 as Failure

**Commit:** `0c925b56`
**Date:** 2026-03-13

## Problem
Two interrelated issues caused incorrect move success/failure detection. First, `CombineResults` was missing Showdown's type priority logic, so expressions like `combineResults(0, false)` returned `false` instead of `0`. Second, the post-loop `moveSucceeded` check treated damage of 0 as failure, preventing moves like U-turn through Disguise (which deals 0 damage) from triggering their `selfSwitch` effect.

## Root Cause
Showdown's `combineResults` uses a `resultsPriorities` array that maps JS `typeof` strings to indices: `['undefined'(0), 'string'(1), 'object'(2), 'boolean'(3), 'number'(4)]`. If the left operand has a higher type priority, it wins regardless of truthiness. The C# implementation omitted this check entirely, believing it was dead code. Additionally, the `moveSucceeded` logic used a simplified boolean check instead of Showdown's exact `!didAnything && didAnything !== 0` condition, which specifically excludes 0 from the failure case.

## Fix
Added `GetJsTypePriority()` methods for both `BoolIntUndefinedUnion` and `BoolIntEmptyUndefinedUnion` that return the correct JS typeof index. Rewrote `CombineResults` to check type priority before truthiness. Rewrote the post-loop move success check to compute `didAnythingFalsy && didAnythingNotZero`, matching Showdown's exact condition. Also fixed stalling move definitions (Protect, Endure, Baneful Bunker, etc.) to use `VolatileStatus = ConditionId.Stall` with the specific protection volatile added in `OnHit`, matching Showdown's pattern.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.ResultCombining.cs` -- Added `GetJsTypePriority()` and rewrote both `CombineResults` overloads with type priority logic
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveEffects.cs` -- Rewrote `moveSucceeded` / `moveFailed` logic to match Showdown's `!didAnything && didAnything !== 0`
- `ApogeeVGC/Data/Moves/MovesABC.cs` -- Fixed Baneful Bunker and Burning Bulwark volatile/stall ordering
- `ApogeeVGC/Data/Moves/MovesDEF.cs` -- Fixed Detect and Endure volatile/stall ordering
- `ApogeeVGC/Data/Moves/MovesPQR.cs` -- Fixed Protect volatile/stall ordering
- `ApogeeVGC/Data/Moves/MovesSTU.cs` -- Fixed Silk Trap and Spiky Shield volatile/stall ordering
- `ApogeeVGC/Data/Conditions/ConditionsSTU.cs` -- Cleaned up Stall condition debug logging
- `ApogeeVGC/Sim/BattleClasses/Battle.Events.cs` -- Added EachEvent speed trace logging
- `ApogeeVGC/Sim/Core/Driver.EquivalenceBatch.cs` -- Increased batch test count to 2000
- `ApogeeVGC/Sim/Core/Driver.EquivalenceTest.cs` -- Enabled PRNG tracing for equivalence tests
- `ApogeeVGC/Sim/Core/Driver.cs` -- Fixed equivalence test file extension
- `Tools/EquivalenceTest/run_showdown_traced.js` -- Enhanced stall/RNG tracing in Showdown runner

## Pattern
JS semantic gap: Showdown's `combineResults` relies on JS `typeof` ordering and falsy-but-not-zero distinction, both of which required explicit modeling in the C# type system.
