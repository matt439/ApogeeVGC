# Curse Move Target Type in Request Data

**Commit:** `600b48c4`
**Date:** 2026-03-11

## Problem
Non-Ghost Pokemon using Curse showed the wrong target type in the move request data. The request listed Curse as targeting "normal" (an adjacent opponent), but Showdown shows it as targeting "self" for non-Ghost users. This caused the choice validation logic to incorrectly require a target location for Curse.

## Root Cause
Showdown's `getMoves()` overrides Curse's target to `self` (via `move.nonGhostTarget`) when the user is not Ghost-type, and the choice validation in `chooseMove()` reads the target type from the request data. The C# code was not applying this override in the request builder, and the choice validator was reading the target type from the base move definition instead of the request.

## Fix
Added a target override in `Pokemon.GetMoves()` that sets Curse's target to `NonGhostTarget ?? MoveTarget.Self` for non-Ghost users (and also handles Terapagos-Stellar's TeraStarStorm spread targeting). Updated `Side.ChooseMove()` to read the target type from the request's move data instead of the base move definition.

## Files Changed
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Moves.cs` — Add Curse and TeraStarStorm target overrides in `GetMoves()`
- `ApogeeVGC/Sim/SideClasses/Side.Choices.cs` — Read target type from request moves instead of base move definition

## Pattern
Request data not reflecting runtime move property overrides. Showdown's request builder applies special-case overrides (like Curse's non-Ghost targeting) that must be mirrored in the C# request builder, and validators must read from the request rather than the static move data.
