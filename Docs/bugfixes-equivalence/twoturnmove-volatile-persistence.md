# TwoTurnMove Volatile Not Persisting Through Residual

**Commit:** `17fc18f2`
**Date:** 2026-03-13

## Problem
Two-turn moves (Shadow Force, Phantom Force, Fly, Solar Beam, etc.) were removing the generic `TwoTurnMove` volatile during the move's `OnTryMove` handler instead of the move-specific volatile. This caused the `TwoTurnMove` volatile to be absent during the residual phase, resulting in fewer residual handlers and PRNG call count divergence.

## Root Cause
In Showdown, the `OnTryMove` handler for two-turn moves calls `attacker.removeVolatile(move.id)` -- removing the move-specific volatile (e.g., `shadowforce`), not the generic `twoturnmove`. The generic `twoturnmove` volatile persists and is only removed via its residual duration mechanism (duration countdown). The C# implementation was incorrectly calling `RemoveVolatile(ConditionId.TwoTurnMove)` in `OnTryMove`, removing the generic volatile prematurely. This meant the residual phase had one fewer handler to process, shifting PRNG call counts. Additionally, 5 move-specific marker conditions (IceBurn, MeteorBeam, SkyAttack, SolarBeam, SolarBlade) were missing from the conditions library, preventing `TwoTurnMove.OnStart` from adding their volatiles.

## Fix
Changed all 7 two-turn move `OnTryMove` handlers across 4 files to remove the move-specific volatile (resolved by parsing the move ID to a ConditionId) instead of the generic TwoTurnMove volatile. Registered the 5 missing marker conditions and their corresponding ConditionId enum entries.

## Files Changed
- `ApogeeVGC/Data/Moves/MovesGHI.cs` -- Ice Burn OnTryMove: remove move-specific volatile
- `ApogeeVGC/Data/Moves/MovesMNO.cs` -- Meteor Beam OnTryMove: remove move-specific volatile
- `ApogeeVGC/Data/Moves/MovesPQR.cs` -- Phantom Force OnTryMove: remove move-specific volatile
- `ApogeeVGC/Data/Moves/MovesSTU.cs` -- Shadow Force, Sky Attack, Solar Beam, Solar Blade OnTryMove: remove move-specific volatile
- `ApogeeVGC/Data/Conditions/ConditionsGHI.cs` -- Added IceBurn marker condition
- `ApogeeVGC/Data/Conditions/ConditionsMNO.cs` -- Added MeteorBeam marker condition
- `ApogeeVGC/Data/Conditions/ConditionsSTU.cs` -- Added SkyAttack, SolarBeam, SolarBlade marker conditions
- `ApogeeVGC/Sim/Conditions/ConditionId.cs` -- Added 5 new ConditionId enum entries

## Pattern
Wrong volatile being removed, causing premature cleanup and residual handler count mismatch with PRNG divergence.
