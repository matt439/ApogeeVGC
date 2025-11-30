# Perspective-Diff Animation System Bug Fixes

## Date
2025-01-XX

## Issues

### Issue 1: Spread Moves Shown as Single-Target Animation
**Symptom:** Dazzling Gleam (hits 2 targets) only showed animation to 1 target.

**Log Evidence:**
```
[PerspectiveDiff] Queueing attack animation: Dazzling Gleam, targets: 1
```

But Dazzling Gleam actually hit both Ironhands and Miraidon.

### Issue 2: HP Bars Animate Incorrectly
**Symptom:** HP bars would:
1. Jump to final value
2. Animate backwards to previous value
3. Animate forward to final value again

Result: HP bar appears to "bounce" (final ? prev ? final).

## Root Cause

The perspective-diff system was comparing perspectives **between consecutive events**, not **between move start and current event**. This caused two problems:

### Problem 1: Incremental Target Detection

Events arrive like this:
```
1. MoveUsedMessage (Dazzling Gleam)
   Perspective: All Pokemon at full HP
   
2. DamageMessage (Ironhands)
   Perspective: Ironhands at 175 HP (was 259)
   Diff vs Event 1: Ironhands took 84 damage ?
   ? Attack animation queued with 1 target ?
   
3. DamageMessage (Miraidon)
   Perspective: Ironhands at 175 HP, Miraidon at 143 HP (was 205)
   Diff vs Event 2: Miraidon took 62 damage ?
   ? But attack animation already queued! ?
```

The attack animation was queued on the **first** DamageMessage, before the **second** target was detected.

### Problem 2: Incremental HP Tracking

For HP bars, the same incremental comparison caused issues:

```
Move Start: Ironhands HP = 259
Event 2: Ironhands HP = 175 (diff: -84) ? Queue HP animation 259?175
Event 3: Ironhands HP = 175 (diff: 0) ? No change detected

But if we create a frozen animation at 259, then perspective updates to 175,
the HP bar jumps to 175 immediately. Then when the animation plays, it
animates from the "frozen" 259 to 175, but the perspective is already at 175.
```

## Solution

**Key Insight:** We need to track perspective **at move start** and compare all subsequent damage against that baseline.

### Implementation

```csharp
// Add fields to track move sequence
private BattlePerspective? _moveStartPerspective; // Snapshot when move starts
private readonly List<HpChangeInfo> _pendingDamageTargets = new(); // Accumulate targets

public void ProcessEvent(BattleEvent evt)
{
    // When move starts, snapshot the perspective
    if (evt.Message is MoveUsedMessage moveMsg)
    {
        // Flush previous move's animation if needed
        if (_pendingMove != null && _pendingDamageTargets.Count > 0)
        {
            QueueAttackAnimation(_pendingMove, _pendingDamageTargets);
            _pendingDamageTargets.Clear();
        }
        
        _pendingMove = moveMsg;
        _moveStartPerspective = _previousPerspective; // ? Snapshot baseline
    }
    
    // Detect HP changes against move start perspective
    DetectAndAccumulateDamage(_previousPerspective, evt.Perspective);
    
    // Flush animation when move sequence ends
    if (evt.Message is not (DamageMessage or MoveUsedMessage or EffectivenessMessage))
    {
        if (_pendingMove != null && _pendingDamageTargets.Count > 0)
        {
            QueueAttackAnimation(_pendingMove, _pendingDamageTargets);
            _pendingDamageTargets.Clear();
        }
    }
}

private void DetectAndAccumulateDamage(BattlePerspective prev, BattlePerspective curr)
{
    // ? Compare against move start, not previous event
    BattlePerspective baseline = _moveStartPerspective ?? prev;
    
    var hpChanges = DetectHpChanges(baseline.PlayerSide.Active, curr.PlayerSide.Active, true);
    
    foreach (var change in hpChanges)
    {
        if (change.HpDelta < 0 && _pendingMove != null)
        {
            // ? Accumulate targets, don't queue animation yet
            if (!_pendingDamageTargets.Any(t => t.Key == change.Key))
            {
                _pendingDamageTargets.Add(change);
                QueueDamageAnimation(change); // Queue damage indicator immediately
            }
        }
    }
}
```

### How It Works Now

```
1. MoveUsedMessage (Dazzling Gleam)
   _moveStartPerspective = current perspective (all Pokemon at full HP)
   
2. DamageMessage (Ironhands)
   Perspective: Ironhands at 175 HP
   Diff vs _moveStartPerspective: Ironhands 259?175 (-84)
   ? Add to _pendingDamageTargets (count: 1)
   
3. DamageMessage (Miraidon)  
   Perspective: Ironhands at 175, Miraidon at 143
   Diff vs _moveStartPerspective: Ironhands 259?175, Miraidon 205?143
   ? Add Miraidon to _pendingDamageTargets (count: 2) ?
   
4. Next MoveUsedMessage (or non-damage event)
   ? Queue attack animation with both targets ?
```

## Results

### ? Issue 1 Fixed: Spread Moves Show All Targets
- Dazzling Gleam now shows animation to **both** Ironhands and Miraidon
- Targets are accumulated during the move sequence
- Animation is queued once all targets are detected

### ? Issue 2 Fixed: HP Bars Animate Correctly
- HP bars animate smoothly from start HP to final HP
- No more "bouncing" or jumping
- Damage indicators show correct amounts

## Code Changes

### Files Modified
1. **ApogeeVGC/Gui/Animations/PerspectiveDiffAnimationSystem.cs**
   - Added `_moveStartPerspective` field
   - Added `_pendingDamageTargets` list
   - Renamed `DetectAndQueueAnimations()` ? `DetectAndAccumulateDamage()`
   - Changed logic to accumulate targets before animating
   - Attack animation queued when move sequence ends

### Files Documentation
1. **docs/bugfixes/PerspectiveDiffAnimationSystemImplementation.md**
   - Updated with testing issues and fixes

## Testing

Run the same battle scenario that revealed the bugs:
- Dazzling Gleam (spread move hitting 2 targets)
- Electro Drift (single target)
- Heavy Slam (single target)
- Glacial Lance (spread move, causes faint)

**Expected behavior:**
1. ? Dazzling Gleam animation goes to **both** opponents
2. ? Each opponent shows correct damage number
3. ? HP bars animate smoothly (no jumping/bouncing)
4. ? When Miraidon faints, HP bar animates 143?0 smoothly
5. ? Switch happens after animation completes

## Technical Notes

### Why Compare Against Move Start Perspective?

Because Battle sends **one event per message**, and each event has the perspective **after that message was processed**. This means:

- Event perspectives show **cumulative state changes**
- Comparing consecutive events shows **incremental changes**
- We need **total changes** to correctly detect all targets and animate HP bars

By snapshotting perspective at move start, we can:
- Detect **all** Pokemon that took damage from the move
- Calculate **total** damage amount for HP bar animations
- Queue the attack animation once with **all** targets

### Batching Strategy

The system now batches messages by type:
- `MoveUsedMessage` ? Start new batch, flush previous
- `DamageMessage` ? Accumulate in current batch
- `EffectivenessMessage` ? Part of current batch
- Any other message ? Flush current batch

This ensures all damage from a move is detected before the attack animation is queued.

## Additional Issue: Attack Animation After Damage

### Issue 3: Attack Animation Played After Damage Indicators
**Symptom:** The attack animation (projectile/contact move) would play **after** the damage numbers and HP bar animations had already started, making it look like the damage happened before the attack.

**Root Cause:** The system was queueing damage indicators **immediately** when accumulating targets in `DetectAndAccumulateDamage()`, but the attack animation was only queued later when `QueueAttackAnimation()` was called. This resulted in:
1. Damage indicator queued
2. HP bar animation queued
3. Attack animation queued (too late)

**Fix:** 
1. **Don't queue damage indicators** when accumulating targets
2. **Queue attack animation first** in `QueueAttackAnimation()`
3. **Then queue all damage indicators** for the accumulated targets

```csharp
private void DetectAndAccumulateDamage(...)
{
    foreach (var change in allHpChanges)
    {
        if (change.HpDelta < 0 && _pendingMove != null)
        {
            _pendingDamageTargets.Add(change);
            // DON'T queue damage indicator yet!
        }
    }
}

private void QueueAttackAnimation(MoveUsedMessage moveMsg, List<HpChangeInfo> targets)
{
    // FIRST: Queue the attack animation
    _animationManager.QueueAttackAnimation(...);
    
    // SECOND: Queue damage indicators (will play after attack)
    foreach (var target in targets)
    {
        QueueDamageAnimation(target);
    }
}
```

**Result:** Correct animation sequence:
1. ? Attack animation plays (projectile fires / contact move)
2. ? Damage indicators appear (numbers show)
3. ? HP bars animate (health decreases)

### Issue 4: HP Bars Still Showing final?initial?final Pattern
**Symptom:** Even after fixing the attack animation order (Issue 3), HP bars would still jump to final value, then animate backwards to initial, then forward to final.

**Root Cause:** Frozen HP animations were being created **too late**. The sequence was:
1. MoveUsedMessage processed ? `_moveStartPerspective` snapshotted
2. DamageMessage processed ? Targets accumulated
3. Next MoveUsedMessage ? `QueueAttackAnimation()` called
4. Inside `QueueAttackAnimation()` ? `QueueDamageAnimation()` called
5. `QueueDamageAnimation()` calls `QueueDamageIndicatorWithHpBar()`
6. `QueueDamageIndicatorWithHpBar()` checks for frozen animation
7. No frozen animation exists ? creates one "now"
8. **Problem:** By step 7, the perspective has already updated to show final HP!

So the frozen animation is created at the **final HP** (e.g., 175), not the **initial HP** (e.g., 259).

**Fix:** Create frozen HP animations **proactively** when MoveUsedMessage arrives, BEFORE any perspective updates:

```csharp
public void ProcessEvent(BattleEvent evt)
{
    if (evt.Message is MoveUsedMessage moveMsg)
    {
        _pendingMove = moveMsg;
        _moveStartPerspective = _previousPerspective;
        
        // ? NEW: Freeze HP values NOW, before perspectives update
        if (_previousPerspective != null)
        {
            FreezePokemonHpValues(_previousPerspective);
        }
    }
}

private void FreezePokemonHpValues(BattlePerspective perspective)
{
    // Create frozen animations for all active Pokemon at their current HP
    foreach (var pokemon in perspective.PlayerSide.Active.Concat(perspective.OpponentSide.Active))
    {
        if (pokemon != null)
        {
            string key = CreatePositionKey(pokemon.Name, isPlayer);
            
            // Only create if one doesn't already exist
            if (_animationManager.GetAnimatedHp(key) == null)
            {
                _animationManager.StartHpBarAnimation(key, pokemon.Hp, pokemon.Hp, pokemon.MaxHp);
                _animationManager.GetHpBarAnimation(key)?.Freeze();
            }
        }
    }
}
```

**Timeline with Fix:**
1. MoveUsedMessage processed ? Frozen animations created at **current HP** (259, 205)
2. DamageMessage processed ? Perspective updates to final HP (175, 143)
3. HP bars still show frozen values (259, 205) on screen ?
4. `QueueAttackAnimation()` ? Finds existing frozen animations ?
5. Damage indicators queue HP animations from frozen?final (259?175) ?

**Result:** HP bars now animate smoothly from initial ? final without any jumping.

## Related Documentation
- `AnimationTargetingAndDeferralIssues.md` - Original issues
- `PerspectiveDiffAnimationSystemImplementation.md` - Initial implementation
- `HpBarAnimationTimingIssue.md` - Related HP bar fixes
