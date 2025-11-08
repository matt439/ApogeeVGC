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
        _currentHighlightedIndex = 0;
        _lockedInPositions.Clear();
        _lockedInIndices.Clear();

        // No buttons needed for keyboard-only input
        // Pokemon display and highlighting will be handled in BattleRenderer
    }

    private void RenderTeamPreviewUi()
    {
        // Draw instruction text
        string instructionText =
            "Use LEFT/RIGHT arrows to navigate. Press ENTER to lock in. Lock in all 6 Pokemon.";
        var instructionPos = new Vector2(
            (_graphicsDevice.Viewport.Width - _font.MeasureString(instructionText).X) / 2f,
            _graphicsDevice.Viewport.Height / 2f);
        _spriteBatch.DrawString(_font, instructionText, instructionPos, Color.White);

        // Draw locked-in status
        if (_lockedInPositions.Count > 0)
        {
            string statusText = $"Locked in {_lockedInPositions.Count}/6 Pokemon";
            var statusPos = new Vector2(
                (_graphicsDevice.Viewport.Width - _font.MeasureString(statusText).X) / 2f,
                _graphicsDevice.Viewport.Height / 2f + 30);
            _spriteBatch.DrawString(_font, statusText, statusPos, Color.Lime);

            // Show the order
            string orderText =
                $"Order: {string.Join(" -> ", _lockedInPositions.Select(i => $"#{i + 1}"))}";
            var orderPos = new Vector2(
                (_graphicsDevice.Viewport.Width - _font.MeasureString(orderText).X) / 2f,
                _graphicsDevice.Viewport.Height / 2f + 60);
            _spriteBatch.DrawString(_font, orderText, orderPos, Color.Yellow);
        }
    }

    private void ProcessTeamPreviewKeyboardInput(KeyboardState keyboardState)
    {
        if (_currentRequest is not TeamPreviewRequest request) return;

        int teamSize = request.Side.Pokemon.Count;

        // Left arrow - move highlight left
        if (IsKeyPressed(keyboardState, Keys.Left))
        {
            Console.WriteLine(
                $"[ChoiceInputManager] Left arrow pressed, moving from {_currentHighlightedIndex}");
            _currentHighlightedIndex--;
            if (_currentHighlightedIndex < 0)
                _currentHighlightedIndex = teamSize - 1; // Wrap around
            Console.WriteLine(
                $"[ChoiceInputManager] New highlight index: {_currentHighlightedIndex}");
        }

        // Right arrow - move highlight right
        if (IsKeyPressed(keyboardState, Keys.Right))
        {
            Console.WriteLine(
                $"[ChoiceInputManager] Right arrow pressed, moving from {_currentHighlightedIndex}");
            _currentHighlightedIndex++;
            if (_currentHighlightedIndex >= teamSize)
                _currentHighlightedIndex = 0; // Wrap around
            Console.WriteLine(
                $"[ChoiceInputManager] New highlight index: {_currentHighlightedIndex}");
        }

        // Enter - lock in the currently highlighted Pokemon
        if (IsKeyPressed(keyboardState, Keys.Enter))
        {
            Console.WriteLine(
                $"[ChoiceInputManager] Enter pressed, attempting to lock in Pokemon {_currentHighlightedIndex}");
            // Check if this Pokemon is already locked in
            if (!_lockedInIndices.Contains(_currentHighlightedIndex))
            {
                _lockedInPositions.Add(_currentHighlightedIndex);
                _lockedInIndices.Add(_currentHighlightedIndex);
                Console.WriteLine(
                    $"[ChoiceInputManager] Locked in Pokemon {_currentHighlightedIndex}, total locked: {_lockedInPositions.Count}/{teamSize}");

                // If all Pokemon are locked in, submit the choice
                if (_lockedInPositions.Count == teamSize)
                {
                    Console.WriteLine(
                        $"[ChoiceInputManager] All Pokemon locked in, submitting choice");
                    SubmitTeamPreviewChoice();
                }
            }
            else
            {
                Console.WriteLine(
                    $"[ChoiceInputManager] Pokemon {_currentHighlightedIndex} already locked in");
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
        // Map the locked-in Pokemon indices to actual Pokemon from the request
        var actions = _lockedInPositions.Select((pokemonIndex, orderPosition) =>
        {
            Console.WriteLine(
                $"[SubmitTeamPreviewChoice] Creating action: pokemonIndex={pokemonIndex}, orderPosition={orderPosition}, Priority={-orderPosition}");
            return new ChosenAction
            {
                Choice = ChoiceType.Team,
                Pokemon = null, // Will be filled in by battle system
                MoveId = MoveId.None,
                Index = pokemonIndex, // This is the key field - which Pokemon position
                Priority = -orderPosition, // Earlier picks have higher priority
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
        if (_requestType == BattleRequestType.TeamPreview && _currentRequest is TeamPreviewRequest)
        {
            ProcessTeamPreviewKeyboardInput(keyboardState);
            return;
        }

        // For main battle turn start, use arrow key navigation
        if (_requestType == BattleRequestType.TurnStart && _currentRequest is MoveRequest)
        {
            ProcessMainBattleKeyboardInput(keyboardState);
            return;
        }

        // Legacy numeric input for other request types (force switch, etc.)
        // Determine max numbers based on request type
        int maxNumbers = _requestType switch
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
        if (_requestType == BattleRequestType.TeamPreview)
            return;

        // Disable mouse input for main battle (arrow key navigation only)
        if (_requestType == BattleRequestType.TurnStart && _currentRequest is MoveRequest)
            return;

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

    private void ProcessNumericInput(int number)
    {
        // Disable numeric input for team preview
        if (_requestType == BattleRequestType.TeamPreview)
            return;

        if (_currentRequest is MoveRequest moveRequest)
        {
            if (number <= moveRequest.Active[0].Moves.Count)
            {
                SelectMove(number - 1, moveRequest.Active[0]);
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
        PokemonPlayerPerspective? pokemon = _perspective.PlayerSide.Active[_selectedPokemonIndex];
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
        // TODO: Switch to switch UI if currently in move selection
    }

    private void ToggleTerastallize()
    {
        if (_pendingChoice != null)
        {
            _pendingChoice.Terastallize = !_pendingChoice.Terastallize;
        }
    }
}