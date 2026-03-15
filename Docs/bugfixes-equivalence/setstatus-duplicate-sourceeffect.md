# SetStatus Not Matching Condition sourceEffect in Duplicate-Status Else Branch

**Commit:** `9c45e47c`
**Date:** 2026-03-13

## Problem
When Synchronize tried to apply a status back to a Pokemon that already had a different status, the `-fail` and `[still]` protocol messages were not emitted, causing divergence from Showdown's output.

## Root Cause
Showdown uses duck-typed JavaScript where `(sourceEffect as Move)?.status` matches any object with a `status` field, including the Synchronize hack object `{ status: status.id, id: 'synchronize' }`. The C# `else if` branch in `SetStatus` only checked for `ActiveMove` with a non-null `Status`, missing the case where `sourceEffect` is a `Condition` object (which Synchronize passes as the status condition). This meant the failure branch that emits `-fail` and `[still]` was never entered for Synchronize.

## Fix
Extended the pattern match to also accept `Condition { EffectType: EffectType.Status }` alongside `ActiveMove { Status: not null }` in the else-if guard.

## Files Changed
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Status.cs` -- Added `or Condition { EffectType: EffectType.Status }` to the duplicate-status else-if pattern match

## Pattern
Missing duck-type coverage when translating JavaScript's structural typing to C#'s nominal type checks.
