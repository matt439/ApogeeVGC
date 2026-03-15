# Secondary Effect PRNG Calls Skipped on Substitute

**Commit:** `f0ac6ec5`
**Date:** 2026-03-12

## Problem
When a Substitute absorbed a hit, secondary effect processing was entirely skipped for that target. This caused PRNG call counts to diverge from Showdown, because Showdown still rolls the random number for secondary effect chances even when the target is null (absorbed by Substitute).

## Root Cause
In Showdown, when a Substitute absorbs a hit, `targets[i]` is set to `null` (not `false`). The secondary effects loop checks `target === false` to skip, so `null` targets still enter the loop body and consume PRNG rolls. The C# code was using a `not PokemonPokemonUnion` check that treated both null and false targets the same way, skipping all non-Pokemon targets.

## Fix
Changed the skip condition from `not PokemonPokemonUnion` to `is FalsePokemonUnion`, allowing `NullPokemonUnion` targets to proceed through the loop. PRNG calls for secondary chance rolls are now consumed even for substitute-absorbed targets, but `MoveHit` is only called when the target is non-null.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveEffects.cs` — change skip condition; guard MoveHit call behind null check; conditionally skip ModifySecondaries event for null targets

## Pattern
JS null vs false distinction: JavaScript distinguishes `null` from `false` in strict equality checks. C# union types that collapse both into a single "not the expected type" check lose this distinction, causing skipped side effects (especially PRNG consumption).
