# Missing SlotCondition on Wish Move Definition

**Commit:** `cc501c36`
**Date:** 2026-03-12

## Problem
The Wish move definition was missing its `SlotCondition` property, so using Wish did not apply the slot condition that heals the Pokemon in the slot after one turn. This caused Wish to have no delayed healing effect, diverging from Showdown.

## Root Cause
The move definition had a TODO comment noting that Wish's slot condition was not implemented due to missing infrastructure. The infrastructure was later added but the Wish definition was never updated to use it.

## Fix
Added `SlotCondition = ConditionId.Wish` to the Wish move definition, replacing the stale TODO comment.

## Files Changed
- `ApogeeVGC/Data/Moves/MovesVWX.cs` -- added SlotCondition property to Wish move

## Pattern
Stale TODO / incomplete implementation: infrastructure was built but existing move definitions referencing it via TODO comments were not updated. Search for TODO comments that reference now-available features.
