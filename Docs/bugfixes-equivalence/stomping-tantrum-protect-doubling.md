# Stomping Tantrum Incorrectly Doubling After Protect

**Commit:** `5a74653b`
**Date:** 2026-03-12

## Problem
Stomping Tantrum was incorrectly doubling its base power after the user's previous move was blocked by Protect. In Showdown, a Protect block does not count as a "failed move" for Stomping Tantrum's purposes.

## Root Cause
The `UseMove` failure path (when a move is blocked by protection) was explicitly setting `pokemon.MoveThisTurnResult = false`. This caused Stomping Tantrum to see the previous turn as a failure and double its power. In Showdown, the `moveThisTurnResult` is not set to false in this code path; it remains unset, which Stomping Tantrum does not treat as a failure.

## Fix
Removed the `pokemon.MoveThisTurnResult = false` assignment from the protection-blocked failure path in `UseMove`.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Moves.cs` — removed spurious MoveThisTurnResult assignment

## Pattern
Over-eager state mutation: Adding extra state updates not present in Showdown's code can cause downstream effects in other moves/abilities that read that state. Only set state that Showdown explicitly sets in the same code path.
