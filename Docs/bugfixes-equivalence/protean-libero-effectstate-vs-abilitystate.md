# Protean/Libero Using Event-scoped EffectState Instead of Persistent AbilityState

**Commit:** `ab220372`
**Date:** 2026-03-12

## Problem
Protean and Libero checked and set their "already activated this switch-in" flag on `battle.EffectState` (the event-scoped state) instead of `source.AbilityState` (the persistent per-Pokemon ability state). This meant the flag was lost after the event finished, allowing the ability to activate multiple times per switch-in.

## Root Cause
In Showdown, `this.effectData` inside an ability handler refers to the Pokemon's persistent ability state. In the C# port, `battle.EffectState` is event-scoped and does not persist between events. The correct C# equivalent is `source.AbilityState`.

## Fix
Changed both Protean and Libero to read and write their activation flag (`Protean`/`Libero`) on `source.AbilityState` instead of `battle.EffectState`.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesJKL.cs` -- Libero: use source.AbilityState instead of battle.EffectState
- `ApogeeVGC/Data/Abilities/AbilitiesPQR.cs` -- Protean: use source.AbilityState instead of battle.EffectState

## Pattern
EffectState scope mismatch: Showdown's `this.effectData` in ability handlers is persistent per-Pokemon state, not event-scoped. In C#, use `pokemon.AbilityState` for persistent ability state, not `battle.EffectState`.
