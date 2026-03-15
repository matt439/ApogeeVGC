# RunEvent Incorrectly Skipping Fainted Pokemon Handlers

**Date:** 2026-03-14

## Problem
Magician ability could steal items from fainted Pokemon even when the item had an `OnTakeItem` guard preventing theft (e.g., Sky Plate on Arceus). This caused protocol divergence: C# emitted `|-item|` for the stolen item while Showdown correctly skipped the steal and continued to residual effects like Leech Seed damage.

## Root Cause
C#'s `RunEvent` had a fainted Pokemon handler skip that was too aggressive. The check:
```csharp
if (effectHolder is PokemonEffectHolder { Pokemon.Fainted: true })
{
    if (handler.State?.IsSlotCondition != true) continue;
}
```
was present in `RunEvent`, causing ALL event handlers from fainted Pokemon to be skipped.

In Showdown, this fainted check (`battle.ts` line 512) only exists in `fieldEvent` (used for Residual, SwitchIn, Weather iterations), NOT in `runEvent` (used for targeted events like TakeItem).

When Magician called `TakeItem` on a fainted Arceus holding Sky Plate, C# skipped the Sky Plate's `OnTakeItem` handler (which returns false for species #493 Arceus), allowing the steal to proceed. Showdown correctly executed the handler, blocking the steal.

## Fix
Removed the fainted handler skip from `RunEvent`. The check remains in `FieldEvent` where it belongs — matching Showdown's architecture where only `fieldEvent` skips fainted Pokemon handlers.

## Files Changed
- `ApogeeVGC/Sim/BattleClasses/Battle.Events.cs` — Removed fainted Pokemon handler skip from RunEvent

## Impact
This fix affects any battle where an event (like TakeItem, AfterTakeItem, etc.) targets a fainted Pokemon that has handlers registered on its item, ability, or conditions. The most common case is item-stealing abilities (Magician, Pickpocket) interacting with items that have OnTakeItem guards (Plates, Mega Stones, Z-Crystals).

## Pattern
Showdown separates event processing into `runEvent` (targeted, no fainted skip) and `fieldEvent` (iterates all active Pokemon, skips fainted). C# must maintain this same separation — fainted handler skips should only apply during field-level event iteration.
