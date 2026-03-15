# Constant Union Event Handlers Silently Ignored

**Commit:** `a06f7cd3`
**Date:** 2026-03-12

## Problem
Event handlers defined as constant values within union types (e.g., `OnCriticalHit = false`, `OnFlinch = true`) were silently ignored by the event system. This caused abilities and moves with constant-value event overrides to not take effect.

## Root Cause
The `UnionEventHandlerInfo<TUnion>` class inherited from `EventHandlerInfo` but did not implement the `IUnionEventHandler` interface. The event resolution system checks for `IUnionEventHandler` to detect constant values (booleans that short-circuit without calling a delegate). Without this interface, the wrapper was never recognized as containing a constant, so the constant value was never returned.

## Fix
Made `UnionEventHandlerInfo<TUnion>` implement `IUnionEventHandler`, delegating `GetDelegate()`, `IsConstant()`, and `GetConstantValue()` to the inner `UnionValue`.

## Files Changed
- `ApogeeVGC/Sim/Events/UnionEventHandlerInfo.cs` — implement IUnionEventHandler interface on the wrapper type

## Pattern
Missing interface implementation: When a wrapper/adapter type does not implement the interface that downstream consumers check for, the wrapped value is invisible to those consumers. Constant-value event handlers are a special case that requires explicit interface support.
