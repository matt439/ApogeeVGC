# Multi-Accuracy Truncation Losing Fractional Accuracy

**Date:** 2026-03-14

## Problem
Multi-hit moves with `multiaccuracy` (Triple Axel, Triple Kick, Population Bomb) would sometimes miss a hit that Showdown allows. The accuracy check on subsequent hits (hit > 1) diverged from Showdown, causing PRNG desync and different hit counts.

## Root Cause
In the multi-accuracy code path in `BattleActions.HitSteps`, after applying accuracy/evasion boost multipliers (which use floating-point `boostTable` values like `4/3`, `5/3`, etc.), the accuracy was truncated to an integer:

```csharp
accuracy = IntTrueUnion.FromInt((int)accValue);
```

For example, Triple Axel with base accuracy 90 and a +1 accuracy boost gives `90 * (4/3) = 67.5` (after evasion). C# truncated this to 67.

Showdown keeps accuracy as a float throughout the boost calculation and only converts to integer for the `randomChance(accuracy, 100)` call, where `random(100) < 67.5` passes for a random value of 67. With C#'s truncation to 67, `random(100) < 67` fails for value 67.

## Fix
Changed the truncation to use `Math.Ceiling` instead of simple cast:

```csharp
accuracy = IntTrueUnion.FromInt((int)Math.Ceiling(accValue));
```

This rounds 67.5 up to 68, which correctly passes the `RandomChance(68, 100)` check for value 67, matching Showdown's behavior.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.HitSteps.cs` — Use `Math.Ceiling` for multi-accuracy truncation

## Impact
Affects any battle where a multi-accuracy move (Triple Axel, Triple Kick, Population Bomb) is used with non-zero accuracy or evasion boosts that produce a fractional accuracy value.

## Pattern
When converting Showdown's float-based comparisons to C#'s integer-based comparisons, use `Math.Ceiling` rather than truncation to preserve the "less than" semantics: `random < 67.5` (float) is equivalent to `random < 68` (int).
