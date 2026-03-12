# Ogerpon Mask OnTakeItem BaseSpecies Chain

**Commit:** `282d9515`
**Date:** 2026-03-12

## Problem
Ogerpon's mask items (Cornerstone Mask, Hearthflame Mask, Wellspring Mask) could be removed by effects like Knock Off, even though Showdown prevents this. The `OnTakeItem` check was failing to identify Ogerpon mask forms.

## Root Cause
The check used `pokemon.BaseSpecies.Id == SpecieId.Ogerpon`, but for Ogerpon mask forms (e.g., Ogerpon-Cornerstone), `BaseSpecies.Id` returns the form ID (e.g., `SpecieId.OgerponCornerstone`), not the root species. Showdown's `baseSpecies` property walks up the species chain to the root, so checking `baseSpecies === 'Ogerpon'` matches all forms.

## Fix
Changed the check to `pokemon.BaseSpecies.BaseSpecies == SpecieId.Ogerpon`, which follows the species chain one level further to reach the root species, matching Showdown's behavior.

## Files Changed
- `ApogeeVGC/Data/Items/ItemsABC.cs` — Fix Cornerstone Mask `OnTakeItem` check
- `ApogeeVGC/Data/Items/ItemsGHI.cs` — Fix Hearthflame Mask `OnTakeItem` check
- `ApogeeVGC/Data/Items/ItemsVWX.cs` — Fix Wellspring Mask `OnTakeItem` check

## Pattern
Species identity chain traversal mismatch. In Showdown, `baseSpecies` recursively resolves to the root species, but in C# the `BaseSpecies` property only goes one level. For forme-based checks, the C# code must traverse `BaseSpecies.BaseSpecies` to match.
