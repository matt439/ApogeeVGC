# Ghost-Type Pokemon Incorrectly Trapped

**Commit:** `06c959f4`
**Date:** 2026-03-12

## Problem
Ghost-type Pokemon were being trapped by moves like Spirit Shackle and Anchor Shot, which should not be able to trap Ghost types. This caused battle flow divergence in equivalence tests.

## Root Cause
The `Trapped` condition definition was missing its `ImmuneTypes` property. In Showdown, Ghost-type Pokemon are immune to the trapped condition, but the C# port had not declared this type immunity on the condition record.

## Fix
Added `ImmuneTypes = [PokemonType.Ghost]` to the Trapped condition definition.

## Files Changed
- `ApogeeVGC/Data/Conditions/ConditionsSTU.cs` — added Ghost type immunity to Trapped condition
- `ApogeeVGC/Sim/Core/Driver.cs` — changed default equivalence test fixture index (incidental)

## Pattern
Missing condition property: When porting Showdown condition/status definitions, all properties (especially immunity lists) must be transferred. A missing immunity field silently allows effects that should be blocked.
