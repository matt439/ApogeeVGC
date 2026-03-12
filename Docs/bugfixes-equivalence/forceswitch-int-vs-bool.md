# ForceSwitch canSwitch Int vs Bool

**Commit:** `a482a92c`
**Date:** 2026-03-12

## Problem
Force-switch moves (e.g., Whirlwind, Dragon Tail) were not correctly emitting the `[still]` tag when the target had no Pokemon to switch to. The move's fail path was not triggering because the integer return value of `CanSwitch` was being used directly as a hit result instead of being converted to a boolean.

## Root Cause
In Showdown, `canSwitch()` returns an integer count, but the force-switch logic coerces it to boolean with `!!this.battle.canSwitch(target.side)`. The C# code was storing the raw int in `hitResult`, so a count of 0 was treated differently from a boolean `false`, causing the downstream fail/still logic to diverge.

## Fix
Changed the `CanSwitch` result from `int` to `bool` via `!= 0`, then wrapped it with `BoolIntUndefinedUnion.FromBool()`. Also removed a redundant else branch in `forceSwitch()` that set `damage[i] = false` for live targets that can't switch, since this case is already handled by `RunMoveEffects`.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/BattleActions.MoveEffects.cs` — convert CanSwitch int to bool; remove redundant else branch

## Pattern
JS truthiness coercion mismatch: JavaScript's `!!intValue` converts 0 to false, but C# treats 0 as a valid non-null integer. Anywhere Showdown coerces a count/int to boolean, the C# port must do so explicitly.
