# Sync Simulator Request After Battle End Fix

## Problem Summary
When running a synchronous battle test (`SyncSimulator`), the battle crashed with `InvalidOperationException: Cannot request choices from players: Some sides have no active request` after one side's last Pokémon fainted and switched back in while already fainted. This occurred specifically when `GetRequests()` detected a win condition and called `Lose()`, but the simulator still attempted to request player choices.

## Symptoms
- Battle executes normally until a side loses all Pokémon
- `GetRequests()` detects the loss condition and calls `Lose(side)`
- Battle properly sets `Ended = true` and `Winner` field
- Control returns to `SyncSimulator.Run()` which attempts to call `RequestPlayerChoices()`
- Exception thrown: `Cannot request choices from players: Some sides have no active request`

## Debug Output Pattern
```
[GetMoveRequestData] Calyrex-Ice: Has ChoiceLock=False, Item=Leftovers
[GetMoveRequestData] Calyrex-Ice: After DisableMove event, move states:
  - Glacial Lance: Disabled=BoolBoolHiddenUnion { Value = False }
  - Leech Seed: Disabled=BoolBoolHiddenUnion { Value = False }
  - Trick Room: Disabled=BoolBoolHiddenUnion { Value = False }
  - Protect: Disabled=BoolBoolHiddenUnion { Value = False }
ERROR: Random 2 has no active non-fainted Pokemon during move request phase
  PokemonLeft: 0
  Active Pokemon: Calyrex-Ice(Fainted:True)
  Available switches: 0
  Random 2 has no Pokemon left - they lose
Generated 0 requests
[MakeRequest] EXIT: queue size = 0
TurnLoop returned
Battle error: InvalidOperationException: Cannot request choices from players: Some sides have no active request
```

## Root Cause
There are **two interconnected issues**:

### Issue 1: `Lose()` doesn't end the battle when `PokemonLeft` is already 0
In `Battle.Fainting.cs`, the `Lose()` method has an early return:

```csharp
public bool Lose(Side? side)
{
    if (side is null) return false;
    
    // Already no Pokémon left
    if (side.PokemonLeft <= 0) return false;  // <-- PROBLEM!
    
    side.PokemonLeft = 0;
    // ... calls FaintMessages() which calls CheckWin() which calls Win()
}
```

When `GetRequests()` detects a side has no active Pokemon and `PokemonLeft = 0`, it calls `Lose(side)`. However, `Lose()` immediately returns `false` without ending the battle, leaving `Ended = false`.

### Issue 2: Simulator doesn't check if battle ended during request generation
In `SyncSimulator.Run()`, the loop structure is:

```csharp
while (!Battle.Ended)
{
    if (Battle.RequestState != RequestState.None)
    {
        Battle.RequestPlayerChoices();  // Throws if no requests exist
    }
}
```

The problem flow:
1. `EndTurn()` calls `MakeRequest(RequestState.Move)` which sets `RequestState = Move`
2. `MakeRequest()` calls `GetRequests(RequestState.Move)`
3. `GetRequests()` detects side has no active non-fainted Pokémon and `PokemonLeft = 0`
4. `GetRequests()` calls `Lose(side)` which returns `false` without setting `Ended = true` ?
5. `GetRequests()` returns empty list
6. `MakeRequest()` checks `if (Ended)` but it's still `false`, so continues
7. `MakeRequest()` completes and returns to `SyncSimulator.Run()`
8. Loop checks `while (!Battle.Ended)` - true, continues
9. Checks `if (Battle.RequestState != RequestState.None)` - true (still set to `Move`)
10. Calls `Battle.RequestPlayerChoices()` ? but there are no active requests
11. `RequestPlayerChoices()` throws exception

## Solution
**Two fixes were required:**

### Fix 1: Call `Win()` directly in `GetRequests()` (Primary Fix)
In `Battle.Requests.cs`, when detecting a side has lost, call `Win(side.Foe)` instead of `Lose(side)`:

```csharp
if (availableSwitches == 0 && side.PokemonLeft <= 0)
{
    // This side has lost - end the battle
    // Note: We call Win() directly on the foe because Lose() checks if PokemonLeft <= 0
    // and returns early without ending the battle if it's already 0
    Debug($"  {side.Name} has no Pokemon left - they lose");
    Win(side.Foe);  // Changed from Lose(side)
    return new List<IChoiceRequest>();
}
```

This ensures `Ended = true` is properly set when the battle ends during request generation.

### Fix 2: Add defensive check in `SyncSimulator.Run()` (Secondary Fix)
Added a battle-ended check immediately before calling `RequestPlayerChoices()`:

```csharp
while (!Battle.Ended)
{
    if (Battle.RequestState != RequestState.None)
    {
        // Double-check that battle hasn't ended before requesting choices
        // This can happen if GetRequests() detects a win/loss condition
        if (Battle.Ended)
        {
            break;
        }

        Battle.RequestPlayerChoices();
        // ...
    }
}
```

This provides defense-in-depth, protecting against similar issues in other code paths.

## Why These Fixes Are Correct

### Fix 1: Calling `Win()` directly
- **Correct Logic**: When `PokemonLeft = 0`, the side has already lost - no need for `Lose()` to set it to 0 again
- **Proper Battle End**: `Win()` properly sets `Ended = true`, emits events, and clears request state
- **Matches Intent**: The code's intent is to end the battle, not to force a side to lose when they've already lost

### Fix 2: Defensive check in simulator
- **Defense-in-Depth**: Protects against similar issues in other code paths
- **State Consistency**: Handles cases where `RequestState` is set before battle ends
- **Minimal Change**: Single check prevents exceptions without changing battle lifecycle logic
- **TypeScript Alignment**: TypeScript Pokémon Showdown likely has similar checks

## Alternative Approaches Considered
1. **Fix `Lose()` to handle `PokemonLeft = 0`**: Would change battle lifecycle semantics
2. **Clear `RequestState` in `Lose()`**: More invasive, affects multiple systems
3. **Check `Ended` in `RequestPlayerChoices()`**: Would mask the problem rather than fix root cause

## Testing
Run the test that triggered the issue:
```csharp
// ApogeeVGC.Sim.Core.Driver.RunRandomVsRandomSinglesTest()
const bool debug = true;
var battleOptions = new BattleOptions
{
    Id = FormatId.CustomSingles,
    Player1Options = new PlayerOptions
    {
        Type = Player.PlayerType.Random,
        Team = TeamGenerator.GenerateTestTeam(Library),
        Seed = new PrngSeed(PlayerRandom1Seed),
    },
    Player2Options = new PlayerOptions
    {
        Type = Player.PlayerType.Random,
        Team = TeamGenerator.GenerateTestTeam(Library),
        Seed = new PrngSeed(PlayerRandom2Seed),
    },
    Debug = debug,
    Sync = true,
    Seed = new PrngSeed(BattleSeed),
};
var simulator = new SyncSimulator();
SimulatorResult result = simulator.Run(Library, battleOptions, printDebug: debug);
```

**Expected Result**: Battle completes successfully, determines winner correctly, no exception thrown.

## Related Bug Fixes
- **Battle End Condition Null Request Fix**: Similar issue where battle ending mid-loop caused choice requests for losing player
- **Endless Battle Loop Fix**: Also involved battle not properly detecting end conditions during request generation

## Files Modified
- `ApogeeVGC/Sim/BattleClasses/Battle.Requests.cs` - Changed `Lose(side)` to `Win(side.Foe)` in `GetRequests()`
- `ApogeeVGC/Sim/Core/SyncSimulator.cs` - Added `if (Battle.Ended)` check before `RequestPlayerChoices()`

## Impact
- **Severity**: High (crashes synchronous battles when win condition detected during request generation)
- **Scope**: All synchronous battle simulations, primarily affects AI training and testing
- **Breaking Changes**: None
- **Performance**: Negligible (single boolean check)

## Keywords
`SyncSimulator`, `RequestPlayerChoices`, `InvalidOperationException`, `no active request`, `battle end`, `GetRequests`, `Lose`, `RequestState`, `battle lifecycle`, `synchronous simulation`, `win condition`, `request generation`, `state consistency`, `defensive programming`

---

**Date Fixed**: 2025-01-19  
**Fixed By**: GitHub Copilot  
**Related Issue**: Exception when testing battle with `SyncSimulator`
