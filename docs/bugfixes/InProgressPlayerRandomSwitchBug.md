# [RESOLVED] Player Random Switch Choice Bug - Doubles Battle Infinite Loop

## Status: FIXED

**Last Updated**: 2025-01-20  
**Priority**: High (Now Resolved)  
**Systems Affected**: PlayerRandom AI, SyncSimulator, doubles battles with forced switches

---

## Problem Summary

When running random vs random VGC Regulation I battles (doubles format), the simulator entered an infinite validation retry loop when PlayerRandom generated switch choices for initial lead selection or mid-battle forced switches. The validation consistently failed with "Can't switch: You sent more switches than Pokémon that need to switch" even when the correct number of switches were generated.

### Root Cause

The bug was in `Side.Choices.cs` method `ClearChoice()` (line 568):

```csharp
// BEFORE (BUGGY):
int canSwitchOut = Active.Count(pokemon => pokemon?.SwitchFlag.IsTrue() == true);
```

This only counted active Pokemon with `SwitchFlag.IsTrue()`, which returned **0** when:
- Initial lead switch-in: `Active = [null, null]` (empty slots need filling)
- No Pokemon had `SwitchFlag` set yet because they were null

This caused `ForcedSwitchesLeft` to be set to 0, even though 2 switches were needed. The validation in `ChooseSwitch()` then rejected all switch attempts because the choice index exceeded the (incorrectly calculated) allowed switches.

### Solution

Changed `ClearChoice()` to count **null slots** as needing switches:

```csharp
// AFTER (FIXED):
int canSwitchOut = Active.Count(pokemon => pokemon == null || pokemon.SwitchFlag.IsTrue());
```

This correctly identifies that empty Active slots need to be filled with switches, matching the behavior of the `ForceSwitch` table generation in `Battle.Requests.cs`.

---

## Status: FIXED

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

## Changes Made

### 1. SyncSimulator.cs - Added Retry Limit ? IMPLEMENTED

**File**: `ApogeeVGC/Sim/Core/SyncSimulator.cs`  
**Method**: `OnChoiceRequested()`  
**Lines**: ~156-210

**Purpose**: Defensive fix to prevent infinite loops and provide diagnostics.

**Changes**:
- Added `MAX_RETRY_ATTEMPTS = 100` constant
- Implemented retry loop with counter
- Logs each failed validation attempt with error details
- Throws `InvalidOperationException` with full diagnostics if max retries exceeded
- This revealed the root cause validation error

### 2. PlayerRandom.cs - Enhanced Logging ? IMPLEMENTED

**File**: `ApogeeVGC/Sim/Player/PlayerRandom.cs`  
**Method**: `GetRandomSwitchChoice()`  
**Lines**: ~353-430

**Purpose**: Diagnostic logging to understand switch generation.

**Added logging for**:
- SideId and ForceSwitch table contents
- Active Pokemon count and details
- Available Pokemon to switch in
- Each switch selection with index
- Total actions generated

### 3. Side.Choices.cs - Fixed ClearChoice() ? FIXED ROOT CAUSE

**File**: `ApogeeVGC/Sim/SideClasses/Side.Choices.cs`  
**Method**: `ClearChoice()`  
**Lines**: ~558-595

**Purpose**: Fix ForcedSwitchesLeft calculation for null Active slots.

**Changes**:
```csharp
// BEFORE:
int canSwitchOut = Active.Count(pokemon => pokemon?.SwitchFlag.IsTrue() == true);

// AFTER:
int canSwitchOut = Active.Count(pokemon => pokemon == null || pokemon.SwitchFlag.IsTrue());
```

**Added logging**:
- Active.Count, canSwitchOut, canSwitchIn values
- ForcedSwitchesLeft and ForcedPassesLeft
- Each Active slot status (null or Pokemon with SwitchFlag)

---

## Technical Details

### Why Null Slots Need Switches

In doubles battles:
1. `Active` array is initialized with 2 null entries: `[null, null]`
2. After team preview, Pokemon need to be switched into these empty slots
3. The request system generates `ForceSwitch = [true, true]` based on these slots needing fills
4. `ClearChoice()` must count these null slots to set `ForcedSwitchesLeft = 2`
5. Validation then accepts 2 switch actions

### Lead Selection vs Mid-Battle Switches

- **Lead selection** (battle start after team preview):
  - `Active = [null, null]` ? empty slots
  - `ForceSwitch = [true, true]` ? both need switches
  - `ForcedSwitchesLeft` should be 2

- **Mid-battle forced switch** (Pokemon fainted):
  - `Active = [PokemonA, null]` ? one empty slot
  - `ForceSwitch = [false, true]` ? one needs switch
  - `ForcedSwitchesLeft` should be 1

The fix handles both cases correctly.

---

## Reproduction (Before Fix)

**Seeds that triggered timeout**:
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
