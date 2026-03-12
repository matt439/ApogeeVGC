# TryAddVolatile Prefixed Handlers Not Converting Null to Falsy

**Commit:** `e30714b5`
**Date:** 2026-03-12

## Problem
When prefixed TryAddVolatile event handlers (OnAny, OnFoe, OnSource, OnAlly) returned `null`, the result was not converted to a falsy `RelayVar`. This meant the volatile was incorrectly added even when a handler intended to block it, causing divergence in equivalence tests.

## Root Cause
In Showdown, a handler returning `null` is falsy and blocks the volatile from being added. The C# handler wrappers passed through `null` without conversion, and the calling code in `Pokemon.Status.cs` only checked for `BoolRelayVar { Value: false }` or raw `null` -- but a `null` from a handler wrapper was not reliably propagated. The fix adds an explicit `NullRelayVar` type to distinguish "handler returned null" from "no handler ran."

## Fix
Added `if (result == null) return new NullRelayVar();` to all four prefixed TryAddVolatile handler wrappers, and updated the check in `Pokemon.Status.cs` to also match `NullRelayVar`.

## Files Changed
- `ApogeeVGC/Sim/Events/Handlers/EventMethods/OnAnyTryAddVolatileEventInfo.cs` -- convert null result to NullRelayVar
- `ApogeeVGC/Sim/Events/Handlers/EventMethods/OnFoeTryAddVolatileEventInfo.cs` -- convert null result to NullRelayVar
- `ApogeeVGC/Sim/Events/Handlers/EventMethods/OnSourceTryAddVolatileEventInfo.cs` -- convert null result to NullRelayVar
- `ApogeeVGC/Sim/Events/Handlers/PokemonEventMethods/OnAllyTryAddVolatileEventInfo.cs` -- convert null result to NullRelayVar
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Status.cs` -- added NullRelayVar to the falsy check

## Pattern
JS null vs C# null semantics: Showdown uses JavaScript truthiness (null is falsy), but C# null in pattern matching can behave differently from an explicit "null value" wrapper. Handler return values that are null in JS need explicit falsy representation in C#.
