using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Team preview and legacy input functionality for ChoiceInputManager.
/// </summary>
public partial class ChoiceInputManager
{
    private void SetupTeamPreviewUi()
    {
        // Reset team preview state
        CurrentHighlightedIndex = 0;
        _lockedInPositions.Clear();
        _lockedInIndices.Clear();

        // No buttons needed for keyboard-only input
        // Pokemon display and highlighting will be handled in BattleRenderer
    }

    private void RenderTeamPreviewUi()
    {
        // Get the required team size from the request
        int requiredTeamSize = 6; // Default
        if (_currentRequest is TeamPreviewRequest tpr)
        {
            requiredTeamSize = tpr.MaxChosenTeamSize ?? tpr.Side.Pokemon.Count;
        }
        
        // Draw instruction text
        string instructionText =
            $"Use LEFT/RIGHT arrows to navigate. Press ENTER to lock in. Press A to auto-lock first {AutoLockPokemonCount}. Lock in {requiredTeamSize} Pokemon.";
        var instructionPos = new Vector2(
            (graphicsDevice.Viewport.Width - font.MeasureString(instructionText).X) / CenteringDivisor,
            graphicsDevice.Viewport.Height / CenteringDivisor);
        spriteBatch.DrawString(font, instructionText, instructionPos, Color.White);

        // Draw locked-in status
        if (_lockedInPositions.Count > 0)
        {
            string statusText = $"Locked in {_lockedInPositions.Count}/{requiredTeamSize} Pokemon";
            var statusPos = new Vector2(
                (graphicsDevice.Viewport.Width - font.MeasureString(statusText).X) / CenteringDivisor,
                graphicsDevice.Viewport.Height / CenteringDivisor + TeamPreviewStatusYOffset);
            spriteBatch.DrawString(font, statusText, statusPos, Color.Lime);

            // Show the order
            string orderText =
                $"Order: {string.Join(" -> ", _lockedInPositions.Select(i => $"#{i + 1}"))}";
            var orderPos = new Vector2(
                (graphicsDevice.Viewport.Width - font.MeasureString(orderText).X) / CenteringDivisor,
                graphicsDevice.Viewport.Height / CenteringDivisor + TeamPreviewOrderYOffset);
            spriteBatch.DrawString(font, orderText, orderPos, Color.Yellow);
        }
    }

    private void ProcessTeamPreviewKeyboardInput(KeyboardState keyboardState)
    {
        if (_currentRequest is not TeamPreviewRequest request) return;

        int teamSize = request.Side.Pokemon.Count;
        int requiredTeamSize = request.MaxChosenTeamSize ?? teamSize;

        // Left arrow - move highlight left
        if (IsKeyPressed(keyboardState, Keys.Left))
        {
            Console.WriteLine(
                $"[ChoiceInputManager] Left arrow pressed, moving from {CurrentHighlightedIndex}");
            CurrentHighlightedIndex--;
            if (CurrentHighlightedIndex < 0)
                CurrentHighlightedIndex = teamSize - 1; // Wrap around
            Console.WriteLine(
                $"[ChoiceInputManager] New highlight index: {CurrentHighlightedIndex}");
        }

        // Right arrow - move highlight right
        if (IsKeyPressed(keyboardState, Keys.Right))
        {
            Console.WriteLine(
                $"[ChoiceInputManager] Right arrow pressed, moving from {CurrentHighlightedIndex}");
            CurrentHighlightedIndex++;
            if (CurrentHighlightedIndex >= teamSize)
                CurrentHighlightedIndex = 0; // Wrap around
            Console.WriteLine(
                $"[ChoiceInputManager] New highlight index: {CurrentHighlightedIndex}");
        }

        // A key - automatically lock in first 4 Pokemon
        if (IsKeyPressed(keyboardState, Keys.A))
        {
            Console.WriteLine("[ChoiceInputManager] 'A' key pressed, auto-locking first 4 Pokemon");
            
            // Clear any existing locked selections
            _lockedInPositions.Clear();
            _lockedInIndices.Clear();
            
            // Lock in the first N Pokemon (or fewer if team has less than N)
            int pokemonToLock = Math.Min(AutoLockPokemonCount, teamSize);
            for (int i = 0; i < pokemonToLock; i++)
            {
                _lockedInPositions.Add(i);
                _lockedInIndices.Add(i);
                Console.WriteLine($"[ChoiceInputManager] Auto-locked Pokemon {i}");
            }
            
            Console.WriteLine(
                $"[ChoiceInputManager] Auto-locked {pokemonToLock} Pokemon, total locked: {_lockedInPositions.Count}/{requiredTeamSize}");
            
            // If we've locked in the required number, submit immediately
            if (_lockedInPositions.Count >= requiredTeamSize)
            {
                Console.WriteLine(
                    $"[ChoiceInputManager] Required {requiredTeamSize} Pokemon locked in, submitting choice");
                SubmitTeamPreviewChoice();
            }
        }

        // Enter - lock in the currently highlighted Pokemon
        if (IsKeyPressed(keyboardState, Keys.Enter))
        {
            Console.WriteLine(
                $"[ChoiceInputManager] Enter pressed, attempting to lock in Pokemon {CurrentHighlightedIndex}");
            // Check if this Pokemon is already locked in
            if (!_lockedInIndices.Contains(CurrentHighlightedIndex))
            {
                _lockedInPositions.Add(CurrentHighlightedIndex);
                _lockedInIndices.Add(CurrentHighlightedIndex);
                Console.WriteLine(
                    $"[ChoiceInputManager] Locked in Pokemon {CurrentHighlightedIndex}, total locked: {_lockedInPositions.Count}/{requiredTeamSize}");

                // If required number of Pokemon are locked in, submit the choice
                if (_lockedInPositions.Count == requiredTeamSize)
                {
                    Console.WriteLine(
                        $"[ChoiceInputManager] Required {requiredTeamSize} Pokemon locked in, submitting choice");
                    SubmitTeamPreviewChoice();
                }
            }
            else
            {
                Console.WriteLine(
                    $"[ChoiceInputManager] Pokemon {CurrentHighlightedIndex} already locked in");
            }
        }
    }

    private void SubmitTeamPreviewChoice()
    {
        if (_pendingChoice == null || _choiceCompletionSource == null)
            return;

        if (_currentRequest is not TeamPreviewRequest)
            return;

        Console.WriteLine(
            $"[SubmitTeamPreviewChoice] Building team preview choice from {_lockedInPositions.Count} locked positions");

        // Build the team preview choice with the locked-in order
        // _lockedInPositions contains the Pokemon indices in the order they were locked in
        // For each position in the new team order:
        //   - Index = newPosition (where this Pokemon will be placed in the team: 0, 1, 2, ...)
        //   - TargetLoc = originalPokemonIndex (which Pokemon from the original team to use)
        //   - Priority = -newPosition (earlier positions have higher priority for sorting)
        var actions = _lockedInPositions.Select((originalPokemonIndex, newPosition) =>
        {
            Console.WriteLine(
                $"[SubmitTeamPreviewChoice] Creating action: originalPokemonIndex={originalPokemonIndex}, newPosition={newPosition}, Priority={-newPosition}");
            return new ChosenAction
            {
                Choice = ChoiceType.Team,
                Pokemon = null, // Will be filled in by battle system using TargetLoc
                MoveId = MoveId.None,
                Index = newPosition, // The new position in the team (0, 1, 2, ...)
                TargetLoc = originalPokemonIndex, // Which Pokemon from the original team to use
                Priority = -newPosition, // Earlier picks have higher priority (0, -1, -2, ...)
            };
        }).ToList();

        Console.WriteLine($"[SubmitTeamPreviewChoice] Created {actions.Count} actions");
        _pendingChoice.Actions = actions;
        Console.WriteLine(
            $"[SubmitTeamPreviewChoice] Set _pendingChoice.Actions to {_pendingChoice.Actions.Count} actions");

        // Submit the choice
        SubmitChoice();
    }

    private void ProcessKeyboardInput(KeyboardState keyboardState)
    {
        // Handle team preview keyboard controls separately
        if (CurrentRequestType == BattleRequestType.TeamPreview && _currentRequest is TeamPreviewRequest)
        {
            ProcessTeamPreviewKeyboardInput(keyboardState);
            return;
        }

        // For main battle turn start, use arrow key navigation
        if (CurrentRequestType == BattleRequestType.TurnStart && _currentRequest is MoveRequest)
        {
            ProcessMainBattleKeyboardInput(keyboardState);
            return;
        }

        // Legacy numeric input for other request types (force switch, etc.)
        // Determine max numbers based on request type
        int maxNumbers = CurrentRequestType switch
        {
            BattleRequestType.TeamPreview => 6, // Team preview allows 1-6
            _ => 6, // Default to 6 for safety
        };

        // Number keys 1-6 (or 1-4 for moves) for quick selection
        for (int i = 0; i < maxNumbers; i++)
        {
            Keys numberKey = Keys.D1 + i;
            if (IsKeyPressed(keyboardState, numberKey))
            {
                _keyboardInput += (i + 1).ToString();
                ProcessNumericInput(i + 1);
                break;
            }
        }

        // Enter to submit
        if (IsKeyPressed(keyboardState, Keys.Enter))
        {
            SubmitChoice();
        }

        // Backspace to clear input
        if (IsKeyPressed(keyboardState, Keys.Back))
        {
            if (_keyboardInput.Length > 0)
            {
                _keyboardInput = _keyboardInput[..^1];
            }
        }

        // S for switch
        if (IsKeyPressed(keyboardState, Keys.S))
        {
            ShowSwitchOptions();
        }

        // T for Terastallize
        if (IsKeyPressed(keyboardState, Keys.T))
        {
            ToggleTerastallize();
        }
    }

    private void ProcessMouseInput(MouseState mouseState)
    {
        // Disable mouse input for team preview
        if (CurrentRequestType == BattleRequestType.TeamPreview)
            return;

        // Handle mouse input for target selection (clicking Pokemon boxes)
        if (CurrentRequestType == BattleRequestType.TurnStart && _currentRequest is MoveRequest)
        {
            bool isTargetSelection = MainBattleState == MainBattlePhaseState.TargetSelectionFirstPokemon ||
                                     MainBattleState == MainBattlePhaseState.TargetSelectionSecondPokemon;

            if (isTargetSelection && IsMouseClicked(mouseState))
            {
                ProcessTargetSelectionMouseClick(mouseState);
            }
            return; // No other mouse input during main battle
        }

        // Check button clicks (for legacy UI only)
        if (IsMouseClicked(mouseState))
        {
            var mousePos = new Point(mouseState.X, mouseState.Y);
            foreach (ChoiceButton button in _buttons)
            {
                if (button.Bounds.Contains(mousePos))
                {
                    button.OnClick();
                    break;
                }
            }
        }
    }

    private void ProcessTargetSelectionMouseClick(MouseState mouseState)
    {
        if (_battleRenderer == null) return;

        var mousePos = new Point(mouseState.X, mouseState.Y);

        // Check each valid target to see if it was clicked
        foreach (int targetLoc in ValidTargets)
        {
            Rectangle? box;

            if (targetLoc > 0)
            {
                // Opponent target (positive location)
                box = _battleRenderer.GetOpponentPokemonBox(targetLoc - 1);
            }
            else
            {
                // Ally target (negative location)
                box = _battleRenderer.GetPlayerPokemonBox((-targetLoc) - 1);
            }

            if (box.HasValue && box.Value.Contains(mousePos))
            {
                // Target was clicked - handle selection
                int pokemonIndex = MainBattleState == MainBattlePhaseState.TargetSelectionFirstPokemon ? 0 : 1;
                HandleTargetSelection(pokemonIndex, targetLoc);
                break;
            }
        }
    }

    private void ProcessNumericInput(int number)
    {
        // Disable numeric input for team preview
        if (CurrentRequestType == BattleRequestType.TeamPreview)
            return;

        if (_currentRequest is MoveRequest moveRequest)
        {
            // Check if first active Pokemon has valid request data (not fainted)
            if (moveRequest.Active[0] is { } firstPokemon && number <= firstPokemon.Moves.Count)
            {
                SelectMove(number - 1, firstPokemon);
            }
        }
        else if (_currentRequest is SwitchRequest or TeamPreviewRequest)
        {
            // Handle numeric switch/team selection
            if (_currentRequest is SwitchRequest sr && number <= sr.Side.Pokemon.Count)
            {
                SelectSwitch(number - 1, sr);
            }
            else if (_currentRequest is TeamPreviewRequest tpr && number <= tpr.Side.Pokemon.Count)
            {
                SelectTeamPosition(number - 1);
            }
        }
    }

    private void SetupSwitchRequestUi(SwitchRequest request)
    {
        int x = LeftMargin;
        int y = TopMargin;

        // Show available Pokemon to switch to
        // Filter out active Pokemon (those with Active = true)
        var availablePokemon = request.Side.Pokemon
            .Where(p => !p.Active)
            .ToList();

        int pokemonIndex = 1;
        foreach (PokemonSwitchRequestData _ in availablePokemon)
        {
            // Get Pokemon name - we'll need to derive it from available data
            // Since Details is commented out, we can use the Pokemon's actual data
            // For now, use a simple indexing approach
            int index = pokemonIndex;
            var button = new ChoiceButton(
                new Rectangle(x, y, ButtonWidth, ButtonHeight),
                $"{pokemonIndex}. Pokemon {pokemonIndex}",
                Color.Green,
                () => SelectSwitch(index - 1, request)
            );

            _buttons.Add(button);
            y += ButtonHeight + ButtonSpacing;
            pokemonIndex++;
        }

        // Add submit button
        y += ButtonSpacing;
        var submitButton = new ChoiceButton(
            new Rectangle(x, y, ButtonWidth, ButtonHeight),
            "Enter. Submit",
            Color.Orange,
            SubmitChoice
        );
        _buttons.Add(submitButton);
    }

    private void SelectMove(int moveIndex, PokemonMoveRequestData pokemonRequest)
    {
        if (_perspective == null || _pendingChoice == null) return;

        // Get the Pokemon
        PokemonPerspective? pokemon = _perspective.PlayerSide.Active[_selectedPokemonIndex];
        if (pokemon == null) return;

        PokemonMoveData moveData = pokemonRequest.Moves[moveIndex];

        // Create the chosen action
        var action = new ChosenAction
        {
            Choice = ChoiceType.Move,
            Pokemon = null, // Will be set by battle
            MoveId = moveData.Id,
            TargetLoc = 0, // Can be enhanced to allow target selection
        };

        // Add to pending choice
        var actions = _pendingChoice.Actions.ToList();
        actions.Add(action);
        _pendingChoice.Actions = actions;

        _keyboardInput = "";
    }

    private void SelectSwitch(int pokemonIndex, SwitchRequest request)
    {
        if (_pendingChoice == null) return;

        PokemonSwitchRequestData? pokemon = request.Side.Pokemon.Where((p, _) => !p.Active)
            .ElementAtOrDefault(pokemonIndex);
        if (pokemon == null) return;

        // Create switch action
        var action = new ChosenAction
        {
            Choice = ChoiceType.Switch,
            Pokemon = null, // Will be set by battle
            MoveId = MoveId.None,
            Index = pokemonIndex,
        };

        var actions = _pendingChoice.Actions.ToList();
        actions.Add(action);
        _pendingChoice.Actions = actions;

        _keyboardInput = "";
    }

    private void SelectTeamPosition(int position)
    {
        if (_pendingChoice == null) return;

        // For team preview, record the order
        var action = new ChosenAction
        {
            Choice = ChoiceType.Team,
            Pokemon = null,
            MoveId = MoveId.None, // Required field
            Index = position,
            Priority = -_pendingChoice.Actions.Count, // Earlier picks have higher priority
        };

        var actions = _pendingChoice.Actions.ToList();
        actions.Add(action);
        _pendingChoice.Actions = actions;

        _keyboardInput = "";
    }

    private void ShowSwitchOptions()
    {
        // Only transition if we're in the main battle turn start phase
        if (CurrentRequestType != BattleRequestType.TurnStart || _currentRequest is not MoveRequest)
            return;

        // Transition to switch selection based on current state
        switch (MainBattleState)
        {
            case MainBattlePhaseState.MoveSelectionFirstPokemon:
                TransitionToState(MainBattlePhaseState.SwitchSelectionFirstPokemon);
                break;
            case MainBattlePhaseState.MoveSelectionSecondPokemon:
                TransitionToState(MainBattlePhaseState.SwitchSelectionSecondPokemon);
                break;
            case MainBattlePhaseState.MainMenuFirstPokemon:
                // Already in main menu, just go to switch selection
                TransitionToState(MainBattlePhaseState.SwitchSelectionFirstPokemon);
                break;
            case MainBattlePhaseState.MainMenuSecondPokemon:
                // Already in main menu, just go to switch selection
                TransitionToState(MainBattlePhaseState.SwitchSelectionSecondPokemon);
                break;
        }
    }

    private void ToggleTerastallize()
    {
        if (_pendingChoice != null)
        {
            _pendingChoice.Terastallize = !_pendingChoice.Terastallize;
        }
    }
}