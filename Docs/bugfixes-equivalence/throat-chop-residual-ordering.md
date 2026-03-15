# Throat Chop Residual Ordering

**Commit:** `39557140`
**Date:** 2026-03-12

## Problem
Throat Chop's residual (end-of-turn duration tick) was firing at the wrong point in the residual order, causing the volatile's end message to appear at a different position in the protocol log compared to Showdown.

## Root Cause
Showdown defines Throat Chop's residual with `order: 22`, but the C# code had the `OnResidualOrder = 22` line commented out and no residual handler, so the volatile's duration was ticking at the default residual order instead.

## Fix
Added an explicit `OnResidual` handler with `order: 22` using `OnResidualEventInfo.Create`, replacing the commented-out `OnResidualOrder` property. The handler body is empty (the framework handles duration ticking), but the order parameter ensures correct sequencing.

## Files Changed
- `ApogeeVGC/Data/Conditions/ConditionsSTU.cs` — Add `OnResidual` with `order: 22` to Throat Chop volatile

## Pattern
Missing or commented-out event ordering parameter. Showdown's residual system relies on explicit `order` values to sequence end-of-turn effects; omitting them causes silent reordering that only shows up in protocol log comparison.
