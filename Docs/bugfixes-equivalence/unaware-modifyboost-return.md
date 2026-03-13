# Unaware Not Returning Modified Boosts Table from ModifyBoost Event

**Commit:** `888cbf69`
**Date:** 2026-03-13

## Problem
Unaware's boost-zeroing logic had no effect, causing accuracy and evasion boosts to apply when they should have been ignored. This led to incorrect hit/miss outcomes -- for example, Cetitan's Ice Shard missing Skeledirge because a -1 accuracy boost from Muddy Water was not being zeroed out by Unaware.

## Root Cause
In Showdown, `onAnyModifyBoosts` handlers modify the `boosts` object in-place via JS reference semantics, so returning `undefined` preserves the mutations. In C#, `SparseBoostsTable` is a value type passed by copy. Unaware's handler was modifying the local copy (zeroing accuracy/evasion) but returning `VoidReturn`, which caused the event system to discard the modified copy and keep the original unmodified boosts. Additionally, the ModifyBoost callers only handled `BoostsTableRelayVar` but the event wrapper returned `SparseBoostsTableRelayVar`.

## Fix
Changed Unaware's handler to return the modified `boosts` object instead of `VoidReturn`. Added `SparseBoostsTableRelayVar` handling in all five ModifyBoost call sites (four in `HitStepAccuracy` and one in `Pokemon.GetStat`), converting via `ToBoostsTable()`.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesSTU.cs` -- Unaware handler returns modified boosts instead of `VoidReturn`
- `ApogeeVGC/Sim/BattleClasses/BattleActions.HitSteps.cs` -- Handle `SparseBoostsTableRelayVar` in four accuracy/evasion boost sites
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Stats.cs` -- Handle `SparseBoostsTableRelayVar` in `GetStat`
- `ApogeeVGC/Sim/Core/Driver.cs` -- Unrelated: fix equivalence test file extension

## Pattern
JS reference vs. C# value semantics: in-place mutation of a value-type parameter requires an explicit return to propagate changes through the event system.
