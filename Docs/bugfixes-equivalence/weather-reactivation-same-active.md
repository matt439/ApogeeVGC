# Weather Re-activation When Same Weather Already Active

**Commit:** `0dc4517d`
**Date:** 2026-03-12

## Problem
When a weather-setting ability activated while the same weather was already active (e.g., Drought triggering when Sun is already up), the C# sim sometimes re-activated the weather instead of silently returning false, causing extra protocol messages that diverged from Showdown.

## Root Cause
The C# code had a special case for ability-sourced weather: if the weather had no duration (permanent weather from an ability), it would allow re-activation. In Gen 6+ Showdown, setting weather that is already active always returns false regardless of the source or duration.

## Fix
Simplified the duplicate-weather check to unconditionally return `false` when the requested weather matches the current weather, removing the ability/duration special case.

## Files Changed
- `ApogeeVGC/Sim/FieldClasses/Field.cs` — Remove ability-specific re-activation logic, always return false for duplicate weather

## Pattern
Over-complicated conditional logic that attempted to handle edge cases not present in the target generation. Gen 9 (and Gen 6+) uses simpler weather rules than earlier generations.
