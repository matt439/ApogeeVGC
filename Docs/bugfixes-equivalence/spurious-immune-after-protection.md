# Spurious |-immune| After Protection Moves and Missing Heal Block OnBeforeMove

**Commit:** `3734a07e`
**Date:** 2026-03-12

## Problem
Two issues: (1) Protection moves like Baneful Bunker that apply status conditions were emitting a spurious `|-immune|` protocol message when the target was immune to the status. (2) Heal Block (from Psychic Noise) did not prevent healing moves from executing, only blocking passive healing via `OnTryHeal`.

## Root Cause
(1) The `|-immune|` message was shown whenever `sourceEffect` was any `Condition`, but protection conditions (EffectType.Condition) should not trigger it -- only true status conditions (EffectType.Status) like those from Synchronize should. (2) Showdown's Heal Block condition includes an `onBeforeMove` handler (priority 6) that blocks moves with the `heal` flag before execution. This handler was missing from the C# port.

## Fix
(1) Narrowed the `|-immune|` check from `sourceEffect is ... or Condition` to `sourceEffect is ... or Condition { EffectType: EffectType.Status }`. (2) Added an `OnBeforeMove` handler to the Heal Block condition that blocks moves with the `heal` flag and emits the appropriate `cant` message.

## Files Changed
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Status.cs` -- narrowed immune message to status-type conditions only
- `ApogeeVGC/Data/Conditions/ConditionsPQR.cs` -- added OnBeforeMove handler for Heal Block

## Pattern
Overly broad type check: the C# code matched all `Condition` instances where Showdown distinguishes between condition subtypes. Also, missing event handler that was noted as a TODO but not yet implemented.
