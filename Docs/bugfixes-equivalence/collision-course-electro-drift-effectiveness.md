# Collision Course / Electro Drift Super-Effective Check

**Commit:** `4963ec43`
**Date:** 2026-03-12

## Problem
Collision Course and Electro Drift were not correctly applying their 1.33x base power boost against super-effective targets. The super-effectiveness check was comparing the wrong value, causing the boost to trigger (or not trigger) incorrectly.

## Root Cause
`RunEffectiveness()` returns a raw enum/integer value representing type effectiveness, not the actual damage modifier. The code was comparing this raw value with `> 0`, but the threshold for super-effectiveness depends on the modifier scale, not the raw enum. The `.ToModifier()` conversion was missing, so the comparison was against the enum's underlying integer instead of the actual effectiveness multiplier.

## Fix
Added `.ToModifier()` calls to convert the effectiveness result to its actual modifier value before comparing with `> 0` (Collision Course) and `<= 0` (Electro Drift).

## Files Changed
- `ApogeeVGC/Data/Moves/MovesABC.cs` — add `.ToModifier()` to Collision Course effectiveness check
- `ApogeeVGC/Data/Moves/MovesDEF.cs` — add `.ToModifier()` to Electro Drift effectiveness check

## Pattern
Enum value vs semantic value: When a method returns a typed enum or encoded value, comparisons must use the decoded/converted form. Raw enum integers may not have the expected numeric relationship (e.g., "super effective" might not be `> 0` at the raw level).
