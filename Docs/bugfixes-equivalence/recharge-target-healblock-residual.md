# Recharge Pseudo-Move Target Type and HealBlock Residual Ordering

**Commit:** `cee7667d`
**Date:** 2026-03-12

## Problem
The Recharge pseudo-move was not consuming a PRNG call for random foe targeting, and the HealBlock condition was missing a residual order value, both causing PRNG stream divergence from Showdown.

## Root Cause
In Showdown, the "recharge" pseudo-move has no explicit target type (undefined), which causes `getTarget` to fall through to `getRandomTarget` then `randomFoe()`, consuming one PRNG call. The C# implementation had `Target = MoveTarget.Self`, which skips target resolution entirely and consumes no PRNG. Separately, Showdown's healblock condition has `onResidualOrder: 20`, which affects the sort order of residual handlers. The C# HealBlock condition had no `OnResidual` handler at all, so it was absent from the residual sort, changing the order in which tied handlers were processed.

## Fix
Changed Recharge's target from `MoveTarget.Self` to `MoveTarget.None` to trigger the random foe selection path. Added an `OnResidual` handler with `order: 20` (empty callback) to HealBlock so it participates in residual sorting at the correct position.

## Files Changed
- `ApogeeVGC/Data/Moves/MovesPQR.cs` -- Changed Recharge target from MoveTarget.Self to MoveTarget.None
- `ApogeeVGC/Data/Conditions/ConditionsPQR.cs` -- Added OnResidual handler with order=20 to HealBlock condition
- `ApogeeVGC/Program.cs` -- Improved CLI argument parsing to accept bare mode names
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Damage.cs` -- Removed extraneous blank lines
- `ApogeeVGC/Sim/Core/Driver.EquivalenceBatch.cs` -- Increased batch test count to 500, added TRACE_SEED env var support
- `ApogeeVGC/Sim/Utils/Prng.cs` -- Added TraceEnabled property with stack-trace logging for PRNG debugging

## Pattern
Incorrect move target type and missing residual handler registration causing PRNG call count divergence.
