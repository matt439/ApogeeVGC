# Dancer Ability effectOrder Tiebreaker Sort Direction

## Summary

The Dancer ability sorts tied-speed Pokemon by `abilityState.effectOrder`, but the C# sort was ascending (oldest first) while Showdown sorts descending (most recent first). This caused the wrong Dancer to activate first when multiple dancers had equal speed stats.

## Root Cause

When multiple Pokemon with the Dancer ability have equal speed, Showdown breaks ties using `effectOrder` in descending order (most recently switched-in first). The C# implementation sorted ascending, reversing the tiebreak.

## Fix

Reversed the sort comparison for `effectOrder` in the Dancer activation logic to sort descending, matching Showdown's behavior.

## Commit

`080b9a53` — Fix Dancer ability effectOrder tiebreaker sort direction

## Files Changed

- `ApogeeVGC/Sim/BattleClasses/BattleActions.Moves.cs`
