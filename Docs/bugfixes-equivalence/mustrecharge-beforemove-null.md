# MustRecharge OnBeforeMove Not Preventing Move Execution

**Commit:** `62fcd862`
**Date:** 2026-03-12

## Problem
The MustRecharge condition's `OnBeforeMove` handler returned C# `null`, which was not recognized as a falsy result by the move execution code. This allowed a recharging Pokemon to still execute its move on the recharge turn, diverging from Showdown.

## Root Cause
Same JS-null-is-falsy issue as TryAddVolatile: the MustRecharge handler returned `null` to signal "block this move," but `BattleActions.Moves.cs` only checked for `BoolRelayVar { Value: false }` or raw `null`. The raw `null` from the handler wrapper was not consistently reaching the check. Showdown's handler returns `null` which is falsy, preventing the move.

## Fix
Changed the MustRecharge `OnBeforeMove` handler to return `new NullRelayVar()` instead of `null`, and updated the `willTryMove` check in `BattleActions.Moves.cs` to also match `NullRelayVar`.

## Files Changed
- `ApogeeVGC/Data/Conditions/ConditionsMNO.cs` -- return NullRelayVar instead of null
- `ApogeeVGC/Sim/BattleClasses/BattleActions.Moves.cs` -- added NullRelayVar to the move-prevention check

## Pattern
JS null vs C# null semantics: handler returns meant to be falsy (blocking) must use NullRelayVar rather than C# null to be correctly detected by downstream pattern matching.
