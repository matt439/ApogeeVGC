# GUI Architecture Overhaul: Perspective-Per-Message

## Problem Statement

The current GUI architecture maintains its own battle state (`GuiBattleState`) that must be kept in sync with the battle engine through incremental message processing. This leads to:

1. **State Synchronization Bugs**: GUI state and battle perspective frequently mismatch
2. **Complex Initialization Logic**: Need special handling for team preview ? battle transition
3. **Message Order Dependencies**: Switch messages must be processed before damage, etc.
4. **Validation Failures**: Constant HP mismatches and state inconsistencies
5. **~500+ Lines of State Management**: `GuiBattleState`, `EventProcessor`, state validation, etc.

## Proposed Solution

**Battle engine sends `(BattlePerspective, BattleMessage)` pairs instead of just messages.**

The battle perspective represents the **authoritative state after the message was processed**. The GUI:
- Uses the message to select which animation to play
- Uses the perspective to know where to render Pokémon
- Does NOT maintain its own state

## Architecture Changes

### 1. New Message Container

Create a new type to hold the perspective + message pair:

```csharp
namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Represents a battle event: a message and the resulting perspective after that message was processed
/// </summary>
public record BattleEvent
{
    /// <summary>
    /// The message describing what happened
    /// </summary>
    public required BattleMessage Message { get; init; }
    
    /// <summary>
    /// The battle perspective AFTER this message was processed
    /// This is the authoritative state - GUI renders this directly
    /// </summary>
    public required BattlePerspective Perspective { get; init; }
}
```

**File to create**: `ApogeeVGC/Sim/BattleClasses/BattleEvent.cs`

### 2. Modify Battle Message Emission

Update `Battle.Logging.cs` to generate perspectives with messages.

**Current code** (in `Battle.Logging.cs`):
```csharp
public void AddMessage(BattleMessage message)
{
    if (DisplayUi)
    {
        _pendingMessages.Add(message);
    }
}
```

**New code**:
```csharp
public void AddMessage(BattleMessage message)
{
    if (DisplayUi)
    {
        // Generate perspective for this player
        BattlePerspective perspective = GetPerspective(SideId.P1); // or track which player
        
        var battleEvent = new BattleEvent
        {
            Message = message,
            Perspective = perspective
        };
        
        _pendingEvents.Add(battleEvent);
    }
}
```

**Key points**:
- Only generate perspectives when `DisplayUi == true` (no performance cost for non-GUI battles)
- Rename `_pendingMessages` ? `_pendingEvents`
- Change `List<BattleMessage>` ? `List<BattleEvent>`
- Update `FlushMessages()` ? `FlushEvents()`

**Files to modify**:
- `ApogeeVGC/Sim/BattleClasses/Battle.Logging.cs`
- `ApogeeVGC/Sim/BattleClasses/Battle.Core.cs` (field declarations)

### 3. Update Player Interface

Change `IPlayer.UpdateMessages` to accept events instead of just messages.

**Current interface**:
```csharp
void UpdateMessages(IEnumerable<BattleMessage> messages);
```

**New interface**:
```csharp
void UpdateEvents(IEnumerable<BattleEvent> events);
```

**Files to modify**:
- `ApogeeVGC/Sim/Player/IPlayer.cs`
- `ApogeeVGC/Sim/Player/PlayerGui.cs`
- `ApogeeVGC/Sim/Player/PlayerRandom.cs` (if it implements UpdateMessages)
- Any other IPlayer implementations

### 4. Simplify GUI Event Processing

Remove all state management and just render perspectives.

**Remove these files entirely**:
- `ApogeeVGC/Gui/State/GuiBattleState.cs`
- `ApogeeVGC/Gui/State/PokemonState.cs`
- `ApogeeVGC/Gui/EventProcessing/EventProcessor.cs`
- `ApogeeVGC/Gui/EventProcessing/BattleEventQueue.cs`

**Modify `PlayerGui.cs`**:
```csharp
public void UpdateEvents(IEnumerable<BattleEvent> events)
{
    // No longer building turn batches - just forward events to coordinator
    foreach (var evt in events)
    {
        ChoiceCoordinator.AddBattleEvent(evt);
    }
}
```

**Modify `GuiChoiceCoordinator.cs`**:

Remove all turn batch logic. Replace with simple event queue:

```csharp
private readonly ConcurrentQueue<BattleEvent> _eventQueue = new();

public void AddBattleEvent(BattleEvent evt)
{
    _eventQueue.Enqueue(evt);
    Console.WriteLine($"[GuiChoiceCoordinator] Enqueued event: {evt.Message.GetType().Name}");
}

public bool TryDequeueEvent(out BattleEvent? evt)
{
    return _eventQueue.TryDequeue(out evt);
}
```

**Files to modify**:
- `ApogeeVGC/Gui/GuiChoiceCoordinator.cs`
- `ApogeeVGC/Sim/Player/PlayerGui.cs`

### 5. Simplify BattleGame Update Loop

Remove all state initialization and validation logic.

**Modify `BattleGame.Update`**:

```csharp
protected override void Update(GameTime gameTime)
{
    try
    {
        // Process events from battle thread
        while (_choiceCoordinator.TryDequeueEvent(out BattleEvent? evt) && evt != null)
        {
            ProcessBattleEvent(evt);
        }
        
        // Process choice requests (unchanged)
        while (_choiceCoordinator.TryDequeueRequest(out PendingChoiceRequest? pendingRequest) && 
               pendingRequest != null)
        {
            // ... existing choice processing code ...
        }
        
        // Update systems (unchanged)
        _choiceInputManager?.Update(gameTime);
        _animationManager?.Update(gameTime);
        
        // Check exit conditions (unchanged)
        if (_shouldExit) Exit();
        if (_battleRunner is { IsCompleted: true } && !_battleCompleteShown)
        {
            Console.WriteLine($"Battle complete! Winner: {_battleRunner.Result}");
            _battleCompleteShown = true;
        }
        
        base.Update(gameTime);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[BattleGame.Update] EXCEPTION: {ex.Message}");
        throw;
    }
}

private void ProcessBattleEvent(BattleEvent evt)
{
    // Store perspective for rendering
    lock (_stateLock)
    {
        _currentBattlePerspective = evt.Perspective;
    }
    
    // Add message to display queue
    lock (_stateLock)
    {
        _messageQueue.Add(evt.Message);
        if (_messageQueue.Count > MaxMessagesDisplayed)
        {
            _messageQueue.RemoveRange(0, _messageQueue.Count - MaxMessagesDisplayed);
        }
    }
    
    // Process message for animations
    _animationCoordinator?.ProcessMessage(evt.Message);
}
```

**Files to modify**:
- `ApogeeVGC/Gui/BattleGame.cs`

### 6. Simplify Rendering

Always render from `_currentBattlePerspective`. No more state-based vs perspective-based rendering.

**Modify `BattleGame.Draw`**:

```csharp
protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);
    _spriteBatch?.Begin();

    // Get current perspective
    BattlePerspective? perspective;
    lock (_stateLock)
    {
        perspective = _currentBattlePerspective;
    }

    // Register Pokemon positions for animations
    if (_animationCoordinator != null && perspective != null && 
        perspective.PerspectiveType == BattlePerspectiveType.InBattle)
    {
        RegisterPokemonPositions(perspective);
    }

    // Always render from perspective - single code path!
    _battleRenderer?.Render(gameTime, perspective);
    
    // Render UI and messages
    _choiceInputManager?.Render(gameTime);
    _messageRenderer?.Render(gameTime, GetMessages());

    _spriteBatch?.End();
    base.Draw(gameTime);
}

private void RegisterPokemonPositions(BattlePerspective perspective)
{
    _animationCoordinator!.ClearPokemonPositions();
    _animationCoordinator.SetPlayerSideId(SideId.P1);
    
    // Register player active Pokemon
    for (int slot = 0; slot < perspective.PlayerSide.Active.Count; slot++)
    {
        var pokemon = perspective.PlayerSide.Active[slot];
        if (pokemon != null)
        {
            int xPosition = 20 + (slot * 188);
            var position = new Vector2(xPosition + 64, 400 + 64);
            _animationCoordinator.RegisterPokemonPosition(pokemon.Name, position, slot, true);
        }
    }
    
    // Register opponent active Pokemon
    for (int slot = 0; slot < perspective.OpponentSide.Active.Count; slot++)
    {
        var pokemon = perspective.OpponentSide.Active[slot];
        if (pokemon != null)
        {
            int xPosition = 480 + (slot * 188);
            var position = new Vector2(xPosition + 64, 80 + 64);
            _animationCoordinator.RegisterPokemonPosition(pokemon.Name, position, slot, false);
        }
    }
}
```

**Remove from `BattleRenderer.cs`**:
- `RenderBattleState()` method
- `RenderPlayerPokemonFromState()` method
- `RenderOpponentPokemonFromState()` method
- `RenderPokemonState()` method

**Keep only**:
- `Render(BattlePerspective)` method
- `RenderTeamPreview()` method
- `RenderInBattle()` method

**Files to modify**:
- `ApogeeVGC/Gui/BattleGame.cs`
- `ApogeeVGC/Gui/Rendering/BattleRenderer.cs`

## Implementation Checklist

### Phase 1: Create New Types
- [ ] Create `BattleEvent.cs` record

### Phase 2: Modify Battle Engine
- [ ] Update `Battle.Logging.cs` to create `BattleEvent` objects
- [ ] Change `_pendingMessages` ? `_pendingEvents` (type change)
- [ ] Update `AddMessage()` to call `GetPerspective()`
- [ ] Update `FlushMessages()` ? `FlushEvents()`
- [ ] Guard perspective generation with `if (DisplayUi)`

### Phase 3: Update Player Interface
- [ ] Change `IPlayer.UpdateMessages()` ? `UpdateEvents()`
- [ ] Update `PlayerGui.UpdateEvents()` implementation
- [ ] Update `PlayerRandom.UpdateEvents()` (or remove if unused)
- [ ] Remove `PlayerGui.UpdateUi()` if it exists

### Phase 4: Simplify GUI
- [ ] Delete `GuiBattleState.cs`
- [ ] Delete `PokemonState.cs`
- [ ] Delete `EventProcessor.cs`
- [ ] Delete `BattleEventQueue.cs`
- [ ] Delete `TurnEventBatch.cs`

### Phase 5: Update GuiChoiceCoordinator
- [ ] Remove all turn batch fields/methods
- [ ] Add `ConcurrentQueue<BattleEvent> _eventQueue`
- [ ] Implement `AddBattleEvent(BattleEvent)`
- [ ] Implement `TryDequeueEvent(out BattleEvent?)`
- [ ] Remove `StartTurnBatch()`, `CompleteTurnBatch()`, etc.

### Phase 6: Simplify BattleGame
- [ ] Remove `_battleState` field
- [ ] Remove `_eventQueue` field
- [ ] Remove `_eventProcessor` field
- [ ] Simplify `Update()` to just process events
- [ ] Add `ProcessBattleEvent(BattleEvent)` method
- [ ] Remove state initialization logic
- [ ] Remove state validation logic

### Phase 7: Simplify Rendering
- [ ] Update `BattleGame.Draw()` to only use perspective
- [ ] Remove state-based rendering check (`hasActivePokemon`)
- [ ] Remove `RenderBattleState()` from `BattleRenderer`
- [ ] Keep only perspective-based rendering

### Phase 8: Testing
- [ ] Test team preview ? battle transition
- [ ] Test switch-ins (like Ursaluna replacing fainted Pokemon)
- [ ] Test animations trigger correctly
- [ ] Test HP bars update correctly
- [ ] Test message display
- [ ] Verify no performance issues (perspectives only created when `DisplayUi == true`)

## Expected Benefits

1. **~500 lines of code removed**: All state management gone
2. **No sync bugs possible**: GUI always renders authoritative state
3. **Simpler to debug**: Single rendering path, clear data flow
4. **Easier to extend**: Adding new message types is trivial
5. **All existing bugs fixed**: Team preview, switch-ins, HP validation, etc.

## Performance Notes

- Perspectives are only generated when `Battle.DisplayUi == true`
- Non-GUI battles (tournaments, benchmarks) have zero overhead
- GUI battles: ~1 perspective per message (acceptable for real-time play)
- Perspective generation is fast (just copying references to existing Pokemon)

## Migration Notes

### Breaking Changes

- `IPlayer.UpdateMessages(IEnumerable<BattleMessage>)` ? `UpdateEvents(IEnumerable<BattleEvent>)`
- `GuiChoiceCoordinator` public API changes (turn batch methods removed)

### Backwards Compatibility

If other players (non-GUI) don't need events:
- Keep `UpdateMessages()` as default implementation (does nothing)
- Add `UpdateEvents()` as optional override
- Only `PlayerGui` needs to implement `UpdateEvents()`

## Questions for Implementation

1. **Which player's perspective?** 
   - Always P1? 
   - Track perspective per player?
   - Current assumption: GUI player is always P1 (Matt)

2. **Team preview handling?**
   - Should team preview also send events?
   - Or keep legacy `UpdateUi()` for team preview only?
   - Recommendation: Send events for team preview too (consistency)

3. **Old message queue?**
   - Keep for display purposes?
   - Yes, messages are still shown in message log

## File Summary

### Files to Create
- `ApogeeVGC/Sim/BattleClasses/BattleEvent.cs`

### Files to Delete
- `ApogeeVGC/Gui/State/GuiBattleState.cs`
- `ApogeeVGC/Gui/State/PokemonState.cs`
- `ApogeeVGC/Gui/EventProcessing/EventProcessor.cs`
- `ApogeeVGC/Gui/EventProcessing/BattleEventQueue.cs`
- `ApogeeVGC/Gui/EventProcessing/TurnEventBatch.cs` (if exists)

### Files to Modify (Major Changes)
- `ApogeeVGC/Sim/BattleClasses/Battle.Logging.cs` - Generate events instead of messages
- `ApogeeVGC/Sim/BattleClasses/Battle.Core.cs` - Field type changes
- `ApogeeVGC/Sim/Player/IPlayer.cs` - Interface change
- `ApogeeVGC/Sim/Player/PlayerGui.cs` - Implement UpdateEvents
- `ApogeeVGC/Gui/GuiChoiceCoordinator.cs` - Remove turn batches, add event queue
- `ApogeeVGC/Gui/BattleGame.cs` - Simplify update/draw loops
- `ApogeeVGC/Gui/Rendering/BattleRenderer.cs` - Remove state-based rendering

### Files to Modify (Minor Changes)
- `ApogeeVGC/Sim/Player/PlayerRandom.cs` - Update interface implementation
- `ApogeeVGC/Gui/Animations/AnimationCoordinator.cs` - May need minor adjustments

## Code Snippets for Reference

### Current Message Flow
```
Battle ? AddMessage(BattleMessage) 
      ? _pendingMessages.Add(message)
      ? FlushMessages()
      ? player.UpdateMessages(messages)
      ? PlayerGui.UpdateMessages()
      ? ChoiceCoordinator.AddEventToTurnBatch()
      ? BattleGame.Update() processes turn batch
      ? EventProcessor.ProcessEvent()
      ? GuiBattleState.ProcessMessage() (updates state)
      ? BattleRenderer.RenderBattleState()
```

### New Event Flow
```
Battle ? AddMessage(BattleMessage)
      ? GetPerspective(player)
      ? new BattleEvent { Message, Perspective }
      ? _pendingEvents.Add(event)
      ? FlushEvents()
      ? player.UpdateEvents(events)
      ? PlayerGui.UpdateEvents()
      ? ChoiceCoordinator.AddBattleEvent()
      ? BattleGame.Update() processes event
      ? ProcessBattleEvent() (stores perspective, queues animation)
      ? BattleRenderer.Render(perspective)
```

**Much simpler!** 6 steps removed, no state management needed.

---

## Summary

This overhaul replaces the complex GUI state management system with a simple "perspective-per-message" architecture. The battle engine remains the single source of truth, and the GUI becomes a simple renderer + animation player. All current bugs (team preview, switch-ins, state validation) are automatically fixed by eliminating the state synchronization problem entirely.
