# [from] sourceEffect Check Using EffectType Instead of Reference Equality

**Commit:** `e76b66e1`
**Date:** 2026-03-11

## Problem
The `[from]` attribution tag on move usage messages was sometimes missing (e.g., `[from] ability: Dancer` not appearing) or incorrectly present, causing protocol mismatches with Showdown.

## Root Cause
The code checked `sourceEffect != Battle.Effect` using reference equality to skip the default format effect. However, during ability handler dispatch, `Battle.Effect` is temporarily set to the ability being processed, so the comparison against the format effect would succeed even when it should not, incorrectly adding `[from]` tags. Showdown checks `!(sourceEffect as Format)?.effectType` -- i.e., it skips Format-type effects by type, not by identity.

## Fix
Changed the check from reference equality (`sourceEffect != Battle.Effect`) to a type check (`sourceEffect.EffectType != EffectType.Format`), which correctly filters out the format effect regardless of what `Battle.Effect` is currently set to.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Moves.cs` — Replace reference equality with `EffectType` check for `[from]` attribution

## Pattern
Reference equality vs. type-based filtering. In Showdown, the `Battle.effect` reference is mutated during event dispatch, so comparisons against it are unreliable. Type-based checks are more robust.
