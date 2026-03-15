# Missing Paradox Tags on Gen 9 Paradox Pokemon (984-995)

**Commit:** `cb7eae62`
**Date:** 2026-03-13

## Problem
Booster Energy could be swapped away from Paradox Pokemon via Trick when it should have been unremovable, because the `onTakeItem` check for Paradox species was failing.

## Root Cause
Twelve Gen 9 Paradox Pokemon (Great Tusk, Scream Tail, Brute Bonnet, Flutter Mane, Slither Wing, Sandy Shocks, Iron Treads, Iron Bundle, Iron Hands, Iron Jugulis, Iron Moth, and Iron Thorns -- species IDs 984-995) were missing `Tags = [SpeciesTag.Paradox]` in their species data definitions. Booster Energy's `onTakeItem` handler checks for this tag to determine whether the item is removable; without it, the check returned false, allowing Trick to succeed.

## Fix
Added `Tags = [SpeciesTag.Paradox]` to all 12 affected species entries in `SpeciesData0951to1000.cs`.

## Files Changed
- `ApogeeVGC/Data/SpeciesData/SpeciesData0951to1000.cs` -- Added `Tags = [SpeciesTag.Paradox]` to 12 Paradox Pokemon entries (Great Tusk through Iron Thorns)

## Pattern
Missing data annotation -- species metadata tag omitted from data definitions, causing downstream item interaction logic to fail.
