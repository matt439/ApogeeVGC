# SingleEvent Defaulting relayVar to True

**Commit:** `a21fff91`
**Date:** 2026-03-12

## Problem
When `SingleEvent` was called without a `relayVar` argument, it remained `null` instead of defaulting to `true`. This caused downstream event handlers to receive an unexpected null relay variable, leading to incorrect control flow in equivalence tests.

## Root Cause
Showdown's `singleEvent` function (line 593-594) explicitly defaults `relayVar` to `true` when it is `undefined`. The C# port did not replicate this default, leaving `relayVar` as `null`.

## Fix
Added `relayVar ??= BoolRelayVar.True;` at the start of the `SingleEvent` method to match Showdown's defaulting behavior.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/Battle.Events.cs` -- added null-coalescing default for relayVar

## Pattern
JS undefined-defaults not ported to C#: Showdown frequently uses `if (x === undefined) x = defaultValue` patterns that have no automatic equivalent in C#.
