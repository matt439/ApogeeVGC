# Battle End Condition Null Request Fix

## Problem Summary

When a Pokémon fainted and caused one side to have no Pokémon left, the battle attempted to request moves from a player with no active Pokémon, resulting in `ArgumentNullException: Choice request cannot be null (Parameter 'request')`. The exception occurred in `PlayerRandom.GetNextChoiceFromAll` when it received a null request parameter.

### Symptom

```
Random 1 has no Pokemon left to switch in, losing
Battle ended after checking win conditions
TurnLoop returned
Battle ended during TurnLoop, exiting
[PlayerRandom.GetChoiceSync] Called for P2
Battle error: ArgumentNullException: Choice request cannot be null (Parameter 'request')
```

The battle correctly detected the win condition and set `Ended = true`, but continued to process choice requests for the remaining player, passing null requests and causing a crash.

## Root Cause

The issue had multiple interconnected causes in the request/choice flow:

### 1. Request Assignment Timing
In `Battle.Requests.cs`, the `MakeRequest` method:
1. Generated requests for all sides (including WaitRequest for the losing side)
2. Detected the battle should end after finding a side with fainted active Pokémon
3. Called `CheckWin()` which set `Ended = true`
4. Set `RequestState = None` to indicate no more requests
5. But requests had **already been assigned** to `Side.ActiveRequest` (lines 187-191)
6. The method returned, but sides still had requests assigned

### 2. Request Processing Loop
In `Battle.Requests.cs`, the `RequestPlayerChoices` method:
1. Started looping through sides to emit choice request events
2. Called `RequestPlayerChoice` for the first side
3. During event handling, the battle ended (via the flow: OnChoiceRequested ? Choose ? CommitChoices ? TurnLoop ? EndTurn ? MakeRequest ? CheckWin)
4. The `MakeRequest` early return cleared `RequestState` and `ActiveRequest`, but the loop in `RequestPlayerChoices` **continued**
5. Attempted to call `RequestPlayerChoice` for the next side with a null `ActiveRequest`
6. This passed `null` to `OnChoiceRequested`, which passed it to `PlayerRandom.GetChoiceSync`, causing the exception

### 3. Missing Loop Exit Check
`RequestPlayerChoices` didn't check if the battle had ended between processing each side's request, so it continued iterating even after the battle ended mid-loop.

## Solution

Applied three defensive fixes to `Battle.Requests.cs`:

### Fix 1: Clear ActiveRequests When Battle Ends in MakeRequest (Lines 177-186)

When `MakeRequest` detects the battle has ended after generating WaitRequests:

```csharp
if (Ended)
{
    Debug("Battle ended after checking win conditions");
    // Battle has ended - clear request state and active requests
    RequestState = RequestState.None;
    foreach (Side s in Sides)
    {
        s.ActiveRequest = null;
    }
    return;
}
```

This ensures that if `RequestPlayerChoices` is somehow called after `MakeRequest` returns, it will immediately throw a clear exception at line 640 instead of passing null requests to handlers.

### Fix 2: Check Battle End in RequestPlayerChoices Loop (Lines 655-659)

Added a check inside the `RequestPlayerChoices` loop before processing each side:

```csharp
// Check if battle ended (can happen during event handling)
if (Ended)
{
    Debug("Battle ended during RequestPlayerChoices, stopping");
    return;
}
```

This is the **critical fix** that prevents the null request from being passed. When the first side's choice triggers the battle end, this check catches it before attempting to process the next side.

### Fix 3: Early Return if Battle Already Ended (Lines 21-25)

The existing check at the start of `MakeRequest`:

```csharp
// Don't make requests if battle has ended
if (Ended)
{
    Debug("MakeRequest: Battle has ended, not making new request");
    return;
}
```

This prevents `MakeRequest` from being called after the battle has already ended, though the other fixes handle the edge case where it ends mid-request-generation.

## Call Flow Analysis

### Normal Flow (No Battle End)
```
SyncSimulator.Run (line 69)
 ??> Battle.RequestPlayerChoices
      ??> RequestPlayerChoice(P1) ? OnChoiceRequested ? GetChoiceSync ? Choose ? CommitChoices ? TurnLoop
      ??> RequestPlayerChoice(P2) ? OnChoiceRequested ? GetChoiceSync ? Choose ? CommitChoices ? TurnLoop
```

### Problematic Flow (Battle Ends During First Request)
```
SyncSimulator.Run (line 69)
 ??> Battle.RequestPlayerChoices
      ??> RequestPlayerChoice(P1) ? OnChoiceRequested ? GetChoiceSync
      ?    ??> Choose ? CommitChoices ? TurnLoop ? RunAction(Residual) ? FaintMessages
      ?         ??> CheckWin sets Ended=true, returns to TurnLoop ? returns to CommitChoices
      ?              ??> CommitChoices checks Ended, returns to Choose ? returns to OnChoiceRequested
      ?                   ??> OnChoiceRequested returns to RequestPlayerChoice
      ?                        ??> RequestPlayerChoice returns to RequestPlayerChoices **loop continues**
      ?
      ??> [FIX 2 HERE] Check if Ended, if yes return before calling RequestPlayerChoice(P2)
           ??> ? WITHOUT FIX: RequestPlayerChoice(P2, null) ? null passed to GetChoiceSync ? ArgumentNullException
```

## Files Modified

### ApogeeVGC/Sim/BattleClasses/Battle.Requests.cs

**Lines 177-186**: Enhanced battle end detection in `MakeRequest`
- Clear `RequestState` and all `ActiveRequest`s when battle ends mid-request-generation
- Prevents any subsequent calls from attempting to process stale requests

**Lines 655-659**: Added battle end check in `RequestPlayerChoices` loop
- Check `if (Ended)` before processing each side's request
- Return immediately if battle ended during previous side's request handling
- **This is the primary fix** that prevents the null request exception

## Testing

### Test Case: Single Elimination Battle
1. Set up a battle where one side has only one Pokémon
2. Have that Pokémon take lethal damage
3. Verify the battle ends correctly without exceptions
4. Confirm no requests are made to the losing player
5. Confirm the winning player doesn't receive an invalid request

### Verification Output
```
Random 1 has no Pokemon left to switch in, losing
Battle ended after checking win conditions
TurnLoop returned
Battle ended during TurnLoop, exiting
Battle ended during RequestPlayerChoices, stopping  ? Fix 2 working
[Driver] Battle completed with result: Player2Win

=== Battle Complete ===
Winner: Player2Win
```

### Related Test Cases
- Battle ending during Residual phase (turn end effects)
- Battle ending during move execution (recoil knockout)
- Battle ending during switch-in (hazard damage)
- Both players losing last Pokémon simultaneously (tie scenario)

## Key Insights

### 1. Asynchronous Event Handling in Synchronous Code
Even though `SyncSimulator` runs "synchronously," the event-driven architecture means control flow can deeply nest:
- `RequestPlayerChoices` calls an event
- Event handler calls `Choose`
- `Choose` calls `CommitChoices`  
- `CommitChoices` calls `TurnLoop`
- `TurnLoop` detects battle end and returns
- Control unwinds back to `RequestPlayerChoices` **which is still in its loop**

### 2. State Changes During Iteration
When iterating over a collection and triggering events, you must check for state changes (like `Ended`) between iterations, not just at the start.

### 3. Defense in Depth
The fix applies three layers of defense:
1. Early return if already ended (prevents unnecessary work)
2. Clear requests when ending mid-generation (prevents stale data)
3. Check ended status in processing loop (catches mid-loop endings)

### 4. Request State vs. ActiveRequest
Two separate state indicators must be kept in sync:
- `Battle.RequestState` (enum indicating what type of request is active)
- `Side.ActiveRequest` (the actual request object)

When the battle ends, both must be cleared to maintain consistency.

## Related Bug Fixes

### Similar Pattern: Request Validation
- This fix is similar to other request/choice validation fixes
- Always validate that battle state is consistent before processing requests
- Check for battle end conditions at every point where control returns from nested calls

### Prevention Guidelines
When implementing new request/choice flows:
1. Always check `if (Ended)` before making requests
2. Always check `if (Ended)` between processing multiple entities in loops
3. Clear all request state (`RequestState` and `ActiveRequest`) when battle ends
4. Never assume state remains constant during nested event/method calls
5. Add defensive null checks when accessing request data

## Keywords

`battle end`, `win condition`, `null request`, `ArgumentNullException`, `MakeRequest`, `RequestPlayerChoices`, `ActiveRequest`, `RequestState`, `fainting`, `PokemonLeft`, `CheckWin`, `event handling`, `synchronous simulator`, `request loop`, `state consistency`, `mid-loop exit`, `defensive programming`

---

**Severity**: High  
**Systems Affected**: Battle lifecycle, request/choice system, win condition detection  
**Fix Type**: Defensive state validation  
**Related Files**: `Battle.Requests.cs`, `SyncSimulator.cs`  
**Date Fixed**: 2025-01-XX
