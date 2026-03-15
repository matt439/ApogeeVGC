# Synchronize Duplicate Status Not Emitting -fail Protocol Message

**Commit:** `0e9c5f08`
**Date:** 2026-03-13

## Problem
When Synchronize attempted to apply a status to a Pokemon that already had that same status, the expected `-fail` protocol message was not emitted, causing protocol output divergence from Showdown.

## Root Cause
The duplicate status check in `SetStatus` only matched `ActiveMove` as the `sourceEffect` type (via `sourceEffect is ActiveMove move && move.Status == Status`). Synchronize passes a `Condition` object with `EffectType.Status` as the `sourceEffect`, matching Showdown's duck-typed hack object `{ status: status.id, id: 'synchronize' }`. Since `Condition` was not handled, the duplicate status branch was never entered for Synchronize, and the `-fail` message was silently skipped.

## Fix
Added a second check alongside the `ActiveMove` pattern to also match `Condition` objects with `EffectType.Status` whose `Id` equals the current status, using a combined boolean `sourceHasMatchingStatus`.

## Files Changed
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Status.cs` -- Extended duplicate status check to handle `Condition` sourceEffect from Synchronize

## Pattern
Incomplete type coverage in duck-type translation -- missing `Condition` as a valid sourceEffect type alongside `ActiveMove`.
