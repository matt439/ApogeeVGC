# Status Pattern Matching Bug in Ability OnSetStatus Handlers

**Commit:** `8214f979`
**Date:** 2026-03-11

## Problem
Status-immunity abilities (Immunity, Insomnia, Leaf Guard, Limber, Magma Armor, Purifying Salt, Shields Down, Vital Spirit, Water Bubble, Water Veil, and Pastel Veil) were showing the `[from] ability:` immunity message even when status was applied by a secondary effect (e.g., Scald's 30% burn), not just primary status moves. This produced protocol messages that Showdown does not emit.

## Root Cause
The pattern `effect is ActiveMove { Status: not ConditionId.None }` was intended to match only moves whose primary effect is a status (like Thunder Wave). However, in C#, when `Status` is `null` (as it is for moves that inflict status only as a secondary effect), the pattern `not ConditionId.None` evaluates to `true` because `null` is indeed not equal to `ConditionId.None`. In Showdown, the equivalent check `(effect as Move)?.status` is falsy for `undefined`/`null`.

## Fix
Changed all occurrences from `Status: not ConditionId.None` to `Status: not null`, which correctly matches only moves that have an explicit primary status effect, excluding secondary-effect status application.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesGHI.cs` — Fix Immunity and Insomnia
- `ApogeeVGC/Data/Abilities/AbilitiesJKL.cs` — Fix Leaf Guard and Limber
- `ApogeeVGC/Data/Abilities/AbilitiesPQR.cs` — Fix Pastel Veil and Purifying Salt
- `ApogeeVGC/Data/Abilities/AbilitiesSTU.cs` — Fix Shields Down and Magma Armor (Thermal Exchange)
- `ApogeeVGC/Data/Abilities/AbilitiesVWX.cs` — Fix Vital Spirit, Water Bubble, and Water Veil

## Pattern
C# `null` vs. enum default semantics in pattern matching. The pattern `not SomeEnum.Value` matches `null` in C# because `null != SomeEnum.Value` is true, whereas in TypeScript the equivalent check is falsy for `undefined`/`null`. Use `not null` to match Showdown's truthiness checks.
