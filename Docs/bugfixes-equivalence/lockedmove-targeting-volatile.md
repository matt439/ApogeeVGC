# Lockedmove Targeting Using Wrong Volatile and Metronome Check

**Commit:** `dfac7d97`
**Date:** 2026-03-13

## Problem
Two-turn moves (Shadow Force, Fly, etc.) were resolving their locked-move target location from the wrong volatile, and the Metronome branch in `TwoTurnMove.OnStart` was triggering incorrectly for non-Metronome sources, causing the target to be recalculated instead of using the stored value.

## Root Cause
In `Side.Choices.cs`, the code looked up the target location from `ConditionId.TwoTurnMove` (the generic volatile), but Showdown reads it from `pokemon.volatiles[lockedMoveID]` -- the move-specific volatile (e.g., `ConditionId.ShadowForce`). Since the target location is stored on the move-specific volatile, reading from `TwoTurnMove` returned no stored target. Separately, in `ConditionsSTU.cs`, the `TwoTurnMove.OnStart` Metronome branch checked `SourceEffect: not null`, but the format effect also has a non-null `SourceEffect`, incorrectly triggering the Metronome code path and recalculating the target from the defender parameter instead of using `LastMoveTargetLoc`.

## Fix
Changed `Side.Choices.cs` to parse the locked move ID to a `ConditionId` and look up the target from that move-specific volatile. Changed the Metronome branch condition from `SourceEffect: not null` to `SourceEffect: MoveEffectStateId` to only match when a move calls another move (the actual Metronome pattern).

## Files Changed
- `ApogeeVGC/Sim/SideClasses/Side.Choices.cs` -- Look up target location from move-specific volatile instead of TwoTurnMove
- `ApogeeVGC/Data/Conditions/ConditionsSTU.cs` -- Narrow Metronome branch check to require MoveEffectStateId source effect type

## Pattern
Wrong volatile key lookup and overly broad type check causing incorrect targeting resolution.
