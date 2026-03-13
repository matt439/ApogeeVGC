# Pseudo-Weather Add/Remove Spuriously Calling UpdateSpeed

**Date:** 2026-03-14

## Problem
When Trick Room ended during `FieldEvent('Residual')`, `eachEvent('Update')` processed Sitrus Berry consumption in the wrong order. Tropius (p1b) ate its Sitrus Berry before Alcremie (p2b) in C#, but Showdown processed Alcremie first.

In the failing test (seed 18244/battle 030693), Trick Room ended at residual order 27. C#'s `RemovePseudoWeather` called `Battle.UpdateSpeed()` immediately after removing Trick Room, recalculating all Pokemon speeds without Trick Room inversion. When `eachEvent('Update')` ran afterward, it sorted by these new (non-Trick-Room) speeds, producing a different order than Showdown.

## Root Cause
C#'s `Field.AddPseudoWeather()` and `Field.RemovePseudoWeather()` both called `Battle.UpdateSpeed()` after modifying the pseudo-weather state. Showdown's `addPseudoWeather()` and `removePseudoWeather()` do NOT do this — speed updates only happen at the start of each action via the explicit `this.updateSpeed()` call in the action loop.

The extra `UpdateSpeed()` calls caused Pokemon speeds to be recalculated mid-residual when Trick Room ended, changing the speed ordering for subsequent `eachEvent` calls that should have used the cached speeds from the start of the residual action.

## Fix
Removed `Battle.UpdateSpeed()` calls from both `Field.AddPseudoWeather()` and `Field.RemovePseudoWeather()`. Speed updates are already handled by `Battle.UpdateSpeed()` at the start of each action in the action loop, matching Showdown's behavior.

## Files Changed
- `ApogeeVGC/Sim/FieldClasses/Field.cs` -- removed spurious UpdateSpeed() calls from AddPseudoWeather and RemovePseudoWeather

## Pattern
Eagerly updating derived state: The C# port added "helpful" UpdateSpeed() calls that Showdown doesn't have. Showdown relies on speed being a cached value that's updated once per action — mid-action changes to speed-affecting conditions (Trick Room, Tailwind ending) intentionally don't update cached speeds until the next action.
