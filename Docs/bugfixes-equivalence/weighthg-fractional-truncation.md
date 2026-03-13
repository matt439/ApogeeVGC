# WeightHg Truncating Fractional Kg Before Multiplying by 10

**Commit:** `59087a15`
**Date:** 2026-03-13

## Problem
Weight-dependent moves like Heavy Slam and Grass Knot used incorrect weight values because fractional kilograms were lost during the hectogram conversion.

## Root Cause
The `WeightHg` property was computed as `(int)WeightKg * 10`. Due to C# operator precedence, the `(int)` cast applied to `WeightKg` first, truncating the fractional part before multiplication. For example, a Pokemon weighing 125.8 kg would be cast to 125, then multiplied to 1250 hg, instead of the correct 1258 hg. This caused weight-based damage calculations to use slightly wrong values.

## Fix
Changed the expression to `(int)(WeightKg * 10)` so the multiplication by 10 happens on the `double` value before the integer truncation.

## Files Changed
- `ApogeeVGC/Sim/SpeciesClasses/Species.cs` -- Fixed `WeightHg` property to parenthesize multiplication before cast

## Pattern
Operator precedence error: cast applied before arithmetic instead of after, causing premature truncation of fractional values.
