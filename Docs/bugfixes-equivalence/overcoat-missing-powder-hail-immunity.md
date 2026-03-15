# Overcoat Missing Powder and Hail/Snow Immunity

**Date:** 2026-03-13

## Problem
Overcoat ability's `OnImmunity` handler only checked for Sandstorm immunity, missing both Hail/Snow and Powder immunity checks. This caused Effect Spore to incorrectly trigger on Pokemon with Overcoat (e.g., Mandibuzz), applying sleep status when Showdown correctly blocks it.

## Root Cause
The C# Overcoat implementation was incomplete — it only had `ConditionId.Sandstorm` in its `OnImmunity` handler. Showdown's implementation checks three conditions: `sandstorm`, `hail`, and `powder`.

In the failing test (seed 2732), Mandibuzz with Overcoat used a contact move against Vileplume with Effect Spore. Effect Spore checks `source.RunStatusImmunity('powder')` before applying status. Since C# Overcoat didn't return `false` for the Powder immunity check, Effect Spore incorrectly triggered.

## Fix
Added `ConditionId.Snowscape` (C# equivalent of Showdown's `hail`) and `ConditionId.Powder` to the Overcoat `OnImmunity` handler's condition check.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesMNO.cs` -- added Snowscape and Powder to Overcoat's OnImmunity check

## Pattern
Incomplete ability immunity checks: When porting abilities with multiple immunity conditions, all conditions must be included. The Safety Goggles item had the correct pattern (checking both Sandstorm and Powder) that should have been mirrored.
