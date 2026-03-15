# Berserk Ability Fails to Block Berry Consumption on Confusion Self-Hit

**Date:** 2026-03-14

## Problem
When a Pokemon with Berserk ability and Sitrus Berry took confusion self-damage dropping below half HP, C# consumed the Sitrus Berry immediately. Showdown correctly delayed the berry consumption because Berserk's `onTryEatItem` handler blocks healing berries until `onAfterMoveSecondary` fires.

## Root Cause
Berserk's `OnDamage` handler in C# used `effect is not ActiveMove` to determine whether the damage came from a move. When it wasn't an ActiveMove, `CheckedBerserk` was set to `true` (allowing berry eating).

However, confusion self-damage uses `PseudoMoveEffect.Confused` which has `EffectType.Move` but is NOT an `ActiveMove`. This caused C# to treat confusion as a non-move effect, setting `CheckedBerserk = true`, which allowed the Sitrus Berry to be consumed immediately.

In Showdown, the equivalent check is `effect.effectType === "Move"`, which correctly matches both regular ActiveMoves and the confusion pseudo-move object `{effectType: 'Move'}`. Since the confusion pseudo-move is not multihit and not Sheer Force, `checkedBerserk` is set to `false`, blocking berry consumption until `onAfterMoveSecondary` (which never fires for confusion since the move is blocked).

## Fix
Changed the Berserk `OnDamage` handler to check `effect?.EffectType == EffectType.Move` instead of `effect is ActiveMove`. This correctly treats PseudoMoveEffect (confusion) as a Move-type effect, matching Showdown's `effectType === "Move"` guard.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesABC.cs` — Berserk OnDamage handler

## Pattern
When checking if a damage effect is a "move" for ability interactions, use `EffectType == EffectType.Move` rather than type-checking for `ActiveMove`. The `PseudoMoveEffect` class (used for confusion, etc.) has `EffectType.Move` but is not an `ActiveMove`, so type-checking misses it.
