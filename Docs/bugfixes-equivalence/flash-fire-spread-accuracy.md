# Flash Fire Not Setting move.accuracy=true for Spread Moves

**Commit:** `396ee8a6`
**Date:** 2026-03-12

## Problem
When a spread Fire move (e.g., Heat Wave) hit a Flash Fire Pokemon first, subsequent targets still underwent an accuracy roll, causing an extra PRNG call that desynchronized the random number sequence from Showdown.

## Root Cause
In Showdown, Flash Fire's `onTryHit` handler sets `move.accuracy = true` when absorbing a Fire-type move. This bypasses the accuracy check for remaining targets of the same spread move. The C# implementation had a comment claiming that `Accuracy` was init-only and could not be modified, but the property actually had a public setter. Without setting accuracy to `true`, the accuracy step still consumed a PRNG call for the remaining targets.

## Fix
Added `move.Accuracy = IntTrueUnion.FromTrue()` to Flash Fire's `OnTryHit` handler, matching Showdown's behavior of skipping accuracy checks for remaining targets after absorption.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesDEF.cs` -- Set `move.Accuracy = IntTrueUnion.FromTrue()` in Flash Fire's `OnTryHit` handler

## Pattern
Missing state mutation: a move property that should have been set during ability processing was left unchanged due to an incorrect assumption about property accessibility.
