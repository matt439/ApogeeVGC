# Endless Battle Loop Fix

## Problem Summary

The battle entered an infinite loop when one side had all active Pokemon fainted but still had Pokemon available in reserve. The battle would continue for 1000+ turns until hitting the turn limit, with one player receiving `WaitRequest` while the other player could switch Pokemon indefinitely.

**Symptoms**:
- Battle exceeds turn limit (1000 turns)
- Console shows: `WARNING: Random 1 has no active non-fainted Pokemon during move request phase`
- Console shows: `WARNING: Random 1 received WaitRequest but has fainted active Pokemon`
- One player continuously switches while the other waits
- Battle eventually throws `BattleTurnLimitException`

**Debug Output Example**:
```
[GetRequests] Checking 2 sides
WARNING: Random 1 has no active non-fainted Pokemon during move request phase
  PokemonLeft: 5
  This usually indicates a bug in the fainting/switching logic
Generated 2 requests
WARNING: Random 1 received WaitRequest but has fainted active Pokemon
This indicates the battle should have ended or switched before requesting moves
Assigned request to Random 1: WaitRequest
Assigned request to Random 2: MoveRequest
```

## Root Cause

The endless loop occurred due to a battle state inconsistency:

1. **Active Pokemon Fainted Without Proper Processing**: A side's active Pokemon fainted, but the faint wasn't properly processed through `FaintMessages()` and `CheckFainted()`, so `SwitchFlag` was never set to `true`.

2. **Move Request Generated Instead of Switch Request**: Without `SwitchFlag` set, `EndTurn()` called `MakeRequest(RequestState.Move)` instead of detecting that a switch was needed.

3. **WaitRequest Created for Invalid State**: `GetRequests()` detected the side had no active non-fainted Pokemon and created a `WaitRequest`, but didn't check if the battle should end.

4. **No Win Condition Check**: The battle never checked if the side with fainted active Pokemon had actually lost (no Pokemon left to switch in vs. still having Pokemon in reserve).

5. **Loop Continuation**: The opponent player kept making valid switch choices while the player with no active Pokemon kept receiving `WaitRequest`, creating an endless cycle.

**Key Issue**: The code path that leads to this state is an error condition - it indicates that somewhere earlier in the battle flow, Pokemon fainting wasn't properly detected or switch flags weren't set. However, the battle needs to recover gracefully from this error state instead of looping forever.

## Solution

### Modified Files

#### 1. **Battle.Requests.cs - GetRequests() method**

Added comprehensive error recovery when a side has no active non-fainted Pokemon during move request:

```csharp
else
{
    // If a side has no active non-fainted Pokemon during a move request,
    // this indicates either:
    // 1. The battle should have ended (this side has no Pokemon left)
    // 2. A switch should have been requested before this point
    // 
    // This is an error state - the battle logic should have called
    // FaintMessages() which would have checked win conditions and/or
    // set switch flags, leading to a SwitchIn request instead of a Move request.
    Debug(
        $"ERROR: {side.Name} has no active non-fainted Pokemon during move request phase");
    Debug($"  PokemonLeft: {side.PokemonLeft}");
    Debug($"  Active Pokemon: {string.Join(", ", side.Active.Select(p => p == null ? "null" : $"{p.Name}(Fainted:{p.Fainted})"))}");
    Debug($"  Active count: {side.Active.Count(p => p != null)}");
    Debug($"  Non-fainted count: {side.Active.Count(p => p is { Fainted: false })}");
    
    // This should NEVER happen in normal battle flow
    // It means Pokemon fainted but FaintMessages/CheckFainted didn't properly handle it
    // Force a switch request instead of a move request
    Debug($"  FORCING switch flags for all fainted active Pokemon");
    
    foreach (Pokemon? pokemon in side.Active)
    {
        if (pokemon is { Fainted: true })
        {
            pokemon.SwitchFlag = true;
            Debug($"  Set SwitchFlag=true for {pokemon.Name}");
        }
    }
    
    // Now check if this side has any Pokemon left to switch in
    int availableSwitches = CanSwitch(side);
    Debug($"  Available switches: {availableSwitches}");
    
    if (availableSwitches == 0 && side.PokemonLeft <= 0)
    {
        // This side has lost - end the battle
        Debug($"  {side.Name} has no Pokemon left - they lose");
        Lose(side);
        return new List<IChoiceRequest>();
    }
    
    // If we get here, switch flags are set - return to allow switch request
    // The battle will detect the switch flags and make a switch request next turn
    Debug($"  Switch flags set - battle should request switches next turn");
}
```

**Key Changes**:
- **Comprehensive Debugging**: Added detailed logging of battle state
- **Force Switch Flags**: Sets `SwitchFlag = true` on all fainted active Pokemon
- **Check Available Switches**: Determines if the side can actually switch
- **Call Lose()**: If no switches available and no Pokemon left, properly end the battle
- **Early Return**: Returns empty request list if battle ends

#### 2. **Battle.Requests.cs - MakeRequest() method**

Added safety checks to handle battle ending during request generation:

```csharp
// Generate appropriate requests for the current state
var requests = GetRequests(type.Value);

Debug($"Generated {requests.Count} requests");

// Check if battle ended during request generation
if (Ended)
{
    Debug("Battle ended during GetRequests, exiting MakeRequest");
    return;
}

// Note: Win condition checking is now done in GetRequests() when detecting
// sides with no active Pokemon, so we don't need to check again here

// Assign requests to each side
for (int i = 0; i < Sides.Count && i < requests.Count; i++)
{
    Sides[i].ActiveRequest = requests[i];

    Debug($"Assigned request to {Sides[i].Name}: {requests[i].GetType().Name}");
}
```

**Key Changes**:
- **Early Return on Ended**: Checks if battle ended during `GetRequests()` and returns immediately
- **Bounds Checking**: Added `i < requests.Count` to prevent index out of range if empty list returned

#### 3. **Battle.Fainting.cs - CheckFainted() method**

Added validation logging to detect problematic battle states:

```csharp
public void CheckFainted()
{
    // Iterate through all sides in the battle
    foreach (Pokemon pokemon in Sides.SelectMany(side => side.Active.OfType<Pokemon>()
                 .Where(pokemon => pokemon.Fainted)))
    {
        // Mark that this Pokémon needs to be switched out
        pokemon.SwitchFlag = true;
    }
    
    // CRITICAL: After setting switch flags, verify battle state is consistent
    // If any side has all active Pokemon fainted and no Pokemon left to switch in,
    // they should lose the battle immediately
    foreach (Side side in Sides)
    {
        // Check if all active slots have fainted Pokemon
        bool allActiveFainted = side.Active.All(p => p == null || p.Fainted);
        
        if (allActiveFainted && side.PokemonLeft <= 0)
        {
            Debug($"[CheckFainted] {side.Name} has all active Pokemon fainted and no Pokemon left to switch");
            Debug($"[CheckFainted] This side should lose the battle");
            // Don't call Win/Lose here - just ensure CheckWin will be called next
            // The battle loop will handle this properly
        }
    }
}
```

**Key Changes**:
- **State Validation**: Checks for the problematic state after setting switch flags
- **Debug Logging**: Logs when a side should lose but hasn't yet
- **Non-Invasive**: Doesn't change battle flow, just adds observability

## Why This Works

The fix operates on three levels:

### 1. **Defensive Recovery** (Primary Fix)
When `GetRequests()` detects the error state (no active non-fainted Pokemon), it:
- Forces switch flags to be set (the state that should already exist)
- Checks if switches are possible
- Calls `Lose(side)` if not, properly ending the battle
- Returns empty list to prevent further processing if battle ended

This is a **defensive recovery mechanism** that handles the case where earlier battle logic failed to properly process faints.

### 2. **Early Exit Prevention** (Safety Check)
`MakeRequest()` checks if the battle ended during `GetRequests()` and returns immediately, preventing:
- Attempting to assign requests when the battle has ended
- Index out of range errors if empty list returned
- Emitting choice requests for a battle that's already over

### 3. **State Observability** (Debugging Aid)
`CheckFainted()` validation logging helps:
- Detect when the problematic state occurs
- Track if it's happening frequently (indicating upstream bug)
- Provide context for future debugging

## Testing

### Test Case 1: Normal Fainting Flow
**Setup**: Battle where Pokemon faint through normal damage
**Expected**: Battle ends normally when one side has no Pokemon left
**Result**: ? Passes - no change to normal flow

### Test Case 2: All Active Pokemon Fainted
**Setup**: Side with all active Pokemon fainted but Pokemon in reserve
**Expected**: Switch request generated, not move request
**Result**: ? Passes - switch flags forced if needed

### Test Case 3: No Pokemon Left to Switch
**Setup**: Side with all active Pokemon fainted and no reserves (`PokemonLeft <= 0`)
**Expected**: Battle ends with that side losing
**Result**: ? Passes - `Lose(side)` called, battle ends properly

### Test Case 4: Verification with Entry Point
**Command**: `dotnet run` (uses `RunRandomVsRandomSinglesTest`)
**Previous Behavior**: Battle ran for 1000 turns and threw `BattleTurnLimitException`
**After Fix**: Battle completes normally with proper winner determination
**Result**: ? Passes consistently across multiple runs

## Call Flow Analysis

### Before Fix
```
EndTurn()
? MakeRequest(RequestState.Move)
  ? GetRequests()
    ? Detects: side.Active has no non-fainted Pokemon
    ? Creates: WaitRequest for that side
    ? NO WIN CHECK
  ? Assigns: WaitRequest to losing side
  ? Assigns: MoveRequest to winning side
? RequestPlayerChoices()
  ? Requests move from winning side
  ? Winning side switches Pokemon
? CommitChoices()
? TurnLoop()
? ... loop repeats indefinitely ...
? Turn 1000: MaybeTriggerEndlessBattleClause() throws exception
```

### After Fix
```
EndTurn()
? MakeRequest(RequestState.Move)
  ? GetRequests()
    ? Detects: side.Active has no non-fainted Pokemon
    ? Forces: SwitchFlag = true on fainted Pokemon
    ? Checks: availableSwitches = CanSwitch(side)
    ? IF availableSwitches == 0 && PokemonLeft <= 0:
      ? Calls: Lose(side)
      ? Returns: empty list
  ? Checks: if (Ended) return;  [EXITS HERE]
```

## Edge Cases Handled

### Case 1: Pokemon Still in Reserve
- **Scenario**: All active fainted, but `PokemonLeft > 0`
- **Behavior**: Forces switch flags, battle transitions to switch request
- **Result**: Normal switch occurs, battle continues

### Case 2: No Pokemon Left
- **Scenario**: All active fainted, `PokemonLeft <= 0`
- **Behavior**: Calls `Lose(side)`, battle ends
- **Result**: Proper win/loss determination

### Case 3: Mid-Turn Fainting
- **Scenario**: Pokemon faints during move execution
- **Behavior**: Normal `FaintMessages()` path, no change
- **Result**: Existing flow handles correctly

### Case 4: Simultaneous Fainting
- **Scenario**: Both sides' Pokemon faint on same turn
- **Behavior**: `CheckWin()` in `Lose()` detects tie/last-faint-wins
- **Result**: Proper tie or win determination

## Related Systems

### CheckWin() Method
The fix relies on `CheckWin()` to properly determine winners:
- Checks if all sides have no Pokemon left (tie)
- Checks if any side has defeated all opposing Pokemon (win)
- Uses `PokemonLeft` counter to track remaining Pokemon
- Called automatically by `Lose()` via `FaintMessages()`

### Switch Request Flow
Normal flow for switches after fainting:
1. Pokemon faints
2. `FaintMessages()` processes faint
3. `CheckFainted()` sets `SwitchFlag = true`
4. `RunAction()` detects switch flags
5. `MakeRequest(RequestState.SwitchIn)` called
6. Switch request generated and processed

The fix essentially "catches up" when this flow was skipped.

## Prevention

To prevent this issue from occurring in the first place:

### 1. **Always Call FaintMessages**
After any action that could cause fainting, ensure `FaintMessages()` is called:
```csharp
// After damage
FaintMessages();
if (Ended) return true;
```

### 2. **Check Switch Flags Before Requests**
Before making move requests, verify switch flags are properly set:
```csharp
CheckFainted(); // Ensure switch flags are set
// Then proceed with requests
```

### 3. **Test Edge Cases**
- Residual damage causing faints
- Recoil damage causing faints  
- Multi-target moves causing simultaneous faints
- Entry hazards causing faints on switch-in

## Keywords

`endless loop`, `infinite battle`, `turn limit`, `BattleTurnLimitException`, `WaitRequest`, `fainted Pokemon`, `switch flag`, `move request`, `battle state`, `win condition`, `GetRequests`, `MakeRequest`, `CheckFainted`, `FaintMessages`, `defensive programming`, `error recovery`, `battle lifecycle`, `request generation`

## Related Bug Fixes

- [Battle End Condition Null Request Fix](BattleEndConditionNullRequestFix.md) - Related issue with request handling after battle ends
- [Wild Charge Recoil Fix](WildChargeRecoilFix.md) - Fainting from recoil damage
- [Leech Seed Bug Fix](LeechSeedBugFix.md) - Residual damage causing faints

## Files Modified

- `ApogeeVGC/Sim/BattleClasses/Battle.Requests.cs`
  - `GetRequests()` - Added error recovery logic
  - `MakeRequest()` - Added battle ended check
- `ApogeeVGC/Sim/BattleClasses/Battle.Fainting.cs`
  - `CheckFainted()` - Added state validation logging

## Commit Message

```
fix: Prevent endless battle loop when active Pokemon fainted without proper switch handling

When a side had all active Pokemon fainted but FaintMessages/CheckFainted didn't
properly set switch flags, the battle would generate WaitRequest and loop indefinitely.

Changes:
- GetRequests: Force switch flags and check win conditions when detecting no active Pokemon
- MakeRequest: Early return if battle ends during request generation  
- CheckFainted: Add validation logging for problematic states

Fixes battle running for 1000+ turns until turn limit exception.
```

---

*Date Fixed*: 2025-01-19  
*Severity*: Critical  
*Systems Affected*: Battle lifecycle, request generation, win condition detection  
*Testing*: Verified with RunRandomVsRandomSinglesTest entry point
