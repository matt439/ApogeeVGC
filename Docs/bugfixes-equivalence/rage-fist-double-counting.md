# Rage Fist Double-Counting TimesAttacked

**Commit:** `bc73ffc9`
**Date:** 2026-03-12

## Problem
Rage Fist's base power was higher than expected because the `TimesAttacked` counter was being incremented twice per hit -- once in `Pokemon.GotAttacked()` and once in `BattleActions.HitSteps` (the `applyDamage` equivalent).

## Root Cause
In Showdown, `timesAttacked` is only incremented in `battle-actions.ts` (inside the damage application step), not in `pokemon.ts`'s `gotAttacked()`. The C# port had an extra `TimesAttacked++` in `GotAttacked()`, causing double-counting.

## Fix
Removed the `TimesAttacked++` from `Pokemon.GotAttacked()` and added a comment clarifying that the increment happens in `BattleActions.HitSteps`, matching the Showdown code location.

## Files Changed
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Combat.cs` — Remove duplicate `TimesAttacked` increment from `GotAttacked()`

## Pattern
Duplicate state mutation placed in two different locations during the JS-to-C# port. When Showdown splits logic across files, the C# port must be careful not to replicate the effect in both the equivalent method and the calling code.
