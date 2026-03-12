# Shared SecondaryEffect Mutation Causing Cross-Battle State Leakage

**Commit:** `20eb383e`
**Date:** 2026-03-11

## Problem
Running multiple battles (e.g., in batch equivalence testing) caused incorrect secondary effect chances. For example, Serene Grace would double a move's secondary effect chance from 30% to 60%, but this mutation persisted into subsequent battles, starting them at 60% and doubling again to 120%.

## Root Cause
`ActiveMove` shared references to the `Secondaries` array and `Self` effect objects from the library's immutable move templates. When game logic mutated these in-place (e.g., Serene Grace doubling `Chance` values), the mutations leaked back to the template, affecting all future battles that used the same move.

## Fix
Deep-copy `Secondaries` (using `s with { }` for each element) and `Self` at three points: the `ActiveMove` constructor, the `Reset()` method, and `ShallowClone()`. This ensures each battle gets independent copies of secondary effect data that can be safely mutated.

## Files Changed
- `ApogeeVGC/Sim/Moves/ActiveMove.cs` — Deep-copy `Secondaries` and `Self` in constructor, `Reset()`, and `ShallowClone()`

## Pattern
Shared mutable state from immutable library templates. When game logic mutates properties of objects obtained from a static data library, those objects must be defensively copied to prevent cross-battle state leakage. This is a classic aliasing bug.
