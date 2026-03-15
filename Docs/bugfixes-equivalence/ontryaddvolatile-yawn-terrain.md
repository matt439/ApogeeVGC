# OnTryAddVolatile null-to-false Mapping Causing Yawn to Fail on Terrain (Follow-Up)

**Commit:** `411cd235`
**Date:** 2026-03-12

## Problem
Yawn was incorrectly producing `-fail` and `[still]` protocol messages when used on a Pokemon standing on Electric, Misty, or Grassy Terrain, instead of being silently blocked.

## Root Cause
When a terrain's `onTryAddVolatile` handler returned `null` to silently block Yawn's volatile, the `OnTryAddVolatileEventInfo` wrapper mapped `null` to `BoolRelayVar.False`. This caused `addVolatile` to return `false` (explicit failure) instead of `null` (silent block). The calling code in `RunMoveEffects` treated `false` as a move failure, emitting `-fail` and `[still]` messages. In Showdown, `null` means "block the volatile silently without failing the move" -- a distinct semantic from `false` which means "the move explicitly failed."

## Fix
Changed the null mapping from `BoolRelayVar.False` to `new NullRelayVar()`, preserving the Showdown distinction between silent blocking (`null`) and explicit failure (`false`).

## Files Changed
- `ApogeeVGC/Sim/Events/Handlers/EventMethods/OnTryAddVolatileEventInfo.cs` -- Changed null handler result mapping from `BoolRelayVar.False` to `NullRelayVar`

## Pattern
Incorrect null-to-false semantic mapping -- collapsing two distinct Showdown return semantics (null = silent block, false = explicit fail) into a single falsy value.
