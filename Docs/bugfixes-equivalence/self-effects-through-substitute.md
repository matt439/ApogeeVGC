# Self Effects Skipped When Hitting Substitute

**Commit:** `8b1965a4`
**Date:** 2026-03-12

## Problem
Self-targeting secondary effects (e.g., Close Combat's Defense and Special Defense drops on the user) were not applied when the move hit a Substitute. This diverged from Showdown, where self effects always fire regardless of Substitute.

## Root Cause
When a Substitute absorbed a hit, the target was set to `PokemonFalseUnion.FromFalse()` (equivalent to `false`). The self-effects loop skipped all non-Pokemon targets with `is not PokemonPokemonUnion`. In Showdown, substitute-absorbed targets are set to `null` (not `false`), and the self-drops loop checks `target === false`, so `null` targets still allow self effects to proceed.

## Fix
Introduced a `NullPokemonUnion` type to represent Showdown's `null` target (distinct from `FalsePokemonUnion`). Changed the substitute-absorbed assignment to use `NullPokemonUnion` instead of `FalsePokemonUnion`. Updated skip conditions in self-effects and self-drops loops to only skip `FalsePokemonUnion`.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveHit.cs` — use NullPokemonUnion for substitute hits
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveEffects.cs` — change skip from `not PokemonPokemonUnion` to `is FalsePokemonUnion`
- `ApogeeVGC/Sim/Utils/Unions/PokemonFalseUnion.cs` — add NullPokemonUnion record type

## Pattern
JS null vs false distinction: Showdown uses `null` and `false` as semantically different sentinel values in arrays. C# union types must model both distinctly, or code paths that check `=== false` will incorrectly match null cases.
