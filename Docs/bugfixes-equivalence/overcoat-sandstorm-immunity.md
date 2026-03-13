# Overcoat Missing OnImmunity Handler for Sandstorm Weather Immunity

**Commit:** `dbf38def`
**Date:** 2026-03-13

## Problem
Pokemon with the Overcoat ability were incorrectly taking Sandstorm damage, causing 5 equivalence test failures.

## Root Cause
Overcoat's Sandstorm immunity was incorrectly commented as "handled in Pokemon.RunImmunity", but `RunStatusImmunity` fires `EventId.Immunity` which requires an actual `OnImmunity` handler registered on the ability. Other sand-immune abilities (Sand Force, Sand Rush, Sand Veil) already had this handler, but Overcoat was missing it. Without the handler, the immunity event had nothing to catch and block the Sandstorm damage.

## Fix
Added an `OnImmunity` handler to Overcoat that returns `false` (immune) when the immunity type is `ConditionId.Sandstorm`, matching the pattern used by other sand-immune abilities.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesMNO.cs` -- Added `OnImmunity` handler for Sandstorm to Overcoat ability definition

## Pattern
Missing event handler -- ability immunity assumed to be handled elsewhere but actually requires an explicit event handler registration.
