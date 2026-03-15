# Confusion Self-Hit Damage Blocked by Magic Guard Due to Wrong EffectType

**Commit:** `aa428c23`
**Date:** 2026-03-12

## Problem
Confusion self-hit damage was incorrectly blocked by Magic Guard. Magic Guard prevents all non-Move indirect damage, but confusion self-hits should bypass it because Showdown treats them as pseudo-move damage.

## Root Cause
Showdown creates a fake `ActiveMove` object `{ id: 'confused', effectType: 'Move' }` for confusion self-damage, giving it `EffectType.Move` so that Magic Guard's check (which only blocks non-Move damage) does not interfere. The C# implementation was passing the actual Confusion `Condition` object (with `EffectType.Condition`) to the `Damage()` call, causing Magic Guard to recognize it as indirect damage and block it entirely.

## Fix
Created a `PseudoMoveEffect` class with `EffectType.Move` and a static `Confused` instance. The confusion residual handler now passes `PseudoMoveEffect.Confused` instead of the Confusion condition. Updated `PrintDamageMessage` to recognize the pseudo-move and still emit the `[from] confusion` attribution tag.

## Files Changed
- `ApogeeVGC/Data/Conditions/ConditionsABC.cs` -- Pass `PseudoMoveEffect.Confused` instead of the Confusion condition to `Damage()`
- `ApogeeVGC/Sim/BattleClasses/Battle.Logging.cs` -- Handle `PseudoMoveEffect { Id: "confused" }` in damage message formatting
- `ApogeeVGC/Sim/Effects/PseudoMoveEffect.cs` -- New lightweight IEffect with `EffectType.Move` for fake move attribution

## Pattern
EffectType misclassification: using the real condition's EffectType instead of mimicking Showdown's fake ActiveMove object for damage source attribution.
