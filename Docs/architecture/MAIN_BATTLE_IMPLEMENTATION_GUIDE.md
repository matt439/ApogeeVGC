# Main Battle Phase GUI Implementation - Scarlet/Violet Style

## Summary

This implementation adds the foundation for a Scarlet/Violet-style main battle phase GUI to the ApogeeVGC battle simulator. The system includes:

1. **State Management**: New enums and classes to track UI navigation state
2. **Timer System**: Three timers (Battle, Player Time, Move Time) displayed on screen
3. **UI Helper Classes**: Factory methods for creating buttons and UI elements
4. **Visual Rendering**: Timer display integrated into BattleRenderer

## What Was Implemented

### 1. MainBattlePhaseState.cs
Created an enum and state tracking class for the main battle UI:

```csharp
- MainBattlePhaseState enum: Tracks which menu the player is currently in
  - MainMenuFirstPokemon: Battle/Pokemon/Run options
  - MainMenuSecondPokemon: Battle/Pokemon/Back options
  - MoveSelectionFirstPokemon: Move selection for first Pokemon
  - MoveSelectionSecondPokemon: Move selection for second Pokemon
  - SwitchSelectionFirstPokemon/SecondPokemon: Pokemon switching
  - ForceSwitch: When a Pokemon faints

- TurnSelectionState class: Tracks player's selections during a turn
  - Stores move/switch choices for both Pokemon
  - Tracks Terastallize status
  - Handles forfeit state
```

### 2. TimerManager.cs
A timer management class that tracks three independent timers:

```csharp
- BattleTime: Total elapsed time in the battle
- PlayerTime: Total time the player has spent making decisions
- MoveTime: Time spent on the current decision

Methods:
- StartBattleTimer(): Start battle timer at battle start
- StartPlayerTimer(): Start when player begins making choice
- StartMoveTimer(): Reset and start when entering move selection
- Update(GameTime): Updates all active timers
- FormatTime(TimeSpan): Formats time as MM:SS.mmm
```

### 3. MainBattleUiHelper.cs
Factory methods for creating UI elements:

```csharp
- CreateMainMenuFirstPokemon(): Creates Battle/Pokemon/Run buttons
- CreateMainMenuSecondPokemon(): Creates Battle/Pokemon/Back buttons
- CreateMoveSelectionButtons(): Creates move selection UI with Tera option
- CreateSwitchSelectionButtons(): Creates Pokemon switching UI
- GetInstructionText(state): Returns instruction text for current state
```

### 4. ChoiceInputManager Updates
Added state fields and timer integration:

```csharp
- Added MainBattlePhaseState field
- Added TurnSelectionState field
- Added TimerManager field
- Added public properties for renderer access
- Integrated timer updates into Update() method
```

### 5. BattleRenderer Updates
Added timer display to the battle renderer:

```csharp
- RenderTimers(): Displays all three timers in top-right corner
  - Battle Time (white)
  - Your Time (yellow)
  - Move Time (lime)
- Integrated into both team preview and in-battle rendering
```

## How the System Works

### Flow for Single Pokemon

1. **Main Menu**: Player sees "Battle", "Pokemon", "Run" options
2. **Battle Selected**: Shows 4 moves with Terastallize option
3. **Move Selected**: Submits choice immediately

### Flow for Two Pokemon

1. **Main Menu Pokemon 1**: Player sees "Battle", "Pokemon", "Run"
2. **Make Selection**: Choose move or switch for Pokemon 1
3. **Main Menu Pokemon 2**: Player sees "Battle", "Pokemon", "Back"
4. **Make Selection**: Choose move or switch for Pokemon 2
5. **Submit**: Both choices submitted together

### Timer Behavior

- **Battle Timer**: Starts at battle beginning, runs continuously
- **Player Timer**: Starts when choice request begins, pauses when submitted
- **Move Timer**: Resets when entering move selection, stops when move selected

## What Needs to Be Completed

The core UI state machine logic in `ChoiceInputManager` needs to be fully implemented. Here's the approach:

### Step 1: Extend SetupChoiceUi()

```csharp
private void SetupChoiceUi()
{
 _buttons.Clear();
  _keyboardInput = "";
   _selectedPokemonIndex = 0;

    if (_currentRequest is MoveRequest moveRequest)
{
        if (_requestType == BattleRequestType.ForceSwitch)
     {
   _mainBattleState = MainBattlePhaseState.ForceSwitch;
      // Setup force switch UI
 }
        else
  {
       _mainBattleState = MainBattlePhaseState.MainMenuFirstPokemon;
     _turnSelection.Reset();
            _buttons = MainBattleUiHelper.CreateMainMenuFirstPokemon(
       moveRequest,
    () => ShowMoveSelectionFirstPokemon(moveRequest),
      () => ShowSwitchSelectionFirstPokemon(moveRequest),
     () => SelectForfeit());
            
   // Start timers
            _timerManager.StartPlayerTimer();
      _timerManager.StartMoveTimer();
        }
 }
    // ... rest of existing code
}
```

### Step 2: Add Navigation Methods

You'll need to implement methods like:

```csharp
private void ShowMoveSelectionFirstPokemon(MoveRequest request)
{
_mainBattleState = MainBattlePhaseState.MoveSelectionFirstPokemon;
    _turnSelection.CurrentPokemonIndex = 0;
   _buttons = MainBattleUiHelper.CreateMoveSelectionButtons(
        request.Active[0],
        request.Active[0].CanTerastallize != null,
  _turnSelection.FirstPokemonTerastallize,
       (moveIndex) => SelectMoveForCurrentPokemon(moveIndex, request),
       () => ToggleTerastallizeForCurrentPokemon(),
        () => GoBackFromMoveSelection(request));
    
    _timerManager.ResetMoveTimer();
 _timerManager.StartMoveTimer();
}

private void SelectMoveForCurrentPokemon(int moveIndex, MoveRequest request)
{
    _timerManager.StopMoveTimer();
    
    if (_turnSelection.CurrentPokemonIndex == 0)
 {
        _turnSelection.FirstPokemonMoveIndex = moveIndex;
        _turnSelection.FirstPokemonTarget = 0; // Default target
      
 if (request.Active.Count > 1)
        {
      // Show menu for second Pokemon
       _mainBattleState = MainBattlePhaseState.MainMenuSecondPokemon;
     _buttons = MainBattleUiHelper.CreateMainMenuSecondPokemon(...);
   }
        else
   {
    SubmitTurnChoice();
   }
    }
 else
  {
        _turnSelection.SecondPokemonMoveIndex = moveIndex;
        SubmitTurnChoice();
    }
}
```

### Step 3: Implement Choice Submission

```csharp
private void SubmitTurnChoice()
{
  if (_pendingChoice == null || _choiceCompletionSource == null)
   return;

    _timerManager.StopMoveTimer();
    _timerManager.PausePlayerTimer();

var actions = new List<ChosenAction>();

    // Handle forfeit
    if (_turnSelection.Forfeit)
    {
  actions.Add(new ChosenAction
   {
            Choice = ChoiceType.Pass,
  Pokemon = null,
      MoveId = MoveId.None,
        });
    }
    else
    {
        // Build actions for first Pokemon
  if (_turnSelection.FirstPokemonSwitchIndex.HasValue)
   {
  // Add switch action
        }
 else if (_turnSelection.FirstPokemonMoveIndex.HasValue)
     {
   // Add move action
     }

 // Build actions for second Pokemon (if applicable)
     // ...
    }

    _pendingChoice.Actions = actions;
    _pendingChoice.Terastallize = _turnSelection.FirstPokemonTerastallize || 
        _turnSelection.SecondPokemonTerastallize;

    SubmitChoice(); // Existing method
}
```

### Step 4: Update Keyboard Input

Extend `ProcessKeyboardInput()` to handle the new states:

```csharp
private void ProcessKeyboardInput(KeyboardState keyboardState)
{
    if (_requestType == BattleRequestType.TeamPreview)
 {
      ProcessTeamPreviewKeyboardInput(keyboardState);
     return;
   }

    if (_currentRequest is MoveRequest moveRequest && 
    _requestType == BattleRequestType.TurnStart)
    {
   ProcessMainBattleKeyboardInput(keyboardState, moveRequest);
        return;
    }
    
 // ... existing legacy code
}

private void ProcessMainBattleKeyboardInput(KeyboardState keyboardState, MoveRequest request)
{
    if (IsKeyPressed(keyboardState, Keys.D1))
        HandleMainBattleOption1(request);
else if (IsKeyPressed(keyboardState, Keys.D2))
     HandleMainBattleOption2(request);
 else if (IsKeyPressed(keyboardState, Keys.D3))
     HandleMainBattleOption3(request);
    else if (IsKeyPressed(keyboardState, Keys.D4))
    HandleMainBattleOption4(request);
 else if (IsKeyPressed(keyboardState, Keys.B))
        HandleBackButton(request);
    else if (IsKeyPressed(keyboardState, Keys.T))
 HandleTerastallizeToggle();
}
```

### Step 5: Update Render Method

Update the `Render()` method to show appropriate UI based on state:

```csharp
public void Render(GameTime gameTime)
{
    if (_currentRequest == null)
return;

  if (_requestType == BattleRequestType.TeamPreview)
    {
 RenderTeamPreviewUi();
return;
    }

 if (_requestType == BattleRequestType.TurnStart && _currentRequest is MoveRequest)
    {
        RenderMainBattleUi();
  return;
    }

    // Legacy rendering
    // ...
}

private void RenderMainBattleUi()
{
    // Draw instruction text
    string instructionText = MainBattleUiHelper.GetInstructionText(_mainBattleState);
spriteBatch.DrawString(font, instructionText, 
        new Vector2(LeftMargin, TopMargin - 60), Color.White);

// Draw buttons
    foreach (ChoiceButton button in _buttons)
        button.Draw(spriteBatch, font, graphicsDevice);

// Draw current selections status
    RenderSelectionStatus();
}
```

## Testing Approach

1. **Single Pokemon Battle**:
   - Verify Battle/Pokemon/Run menu appears
   - Test move selection and submission
 - Test Pokemon switching
   - Verify timers update correctly

2. **Double Pokemon Battle**:
   - Test first Pokemon selection
   - Verify transition to second Pokemon menu
   - Test Back button functionality
 - Verify both choices submitted together

3. **Force Switch**:
   - Faint a Pokemon in battle
   - Verify force switch UI appears
   - Test Pokemon selection

4. **Timers**:
   - Verify Battle Timer runs continuously
  - Verify Player Timer pauses between turns
   - Verify Move Timer resets for each decision

## Key Design Decisions

1. **State Machine**: Using enum-based state machine for clear UI navigation
2. **Immutable Selections**: TurnSelectionState accumulates choices before submission
3. **Timer Independence**: Three separate timers for different time tracking needs
4. **Helper Classes**: Separating UI creation logic for maintainability
5. **Thread Safety**: All state accessed through properties for renderer

## Integration with Existing Code

The implementation integrates with:

- **BattleGame**: No changes needed, already routes requests properly
- **PlayerGui**: No changes needed, uses existing RequestChoiceAsync
- **BattleRenderer**: Updated to display timers
- **ChoiceInputManager**: Extended with new state management

## Future Enhancements

1. **Target Selection**: Currently defaults to target 0, could add UI for selecting targets
2. **Animations**: Add smooth transitions between UI states
3. **Sound Effects**: Add audio feedback for button presses
4. **Visual Polish**: Better button styling, hover effects
5. **Touch Support**: Add touch/mouse input handling
6. **Undo**: Allow undoing selections before final submission

## Files Created/Modified

### Created:
- `ApogeeVGC/Gui/ChoiceUI/MainBattlePhaseState.cs`
- `ApogeeVGC/Gui/ChoiceUI/TimerManager.cs`
- `ApogeeVGC/Gui/ChoiceUI/MainBattleUiHelper.cs`

### Modified:
- `ApogeeVGC/Gui/ChoiceUI/ChoiceInputManager.cs` (state fields and timer integration)
- `ApogeeVGC/Gui/Rendering/BattleRenderer.cs` (timer display)

## Next Steps

To complete the implementation:

1. Implement all navigation methods in ChoiceInputManager
2. Add keyboard/mouse input handlers for new states
3. Implement SubmitTurnChoice() with proper action building
4. Update Render() method to use new UI states
5. Test with single and double battles
6. Add target selection UI (optional)
7. Polish visuals and add animations (optional)

The foundation is in place - the state management, timers, and helper classes are ready. The remaining work is connecting them together in ChoiceInputManager's state machine logic.
