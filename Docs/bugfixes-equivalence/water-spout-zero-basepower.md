# Water Spout/Eruption/Dragon Energy Returning Base Power 0 at Very Low HP

**Commit:** `5e681777`
**Date:** 2026-03-13

## Problem
HP-scaling moves (Water Spout, Eruption, Dragon Energy) computed a base power of 0 when the user had very low HP (e.g., 1 HP with a large max HP). This triggered an early return in `GetDamage` that skipped damage entirely, causing missing type effectiveness messages and damage protocol lines.

## Root Cause
The base power formula `move.BasePower * source.Hp / source.MaxHp` used integer division. When `source.Hp` is 1 and `source.MaxHp` is large (e.g., 300+), the expression `150 * 1 / 300` evaluates to `0` due to integer truncation. Showdown's JavaScript implementation uses `clampIntRange(Math.floor(Math.max(1, hp) * 150 / maxhp), 1)` to guarantee a minimum base power of 1, and also ensures `Math.max(1, hp)` is applied first.

## Fix
Reordered the computation to `Math.Max(1, source.Hp) * move.BasePower / source.MaxHp` and added `battle.ClampIntRange(bp, 1, null)` to enforce a minimum base power of 1, matching Showdown's clamping behavior. Applied to all three moves.

## Files Changed
- `ApogeeVGC/Data/Moves/MovesDEF.cs` -- Fixed Dragon Energy and Eruption base power callbacks with clamping
- `ApogeeVGC/Data/Moves/MovesVWX.cs` -- Fixed Water Spout base power callback with clamping

## Pattern
Integer division truncation: intermediate integer arithmetic producing 0 where a minimum of 1 is required by the source engine.
