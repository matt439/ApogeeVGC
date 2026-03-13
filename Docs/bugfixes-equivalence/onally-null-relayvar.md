# OnAlly* Event Handler Wrappers Not Converting Null Returns to NullRelayVar

**Commit:** `5af37761a0515c1e08e84e43d6bfeff3f044c288`
**Date:** 2026-03-12

## Problem
Flower Veil and Sweet Veil's `OnAllySetStatus` handlers returned `null` when blocking a status, but the event wrapper treated this as passthrough (no opinion) instead of as a blocking result, allowing the status to be applied.

## Root Cause
In the C# event system, returning C# `null` from a handler means "undefined / no opinion / passthrough," while returning `NullRelayVar` means "JS null / falsy / block." The `OnAllySetStatus`, `OnAllyTryHit`, `OnAllyTryHitSide`, and `OnAllyTryHitField` wrapper methods all mapped `null` handler returns to C# `null` (passthrough) instead of `NullRelayVar` (block). The non-ally counterparts of these wrappers already handled this correctly. Additionally, the Flower Veil and Sweet Veil ability implementations themselves returned `null` instead of `false` when blocking.

## Fix
Added `null` handler return -> `NullRelayVar` conversion in all four `OnAlly*` wrapper methods. Fixed Flower Veil and Sweet Veil to return `false` explicitly when blocking a status.

## Files Changed
- `ApogeeVGC/Sim/Events/Handlers/PokemonEventMethods/OnAllySetStatusEventInfo.cs` -- Added null -> NullRelayVar conversion
- `ApogeeVGC/Sim/Events/Handlers/PokemonEventMethods/OnAllyTryHitEventInfo.cs` -- Added null -> NullRelayVar conversion
- `ApogeeVGC/Sim/Events/Handlers/PokemonEventMethods/OnAllyTryHitFieldEventInfo.cs` -- Added null -> NullRelayVar conversion
- `ApogeeVGC/Sim/Events/Handlers/PokemonEventMethods/OnAllyTryHitSideEventInfo.cs` -- Added null -> NullRelayVar conversion
- `ApogeeVGC/Data/Abilities/AbilitiesDEF.cs` -- Flower Veil returns false instead of null when blocking
- `ApogeeVGC/Data/Abilities/AbilitiesSTU.cs` -- Sweet Veil returns false instead of null when blocking

## Pattern
Inconsistent null-to-NullRelayVar mapping between ally and non-ally event handler wrappers.
