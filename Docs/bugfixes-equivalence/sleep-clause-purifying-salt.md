# Sleep Clause Firing Before Purifying Salt's OnSetStatus Handler

**Commit:** `2a432a83`
**Date:** 2026-03-13

## Problem
When a Pokemon with Purifying Salt was targeted by a sleep-inducing move, the Sleep Clause Mod check fired first instead of Purifying Salt's immunity, producing incorrect protocol output (Sleep Clause message instead of `-immune` message).

## Root Cause
In Showdown, both Sleep Clause Mod and Purifying Salt are `onSetStatus` event handlers within the same `RunEvent` call, sorted by speed. Purifying Salt fires first because the Pokemon's speed is greater than zero, while format rules like Sleep Clause have speed zero. In the C# implementation, Sleep Clause was implemented as a procedural check that ran before `RunEvent(SetStatus)`, so it always executed before any ability-based `OnSetStatus` handlers regardless of speed ordering.

## Fix
Moved the `RunEvent(EventId.SetStatus)` call to execute before the Sleep Clause Mod check, so ability handlers like Purifying Salt can block the status first, matching Showdown's unified event handler ordering.

## Files Changed
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Status.cs` -- Reordered RunEvent(SetStatus) to fire before the Sleep Clause Mod procedural check

## Pattern
Procedural check executing before event-driven handlers, violating the reference implementation's unified event ordering by speed.
