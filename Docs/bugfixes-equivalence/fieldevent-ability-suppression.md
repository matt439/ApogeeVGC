# FieldEvent Missing Ability/Item Suppression Checks

**Commit:** `0c5c37192dbe34eb49c3b72e201ed14a9ccc773a`
**Date:** 2026-03-13

## Problem
`FieldEvent`'s residual processing loop invoked ability handlers without checking whether the ability was suppressed, causing Hunger Switch to fire on a transformed Ditto (which has `NoTransform=true` and should have its ability suppressed).

## Root Cause
Showdown's `runEvent` checks `IgnoringAbility()` and `IgnoringItem()` before invoking each handler, skipping handlers for Pokemon whose ability or item is suppressed (e.g. via Gastro Acid, Neutralizing Gas, or Transform's `NoTransform` flag). The `FieldEvent` method in C# was missing these suppression checks, even though `RunEvent` already had them. This meant abilities like Hunger Switch would incorrectly activate during residual processing on Ditto, which should have had its ability suppressed due to Transform.

## Fix
Added `IgnoringAbility()` and `IgnoringItem()` checks before handler invocation in `FieldEvent`, matching Showdown's `runEvent` behavior.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/Battle.Events.cs` -- Added ability and item suppression guard clauses in the FieldEvent handler loop

## Pattern
Missing parity check between parallel event dispatch paths (FieldEvent vs RunEvent) for ability/item suppression.
