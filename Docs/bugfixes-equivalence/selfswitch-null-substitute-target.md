# RunMoveEffects Skipping selfSwitch for Null (Substitute) Targets

**Commit:** `9f17c7aa`
**Date:** 2026-03-12

## Problem
Moves with `selfSwitch` (Flip Turn, U-turn, Volt Switch, etc.) were not triggering the switch-out effect when the hit was absorbed by a Substitute, because the entire per-target effect processing was skipped for null targets.

## Root Cause
In Showdown's `runMoveEffects`, the per-target loop distinguishes between `false` targets (completely skip) and `null` targets (Substitute absorbed the hit). Null targets still execute `selfSwitch`, `selfDestruct`, and the `didAnything`/`damage` combining logic. The C# implementation treated all non-Pokemon targets identically -- both `false` and `null` triggered `continue`, skipping the entire loop body including `selfSwitch`. This prevented Flip Turn/U-turn from switching the user out after breaking or hitting into a Substitute.

## Fix
Restructured the loop to only `continue` for `FalsePokemonUnion` (false targets). For `NullPokemonUnion` (Substitute targets), the target is extracted as `null` and all target-dependent effects (boosts, status, healing, etc.) are guarded by `target != null`, while non-target-dependent effects (selfSwitch, selfDestruct, damage combining) execute unconditionally.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveEffects.cs` -- Restructured target loop to distinguish false vs null targets, moving target-dependent effects under a null guard while keeping selfSwitch/selfDestruct/combining outside

## Pattern
Incorrect null/false target conflation skipping effects that should run regardless of target presence.
