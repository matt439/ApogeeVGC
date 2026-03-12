# AttrLastMove [still] Missing Empty Target Field

**Commit:** `31dcf8b8`
**Date:** 2026-03-12

## Problem
When `[still]` was appended to a move line that had no target field (only 4 pipe-delimited segments), the protocol output was missing an empty target field at position 4. This caused a protocol format divergence from Showdown.

## Root Cause
In Showdown's JavaScript, `parts[4] = ''` on an array of length 4 automatically extends the array to include index 4. The C# code only handled the case where `parts.Length > 4` (replacing an existing target field) but did not handle the case where the array needed to be extended.

## Fix
Added an `else` branch for when `parts.Length <= 4`: a new empty string is appended to the parts list before re-joining, matching JS's implicit array extension behavior.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/Battle.Logging.cs` — handle short move lines by appending empty target field

## Pattern
JS implicit array extension: JavaScript allows assigning to out-of-bounds array indices, implicitly extending the array. C# arrays/lists do not auto-extend, so index assignments must be replaced with explicit append logic when the index may not exist.
