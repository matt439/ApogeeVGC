# Grass-Type Pokemon Not Immune to Powder Effects (Effect Spore)

**Commit:** `3314481d`
**Date:** 2026-03-12

## Problem
Grass-type Pokemon were incorrectly affected by powder-based effects like Effect Spore, receiving sleep/poison/paralysis and consuming extra PRNG calls for status duration.

## Root Cause
The `Powder` condition definition was missing `ImmuneTypes = [PokemonType.Grass]`. In Showdown's type chart, Grass has `powder: 3` (immune), meaning Grass-type Pokemon should be completely immune to powder moves and effects. Without this immunity declaration, the powder type check never blocked the effect for Grass types.

## Fix
Added `ImmuneTypes = [PokemonType.Grass]` to the Powder condition definition in `ConditionsPQR.cs`.

## Files Changed
- `ApogeeVGC/Data/Conditions/ConditionsPQR.cs` -- Added `ImmuneTypes = [PokemonType.Grass]` to the Powder condition

## Pattern
Missing type immunity declaration in condition data -- type chart immunity not translated to the condition's `ImmuneTypes` list.
