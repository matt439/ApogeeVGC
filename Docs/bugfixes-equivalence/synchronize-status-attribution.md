# Synchronize Status Attribution Missing sourceEffect

**Commit:** `b60f956b`
**Date:** 2026-03-12

## Problem
When Synchronize inflicted a status condition on the opposing Pokemon, the protocol output diverged from Showdown. The status was being applied without the correct sourceEffect context, causing incorrect attribution messages.

## Root Cause
In Showdown, Synchronize passes a hack object `{status: status.id, id: 'synchronize'}` without an `effectType` field as the sourceEffect to `trySetStatus`. This prevents the status's `onStart` handler from adding `[from] ability:` attribution. The C# code was passing `null` as sourceEffect, which did not match this behavior -- specifically, it prevented the `|-immune|` message from firing for immune targets (which checks `sourceEffect is ActiveMove or Condition`).

## Fix
Changed `TrySetStatus` to pass the status `Condition` object itself as the `sourceEffect` parameter instead of `null`. Since a Condition's EffectType is "Status" (not "Ability"), it prevents `[from] ability:` attribution while still allowing immune-message checks to work.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesSTU.cs` — pass status Condition as sourceEffect in Synchronize's TrySetStatus call

## Pattern
Showdown hack objects: Showdown sometimes passes ad-hoc objects with specific properties to control downstream behavior. The C# port must find a typed equivalent that produces the same downstream effects, rather than passing null.
