# Synchronize Not Showing Immune Message

**Commit:** `8365ab03`
**Date:** 2026-03-12

## Problem
When Synchronize attempted to inflict a status on a type-immune target (e.g., trying to poison a Steel-type), the `|-immune|` protocol message was not displayed, diverging from Showdown's output.

## Root Cause
The immune message check only looked for `ActiveMove` with a non-null Status field. Synchronize does not pass an ActiveMove as the source effect; it passes the status Condition object itself. In Showdown, the check is `(sourceEffect as Move)?.status`, which is truthy for both actual moves and the Synchronize hack object `{status: id}`. The C# code did not account for the Condition case.

## Fix
Expanded the pattern match from `sourceEffect is ActiveMove { Status: not null }` to `sourceEffect is ActiveMove { Status: not null } or Condition`, so that Synchronize-triggered status attempts also display the immune message.

## Files Changed
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Status.cs` — broadened sourceEffect type check to include Condition

## Pattern
Showdown duck-typing vs C# strict typing: Showdown's `(sourceEffect as Move)?.status` works on any object with a `status` property. C# requires explicit type matching for each possible source effect type that carries status information.
