# Animation Targeting and Event Deferral Issues

## Overview
This document describes ongoing issues with the battle animation system in the ApogeeVGC GUI, specifically related to attack animation targeting and the timing of faint/switch events.

## Current Issues

### 1. Attack Animations Target All Opponents Instead of Actual Targets
**Status:** Not Fixed

**Symptoms:**
- When a move hits specific Pokemon (e.g., Dazzling Gleam hits Ironhands and Miraidon), the attack animation goes to ALL opponents on screen
- Logs show: `[AnimationCoordinator] Found 0 actual target positions from 0 target keys`
- The system falls back to `GetPotentialDefenderPositions()` which returns all opponents

**Example from logs:**
```
[SpreadDamage] Calling PrintDamageMessage for Ironhands, damage=84
[SpreadDamage] Calling PrintDamageMessage for Miraidon, damage=62
[AnimationCoordinator] Trying to trigger animation for Dazzling Gleam
[AnimationCoordinator] Found 0 actual target positions from 0 target keys
[AnimationCoordinator] No defender positions found, falling back to all opponents
```

### 2. Pokemon Switch Happens Immediately Without Waiting for Damage Animations
**Status:** Not Fixed

**Symptoms:**
- When a Pokemon faints from damage (e.g., Miraidon taking 143 damage), the replacement Pokemon (Ursaluna) appears immediately
- HP bar damage animations don't have time to play before the switch
- FaintMessage and SwitchMessage are processed without being deferred

**Example from logs:**
```
[SpreadDamage] Applied 143 damage to Miraidon (new HP: 0)
[GuiChoiceCoordinator] Enqueued event: FaintMessage
[BattleGame.ProcessBattleEvent] Processing event with message type: FaintMessage
[BattleGame.ProcessBattleEvent] Processing event with message type: SwitchMessage
// No "Deferring" message - events processed immediately
```

### 3. Damage Amount Not Shown When Pokemon Faints
**Status:** Partially Fixed (parsing improved but may still have edge cases)

**Symptoms:**
- When damage causes a faint: "Miraidon took damage!" (no amount)
- When damage doesn't cause faint: "Miraidon (Side 2) took 62 damage (69.8% HP remaining)"

**Fix Applied:**
- Modified `ParseDamageMessage` in `Battle.Logging.cs` to extract `[dmg]` tag BEFORE attempting HP parsing
- Added fallback to show damage amount even if HP parsing fails
- This should help but hasn't been verified to fully work yet

## Architecture

### Key Components

#### AnimationCoordinator (`ApogeeVGC/Gui/Animations/AnimationCoordinator.cs`)
- **Purpose:** Links battle messages with animations and tracks attack targets
- **Key Fields:**
  - `_pendingMoveAnimation`: The current MoveUsedMessage being processed
  - `_pendingMoveTargets`: List of target Pokemon keys (format: "Name|SideId")
  - `_animationTriggered`: Flag to prevent duplicate animations
  - `_pokemonPositions`: Dictionary mapping Pokemon keys to screen positions

- **Key Methods:**
  - `HandleMoveUsed()`: Processes MoveUsedMessage, currently clears target list
  - `HandleDamage()`: Processes DamageMessage, adds targets to `_pendingMoveTargets`
  - `TriggerPendingAttackAnimation()`: Creates animation using collected targets
  - `ProcessMessage()`: Main dispatcher for battle messages

#### BattleGame (`ApogeeVGC/Gui/BattleGame.cs`)
- **Purpose:** Main GUI game loop, processes battle events and coordinates animations
- **Key Fields:**
  - `_deferredEvents`: Queue of events waiting for animations to complete
  - `_animationCoordinator`: Handles animation triggering
  - `_animationManager`: Manages active animations

- **Key Methods:**
  - `ProcessBattleEvent()`: Processes individual battle events, handles deferral logic
  - `Update()`: Main game loop, processes events and deferred events

#### AnimationManager (`ApogeeVGC/Gui/Animations/AnimationManager.cs`)
- **Purpose:** Manages animation lifecycle and queuing
- **Key Properties:**
  - `HasActiveAnimations`: Returns true if animations are queued OR active

## Event Flow

### Current Event Processing Order
```
1. Battle simulation enqueues events:
   - MoveUsedMessage (Dazzling Gleam)
   - EffectivenessMessage
   - EffectivenessMessage
   - DamageMessage (Ironhands)
   - DamageMessage (Miraidon)
   - MoveUsedMessage (Electro Drift)
   - ... more messages ...
   - GenericMessage
   - FaintMessage
   - SwitchMessage

2. GUI processes events one at a time:
   - ProcessBattleEvent(MoveUsedMessage) ? HandleMoveUsed() clears targets
   - ProcessBattleEvent(EffectivenessMessage)
   - ProcessBattleEvent(DamageMessage) ? HandleDamage() adds target
   - ProcessBattleEvent(DamageMessage) ? HandleDamage() adds target
   - TriggerPendingAttackAnimation() ? Uses collected targets
   - ProcessBattleEvent(FaintMessage) ? Should defer but doesn't
   - ProcessBattleEvent(SwitchMessage) ? Should defer but doesn't
```

## Root Cause Analysis

### Issue 1: Target List Empty

**Problem:** `HandleMoveUsed()` clears the target list, but it's called BEFORE `HandleDamage()` can add targets.

**Current Code Flow:**
```csharp
// In HandleMoveUsed
_pendingMoveTargets.Clear();  // Clears any existing targets
_pendingMoveAnimation = moveMsg;

// Later, in HandleDamage
_pendingMoveTargets.Add(targetKey);  // Adds target
```

**Why This Fails:**
- Events arrive in order: MoveUsedMessage, DamageMessage, DamageMessage
- `HandleMoveUsed` is called first and clears the list
- `HandleDamage` is called second and adds targets
- BUT then `HandleMoveUsed` is called AGAIN for the next move and clears the list AGAIN
- By the time `TriggerPendingAttackAnimation()` is called, the list is empty

**Attempted Fixes That Failed:**
1. Only clearing targets if it's a "new" move (different Pokemon/move name) - Still cleared at wrong time
2. Never clearing targets in `HandleMoveUsed` - Caused targets to accumulate across turns
3. Clearing targets only on `TurnStartMessage` - Targets still empty when needed

### Issue 2: No Deferral

**Problem:** FaintMessage/SwitchMessage are not deferred to wait for animations.

**Current Deferral Logic:**
```csharp
bool hasActiveAnimations = _animationCoordinator?.HasActiveAnimations() ?? false;
bool shouldDefer = evt.Message is FaintMessage or SwitchMessage && hasActiveAnimations;
```

**Why This Fails:**
- When FaintMessage arrives, animations haven't been QUEUED yet
- `HasActiveAnimations` checks both queued and active animations
- But animations are only queued AFTER `TriggerPendingAttackAnimation()` is called
- `TriggerPendingAttackAnimation()` is called AFTER processing all messages
- So `hasActiveAnimations` is false when FaintMessage is checked

**Attempted Fixes That Failed:**
1. Adding `_processingMoveSequence` flag - Flag not set at right time
2. Checking both `HasActiveAnimations()` AND `_processingMoveSequence` - Still didn't defer

## Attempted Solutions (All Reverted)

### Attempt 1: Smart Target Clearing
- Only clear targets if move is "different" from pending
- **Result:** Still cleared at wrong time, target list empty

### Attempt 2: Never Clear in HandleMoveUsed
- Removed all target clearing from `HandleMoveUsed()`
- Clear only on `TurnStartMessage`
- **Result:** Targets accumulated, animations went to wrong Pokemon

### Attempt 3: Move Sequence Tracking
- Added `_processingMoveSequence` flag
- Set true on MoveUsedMessage, false on TurnStartMessage
- Defer if sequence active OR animations active
- **Result:** Flag timing didn't align with events, made things worse

### Attempt 4: Combined Approach
- Extract damage tag first in parsing (kept)
- Never clear targets in HandleMoveUsed
- Clear on TurnStartMessage
- Add move sequence tracking
- **Result:** Made everything worse, introduced new bugs

## Key Insights

1. **Event Ordering is the Core Problem:**
   - Messages are enqueued by battle sim in a specific order
   - GUI processes them one at a time
   - `TriggerPendingAttackAnimation()` is called BETWEEN message processing
   - Target collection happens at different times than animation triggering

2. **State Management is Complex:**
   - Multiple state variables track animation state
   - State changes happen at different points in event processing
   - Hard to keep all states synchronized

3. **Timing Issues:**
   - Animations are queued asynchronously
   - Deferral checks happen before animations are queued
   - No good way to predict when animations will be ready

## Related Code Locations

### Files Modified
- `ApogeeVGC/Gui/Animations/AnimationCoordinator.cs` - Animation coordination and targeting
- `ApogeeVGC/Gui/BattleGame.cs` - Event processing and deferral logic
- `ApogeeVGC/Sim/BattleClasses/Battle.Logging.cs` - Damage message parsing (kept)

### Key Log Points
```csharp
// AnimationCoordinator
Console.WriteLine("[AnimationCoordinator] New move detected...");
Console.WriteLine("[AnimationCoordinator] Found X actual target positions...");

// BattleGame
Console.WriteLine("[BattleGame.ProcessBattleEvent] Deferring...");
Console.WriteLine("[BattleGame.Update] Processing deferred event...");

// Battle.Combat
Console.WriteLine("[SpreadDamage] Calling PrintDamageMessage...");
```

## Testing Scenario

**Setup:**
- Player: Calyrex-Ice (Leftovers), Miraidon (Choice Specs)
- Opponent: Ironhands (Assault Vest), Miraidon (Choice Specs)
- Moves: Dazzling Gleam (spread), Electro Drift (single), Heavy Slam (single), Glacial Lance (spread)

**Expected Behavior:**
1. Miraidon uses Dazzling Gleam ? Animation goes to Ironhands AND opponent Miraidon only
2. Damage numbers show on both targets
3. Glacial Lance KOs opponent Miraidon
4. Damage animation plays showing "143 damage"
5. HP bar animates from 143 ? 0
6. Faint message shows
7. Switch-in happens AFTER animations complete
8. Ursaluna appears on field

**Actual Behavior:**
1. Dazzling Gleam animation goes to ALL opponents (wrong)
2. Opponent Miraidon switches to Ursaluna immediately (too early)
3. HP animations don't have time to play
4. Damage message shows "Miraidon took damage!" without amount (partially fixed)

## Potential Solutions to Explore

### Solution A: Batch Event Processing
- Process all related messages (Move + Damage + Effectiveness) in one batch
- Collect targets BEFORE triggering animation
- Trigger animation after all targets collected

### Solution B: Two-Pass Processing
- First pass: Collect all targets from DamageMessages
- Second pass: Process MoveUsedMessage and trigger animation
- Requires restructuring event processing

### Solution C: Deferred Animation Triggering
- Don't call `TriggerPendingAttackAnimation()` between each message
- Call it only at specific points (after move sequence complete)
- Requires detecting "end of move sequence"

### Solution D: Event Pre-Processing
- Scan ahead in event queue
- Collect all targets for a move before processing MoveUsedMessage
- Populate `_pendingMoveTargets` before `HandleMoveUsed` runs

## Notes for Future Work

1. The damage parsing fix in `Battle.Logging.cs` seems sound and should be kept
2. Any solution needs to handle the async nature of event processing
3. Consider whether the current architecture (processing messages one at a time) is fundamentally flawed
4. May need to rethink when `TriggerPendingAttackAnimation()` is called
5. Deferral logic needs to happen BEFORE animations are triggered, not after

## Current State

All attempted fixes have been reverted. The code is in its original state with only the damage parsing improvement remaining. The animation targeting and deferral issues are still present and need a new approach.
