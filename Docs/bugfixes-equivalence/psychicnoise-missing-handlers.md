# PsychicNoise Missing OnDisableMove and OnModifyMove Handlers

**Commit:** `c669af9a` + `1e30d009`
**Date:** 2026-03-13

## Problem
The PsychicNoise (Heal Block) condition was missing two event handlers that Showdown's healblock condition defines: `OnDisableMove` and `OnModifyMove`. This caused PRNG divergence whenever a Pokemon had Heal Block active alongside other volatiles that register handlers for the same events.

## Root Cause
Showdown's healblock condition has three move-blocking handlers: `onDisableMove` (marks heal-flag moves as disabled in the request), `onBeforeMove` (blocks heal-flag moves at execution time), and `onModifyMove` (blocks heal-flag moves at the modify stage). The C# PsychicNoise condition only implemented `OnBeforeMove`. When `RunEvent(DisableMove)` or `RunEvent(ModifyMove)` collected handlers, it found fewer than Showdown, meaning no speed-tie shuffle occurred when multiple tied handlers existed (e.g., Encore + Heal Block for DisableMove, or ThroatChop + Heal Block for ModifyMove). This 1-position PRNG shift cascaded into different damage rolls downstream.

## Fix
Added the missing `OnDisableMove` handler that iterates the Pokemon's move slots and disables any with the `Heal` flag, and the missing `OnModifyMove` handler that returns `false` (blocking the move) when `move.Flags.Heal == true`.

## Files Changed
- `ApogeeVGC/Data/Conditions/ConditionsPQR.cs` -- Added OnDisableMove handler to disable heal-flag moves in request, and OnModifyMove handler to block heal-flag moves at the modify stage

## Pattern
Missing event handler causing incorrect RunEvent handler count and PRNG speed-sort divergence.
