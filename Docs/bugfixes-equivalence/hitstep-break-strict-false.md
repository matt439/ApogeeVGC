# hitStepMoveHitLoop Break Check Using Loose Instead of Strict False Equality

**Commit:** `6870b4edc10babe5ca2a6c95039356a9aca55468`
**Date:** 2026-03-12

## Problem
The `hitStepMoveHitLoop` break check was using `val.IsTruthy() || val.IsZero()` which incorrectly treated `null`, `undefined`, and NOT_FAIL values as `false`, causing premature loop exit and missing `eachEvent(Update)` PRNG calls for status moves like Heal Pulse.

## Root Cause
Showdown's actual check is `if (!moveDamage.some(val => val !== false)) break;` -- only literal boolean `false` (strict equality) should keep an entry from triggering the break. The C# implementation used a truthiness check (`IsTruthy() || IsZero()`), which is the inverse of JS loose falsiness. Values like `null`, `undefined`, and NOT_FAIL (empty string) are JS-falsy but are not `=== false`, so they should NOT trigger the break. The overly broad check caused the loop to exit early when all targets had non-false-but-falsy results, skipping subsequent `eachEvent(Update)` calls that consume PRNG values.

## Fix
Changed the break condition to check for strict boolean false: `val is not BoolBoolIntUndefinedUnion { Value: false }`, matching Showdown's `val !== false` strict equality check.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.HitSteps.cs` -- Changed break condition from truthiness/zero check to strict false equality check

## Pattern
Using JS loose falsiness semantics where Showdown uses strict equality (`!== false`), causing over-broad matching.
