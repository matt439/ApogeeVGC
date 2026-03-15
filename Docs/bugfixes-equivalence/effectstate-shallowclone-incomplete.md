# EffectState.ShallowClone Missing Fields

## Commit
`4d847bf3` — Fix EffectState.ShallowClone not copying all fields

## Symptom
Shed Tail / Baton Pass transferred Substitute volatiles had 0 HP, causing the Substitute to break immediately on any hit.

## Root Cause
`EffectState.ShallowClone()` only copied ~8 fields (Id, Target, Source, SourceSlot, SourceEffect, Duration, Started, Ending, LinkedPokemon, LinkedStatus) and had a `// TODO: Copy any other properties` comment. The `Hp` field (and many others like Counter, Move, FromBooster, BestStat, etc.) were not copied, so they defaulted to 0/null in the cloned state.

`ShallowClone` is used by `CopyVolatileFrom` (Baton Pass / Shed Tail) to transfer volatile conditions between Pokemon. `DeepClone` (used for MCTS battle cloning) already copied all fields correctly.

## Fix
Updated `ShallowClone()` to copy ALL value-type fields to match `DeepClone`, and deep-copy reference types (`TypeWas`, `Boosts`) to avoid shared mutation.

## Pattern
**Incomplete copy method** — when adding fields to a data class, all copy/clone methods must be updated. Check both `ShallowClone` and `DeepClone` when adding new fields to `EffectState`.

## Files Changed
- `ApogeeVGC/Sim/Effects/EffectState.cs`
