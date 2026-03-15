# ChainModify Returning Fixed-Point Value That Corrupted Relay Variables

**Commit:** `a3c3739f`
**Date:** 2026-03-12

## Problem
Damage modifiers applied via `ChainModify` were causing exponential damage inflation. For example, Glaive Rush's 2x damage modifier turned 80 damage into 16384 instead of 160.

## Root Cause
`ChainModify` returned the `nextMod` fixed-point integer (e.g., 8192 for a 2x modifier). Expression-bodied event handlers like Glaive Rush's `OnSourceModifyDamage` were implicitly returning this fixed-point value as their handler result. The event system interpreted the returned value as the new relay variable (the damage value), replacing the actual modified damage. Then `FinalModify` applied the modifier again on top of this corrupted value, causing double (or exponential) application. In Showdown, `chainModify` returns `undefined`, and handlers returning `undefined` leave the relay variable unchanged.

## Fix
Changed all three `ChainModify` overloads (int, int[], double) to return `void` instead of `double`, matching Showdown's behavior where `chainModify` returns `undefined`. Updated event handlers in Glaive Rush and Metronome that relied on expression-bodied syntax to use block bodies with explicit `return null` statements.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/Battle.Modifiers.cs` -- Changed `ChainModify(int)`, `ChainModify(int[])`, and `ChainModify(double)` return types from `double` to `void`
- `ApogeeVGC/Data/Conditions/ConditionsGHI.cs` -- Glaive Rush `OnSourceModifyDamage` handler changed to block body with `return null`
- `ApogeeVGC/Data/Conditions/ConditionsMNO.cs` -- Metronome condition handler changed to block body with `return null`

## Pattern
Return value leaking into event relay: a helper method's return value being inadvertently propagated as the event handler's result, corrupting the relay variable.
