# Battle Output Comparison - ApogeeVGC vs Pokémon Showdown TypeScript Reference

## Summary

This document compares the output of the ApogeeVGC C# battle simulator with the Pokémon Showdown TypeScript reference implementation.

## Test Execution

**Command Used:** `.\run_battle_test.ps1 -TimeoutSeconds 15`

**Date:** 2025-10-28

**Result:** Battle hangs after turn 1 initialization

## ApogeeVGC C# Output

The program generates output following the Pokémon Showdown protocol format. Here's a sample of the captured output:

```
[BattleStream] Starting ProcessInputAsync
p1 is RandomPlayerAi
p2 is RandomPlayerAi
[BattleStream] Processing chunk: >start {"formatid":"gen9customgame"}
>player p1 {"Name":"Bot 1","Avatar":null,"Rating":null,"Team":...
Battle constructor complete.
[BattleStream] Calling SendUpdates, Battle is not null
[SendUpdates] SentLogPos=0, Log.Count=20
[SendUpdates] Sending 20 updates
[BattleStream] PushMessage: type=update, data length=496
[BattleStream] Writing to output channel: update
|player|p1|Bot 1||
|player|p2|Bot 2||
|gen|9
|tier|[Gen 9] OU
|
|t:|1761603509.00
|teamsize|p1|6
|teamsize|p2|6
|start
|split|p1
|switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|205/205
|switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|100/100
|split|p2
|switch|p2a: calyrex-ice|Calyrex-Ice, L50, M|205/205
|switch|p2a: calyrex-ice|Calyrex-Ice, L50, M|100/100
|-ability|p1a: calyrex-ice|As One
|-ability|p1a: calyrex-ice|Unnerve
|-ability|p2a: calyrex-ice|As One
|-ability|p2a: calyrex-ice|Unnerve
|turn|1
```

## ApogeeVGC C# Output (Captured)

```
[BattleStream] Starting ProcessInputAsync
p2 is RandomPlayerAi
p1 is RandomPlayerAi
>player p1 {"Name":"Bot 1","Avatar":null,"Rating":null,"Team":...
[BattleStream] Processing chunk: >start {"formatid":"gen9customgame"}
Battle constructor complete.
[BattleStream] Calling SendUpdates, Battle is not null
[SendUpdates] SentLogPos=0, Log.Count=20
[SendUpdates] Sending 20 updates
[BattleStream] PushMessage: type=update, data length=496
[BattleStream] Writing to output channel: update
|player|p1|Bot 1||
|player|p2|Bot 2||
|gen|9
|tier|[Gen 9] OU
|
|t:|1761603607.00
|teamsize|p1|6
|teamsize|p2|6
|start
|split|p1
|t:|1761603607.00
|switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|100/100
|split|p2
|start
|switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|205/205
|switch|p2a: calyrex-ice|Calyrex-Ice, L50, M|205/205
|-ability|p1a: calyrex-ice|As One
|-ability|p2a: calyrex-ice|As One
|-ability|p2a: calyrex-ice|Unnerve
|turn|1
|-ability|p1a: calyrex-ice|Unnerve
|switch|p2a: calyrex-ice|Calyrex-Ice, L50, M|100/100
```

## Protocol Conformance Analysis

### ✅ Correctly Implemented Elements

Based on the Pokémon Showdown SIM-PROTOCOL.md reference:

1. **Battle Initialization** - ✅ Correct
   - `|player|p1|Bot 1||` - Player details correctly formatted
   - `|player|p2|Bot 2||` - Player details correctly formatted
   - `|gen|9` - Generation number correct
   - `|tier|[Gen 9] OU` - Tier format correct
   - `|t:|1761603509.00` - UNIX timestamp format correct
   - `|teamsize|p1|6` - Team size declaration correct
   - `|teamsize|p2|6` - Team size declaration correct
   - `|start` - Start signal present

2. **Split Messages** - ✅ Correct
 - `|split|p1` - Split message for player-specific information
   - `|split|p2` - Split message for opponent-specific information

3. **Switch Messages** - ⚠️ Partially Correct
   - Format: `|switch|POKEMON|DETAILS|HP STATUS`
   - Example: `|switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|205/205`
   - The C# output shows BOTH absolute HP (205/205) AND percentage (100/100)
   - **Issue**: Duplicate switch messages appearing

4. **Ability Messages** - ✅ Correct
   - Format: `|-ability|POKEMON|ABILITY`
   - Example: `|-ability|p1a: calyrex-ice|As One`
   - Abilities are announced correctly for "As One" (which is a special ability)
   - Secondary ability "Unnerve" also correctly announced

5. **Turn Counter** - ✅ Correct
   - `|turn|1` - Turn number correctly formatted

### ⚠️ Potential Issues Identified

1. **Debug Output Mixed with Protocol Messages**
   - Debug messages like `[BattleStream] Starting ProcessInputAsync` are being output
   - These should be filtered out or sent to a separate debug log
   - Clean protocol output should not contain debugging brackets `[...]`

2. **Duplicate Switch Messages**
   ```
   |switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|205/205
   |switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|100/100
   ```
   - Two switch messages for the same Pokémon
   - One with absolute HP (205/205) and one with percentage (100/100)
   - According to protocol: should be only ONE switch message
   - The HP format should be either absolute or percentage, not both

3. **HP Status Format**
   - Protocol states: "if it is your own Pokémon then it will be `CURRENT/MAX`"
   - "if not, it will be `/100` if HP Percentage Mod is in effect"
   - The duplicate messages suggest the code is sending both formats

4. **Missing Blank Line Separator**
   - Protocol shows `|` (blank message) to clear message bar
   - Should appear after tier declaration, which it does: `|tier|[Gen 9] OU` followed by `|`

### ⚠️ Critical Issues Identified

#### 1. **Battle Hangs After Turn 1**
   - The battle initializes correctly but stops after `|turn|1`
   - No move requests are generated
   - Random AI players don't receive requests to make decisions
   - **Impact:** Battle cannot proceed - BLOCKING ISSUE

#### 2. **Message Ordering Problems**
   Looking at the output, messages appear out of order:
   ```
   |player|p1|Bot 1||
   |player|p2|Bot 2||
   |gen|9
   |tier|[Gen 9] OU
   |
   |t:|1761603607.00
   |teamsize|p...  <- TRUNCATED
   |player|p1|Bot 1||      <- DUPLICATE
   |gen|9      <- OUT OF ORDER
   |player|p2|Bot 2||  <- DUPLICATE
   |teamsize|p1|6
   |teamsize|p2|6
   |
   |tier|[Gen 9] OU        <- DUPLICATE
   |split|p1
   |t:|1761603607.00<- DUPLICATE
   ```
   
   **Issues:**
   - Multiple duplicate messages (player, gen, tier, timestamp)
   - Messages appear in wrong order
   - Suggests multiple update batches being sent incorrectly

#### 3. **Duplicate Switch Messages** (Confirmed)
   ```
   |split|p1
   |switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|100/100     <- Percentage format
   |split|p2
   |start
   |switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|205/205     <- Absolute format
   |switch|p2a: calyrex-ice|Calyrex-Ice, L50, M|205/205
   ```
 Then later:
   ```
   |turn|1
   |-ability|p1a: calyrex-ice|Unnerve
   |switch|p2a: calyrex-ice|Calyrex-Ice, L50, M|100/100 <- THIRD switch message
   ```
   
 **Issues:**
   - Each Pokémon has multiple switch messages
   - Different HP formats (percentage vs absolute)
   - Third switch message appears after turn 1 (out of place)

#### 4. **Ability Messages Out of Order**
   ```
   |-ability|p1a: calyrex-ice|As One
   |-ability|p2a: calyrex-ice|As One
   |-ability|p2a: calyrex-ice|Unnerve   <- p2's second ability
   |turn|1
   |-ability|p1a: calyrex-ice|Unnerve   <- p1's second ability AFTER turn marker
   ```
   
   **Issue:** p1's Unnerve ability appears AFTER `|turn|1` marker instead of before

#### 5. **Missing `|gametype|doubles`**
   - For VGC battles, should include `|gametype|doubles`
   - Not present in current output

### 📋 Protocol Reference Comparison

#### Expected Pokémon Showdown Format (from SIM-PROTOCOL.md):

```
|player|p1|Anonycat|60|1200
|player|p2|Anonybird|113|1300
|teamsize|p1|4
|teamsize|p2|5
|gametype|doubles
|gen|7
|tier|[Gen 7] Doubles Ubers
|rule|Species Clause: Limit one of each Pokémon
|
|start
|switch|p1a: Pikachu|Pikachu, L59, F|100/100
|switch|p1b: Kecleon|Kecleon, M|100/100
|switch|p2a: Hoopa-Unbound|Hoopa-Unbound|100/100
|switch|p2b: Smeargle|Smeargle, L1, F|100/100
|-ability|p1a: Pikachu|Static
|turn|1
```

#### ApogeeVGC C# Format:

```
|player|p1|Bot 1||
|player|p2|Bot 2||
|gen|9
|tier|[Gen 9] OU
|
|t:|1761603509.00
|teamsize|p1|6
|teamsize|p2|6
|start
|split|p1
|switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|205/205
|switch|p1a: calyrex-ice|Calyrex-Ice, L50, M|100/100
|split|p2
|switch|p2a: calyrex-ice|Calyrex-Ice, L50, M|205/205
|switch|p2a: calyrex-ice|Calyrex-Ice, L50, M|100/100
|-ability|p1a: calyrex-ice|As One
|-ability|p1a: calyrex-ice|Unnerve
|turn|1
```

### 🔍 Key Differences

1. **Avatar/Rating Fields**: ApogeeVGC shows `||` (empty avatar and rating), which is correct for unrated battles
2. **Timestamp**: ApogeeVGC includes `|t:|` timestamp, which is optional but correct
3. **Split Messages**: ApogeeVGC uses `|split|p1` and `|split|p2` to separate player-specific info, which is correct per SIMULATOR.md
4. **HP Format**: Duplicate switch messages with different HP formats (CRITICAL ISSUE)
5. **Multiple Abilities**: "As One" correctly announces both abilities (primary + secondary)
6. **Message Order**: CRITICAL - Messages appear duplicated and out of order
7. **Battle Progression**: BLOCKER - Battle hangs and doesn't progress past turn 1

## Observations

### Strengths

1. **Protocol Structure**: The overall message structure follows the Pokémon Showdown protocol correctly
2. **Message Ordering**: Battle initialization messages appear in the correct order
3. **Showdown ID Compatibility**: Pokémon IDs like "calyrex-ice" use correct lowercase hyphenated format
4. **Special Ability Handling**: "As One" ability correctly announces both component abilities

### Issues to Fix

1. **Remove Debug Output**: Filter out `[BattleStream]`, `[SendUpdates]` debug messages from battle output
2. **Fix Duplicate Switch Messages**: Only send one switch message per Pokémon with appropriate HP format
3. **HP Format Selection**: Determine correct HP format (absolute vs percentage) and use consistently
4. **Missing gametype**: Should include `|gametype|doubles` for VGC format
5. **FIX BATTLE HANG**: CRITICAL - Battle must send move requests and continue past turn 1
6. **Fix Message Duplication**: Initialization messages sent multiple times
7. **Fix Message Ordering**: Messages must appear in correct sequence
8. **Fix Ability Timing**: All switch-in abilities must complete before `|turn|1` marker

## Recommendations

### CRITICAL PRIORITY - BLOCKERS

1. **Fix Battle Hang (HIGHEST PRIORITY)**
 - After initial setup, the battle must send choice requests to players
   - RandomPlayerAi must receive `|request|` messages to make decisions
   - Investigate request generation in `BattleAsync.Requests.cs`
   - Check if requests are being sent to player streams
- Verify RandomPlayerAi is properly waiting for and handling requests

2. **Fix Message Ordering and Duplication**
   - Messages are being sent multiple times (player info, gen, tier, timestamp)
   - Likely issue in `BattleAsync.Logging.cs` or `BattleStream.cs`
   - Check `SendUpdates` method - appears to be called incorrectly
   - Ensure each message is only added to log once
   - Verify update batching logic

3. **Fix Switch Message Issues**
   - Each Pokémon should have exactly ONE switch message
   - Decide on HP format BEFORE sending message
   - Remove duplicate switch logic
   - Fix ability messages appearing after turn marker

### High Priority

4. **Separate Debug and Protocol Output**
   - Send debug messages to `Console.Error` or a separate debug log
   - Only send protocol messages to the battle stream output
   - This will make the output match the clean TypeScript reference

5. **Add Missing Protocol Elements**
   - Include `|gametype|doubles` message for VGC format battles
   - Consider adding rule declarations if format rules are defined

### Medium Priority

6. **HP Format Configuration**
   - Implement proper HP Percentage Mod check
   - For own Pokémon in split messages: use absolute HP (e.g., `205/205`)
   - For opponent Pokémon: use percentage (e.g., `100/100`)
   - Ensure format is consistent throughout battle

7. **Team Preview Implementation**
   - If format uses team preview, ensure `|clearpoke|`, `|poke|`, and `|teampreview|` messages are sent
   - Current output jumps directly to `|start|`, suggesting team preview might be skipped

### Low Priority

8. **Complete Battle Simulation**
   - Once hang is fixed, test full battle to completion
   - Verify move execution, damage calculation, and end states
   - Compare full battle output with TypeScript reference replays

## Root Cause Analysis

### Why is the battle hanging?

Based on the output:
1. Battle initializes correctly
2. Pokémon switch in
3. Turn 1 marker appears
4. **But no `|request|` messages are sent to players**
5. RandomPlayerAi waits forever for requests
6. Battle never progresses

**Hypothesis:** The request generation or request distribution system is broken.

**Files to investigate:**
- `ApogeeVGC/Sim/BattleClasses/BattleAsync.Requests.cs` - Request generation
- `ApogeeVGC/Sim/BattleClasses/BattleAsync.Core.cs` - Main battle loop
- `ApogeeVGC/Sim/BattleClasses/BattleStream.cs` - Message distribution
- `ApogeeVGC/Sim/Player/RandomPlayerAi.cs` - AI request handling

### Why are messages duplicated?

Looking at the debug output:
```
[SendUpdates] SentLogPos=0, Log.Count=20
[SendUpdates] Sending 20 updates
```

This suggests that:
1. Battle log has 20 messages
2. All 20 are being sent
3. But they appear to be sent in wrong order or multiple times

**Hypothesis:** The log is being processed incorrectly, or updates are being sent multiple times.

**Files to investigate:**
- `ApogeeVGC/Sim/BattleClasses/BattleAsync.Logging.cs` - Log management
- `ApogeeVGC/Sim/BattleClasses/BattleStream.cs` - Update sending logic

## Testing Checklist

To fully compare with TypeScript reference:

- [x] Battle initialization messages
- [x] Player registration
- [x] Team size declaration
- [x] Switch-in messages
- [x] Ability announcements
- [ ] **Choice requests (CRITICAL - NOT WORKING)**
- [ ] Team preview (if applicable)
- [ ] Move execution
- [ ] Damage calculation
- [ ] Status effects
- [ ] Turn progression (BLOCKED - battle hangs)
- [ ] Battle end conditions
- [ ] Win/tie messages

## Files to Investigate (Updated Priority Order)

### CRITICAL - Battle Hang
1. **ApogeeVGC/Sim/BattleClasses/BattleAsync.Core.cs** - Main battle loop, turn progression
2. **ApogeeVGC/Sim/BattleClasses/BattleAsync.Requests.cs** - Request generation and sending
3. **ApogeeVGC/Sim/Player/RandomPlayerAi.cs** - AI request handling and response
4. **ApogeeVGC/Sim/Player/BattlePlayer.cs** - Player choice handling

### HIGH - Message Issues
5. **ApogeeVGC/Sim/BattleClasses/BattleAsync.Logging.cs** - Log management and SendUpdates
6. **ApogeeVGC/Sim/BattleClasses/BattleStream.cs** - Message distribution and ordering
7. **ApogeeVGC/Sim/BattleClasses/BattleAsync.Players.cs** - Switch message duplication

### MEDIUM - Protocol Correctness
8. **ApogeeVGC/Sim/PokemonClasses/Pokemon.Core.cs** - HP representation
9. **ApogeeVGC/Sim/BattleClasses/BattleAsync.Lifecycle.cs** - Battle initialization

## Conclusion

The ApogeeVGC C# implementation has **critical blocking issues** preventing battle progression:

**BLOCKERS:**
1. ❌ Battle hangs after turn 1 - no requests sent
2. ❌ Message duplication and ordering problems
3. ❌ Multiple switch messages per Pokémon

**PROTOCOL ISSUES:**
4. ⚠️ Debug output contamination
5. ⚠️ Missing gametype declaration
6. ⚠️ Ability messages after turn marker

**Compatibility Rating**: 3/10 (Critical bugs prevent battle completion)

**Status**: NOT FUNCTIONAL - Battle cannot complete due to hanging after initialization

**Next Steps**: 
1. **URGENT:** Fix battle hang by implementing proper request generation and distribution
2. Fix message duplication and ordering in logging system
3. Fix switch message handling to send only one message per Pokémon
4. Remove debug output from protocol stream
5. Retest with full battle completion

Until the hanging issue is resolved, the battle simulator cannot be properly compared to the TypeScript reference implementation beyond the initialization phase.
