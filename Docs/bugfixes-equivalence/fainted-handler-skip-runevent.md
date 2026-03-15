# Fainted Pokemon Handler Skip in RunEvent and Well-Baked Body Priority

**Commit:** `e53e68738dd000e4104e1d752527414a8f3ebe47`
**Date:** 2026-03-13

## Problem
Fainted Pokemon's event handlers were not being skipped in `RunEvent`, causing abilities like Gooey to trigger on a fainted Wugtrio. Additionally, Well-Baked Body had an incorrect `priority=1` on its `OnTryHit` handler, causing wrong ordering versus Flash Fire in spread moves.

## Root Cause
Showdown's `runEvent` (battle.ts line 512) skips handlers belonging to fainted Pokemon unless the handler is a slot condition. The C# `RunEvent` implementation was missing this check entirely, so handlers for fainted Pokemon were still being invoked. Separately, Well-Baked Body's `OnTryHit` handler was defined with `priority=1`, but Showdown has no `onTryHitPriority` for this ability, so it should default to 0. The incorrect priority caused it to be ordered before Flash Fire when processing spread moves.

## Fix
Added a fainted Pokemon check in `RunEvent` that skips handlers unless the handler state is a slot condition. Removed the incorrect `priority=1` from Well-Baked Body's `OnTryHit` handler.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/Battle.Events.cs` -- Added fainted Pokemon handler skip with slot condition exception
- `ApogeeVGC/Data/Abilities/AbilitiesVWX.cs` -- Removed incorrect priority=1 from Well-Baked Body OnTryHit

## Pattern
Missing guard clause for fainted Pokemon in event dispatch, combined with incorrect handler priority metadata.
