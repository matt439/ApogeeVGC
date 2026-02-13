# Hidden-Disabled Move Endless Loop Fix

## Summary
**Severity**: Critical  
**Systems Affected**: Choice validation, autoChoose, PlayerRandom, SyncSimulator  
**Trigger**: Imprison condition with last-active Pokemon in doubles  

## Problem

An endless loop occurred when a Pokemon's only usable move was hidden-disabled by Imprison. The `PlayerRandom` (and `Side.ChooseMove`'s autoChoose path) repeatedly selected the hidden-disabled move, which was then rejected by the battle engine's validation, creating an infinite retry cycle.

**Reproduction**: Player1Seed=1453, Player2Seed=1454, BattleSeed=1455

## Root Cause

Three interrelated issues:

### 1. AutoChoose only checked the restricted request view

The `Side.ChooseMove` autoChoose logic (used by both `AutoChoose()` and `SyncSimulator` fallback) only checked the restricted request data (`request.Moves`) for disabled moves. It did NOT cross-check against the unrestricted moves list from `pokemon.GetMoves()`.

In TypeScript, autoChoose checks **both** views (side.ts lines 590-597):
```typescript
if (autoChoose) {
    for (const [i, move] of request.moves.entries()) {
        if (move.disabled) continue;                    // restricted view
        if (i < moves.length && move.id === moves[i].id && moves[i].disabled) continue; // unrestricted view
        moveid = move.id;
        break;
    }
}
```

The C# implementation was missing the second check. When `isLastActive=true`, hidden-disabled moves (from Imprison's `HiddenBoolHiddenUnion`) appear as `Disabled = null` (not disabled) in the restricted request, but as `Disabled = BoolMoveIdBoolUnion(true)` (disabled) in the unrestricted `GetMoves()` output.

### 2. SyncSimulator had no fallback for rejected choices

`SyncSimulator.OnChoiceRequested` called `Battle.Choose()` but did not handle the failure case. When the player's choice was rejected, the main loop would call `RequestPlayerChoices()` again, which re-triggered `OnChoiceRequested` with the same (or similar) request data, leading to the same invalid choice.

### 3. PlayerRandom.IsDisabled was incomplete

`PlayerRandom.IsDisabled` only handled `BoolMoveIdBoolUnion` but not `MoveIdMoveIdBoolUnion` (which indicates a move disabled by a specific source move). Changed to use the `MoveIdBoolUnion.IsTrue()` method which handles all variants.

## Solution

### Side.Choices.cs — AutoChoose dual-check
Added the unrestricted moves list check to the autoChoose path, matching the TypeScript implementation:

```csharp
if (autoChoose)
{
    for (int i = 0; i < request.Moves.Count; i++)
    {
        PokemonMoveData pokemonMoveData = request.Moves[i];

        // Skip if disabled in the restricted request view
        if (pokemonMoveData.Disabled is MoveIdMoveIdBoolUnion or BoolMoveIdBoolUnion { Value: true })
            continue;

        // Also skip if disabled in the unrestricted moves list (catches hidden-disabled)
        if (i < moves.Count && pokemonMoveData.Id == moves[i].Id &&
            moves[i].Disabled is MoveIdMoveIdBoolUnion or BoolMoveIdBoolUnion { Value: true })
            continue;

        moveid = pokemonMoveData.Id;
        break;
    }
}
```

### SyncSimulator.cs — Fallback to AutoChoose
Added fallback logic so that when `Battle.Choose()` fails, `side.AutoChoose()` is used:

```csharp
if (!Battle.Choose(e.SideId, choice))
{
    side.AutoChoose();
    choice = side.GetChoice();
    Battle.Choose(e.SideId, choice);
}
```

### PlayerRandom.cs — Improved IsDisabled
Simplified `IsDisabled` to handle all `MoveIdBoolUnion` variants:

```csharp
private static bool IsDisabled(MoveIdBoolUnion? disabled)
{
    return disabled is not null && disabled.IsTrue();
}
```

## Key Insight

The `HiddenBoolHiddenUnion` disabled state (used by Imprison) is designed to hide move availability information from the opposing player in the UI. When `restrictData=true` (for the last active Pokemon), this hidden state is converted to "not disabled" in the request data. But the battle engine's validation always uses the unrestricted view where the move IS disabled. Any code that makes choices must check both views to avoid this discrepancy.

## Files Modified
- `ApogeeVGC/Sim/SideClasses/Side.Choices.cs` — AutoChoose dual-check
- `ApogeeVGC/Sim/Core/SyncSimulator.cs` — Fallback to AutoChoose on failed choice
- `ApogeeVGC/Sim/Player/PlayerRandom.cs` — Improved IsDisabled method

## Keywords
`endless loop`, `infinite loop`, `hidden disabled`, `Imprison`, `HiddenBoolHiddenUnion`, `autoChoose`, `PlayerRandom`, `SyncSimulator`, `isLastActive`, `restrictData`, `GetMoves`, `GetMoveRequestData`, `DisableMove`, `MaybeDisabled`
