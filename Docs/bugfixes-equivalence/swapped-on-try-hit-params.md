# Swapped OnTryHit Parameters in Soundproof, Sap Sipper, and Sturdy

**Commit:** `a2bc9757`
**Date:** 2026-03-11

## Problem
Soundproof, Sap Sipper, and Sturdy ability handlers had their `source` and `target` parameters swapped, causing the abilities to check the wrong Pokemon. For example, Sap Sipper's `target != source` guard and type check would reference the attacker instead of the defender, potentially blocking or allowing moves incorrectly.

## Root Cause
The `OnTryHitEventInfo.Create` callback signature is `(battle, target, source, move)` matching Showdown's `onTryHit(target, source, move)`. The C# handlers mistakenly used `(battle, source, target, move)`, swapping the defender and attacker.

## Fix
Corrected the parameter order in all three ability handlers to `(battle, target, source, move)`, matching the Showdown convention where the first Pokemon parameter is the defender (target) and the second is the attacker (source).

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesSTU.cs` — Fix Sap Sipper, Soundproof, and Sturdy `OnTryHit` parameter order

## Pattern
Parameter order mismatch in event handler callbacks. Showdown's convention is `(target, source, move)` for `onTryHit`, but it is easy to accidentally swap source and target during porting. Always verify against the TypeScript signature.
