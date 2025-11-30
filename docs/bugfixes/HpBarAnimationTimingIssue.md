# HP Bar Animation Timing Issue

## Problem Description

When damage occurs during battle animations, the HP bar displays incorrectly:
1. HP bar shows the **final damaged value** briefly
2. HP bar jumps back to the **starting value** 
3. HP bar animates down to the **final value**

**Expected behavior:** HP bar should remain at the starting value during the attack animation, then smoothly animate down to the final value when the attack lands.

**Visual symptom:** HP: High ? Low (instant) ? High ? Low (animated)

## Root Cause Analysis

### Architecture Overview

The battle simulation runs on a background thread and sends events to the GUI thread:

```
[Battle Thread]                      [GUI Thread]
  Damage occurs                        
  ? DamageMessage created              
  ? Perspective updated (new HP)       
  ? Event queued                     ? Event received
                                     ? Perspective stored
                                     ? Animation triggered
                                     ? Renderer displays
```

### The Core Problem

**Events contain perspectives with final state**, not intermediate state:

```csharp
// Example: Dazzling Gleam hits two Pokemon
Turn Start: Ironhands 259 HP, Miraidon 205 HP

// Event 1: Damage to Ironhands
DamageMessage {
  Pokemon: "Ironhands",
  Damage: 84,
  RemainingHp: 175
}
Perspective: Ironhands 175 HP, Miraidon 143 HP  // BOTH already damaged!

// Event 2: Damage to Miraidon  
DamageMessage {
  Pokemon: "Miraidon", 
  Damage: 62,
  RemainingHp: 143
}
Perspective: Ironhands 175 HP, Miraidon 143 HP  // Same perspective!
```

**The timing issue:**

1. GUI receives Event 1
2. Stores perspective (Ironhands 175, Miraidon 143)
3. `BattleRenderer` immediately renders from perspective ? **Shows Miraidon at 143 HP**
4. Creates frozen animation for Ironhands at 259 HP ?
5. But no frozen animation exists for Miraidon yet ?
6. GUI receives Event 2
7. Tries to create frozen animation for Miraidon, but perspective already updated

### Why Multiple Fixes Failed

We attempted:

1. **Queuing HP bar animations separately** - Still started too early due to queue processing
2. **Creating frozen animations in `QueueHpBarAnimation`** - Got replaced immediately  
3. **Snapshot perspective comparison** - Worked for detection but animations still started too early
4. **Embedding HP animations in damage indicators** - Still processed during event handling
5. **Delaying registration with Task.Delay** - Asynchronous timing unreliable

**Core issue:** The animation queue is processed **continuously** by `AnimationManager.Update()`, which runs every frame. Any queued animation starts almost immediately, often before all events in a batch are processed.

## Key Components

### BattleGame.cs
- Receives events from battle thread via `GuiChoiceCoordinator`
- Calls `ProcessBattleEvent()` for each event
- Stores `_currentBattlePerspective` for rendering
- Maintains `_hpSnapshotPerspective` for detecting HP changes

### AnimationManager.cs
- Manages `_hpBarAnimations` dictionary
- Calls `Update()` every frame to process animation queue
- Methods:
  - `QueueHpBarAnimation()` - Queues animation and creates frozen animation
  - `RegisterHpBarAnimation()` - Stores animation in dictionary
  - `GetAnimatedHp()` - Returns animated HP value or null

### BattleRenderer.cs
- Renders Pokemon HP bars
- Code:
```csharp
int displayHp = _animationManager?.GetAnimatedHp(pokemonKey) ?? pokemon.Hp;
```
- Falls back to `pokemon.Hp` (from perspective) if no animation exists

### AnimationCoordinator.cs
- Processes messages and triggers animations
- `HandleDamage()` queues attack animation and damage indicators

## Event Processing Flow

```
ProcessBattleEvent(event):
  1. PreservePokemonHpBeforePerspectiveUpdate(event.Perspective)
     - Compare with _hpSnapshotPerspective
     - Create frozen animations for changed HP
  2. ProcessMessage(event.Message) 
     - Queues attack animation
     - Queues damage indicator (with HP animation)
  3. Store event.Perspective in _currentBattlePerspective

Update() [runs every frame]:
  1. AnimationQueue.Update()
     - Dequeues next animation
     - Calls animation.Start()
     - For HP bars: Creates animation and calls RegisterHpBarAnimation()
  2. Render using _currentBattlePerspective
     - Shows animated HP if available
     - Otherwise shows perspective HP
```

## The Race Condition

```
Frame N:
  [Event Processing]
  - Process DamageMessage 1
  - Create frozen animation: Ironhands @ 259 HP ?
  - Store perspective: Ironhands 175 HP, Miraidon 143 HP
  - Queue damage indicator + HP animation for Ironhands
  
Frame N+1:
  [Animation Update] <-- Runs BEFORE Event Processing completes!
  - AnimationQueue.Update()
  - Damage indicator starts
  - Queues HP animation for Ironhands
  - HP animation immediately starts
  - Calls RegisterHpBarAnimation(Ironhands, 259?175)
  - Replaces frozen animation ?
  
  [Event Processing continues]
  - Process DamageMessage 2
  - Try to create frozen animation: Miraidon @ 205 HP
  - But perspective already shows 143 HP!
  - Renderer shows Miraidon @ 143 HP ?
```

## Requirements for a Working Solution

1. **Preserve old HP values** for all Pokemon that will take damage in a batch of events
2. **Keep frozen animations active** until the appropriate attack animation completes
3. **Coordinate timing** between:
   - Event processing (batch of related events)
   - Animation queue processing (continuous)
   - Rendering (every frame)
4. **Handle spread moves** that generate multiple damage messages with the same perspective
5. **Chain animations** when the same Pokemon takes damage multiple times in one turn

## Attempted Solutions Summary

### Attempt 1: Basic Frozen Animations
- Created frozen animations when perspective changed
- **Failed:** Got replaced when damage indicators started

### Attempt 2: Snapshot Perspective
- Maintained separate `_hpSnapshotPerspective` that only updates between animation batches
- **Failed:** HP animations still started during event processing

### Attempt 3: Delayed Registration
- Used `Task.Delay(16)` to delay `RegisterHpBarAnimation` call
- **Failed:** Asynchronous timing unreliable, race conditions persist

### Attempt 4: Queue Management
- Tried to make HP animations wait in queue
- **Failed:** Queue processes continuously, animations start immediately

## Proposed Solution Direction

The fundamental issue is that **event processing and animation processing are interleaved**. We need to:

1. **Batch event processing**: Process all related events (move + damages) before allowing animations to start
2. **Synchronization point**: Explicitly mark when event batches complete
3. **Deferred animation start**: HP bar animations should not start until explicitly triggered after event batch completion

### Option A: Event Batch Boundaries
Mark when a batch of related events starts/ends:
```csharp
ProcessBattleEvent(event):
  if (event.IsStartOfBatch):
    _batchProcessing = true
    CreateFrozenAnimationsForBatch(event.Batch)
  
  ProcessMessage(event.Message)
  
  if (event.IsEndOfBatch):
    _batchProcessing = false
    AllowAnimationsToStart()
```

### Option B: Separate Update Phases
Don't update animation queue during event processing:
```csharp
Update():
  if (!_processingEvents):
    AnimationQueue.Update()  // Only process when not handling events
  
  ProcessQueuedEvents()  // Sets _processingEvents flag
```

### Option C: Manual Animation Coordination
Remove HP bar animations from queue entirely:
```csharp
// Track pending HP animations
_pendingHpAnimations = []

HandleDamage():
  _pendingHpAnimations.Add(new HPAnimation(...))
  // Don't queue yet
  
AllAnimationsComplete():
  // Start all pending HP animations
  foreach (var anim in _pendingHpAnimations):
    anim.Start()
    RegisterHpBarAnimation(anim)
```

## Testing Scenario

Use Dazzling Gleam (hits both opponents) to reproduce:
- Turn start: Ironhands 259 HP, Miraidon 205 HP
- Dazzling Gleam damages both
- Expected final: Ironhands 175 HP, Miraidon 143 HP

Watch for:
- Miraidon HP jumping to 143 before animation starts
- Both HP bars should stay at original values until attack lands
- Both HP bars should animate smoothly down together

## Related Files

- `ApogeeVGC/Gui/BattleGame.cs` - Event processing, perspective storage
- `ApogeeVGC/Gui/Animations/AnimationManager.cs` - Animation lifecycle
- `ApogeeVGC/Gui/Animations/AnimationQueue.cs` - Queue processing
- `ApogeeVGC/Gui/Animations/HpBarAnimation.cs` - HP bar animation logic
- `ApogeeVGC/Gui/Animations/AnimationCoordinator.cs` - Message processing
- `ApogeeVGC/Gui/Rendering/BattleRenderer.cs` - HP display logic

## Log Markers to Watch

```
[BattleGame] Creating frozen animation for {Pokemon}|{Side}: {OldHP} HP (new perspective shows {NewHP})
[QueuedHpBarAnimation] Starting HP animation for {Pokemon}|{Side}: {Old} -> {New}
[QueuedHpBarAnimation] Registered HP animation for {Pokemon}|{Side}
[AnimationManager] Preserving frozen animation for {Pokemon}|{Side} at {HP} HP
```

## Notes

- The issue is timing/coordination, not the animation logic itself
- Frozen animations work correctly when they're not replaced prematurely  
- The animation queue system is designed for sequential animations, but HP bars need to start at precise moments
- Consider whether HP bar animations should even use the queue, or need a separate mechanism
