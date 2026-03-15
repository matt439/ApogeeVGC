# Frozen Cure Message ToString Dump

**Commit:** `b4712b6f`
**Date:** 2026-03-12

## Problem
When a frozen Pokemon used a defrosting move (e.g., Scald, Flare Blitz), the `-curestatus` protocol message included the move's `ToString()` representation (e.g., `move: ActiveMove { ... }`) instead of the move's proper name, causing a protocol mismatch with Showdown.

## Root Cause
The frozen cure handler in `ConditionsDEF.cs` used C# string interpolation `$"[from] move: {move}"`, which calls `ToString()` on the `ActiveMove` object. Showdown emits `[from] move: <MoveName>` using the move's display name.

## Fix
Changed the interpolation to use `move.FullName` (e.g., `"move: Scald"`) instead of `ToString()`, matching Showdown's protocol output. Also improved batch test diagnostics to show context lines around mismatches.

## Files Changed
- `ApogeeVGC/Data/Conditions/ConditionsDEF.cs` — Use `move.FullName` instead of `ToString()` in `-curestatus` message
- `ApogeeVGC/Sim/BattleClasses/Battle.Modifiers.cs` — Comment clarifications for `modify()` rounding
- `ApogeeVGC/Sim/Core/Driver.EquivalenceBatch.cs` — Add 3-line context window around first mismatch in diagnostics

## Pattern
Implicit `ToString()` in C# string interpolation producing debug dump output instead of the protocol-formatted name that Showdown expects. Always use `.FullName` or `.Name` for protocol messages.
