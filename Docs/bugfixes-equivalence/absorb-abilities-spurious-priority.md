# Water Absorb and Volt Absorb Having Spurious priority=1 Breaking Ordering

**Commit:** `769eb585` + `c6cb6dd0`
**Date:** 2026-03-13

## Problem
Water Absorb and Volt Absorb's `OnTryHit` handlers fired before other same-type absorbing abilities (Storm Drain and Motor Drive respectively) on spread moves, breaking the expected left-to-right field position ordering.

## Root Cause
Both Water Absorb and Volt Absorb had an incorrect `priority=1` parameter on their `OnTryHit` event handler registration. In Showdown, all four absorb abilities (Water Absorb, Storm Drain, Volt Absorb, Motor Drive) have default priority 0, and when multiple abilities trigger on the same spread move, field position index breaks the tie (left-to-right ordering). The spurious `priority=1` caused Water Absorb/Volt Absorb to always fire first regardless of field position, desynchronizing the ability activation order and subsequent PRNG calls.

## Fix
Removed the `priority=1` parameter from both Water Absorb and Volt Absorb's `OnTryHit` handler registrations, allowing them to use the default priority of 0 and resolve ties by field position like Showdown.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesVWX.cs` -- Removed `priority=1` from Volt Absorb's `OnTryHit` handler and from Water Absorb's `OnTryHit` handler

## Pattern
Incorrect event handler priority: a non-default priority value that broke the expected tie-breaking order between abilities with equal precedence.
