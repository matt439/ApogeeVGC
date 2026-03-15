# CheckFainted Missing Status Clear for Fainted Pokemon

**Date:** 2026-03-13

## Problem
When a fainted Pokemon remained in `side.Active` (no replacement available, e.g., doubles 2v1), its status condition (burn, poison, etc.) continued to generate event handlers in `FieldEvent` residual processing. This caused incorrect SpeedSort ordering because the extra handler shifted the positions of other handlers (like Tailwind) during selection sort, changing the pre-shuffle order of tied elements.

In the failing test (seed 2732/battle 028477), fainted Tauros's Burn handler (Order=10) caused the selection sort to swap Tailwind P1 from index 0 to a later index. When the two tied Tailwinds were found in the next pass, P2 appeared before P1 in the pre-shuffle order, producing the wrong `|-sideend|` sequence.

## Root Cause
Showdown's `checkFainted()` sets `pokemon.status = 'fnt'` for all fainted Pokemon in `side.active`, which prevents `findPokemonEventHandlers` from finding status handlers (the `fnt` condition has no `onResidual` callback). C#'s `CheckFainted()` only set `pokemon.SwitchFlag = true` without clearing the status.

## Fix
Added `pokemon.Status = ConditionId.None` in `CheckFainted()` for fainted Pokemon, matching Showdown's behavior of overwriting the status. This prevents the fainted Pokemon's status handlers from appearing in subsequent `FieldEvent` handler lists.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/Battle.Fainting.cs` -- set Status to None in CheckFainted for fainted Pokemon

## Pattern
Missing state cleanup on faint: Showdown performs cleanup in `checkFainted()` that the C# port omitted. Any faint-related state (status, switchFlag, etc.) that Showdown clears in `checkFainted` must be mirrored in C#.
