# [IN PROGRESS] Player Random Switch Choice Bug - Doubles Battle Infinite Loop

## Status: PARTIALLY DIAGNOSED - FIX IN PROGRESS

**Last Updated**: 2025-01-20  
**Priority**: High  
**Systems Affected**: PlayerRandom AI, SyncSimulator, doubles battles with forced switches

---

## Problem Summary

When running random vs random VGC Regulation I battles (doubles format), the simulator occasionally times out after 3 seconds. The timeout occurs during battle execution, not from hitting the turn limit. The battle enters an infinite validation retry loop when PlayerRandom generates invalid switch choices.

### Reproduction

**Seeds that trigger timeout (intermittent)**:
- Team 1 Seed: 56202
- Team 2 Seed: 69772  
- Player 1 Seed: 14228
- Player 2 Seed: 3702
- Battle Seed: 11761

**To reproduce**:
1. Set `DriverMode.DebugSingleBattle` in Program.cs
2. Set debug seeds in Driver.cs `RunDebugSingleBattle()` to values above
3. Run battle - may timeout after ~3 seconds

**Note**: The timeout is intermittent and may only occur during parallel execution of many battles. Single-threaded runs with these seeds sometimes complete successfully.

---

## Root Causes Identified

### Primary Issue: GetRandomSwitchChoice() - Incomplete Actions for Multi-Switch

**File**: `ApogeeVGC/Sim/Player/PlayerRandom.cs`  
**Method**: `GetRandomSwitchChoice(SwitchRequest request)`  
**Lines**: ~353-394 (original), ~353-425 (modified)

**Original Behavior**:
- Always generated exactly 1 switch action
- Worked for singles and single forced switches in doubles
- FAILED for doubles when both Pokemon faint (needs 2 switches)

**Validation Errors Observed**:
```
[SyncSimulator] Error: Incomplete choice: ... - missing other pokemon
[SyncSimulator] Error: Can't switch: You can't switch to an active Pokémon
[SyncSimulator] Error: Can't switch: You can't switch to a fainted Pokémon
[SyncSimulator] Error: You sent more switches than Pokémon that need to switch
```

**Expected Behavior**:
- Generate one switch action per `true` value in `request.ForceSwitch` table
- Example: `ForceSwitch = [true, true]` ? generate 2 switch actions
- Example: `ForceSwitch = [false, true]` ? generate 1 switch action

### Secondary Issue: No Retry Limit in SyncSimulator

**File**: `ApogeeVGC/Sim/Core/SyncSimulator.cs`  
**Method**: `OnChoiceRequested()`  
**Lines**: ~156-173 (original)

**Problem**:
- When `Battle.Choose()` returns false (validation failed), the handler just returns
- Main loop in `Run()` sees `RequestState != None` and calls `RequestPlayerChoices()` again
- No retry counter, so invalid choices cause infinite loop
- No timeout mechanism at the choice level

---

## Changes Made (Current State)

### 1. PlayerRandom.cs - Modified GetRandomSwitchChoice() ? IMPLEMENTED

**Status**: Implemented but causing new issues

```csharp
private Choice GetRandomSwitchChoice(SwitchRequest request)
{
    // Determine how many switches are needed based on ForceSwitch table
    int switchesNeeded = request.ForceSwitch.Count(flag => flag);
    
    // Generate one switch action for each required slot
    for (int i = 0; i < switchesNeeded; i++)
    {
        // Pick random Pokemon from available, avoiding duplicates
        // Add to actions list
    }
    
    return new Choice { Actions = actions };
}
```

**Added logging**:
- ForceSwitch table contents
- Number of switches needed
- Each switch selection
- Total actions generated

### 2. SyncSimulator.cs - Added Validation Failure Logging ? IMPLEMENTED

**Lines**: ~156-186

```csharp
private void OnChoiceRequested(object? sender, BattleChoiceRequestEventArgs e)
{
    // ... existing code ...
    
    bool success = Battle.Choose(e.SideId, choice);
    
    // DEBUG: Log if choice failed
    if (!success && PrintDebug)
    {
        Console.WriteLine($"[SyncSimulator] Choice validation FAILED for {e.SideId}");
        Console.WriteLine($"[SyncSimulator] Error: {side.GetChoice().Error}");
        Console.WriteLine($"[SyncSimulator] Choice had {choice.Actions.Count} actions");
        for (int i = 0; i < choice.Actions.Count; i++)
        {
            var action = choice.Actions[i];
            Console.WriteLine($"[SyncSimulator]   Action {i}: {action.Choice}, Move={action.MoveId}, Target={action.TargetLoc}");
        }
    }
}
```

---

## Current Issues with Fix

### Issue 1: "You sent more switches than Pokémon that need to switch"

**Observation from logs**:
```
[PlayerRandom] ForceSwitch table: [False, True]
[PlayerRandom] Switches needed: 1
[PlayerRandom] Generated 1 switch actions
[SyncSimulator] Error: You sent more switches than Pokémon that need to switch
```

**Hypothesis**: The error occurs because:
1. During a MoveRequest, PlayerRandom can voluntarily switch (mix of moves and switches)
2. Choice accumulates actions: [Move, Switch] or [Switch, Move]
3. One action fails validation (e.g., Terastallize used twice)
4. Battle doesn't fully reset Choice state?
5. Next validation sees 2 actions when only 1 switch was expected

**Need to investigate**:
- How Choice.Actions is managed across validation attempts
- Whether Choice is cleared when Battle.Choose() fails
- Whether old actions persist when new request is made

### Issue 2: Timeout During Team Preview / Lead Selection

**Observation from logs**:
```
[IsChoiceDone] Player 1: Switch request - Actions=2, PokemonNeedingSwitch=0, done=True
[IsChoiceDone]   Active Pokemon with SwitchFlag: Poliwrath:False, Tyrogue:False
```

**State**: Both players report `done=True` with 0 Pokemon needing switch but 2 actions generated.

**Questions**:
1. Why is SwitchRequest used for lead selection instead of TeamPreviewRequest?
2. What should ForceSwitch contain during lead selection?
3. Should we always generate actions equal to Active.Count for lead selection?

---

## Next Steps (Priority Order)

### 1. ?? IMMEDIATE: Add Retry Limit to SyncSimulator (Defensive Fix)

Prevent infinite loops while debugging the root cause:

```csharp
private void OnChoiceRequested(object? sender, BattleChoiceRequestEventArgs e)
{
    const int MAX_RETRY_ATTEMPTS = 100;
    int retryCount = 0;
    
    while (retryCount < MAX_RETRY_ATTEMPTS)
    {
        Choice choice = player.GetChoiceSync(e.Request, e.RequestType, e.Perspective);
        
        if (choice.Actions.Count == 0)
        {
            side.AutoChoose();
            choice = side.GetChoice();
        }
        
        bool success = Battle.Choose(e.SideId, choice);
        
        if (success)
        {
            return; // Success, exit
        }
        
        retryCount++;
        
        if (PrintDebug)
        {
            Console.WriteLine($"[SyncSimulator] Choice retry {retryCount}/{MAX_RETRY_ATTEMPTS} for {e.SideId}");
            Console.WriteLine($"[SyncSimulator] Error: {side.GetChoice().Error}");
        }
    }
    
    // Max retries exceeded - throw exception with diagnostics
    throw new InvalidOperationException(
        $"Failed to generate valid choice for {e.SideId} after {MAX_RETRY_ATTEMPTS} attempts. " +
        $"Last error: {side.GetChoice().Error}");
}
```

### 2. Investigate Choice State Management

**Questions to answer**:
1. Is `Choice.Actions` cleared when `Battle.Choose()` fails?
2. Is `Side.Choice` reset between validation attempts?
3. Should we call `side.ClearChoice()` or similar after validation failure?

**Files to examine**:
- `ApogeeVGC/Sim/Choices/Choice.cs` - Choice structure
- `ApogeeVGC/Sim/SideClasses/Side.Choices.cs` - Choice management methods
- `ApogeeVGC/Sim/BattleClasses/Battle.Requests.cs` - Request/validation lifecycle

### 3. Distinguish Between Request Types

**Goal**: Understand when to use ForceSwitch vs Active.Count

**Cases to handle**:
1. **Lead Selection** (start of battle in doubles)
   - Request type: SwitchRequest or something else?
   - ForceSwitch: All false? Or missing?
   - Expected actions: Equal to Active.Count (2 for doubles)

2. **Forced Mid-Battle Switch** (Pokemon fainted)
   - Request type: SwitchRequest with RequestState.SwitchIn
   - ForceSwitch: Indicates which slots need switches
   - Expected actions: Equal to count of `true` in ForceSwitch

3. **Voluntary Switch During Moves** (handled by GetRandomMoveChoice)
   - Request type: MoveRequest
   - Each Pokemon can choose move OR switch
   - GetRandomSwitchChoice should NOT be called for this

**Investigation**:
- Add logging to show RequestState and request type at start of GetRandomSwitchChoice
- Check if there are multiple SwitchRequest scenarios we're not handling
- Look at TypeScript reference to see how it distinguishes these cases

### 4. Review TypeScript Reference Implementation

**File to check**: `pokemon-showdown/sim/battle.ts` and `sim/side.ts`

**Questions**:
1. How does Pokemon Showdown handle switch requests?
2. What does the ForceSwitch array mean in different contexts?
3. Is there a different request type for lead selection?
4. How does the random player decide how many switches to generate?

### 5. Add Comprehensive Logging

Before implementing final fix, add detailed logging to understand state:

```csharp
private Choice GetRandomSwitchChoice(SwitchRequest request)
{
    if (PrintDebug)
    {
        Console.WriteLine($"[PlayerRandom] === GetRandomSwitchChoice Called ===");
        Console.WriteLine($"[PlayerRandom] SideId: {SideId}");
        Console.WriteLine($"[PlayerRandom] Request type: {request.GetType().Name}");
        Console.WriteLine($"[PlayerRandom] ForceSwitch table: [{string.Join(", ", request.ForceSwitch)}]");
        Console.WriteLine($"[PlayerRandom] Side.Active.Count: {request.Side.Pokemon.Count(p => p.Active)}");
        Console.WriteLine($"[PlayerRandom] Side.Pokemon (active): {string.Join(", ", 
            request.Side.Pokemon.Where(p => p.Active).Select(p => p.Details))}");
        Console.WriteLine($"[PlayerRandom] Side.Pokemon (available): {string.Join(", ", 
            request.Side.Pokemon.Where(p => !p.Active && !p.Condition.Contains("fnt")).Select(p => p.Details))}");
    }
    
    // ... rest of method
}
```

### 6. Test Edge Cases

Once fix is implemented, test with:
1. Singles battles (ensure no regression)
2. Doubles with 0 switches needed (lead selection?)
3. Doubles with 1 switch needed (one Pokemon fainted)
4. Doubles with 2 switches needed (both Pokemon fainted)
5. Voluntary switches during move requests (shouldn't call GetRandomSwitchChoice)

---

## Code Locations

### Key Files:
- `ApogeeVGC/Sim/Player/PlayerRandom.cs` - Random player AI
  - `GetRandomMoveChoice()` - Lines ~117-305
  - `GetRandomSwitchChoice()` - Lines ~353-425
  - `GetRandomTargetLocation()` - Lines ~308-335

- `ApogeeVGC/Sim/Core/SyncSimulator.cs` - Synchronous battle runner
  - `Run()` - Main loop, Lines ~36-150
  - `OnChoiceRequested()` - Choice handler, Lines ~156-186

- `ApogeeVGC/Sim/SideClasses/Side.Choices.cs` - Choice validation
  - `Choose()` - Lines ~629-698
  - `IsChoiceDone()` - Lines ~1029-1075
  - `ChooseSwitch()` - Lines ~255-416

- `ApogeeVGC/Sim/BattleClasses/Battle.Requests.cs` - Request management
  - `Choose()` - Lines ~378-429
  - `GetRequests()` - Lines ~206-360
  - `AllChoicesDone()` - Lines ~615-633

### Related Bug Fixes:
- `docs/bugfixes/PlayerRandomDoublesTargetingFix.md` - Fixed targeting for doubles moves
- `docs/bugfixes/EndlessBattleLoopFix.md` - Fixed infinite turn loops

---

## Diagnostic Commands

### Run Debug Battle:
```powershell
cd C:\VSProjects\ApogeeVGC
dotnet run --project ApogeeVGC --configuration Debug --no-build
```

### Run with Timeout:
```powershell
$job = Start-Job -ScriptBlock { 
    cd C:\VSProjects\ApogeeVGC
    dotnet run --project ApogeeVGC --configuration Debug --no-build 2>&1 
}
Wait-Job $job -Timeout 5
Receive-Job $job | Select-Object -Last 100
Stop-Job $job
Remove-Job $job
```

### Filter Relevant Logs:
```powershell
# Switch-related messages
$output | Select-String "PlayerRandom.*Switch|SyncSimulator.*Choice"

# Validation errors
$output | Select-String "SyncSimulator.*Error:"

# Choice completion checks
$output | Select-String "IsChoiceDone"
```

---

## Questions for Investigation

1. **Is Choice.Actions cumulative?**
   - Does it keep actions from previous failed attempts?
   - Should we clear it between attempts?

2. **What triggers SwitchRequest vs MoveRequest?**
   - Is lead selection a SwitchRequest with ForceSwitch=[false, false]?
   - Or is it a different request type entirely?

3. **How does TypeScript handle this?**
   - Does PS use the same ForceSwitch mechanism?
   - Are there multiple types of switch scenarios?

4. **Why does the error say "more switches than needed"?**
   - If we generate exactly ForceSwitch.Count(true) actions, why is it too many?
   - Is the validator checking against a different counter?

5. **When should we use Active.Count vs ForceSwitch.Count()?**
   - Lead selection: Active.Count?
   - Forced switches: ForceSwitch.Count(true)?
   - How to distinguish programmatically?

---

## Testing Strategy

### Phase 1: Defensive Fix
1. Add retry limiter to SyncSimulator
2. Run 1000-battle evaluation
3. Collect all unique error messages
4. Categorize failure modes

### Phase 2: Root Cause Fix
1. Implement proper switch count logic
2. Add comprehensive logging
3. Run single-threaded tests with all edge cases
4. Verify no timeout occurs

### Phase 3: Validation
1. Run 10,000-battle evaluation with multiple thread counts
2. Check for any timeouts or exceptions
3. Verify win rate is balanced (~50/50)
4. Compare turn statistics with baseline

---

## Additional Context

### Format: VGC 2025 Regulation I (Doubles)
- 6 Pokemon per team
- Bring 4 to battle (team preview selection)
- 2 active at a time
- When both faint, must switch both simultaneously

### PlayerRandom Behavior:
- During MoveRequest: Can voluntarily choose to switch instead of move
- During SwitchRequest: Must provide switches for all forced slots
- Uses PRNG with seed for reproducibility

### Validation Flow:
1. PlayerRandom generates Choice with Actions list
2. Battle.Choose(sideId, choice) validates
3. If validation fails, returns false with error message
4. SyncSimulator's OnChoiceRequested returns (no retry)
5. Main loop calls RequestPlayerChoices() again
6. Cycle repeats ? infinite loop if choices always invalid

---

## Git Branch
Current work is on branch: `debug-vgc-reg-i`

## Contact
Original debugging session context preserved in this file for continuity.
