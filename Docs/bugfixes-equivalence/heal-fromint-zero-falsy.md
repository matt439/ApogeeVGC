# Heal() Returning FromInt(0) Instead of FromFalse() When Healing Blocked or Zero

**Commit:** `6e4f56a1` + `6df2a78c`
**Date:** 2026-03-13

## Problem
The `Heal()` method returned `IntFalseUnion.FromInt(0)` in two cases where it should have returned `IntFalseUnion.FromFalse()`: when the TryHeal event blocked healing (e.g., Psychic Noise / Heal Block) and when the computed heal amount was zero. Callers like Heal Pulse that check `IsFalse` to detect failure could not distinguish "heal was blocked" from "heal succeeded with amount 0", causing missing `|-fail|target|heal` messages.

## Root Cause
In JavaScript, `0` is falsy, so callers that use `!!this.heal()` treat a return of `0` the same as `false`. The C# `IntFalseUnion.FromInt(0)` has `IsFalse=false`, making it truthy from the caller's perspective. When the TryHeal RunEvent returned a `BoolRelayVar(false)` (healing blocked by Psychic Noise), the code converted this to `FromInt(0)` instead of propagating the falsy semantics. Similarly, when the heal amount computed to zero, returning `FromInt(0)` signaled success rather than failure.

## Fix
Changed both early-return paths in `Heal()` to return `IntFalseUnion.FromFalse()`: the TryHeal event-blocked branch (when the result is not an `IntRelayVar`) and the zero heal amount branch (when `healAmount.Value == 0`).

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/Battle.Combat.cs` -- Return `FromFalse()` instead of `FromInt(0)` in both the TryHeal-blocked path and the zero-amount path

## Pattern
JS falsy semantics mismatch: C# integer 0 is not falsy, but JS treats `0` and `false` identically in boolean contexts.
