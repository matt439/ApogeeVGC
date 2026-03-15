# Simultaneous Faint Win Condition Reversed

**Commit:** `b77b93a6`
**Date:** 2026-03-12

## Problem
When both sides lost their last Pokemon simultaneously (e.g., from recoil or Life Orb), the wrong side was declared the winner. The simulator was awarding the win to the foe of the last-fainted Pokemon's side, when it should have awarded it to the last-fainted Pokemon's own side.

## Root Cause
In Gen 5+, Showdown calls `this.win(faintData.target.side)` -- the side whose Pokemon fainted last wins, because that Pokemon dealt the final KO before dying to indirect damage. The C# code incorrectly called `Win(faintData.Target.Side.Foe)`, reversing the winner.

## Fix
Changed `Win(faintData.Target.Side.Foe)` to `Win(faintData.Target.Side)` to match Showdown's semantics.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/Battle.Fainting.cs` — correct the side passed to Win() in simultaneous faint case

## Pattern
Inverted logic: A simple semantic inversion where "the side that fainted last loses" was implemented instead of "the side that fainted last wins." Direct comparison with Showdown source is needed for win/loss determination logic.
