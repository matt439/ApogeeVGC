# Perspective-Diff Animation System Implementation

## Overview
Successfully implemented a new animation system that uses perspective diffs instead of message-driven target tracking. This solves all three issues documented in `AnimationTargetingAndDeferralIssues.md`.

## Date
2025-01-XX

## Issues Resolved

### ? Issue 1: Attack Animations Target All Opponents
**Problem:** Attack animations went to all opponents because target list was empty (`_pendingMoveTargets` cleared at wrong time).

**Solution:** Eliminated target list entirely. Targets are now identified by comparing HP values between perspectives:
- Detect which Pokemon took damage: `previousHP > currentHP`
- Queue attack animation with those specific Pokemon as targets
- No more timing issues or state management bugs

### ? Issue 2: Pokemon Switch Without Waiting for Animations
**Problem:** Switch events processed immediately without checking if damage animations were active.

**Solution:** Deferral logic now works correctly:
- `HasActiveAnimations()` is checked before processing faint/switch messages
- Events are deferred until animations complete
- Natural sequencing: attack ? damage ? HP bar ? faint ? switch

### ? Issue 3: Damage Amount Not Shown When Pokemon Faints
**Problem:** Damage parsing failed when Pokemon fainted, showing "took damage!" without amount.

**Solution:** Damage amount calculated directly from perspective diff:
```csharp
int damage = previousPerspective.Pokemon.Hp - currentPerspective.Pokemon.Hp;
```
- Always accurate, even when Pokemon faints (shows HP went from X to 0)
- No parsing required, no edge cases

## Architecture

### New System: `PerspectiveDiffAnimationSystem`

**Location:** `ApogeeVGC/Gui/Animations/PerspectiveDiffAnimationSystem.cs`

**Key Concept:** Use perspectives for state changes, messages for context.

```
???????????????????????????????????????????????????
? BattleEvent                                      ?
???????????????????????????????????????????????????
? • Message: Context (who attacked, what move)    ?
? • Perspective: State after message processed    ?
???????????????????????????????????????????????????
                       ?
???????????????????????????????????????????????????
? PerspectiveDiffAnimationSystem.ProcessEvent()   ?
???????????????????????????????????????????????????
? 1. Track move context from MoveUsedMessage      ?
? 2. Process message indicators (miss, crit)      ?
? 3. Compare perspectives to detect HP changes    ?
? 4. Queue animations based on state changes      ?
???????????????????????????????????????????????????
```

### How It Works

#### 1. HP Change Detection
```csharp
private List<HpChangeInfo> DetectHpChanges(
    IReadOnlyList<PokemonPerspective?> prevActive,
    IReadOnlyList<PokemonPerspective?> currActive,
    bool isPlayer)
{
    // Compare HP for each Pokemon in same slot
    if (prev.Name == curr.Name && prev.Hp != curr.Hp)
    {
        // HP changed - calculate delta
        int hpDelta = curr.Hp - prev.Hp;
        // Negative = damage, positive = healing
    }
}
```

#### 2. Attack Animation Targeting
```csharp
// When HP changes detected AND move is pending:
if (allHpChanges.Any(c => c.HpDelta < 0) && _pendingMove != null)
{
    // Get positions of Pokemon that took damage
    List<Vector2> targets = allHpChanges
        .Where(c => c.HpDelta < 0)
        .Select(c => c.Position)
        .ToList();
    
    // Queue attack animation to those specific targets
    QueueAttackAnimation(_pendingMove, targets);
}
```

#### 3. Switch Detection
```csharp
// Compare Pokemon names in same slot
if (prev?.Name != curr?.Name)
{
    // Different Pokemon = switch occurred
    // HP bar system will show new Pokemon automatically
}
```

### Data Flow

```
Battle Simulation
    ?
BattleEvent { Message, Perspective }
    ?
BattleGame.ProcessBattleEvent()
    ?
PerspectiveDiffAnimationSystem.ProcessEvent()
    ?
???????????????????????????????????????????
? 1. Store move context from message      ?
? 2. Compare perspectives (prev vs curr)  ?
? 3. Detect HP changes                    ?
? 4. Queue attack animation with targets  ?
? 5. Queue damage indicators              ?
? 6. Store perspective for next diff      ?
???????????????????????????????????????????
    ?
AnimationManager (handles playback)
```

## Code Changes

### Files Modified
1. **ApogeeVGC/Gui/BattleGame.cs**
   - Replaced `AnimationCoordinator` with `PerspectiveDiffAnimationSystem`
   - Removed `_hpSnapshotPerspective` field (obsolete)
   - Removed `PreservePokemonHpBeforePerspectiveUpdate()` method (obsolete)
   - Removed manual `TriggerPendingAttackAnimation()` calls (automatic now)
   - Simplified `ProcessBattleEvent()` - just calls `_animationSystem.ProcessEvent(evt)`

### Files Created
1. **ApogeeVGC/Gui/Animations/PerspectiveDiffAnimationSystem.cs**
   - New animation system that uses perspective diffs
   - `ProcessEvent()` - main entry point
   - `DetectHpChanges()` - compares perspectives
   - `QueueAttackAnimation()` - uses detected targets
   - `QueueDamageAnimation()` - calculates damage from HP diff

### Files Unchanged (Can Be Deprecated Later)
1. **ApogeeVGC/Gui/Animations/AnimationCoordinator.cs**
   - Old message-driven system
   - No longer used but kept for reference
   - Can be deleted in future cleanup

## Benefits

### 1. Eliminates State Management Bugs
- No more `_pendingMoveTargets` list to track
- No more timing issues between `HandleMoveUsed` and `HandleDamage`
- No more "when to clear targets" bugs

### 2. Simpler Code
- Old system: ~400 lines with complex state tracking
- New system: ~350 lines, straightforward diff logic
- Easier to understand and maintain

### 3. Accurate Damage Display
- Always shows correct damage amount
- Works even when Pokemon faints
- No parsing edge cases

### 4. Reliable Targeting
- Attack animations go to actual targets
- Based on "who took damage" not "who might take damage"
- No fallbacks to "all opponents"

### 5. Better Animation Timing
- Deferral works correctly
- HP bars animate before switches
- Natural event sequencing

## Testing

### Test Scenario
- **Setup:** Doubles battle, multiple spread moves
- **Moves:** Dazzling Gleam (hits 2), Electro Drift (hits 1), Glacial Lance (KO)
- **Expected:** Animations target correct Pokemon, damage shown, switches wait for animations

### What to Verify
1. ? Dazzling Gleam hits only the two opponents it damaged
2. ? Damage numbers show correct amounts
3. ? When Pokemon faints, damage amount is displayed
4. ? HP bar animates from X ? 0 before switch
5. ? Replacement Pokemon appears after HP animation completes
6. ? No race conditions or timing bugs

## Issues Found During Testing

### Issue 1: Spread Moves Shown as Single-Target
**Problem:** Dazzling Gleam hit 2 Pokemon but animation only showed 1 target.

**Root Cause:** Attack animation was queued on the **first DamageMessage**, before the second target's damage was processed. Each `BattleEvent` has a perspective showing state **after that specific message**, so targets were detected incrementally.

**Fix:** Accumulate all damage targets from the same move before queueing the attack animation. Store `_moveStartPerspective` and compare all subsequent damage against it to detect all targets.

```csharp
// When MoveUsedMessage arrives:
_moveStartPerspective = _previousPerspective; // Snapshot before damage

// When DamageMessage arrives:
// Compare curr vs _moveStartPerspective (not curr vs prev)
// This detects ALL damage from the move, not incremental changes
_pendingDamageTargets.Add(change); // Accumulate

// When next MoveUsedMessage (or non-damage event) arrives:
QueueAttackAnimation(_pendingMove, _pendingDamageTargets); // Use all targets
```

### Issue 2: HP Bars Animate Incorrectly (final?prev?final)
**Problem:** HP bars would jump to final value, then animate backwards to previous, then forward again.

**Root Cause:** Same as Issue 1 - comparing perspectives incrementally meant HP changes were detected as the **difference between consecutive events**, not the **total change from the move**.

**Fix:** Use `_moveStartPerspective` as the baseline for all HP comparisons during a move sequence. This gives the **total damage** from the move, not incremental changes.

### Changes Made
1. Added `_moveStartPerspective` field to store perspective when move starts
2. Added `_pendingDamageTargets` list to accumulate all targets before animating
3. Changed `DetectAndQueueAnimations()` to `DetectAndAccumulateDamage()` 
4. Attack animation is queued when next move starts (or non-damage event processed)
5. HP changes are compared against move start perspective, not previous event

## Performance

### Perspective Generation Overhead
**Concern:** Does generating perspectives for every message hurt performance?

**Answer:** No additional overhead! Battle already generates perspectives for every event:
```csharp
// In Battle.Logging.cs - Add() method
BattlePerspective perspective = GetPerspectiveForSide(SideId.P1, perspectiveType);
PendingEvents.Add(new BattleEvent { Message = parsed, Perspective = perspective });
```

Perspectives are generated once and used by both:
- GUI rendering (to show current state)
- Animation system (to detect changes)

### Comparison Cost
- Comparing two perspectives: O(number of active Pokemon × fields)
- Typical: 4 active Pokemon × ~10 fields = 40 comparisons
- **Negligible** compared to rendering, animation, and game logic

## Future Enhancements

### 1. Stat Boost Animations
```csharp
// Detect stat changes in BoostsTable
if (prev.Boosts != curr.Boosts)
{
    // Show stat up/down indicators
}
```

### 2. Status Condition Icons
```csharp
// Detect status changes
if (prev.Status != curr.Status)
{
    // Show status icon (burn, paralysis, etc.)
}
```

### 3. Switch Animations
```csharp
// When Pokemon switches
if (prev?.Name != curr?.Name)
{
    QueueSwitchOutAnimation(prev);
    QueueSwitchInAnimation(curr);
}
```

### 4. Healing Animations
```csharp
// When HP increases
if (change.HpDelta > 0)
{
    QueueHealingAnimation(change);
    // Could show "+X HP" indicator
}
```

## Migration Notes

### Old AnimationCoordinator Still Exists
The old `AnimationCoordinator.cs` file still exists but is no longer used. It can be safely deleted, but is kept temporarily for:
- Reference if issues arise
- Comparison with new implementation
- Historical documentation

### No Breaking Changes to Battle Simulation
The battle simulation code (`Battle.Logging.cs`) was not modified. The change is entirely within the GUI layer.

### Backwards Compatibility
Team preview and other non-battle screens work unchanged. The new system only activates during `BattlePerspectiveType.InBattle`.

## Related Documentation
- `AnimationTargetingAndDeferralIssues.md` - Original problem description
- `HpBarAnimationTimingIssue.md` - Related HP bar timing fixes
- `GuiTeamPreviewFix.md` - Team preview compatibility

## Credits
Implemented based on insight that Battle already sends perspectives with every message, making perspective-diff approach zero-overhead and architecturally sound.
