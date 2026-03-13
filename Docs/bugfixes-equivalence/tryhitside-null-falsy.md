# TryHitSide/TryHitField Not Treating Null Result as Falsy

**Commit:** `344a4863b456337e508fcfb7624ca21174bce67f`
**Date:** 2026-03-12

## Problem
When Magic Bounce bounced a side-targeting move (e.g. Sticky Web), the TryHitSide handler returned `null`. The C# check only looked for `BoolRelayVar(false)`, treating `null` as truthy, so both the bounced and original move's effects fired (double Sticky Web application).

## Root Cause
Showdown's check is `if (!hitResult)`, which treats `null` as falsy. The C# implementation used `hitResult is not BoolRelayVar { Value: false }`, which only matched explicit `false` and let `null` (from Magic Bounce's handler) pass through as truthy. Additionally, the fail message was shown for all falsy values, but Showdown only shows `-fail` for explicit `false`, not for `null` (silent block).

## Fix
Changed the truthiness check to use `Battle.IsRelayVarTruthy()` which correctly treats `null` as falsy. Added a separate check so that the `-fail` message is only shown when the result is explicit `BoolRelayVar { Value: false }`.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveHit.cs` -- Unified TryHitSide/TryHitField truthiness check and separated fail message logic

## Pattern
Overly narrow falsiness check (explicit false only) instead of using JS-equivalent truthiness evaluation.
