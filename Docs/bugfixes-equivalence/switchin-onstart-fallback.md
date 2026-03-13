# SwitchIn OnStart Fallback Incorrectly Applying When OnAnySwitchIn Exists

**Commit:** `1e5d1721`
**Date:** 2026-03-13

## Problem
Items like White Herb (which have both `OnStart` and `OnAnySwitchIn` handlers) were receiving a spurious `SwitchIn` handler from the `OnStart` fallback, causing them to fire twice and at the wrong priority -- before abilities like Frisk that should run first.

## Root Cause
The fallback logic that promotes `OnStart` to `OnSwitchIn` was checking for the absence of `(EventId.AnySwitchIn, EventPrefix.None)` in the handler cache. However, `OnAnySwitchIn` handlers are actually stored under the key `(EventId.SwitchIn, EventPrefix.Any)`. The incorrect key meant the guard never found the existing `OnAnySwitchIn` handler, so the fallback always applied, creating a duplicate switch-in handler.

## Fix
Changed the cache lookup key from `(EventId.AnySwitchIn, EventPrefix.None, EventSuffix.None)` to `(EventId.SwitchIn, EventPrefix.Any, EventSuffix.None)` to match the actual storage key for `OnAnySwitchIn` handlers.

## Files Changed
- `ApogeeVGC/Sim/Events/EventHandlerInfoMapper.cs` -- Fixed the `ContainsKey` check to use the correct event key tuple for `OnAnySwitchIn`

## Pattern
Event key mismatch -- using a logical event name instead of the actual composite storage key in the handler cache.
