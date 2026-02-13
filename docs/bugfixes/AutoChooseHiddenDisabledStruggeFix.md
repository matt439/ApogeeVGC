# AutoChoose Hidden-Disabled Struggle Fix

## Summary
**Severity**: Critical  
**Systems Affected**: `Pokemon.GetMoves()`, `Side.ChooseMove` (autoChoose path)  
**Trigger**: All non-hidden-disabled moves have 0 PP while Imprison hides other disabled moves  

## Problem

`autoChoose chose a disabled move` exception when all of a Pokemon's moves are effectively disabled but the restricted request view (used for the last active Pokemon in doubles) hides the Imprison-disabled moves.

**Reproduction**: Player1Seed=25834, Player2Seed=25835, BattleSeed=25836, TestingMove=LunarDance

## Root Cause

`Pokemon.GetMoves()` returned `[Struggle]` (single-item list) when no valid moves existed, diverging from the TypeScript `getMoves()` which returns `[]` (empty list).

This caused a mismatch between `request.Moves` (from `GetMoveRequestData`, which uses `restrictData=true` for the last active Pokemon) and `moves` (from `GetMoves(restrictData=false)` in `ChooseMove`):

- `request.Moves` = 4 items (hidden-disabled moves appear as NOT disabled)
- `moves` = 1 item `[Struggle]` (all moves disabled, returns Struggle instead of empty)

The autoChoose loop's index-based comparison (`moves[i].Id == request.Moves[i].Id`) failed because:
- `i=0`: `request.Moves[0].Id = Imprison` ≠ `moves[0].Id = Struggle` → comparison skipped
- Since the IDs don't match, the hidden-disabled check never triggered
- `moveid` was set to `Imprison` (a hidden-disabled move)
- Step 10's disabled check found `Imprison` was not in `moves` (which only contained `Struggle`) → `isEnabled = false` → threw exception

## Solution

### Pokemon.Moves.cs — Return empty list instead of [Struggle]
Changed `GetMoves()` to return `[]` when no valid moves exist, matching TypeScript behavior (`pokemon.ts:1020`). Both callers (`ChooseMove` and `GetMoveRequestData`) already have Struggle fallback logic that handles empty returns.

### Side.Choices.cs — Use IsTrue() for disabled checks
Replaced pattern matching (`is MoveIdMoveIdBoolUnion or BoolMoveIdBoolUnion { Value: true }`) with `IsTrue()` method calls in both the autoChoose loop and Step 10's disabled check. This is more robust and handles all `MoveIdBoolUnion` subtypes uniformly, matching the TypeScript truthy check.

## Why this fixes the bug

When `GetMoves()` returns `[]`:
1. The autoChoose loop's second check `i < moves.Count` is always `false` (moves is empty), so it only relies on the first check (request-view disabled state)
2. Since hidden-disabled moves appear as NOT disabled in the restricted request, they pass the first check
3. `moveid` gets set to a hidden-disabled move (e.g., Imprison)
4. But then `ChooseMove`'s Struggle fallback at Step 9 (`moves.Count == 0`) triggers BEFORE the disabled check at Step 10
5. The Pokemon uses Struggle and `ChooseMove` returns `true`

This matches the TypeScript flow at `side.ts:655`: `else if (!moves.length) { ... struggle ... return true; }`

## Key Insight

The previous `HiddenDisabledMoveEndlessLoopFix` addressed the case where SOME moves were hidden-disabled but others were available. This fix addresses the edge case where ALL non-hidden-disabled moves also become unavailable (e.g., 0 PP), creating a complete deadlock that the autoChoose loop couldn't resolve.

## Files Modified
- `ApogeeVGC/Sim/PokemonClasses/Pokemon.Moves.cs` — Return `[]` instead of `[Struggle]`
- `ApogeeVGC/Sim/SideClasses/Side.Choices.cs` — Use `IsTrue()` in autoChoose loop and disabled check

## Keywords
`autoChoose`, `disabled move`, `Struggle`, `GetMoves`, `hidden disabled`, `Imprison`, `HiddenBoolHiddenUnion`, `restrictData`, `isLastActive`, `PP depletion`
