# AbilityState/ItemState Not Re-initialized on Switch-in

**Commit:** `cd8181ed`
**Date:** 2026-03-12

## Problem
When a Pokemon switched in, its `AbilityState` and `ItemState` were not re-initialized. Flags set during a previous appearance (such as Protean/Libero's "already activated" flag) persisted, preventing abilities from triggering again on subsequent switch-ins.

## Root Cause
Showdown re-initializes effect state for abilities and items as part of its `switchIn` logic, resetting per-switch-in flags. The C# port only updated the `EffectOrder` without reinitializing the state objects themselves.

## Fix
Added calls to `Battle.InitEffectState()` for both `AbilityState` and `ItemState` during switch-in, before setting their `EffectOrder` values.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Switch.cs` -- re-initialize AbilityState and ItemState on switch-in

## Pattern
Missing state reset on switch-in: Showdown resets various per-switch-in state that must be explicitly replicated in C#. Any once-per-switch-in flag stored in effect state needs reinitialization.
