# Magic Bounce sourceEffect Ability Attribution

**Commit:** `465594b2`
**Date:** 2026-03-11

## Problem
When Magic Bounce reflected a move, the protocol log did not show `[from] ability: Magic Bounce` on the reflected move's usage line, diverging from Showdown's output.

## Root Cause
The code passed `battle.Effect` as the `SourceEffect` when calling `UseMove` for the bounced move. During ability handler dispatch, `battle.Effect` is set to the ability currently being processed, but this reference can change. Showdown explicitly passes the Magic Bounce ability object as the source effect to ensure proper `[from]` attribution.

## Fix
Changed both `OnTryHit` and `OnAllyTryHitSide` handlers to pass `battle.Library.Abilities[AbilityId.MagicBounce]` as the `SourceEffect` instead of `battle.Effect`, ensuring the ability is always correctly attributed.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesMNO.cs` — Use explicit Magic Bounce ability reference as `SourceEffect` in both bounce handlers

## Pattern
Relying on `Battle.Effect` (a mutable global) to capture the current ability during event dispatch. When the effect identity matters for attribution, always use an explicit reference to the ability/item/condition.
