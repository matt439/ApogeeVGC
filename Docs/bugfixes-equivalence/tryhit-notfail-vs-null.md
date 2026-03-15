# Distinguish NOT_FAIL from Handler-Null in TryHit Event

**Commit:** `71d26f4102c4c770e08409b9dc8fade151ff63f3`
**Date:** 2026-03-13

## Problem
In Showdown's `hitStepTryHitEvent`, a handler returning `null` (e.g. Earth Eater absorbing a Ground move) gets converted to `false` (failure), while NOT_FAIL (empty string `''`) stays as NOT_FAIL. The C# code was mapping both to `NullRelayVar`, losing this distinction and causing Stomping Tantrum to not double its base power after ability-absorbed moves.

## Root Cause
All TryHit EventInfo wrappers mapped both `EmptyBoolIntEmptyVoidUnion` (NOT_FAIL) and `null` handler returns to `NullRelayVar`. In `hitStepTryHitEvent`, Showdown checks `result !== NOT_FAIL` to decide whether to convert to `false`. When both cases collapsed into the same type, the code could not distinguish "handler blocked silently" (null -> false) from "not a failure" (NOT_FAIL -> keep as-is). Stomping Tantrum checks whether the previous move failed, and the incorrect false-positive "success" from absorbed moves meant it did not get its base power boost.

## Fix
Added a new `EmptyRelayVar` type to represent Showdown's NOT_FAIL. Updated all TryHit EventInfo wrappers to map `Empty` -> `EmptyRelayVar` (keeping `null` -> `NullRelayVar`), and updated `HitStepTryEvent` to convert `NullRelayVar` -> `false` and `EmptyRelayVar` -> NOT_FAIL. Also fixed Protect and Psychic Terrain conditions to return `Empty()` instead of `null`.

## Files Changed
- `ApogeeVGC/Sim/Utils/Unions/RelayVar.cs` -- Added `EmptyRelayVar` record type
- `ApogeeVGC/Sim/BattleClasses/BattleActions.HitSteps.cs` -- Split NullRelayVar/EmptyRelayVar handling in hit result conversion
- `ApogeeVGC/Sim/BattleClasses/Battle.Helpers.cs` -- Added EmptyRelayVar to falsy check in IsRelayVarTruthy
- `ApogeeVGC/Sim/Events/Handlers/EventMethods/OnAnyTryHitEventInfo.cs` -- Empty -> EmptyRelayVar
- `ApogeeVGC/Sim/Events/Handlers/EventMethods/OnFoeTryHitEventInfo.cs` -- Empty -> EmptyRelayVar
- `ApogeeVGC/Sim/Events/Handlers/EventMethods/OnSourceTryHitEventInfo.cs` -- Empty -> EmptyRelayVar
- `ApogeeVGC/Sim/Events/Handlers/EventMethods/OnTryHitEventInfo.cs` -- Empty -> EmptyRelayVar, null -> NullRelayVar
- `ApogeeVGC/Sim/Events/Handlers/MoveEventMethods/OnTryHitEventInfo.cs` -- Empty -> EmptyRelayVar
- `ApogeeVGC/Sim/Events/Handlers/PokemonEventMethods/OnAllyTryHitEventInfo.cs` -- Empty -> EmptyRelayVar
- `ApogeeVGC/Data/Conditions/ConditionsPQR.cs` -- Protect and Psychic Terrain return Empty() instead of null

## Pattern
Collapsed JS null vs empty-string distinction into a single C# type, losing semantic difference needed for downstream branching.
