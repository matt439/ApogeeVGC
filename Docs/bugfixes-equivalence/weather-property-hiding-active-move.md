# Weather Property Hiding Bug in ActiveMove

**Commit:** `d4415ca3`
**Date:** 2026-03-11

## Problem
Weather-setting moves (e.g., Rain Dance, Sunny Day) were not applying their weather effect because the `Weather` property was always `null` on `ActiveMove` instances, even though the base `Move` template had it set correctly.

## Root Cause
`ActiveMove` declares its own `Weather` property that hides the inherited `HitEffect.Weather`. The `Reset()` method was setting `Weather = null` instead of copying from the template, and the C# record copy constructor does not automatically propagate hidden properties from the base type. This meant the weather value from the move definition was lost during `ActiveMove` construction and reset.

## Fix
Changed `Reset()` to copy `Weather` from the template (`Weather = template.Weather`), and added an explicit `Weather = source.Weather` assignment in the copy constructor to ensure the hidden property is correctly propagated.

## Files Changed
- `ApogeeVGC/Sim/Moves/ActiveMove.cs` — Copy `Weather` from template in `Reset()` and from source in copy constructor

## Pattern
C# property hiding (`new` keyword or same-name property in derived record) causing the base class value to be silently shadowed. Record copy constructors and `with` expressions do not propagate hidden properties, so they must be explicitly copied.
