# Extra GetActionSpeed Loop in CommitChoices Causing PRNG Divergence

**Commit:** `3443f5e2`
**Date:** 2026-03-13

## Problem
Speed-tie resolution was incorrect in battles involving Unaware-mirror scenarios (e.g., Ditto Transform), producing wrong turn ordering due to PRNG state divergence.

## Root Cause
Showdown's `commitChoices` only calls `getActionSpeed` once per action, during `resolveAction` which is invoked by `addChoice`. The C# implementation was calling `GetActionSpeed` twice: once during `ResolveAction`/`AddChoice` (matching Showdown) and again in a separate loop over all queued actions. The extra loop produced 4 spurious PRNG calls (from `ModifyBoost` speed-tie shuffles) that shifted the PRNG seed state, causing subsequent speed-tie coin flips to produce different results.

## Fix
Removed the redundant `GetActionSpeed` loop in `CommitChoices`, relying solely on the speed calculation already performed during `AddChoice`.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/Battle.Requests.cs` -- Removed extra `GetActionSpeed` loop and added comment clarifying that speeds are already calculated during `AddChoice`

## Pattern
Duplicate PRNG-consuming operation causing seed state divergence from the reference implementation.
