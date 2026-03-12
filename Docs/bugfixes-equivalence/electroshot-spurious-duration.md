# ElectroShot Volatile Having Spurious Duration=2

**Commit:** `b4b2b70d`
**Date:** 2026-03-12

## Problem
The ElectroShot volatile condition had `Duration = 2` set explicitly, causing the battle engine to process an extra residual tick that consumed an additional PRNG call. This desynchronized the PRNG state from Showdown for the remainder of the battle.

## Root Cause
In Showdown, the ElectroShot volatile is a simple marker without a condition block -- it has no duration of its own. Duration management is handled by the TwoTurnMove condition instead. The C# port incorrectly gave it `Duration = 2`, which caused the engine to count it as an active timed condition and process unnecessary residual events.

## Fix
Removed `Duration = 2` from the ElectroShot condition definition, leaving it as a durationless marker volatile whose lifetime is managed by TwoTurnMove.

## Files Changed
- `ApogeeVGC/Data/Conditions/ConditionsDEF.cs` -- removed spurious Duration=2 from ElectroShot volatile
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Damage.cs` -- whitespace/encoding cleanup
- `ApogeeVGC/Sim/Utils/Prng.cs` -- minor refactor for debuggability (no behavioral change)

## Pattern
Spurious Duration on marker volatiles: some Showdown volatiles are simple markers with no condition block or duration. Adding a duration in C# causes extra residual processing and PRNG consumption, desynchronizing the RNG stream.
