# Hustle Using ChainModify Instead of Direct Modify

**Commit:** `edf7b266`
**Date:** 2026-03-12

## Problem
Hustle's attack multiplier was applied using `ChainModify`, producing different intermediate rounding from Showdown and causing damage values to diverge in equivalence tests.

## Root Cause
Showdown explicitly notes that Hustle's 1.5x attack modifier should be applied directly to the stat rather than chained with other modifiers. `ChainModify` compounds with other modifiers before rounding, while `Modify` applies and truncates independently, yielding different final values due to integer truncation order.

## Fix
Changed the `OnModifyAtk` handler for Hustle from calling `battle.ChainModify(1.5)` (returning `VoidReturn`) to calling `battle.Modify(atk, 1.5)` and returning the modified value directly.

## Files Changed
- `ApogeeVGC/Data/Abilities/AbilitiesGHI.cs` -- switched Hustle's OnModifyAtk from ChainModify to direct Modify

## Pattern
ChainModify vs Modify mismatch: some Showdown modifiers intentionally bypass the chain to apply directly to a stat. Check Showdown comments for "applied directly" notes.
