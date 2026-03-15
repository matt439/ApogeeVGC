# Null didSomething Not Converted to False in RunMoveEffects Damage Combining

**Commit:** `d59c3ee1dfaa5d2c7f6c9e970d49d1d79af28abf`
**Date:** 2026-03-12

## Problem
When `didSomething` was `null` (e.g. Clear Amulet blocking all stat drops), it was passed directly to `CombineResults` without being converted to `false` first, causing targets whose effects were fully blocked to not be properly filtered out of the `[spread]` tag target list.

## Root Cause
Showdown line 1316 explicitly converts null before combining: `damage[i] = this.combineResults(damage[i], didSomething === null ? false : didSomething)`. The C# code was passing the null `didSomething` through to `CombineResults` directly. Since `null` has different combining semantics than `false` (null can be overridden by higher-priority values while false is definitive), this caused the damage entry to retain a truthy value even when the move's effects were completely blocked on that target.

## Fix
Added an explicit null-to-false conversion for `didSomething` before passing it to `CombineResults` for the `damage[i]` update, matching Showdown's ternary. The `didAnything` combination still uses the unconverted value, matching Showdown line 1317.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveEffects.cs` -- Added `NullBoolIntUndefinedUnion` check to convert null didSomething to false before damage combining

## Pattern
Missing explicit null-to-false coercion that Showdown performs inline before a specific combining operation.
