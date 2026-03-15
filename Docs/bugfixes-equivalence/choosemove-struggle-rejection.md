# ChooseMove Rejecting Explicit 'move struggle' Choice

**Commit:** `aa512936`
**Date:** 2026-03-12

## Problem
When a Choice-locked Pokemon's locked move had 0 PP, the AI/player would send `"move struggle"` as the choice. The move choice parser rejected this as an invalid move, preventing the battle from proceeding.

## Root Cause
The choice validation had two blocking checks. In Step 4, the parser validated the move ID against the request's available moves list -- but Struggle is never listed in the request since it is a forced fallback move, so it failed the lookup with "doesn't have a move matching struggle". In Step 10, if Step 4 somehow passed, the code checked the filtered moves array for Struggle, which also wasn't present, causing a "disabled" rejection. Showdown treats explicit Struggle choices as valid, bypassing normal move validation.

## Fix
Added a guard to skip request validation in Step 4 when `moveid == MoveId.Struggle`, and extended the Step 9 Struggle handling to also trigger when the player explicitly chose Struggle (not just when the filtered moves array is empty).

## Files Changed
- `ApogeeVGC/Sim/SideClasses/Side.Choices.cs` -- Skip request validation for Struggle in Step 4; treat explicit Struggle choice same as auto-Struggle in Step 9

## Pattern
Move validation rejecting a legitimate forced-move choice that bypasses normal validation in the reference implementation.
