# Dictionary Iteration Order for Side Conditions and Volatiles

## Summary

C#'s `Dictionary` may reorder entries after deletions, while JavaScript objects maintain insertion order. When a side condition (e.g., Reflect) expires and is removed, subsequent iterations of `SideConditions` could yield a different order than Showdown, causing SpeedSort shuffles to produce different results for tied handlers (e.g., Tailwind from both sides).

## Root Cause

`FindSideEventHandlers` and `FindPokemonEventHandlers` iterated over `Dictionary` entries directly. In JavaScript, `for...in` on an object preserves insertion order. In C#, `Dictionary` does not guarantee order stability after removals — freed slots may be reused, changing enumeration order.

## Fix

Sort dictionary entries by `EffectOrder` (creation order) before iterating in `FindSideEventHandlers` and `FindPokemonEventHandlers`, ensuring deterministic ordering that matches Showdown's insertion-order iteration.

## Commit

`2349cbd2` — Fix dictionary iteration order for side conditions and volatiles

## Files Changed

- `ApogeeVGC/Sim/BattleClasses/Battle.EventHandlers.cs`
