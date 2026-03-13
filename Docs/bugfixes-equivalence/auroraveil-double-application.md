# Aurora Veil Double Application When Both Sides Have It

**Date:** 2026-03-14

## Problem
When both sides had Aurora Veil active simultaneously, C# applied the damage reduction twice (once per side's handler), resulting in less damage than Showdown.

## Root Cause
Aurora Veil's `OnAnyModifyDamage` handler used `target.Side.GetSideCondition(ConditionId.AuroraVeil) != null` as its guard. Since `OnAny` handlers fire from ALL sides' conditions, when both sides had Aurora Veil, the handler fired twice. Both times, the guard checked the defender's side (which always has Aurora Veil), so both applications succeeded.

Showdown uses `this.effectState.target.hasAlly(target)` to verify the handler is executing from the *defender's own side's* Aurora Veil, not the attacker's.

## Fix
Changed the guard to `battle.EffectState.Target is SideEffectStateTarget side && side.Side.HasAlly(target)`, matching the pattern already used by Light Screen and Reflect handlers in the same codebase.

## Files Changed
- `ApogeeVGC/Data/Conditions/ConditionsABC.cs` -- Aurora Veil OnAnyModifyDamage guard condition

## Pattern
Side condition `OnAny` handlers must verify which side's condition is executing via `battle.EffectState.Target`, not by checking the target Pokemon's side directly. The Light Screen and Reflect handlers had the correct pattern already.
