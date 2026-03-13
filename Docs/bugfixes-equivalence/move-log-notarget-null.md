# Move Log Showing Pokemon Name Instead of 'null' for No-Target Moves

**Commit:** `8b83366d`
**Date:** 2026-03-12

## Problem
When a move had no valid target (e.g., Helping Hand with no ally present), the battle log displayed the user's own name as the target instead of the string `"null"`, causing the protocol output to diverge from Showdown.

## Root Cause
The move log line used C#'s null-coalescing operator: `target?.ToString() ?? pokemon.ToString()`. When `target` was null (no valid target), this fell back to `pokemon.ToString()`, outputting the user's name. Showdown's protocol outputs the literal string `"null"` as the target in `[notarget]` scenarios.

## Fix
Changed the fallback from `pokemon.ToString()` to the string literal `"null"` so the log output matches Showdown's protocol exactly.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Moves.cs` -- Changed null-coalescing fallback from pokemon name to string "null" in move log

## Pattern
Incorrect null-coalescing fallback producing wrong protocol output string.
