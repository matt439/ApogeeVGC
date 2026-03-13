# ClearWeather/ClearTerrain Not Writing Back Cleared EffectState

**Commit:** `4eb60ab3`
**Date:** 2026-03-12

## Problem
When weather or terrain expired and was cleared, stale `EffectState` with non-null `Duration` values remained on the `Field` object, causing phantom residual event handlers and spurious PRNG shuffles during end-of-turn processing.

## Root Cause
`ClearEffectState(ref localVar)` takes a `ref` parameter and reassigns the local variable, but the calling code in `ClearWeather` and `ClearTerrain` passed a local copy of `WeatherState`/`TerrainState`. The `ref` reassignment only affected the local variable, not the actual property on the `Field` object. The stale `Duration` values left on the properties caused `FieldEvent` to collect phantom residual handlers, and when multiple phantom handlers existed (e.g., both weather and terrain expired on the same turn), they triggered spurious `Prng.Shuffle()` calls that diverged the PRNG stream.

## Fix
Added write-back assignments (`WeatherState = weatherState` and `TerrainState = terrainState`) after the `ClearEffectState` calls so the cleared state is persisted to the `Field` object's properties.

## Files Changed
- `ApogeeVGC/Sim/FieldClasses/Field.cs` -- Added write-back of cleared `EffectState` to `WeatherState` and `TerrainState` properties after `ClearEffectState`

## Pattern
Ref parameter write-back omission -- local variable reassigned by ref but the source property never updated, leaving stale state.
