# Natural Cure Logging Enum Name Instead of Protocol Status ID

**Commit:** `97618e9a`
**Date:** 2026-03-12

## Problem
Natural Cure's cure message logged the C# enum name (e.g., `"Burn"`, `"Paralysis"`) instead of the Showdown protocol status ID (e.g., `"brn"`, `"par"`), causing protocol output to diverge in equivalence tests.

## Root Cause
The code used `pokemon.Status.ToString()` which produces the C# enum member name. Showdown uses abbreviated protocol IDs for status conditions in its protocol messages (brn, par, psn, tox, slp, frz).

## Fix
Added a switch expression mapping each `ConditionId` to its Showdown protocol abbreviation, replacing the `ToString()` call.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesMNO.cs` -- map ConditionId enum to protocol status strings for Natural Cure

## Pattern
Enum name vs protocol ID mismatch: C# enums produce human-readable names via ToString(), but Showdown protocol uses abbreviated IDs. Any protocol-facing status output needs explicit mapping.
