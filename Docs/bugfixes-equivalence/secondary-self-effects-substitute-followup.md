# Secondary Self Effects Not Firing When Substitute Absorbs Hit (Follow-Up)

**Commit:** `d6b778ce`
**Date:** 2026-03-13

## Problem
When a move with a self-targeting secondary effect (e.g., Aqua Step's speed boost) hit a Substitute, the secondary self effect was not applied to the attacker.

## Root Cause
When a move hits a Substitute, the target becomes `null` in the Secondaries processing method. The code skipped the `MoveHit` call entirely when `target` was `null`, which prevented all secondary effects from firing -- including self-targeting ones that do not depend on the target. In Showdown, `moveHit(null, ...)` is still called, and `spreadMoveHit` skips the damage/hit steps for the null target but still processes `selfDrops` from the secondary's `.self` property.

## Fix
Added an `else if` branch that fires when `target` is null but the secondary has a `.Self` property: it calls `MoveHit` with `source` as both target and source using the secondary's self effect, matching Showdown's `selfDrops` processing path.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveEffects.cs` -- Added null-target branch in Secondaries to process self-targeting effects when Substitute absorbs the hit

## Pattern
Null-target short-circuit skipping self-targeting secondary effects that should still apply to the attacker.
