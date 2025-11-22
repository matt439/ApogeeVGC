using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Main battle UI functionality for ChoiceInputManager - state machine, move/switch selection.
/// </summary>
public partial class ChoiceInputManager
{
    private void SetupMoveRequestUi(MoveRequest request)
    {
        Console.WriteLine(
            $"[ChoiceInputManager.SetupMoveRequestUi] Called with CurrentRequestType={CurrentRequestType}");

        // For TurnStart requests, use the main battle state machine
        if (CurrentRequestType == BattleRequestType.TurnStart)
        {
            Console.WriteLine(
                "[ChoiceInputManager.SetupMoveRequestUi] Routing to SetupMainBattleUi");
            SetupMainBattleUi(request);
            Console.WriteLine(
                $"[ChoiceInputManager.SetupMoveRequestUi] SetupMainBattleUi complete. Button count: {_buttons.Count}");
            return;
        }

        Console.WriteLine("[ChoiceInputManager.SetupMoveRequestUi] Using legacy UI setup");

        // Legacy UI for other request types (force switch, etc.)
        int y = TopMargin;

        // Get the first active Pokemon's moves
        if (request.Active.Count > 0)
        {
            PokemonMoveRequestData pokemonRequest = request.Active[0];
            int moveIndex = 1;

            foreach (PokemonMoveData moveData in pokemonRequest.Moves)
            {
                // Check if move is disabled - handle the union type
                bool disabled = moveData.Disabled switch
                {
                    BoolMoveIdBoolUnion boolUnion => boolUnion.Value,
                    MoveIdMoveIdBoolUnion => false, // If it's a MoveId, it's not disabled
                    null => false,
                    _ => false,
                };

                // Skip disabled moves - don't show them at all
                if (disabled)
                {
                    moveIndex++;
                    continue;
                }

                int index = moveIndex;
                var button = new ChoiceButton(
                    new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
                    $"{moveIndex}. {moveData.Move.Name}",
                    Color.Blue,
                    () => SelectMove(index - 1, pokemonRequest)
                );

                _buttons.Add(button);

                y += ButtonHeight + ButtonSpacing;
                moveIndex++;
            }

            // Add switch button
            y += ButtonSpacing;
            var switchButton = new ChoiceButton(
                new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
                "S. Switch Pokemon",
                Color.Green,
                ShowSwitchOptions
            );
            _buttons.Add(switchButton);

            // Add Terastallize option if available
            if (pokemonRequest.CanTerastallize != null)
            {
                y += ButtonHeight + ButtonSpacing;
                var teraButton = new ChoiceButton(
                    new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
                    "T. Terastallize",
                    Color.Purple,
                    ToggleTerastallize
                );
                _buttons.Add(teraButton);
            }

            // Add submit button
            y += ButtonHeight + ButtonSpacing * 2;
            var submitButton = new ChoiceButton(
                new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
                "Enter. Submit",
                Color.Orange,
                SubmitChoice
            );
            _buttons.Add(submitButton);
        }
    }

    private void SetupMainBattleUi(MoveRequest request)
    {
        Console.WriteLine(
            $"[ChoiceInputManager.SetupMainBattleUi] Called with MainBattleState={MainBattleState}");

        _buttons.Clear();
        _selectedButtonIndex = 0; // Reset selection to first button

        Console.WriteLine(
            "[ChoiceInputManager.SetupMainBattleUi] Cleared buttons and reset selection");

        // Check if there are any valid switch options (non-active, non-fainted Pokemon)
        bool hasValidSwitchOptions = request.Side.Pokemon
            .Select((p, index) => new { Pokemon = p, Index = index })
            .Any(x => !x.Pokemon.Active && !IsFainted(x.Index));

        // Set up buttons based on current state
        switch (MainBattleState)
        {
            case MainBattlePhaseState.MainMenuFirstPokemon:
                Console.WriteLine(
                    "[ChoiceInputManager.SetupMainBattleUi] Creating MainMenuFirstPokemon buttons");
                _buttons = MainBattleUiHelper.CreateMainMenuFirstPokemon(
                    request,
                    () => TransitionToState(MainBattlePhaseState.MoveSelectionFirstPokemon),
                    hasValidSwitchOptions
                        ? () => TransitionToState(MainBattlePhaseState.SwitchSelectionFirstPokemon)
                        : null,
                    () => HandleForfeit()
                );
                Console.WriteLine(
                    $"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for MainMenuFirstPokemon");
                break;

            case MainBattlePhaseState.MoveSelectionFirstPokemon:
                Console.WriteLine(
                    "[ChoiceInputManager.SetupMainBattleUi] Creating MoveSelectionFirstPokemon buttons");
                if (request.Active.Count > 0)
                {
                    PokemonMoveRequestData pokemonData = request.Active[0];
                    bool canTera = pokemonData.CanTerastallize != null;
                    _buttons = MainBattleUiHelper.CreateMoveSelectionButtons(
                        pokemonData,
                        canTera,
                        TurnSelection.FirstPokemonTerastallize,
                        (moveIndex, useTera) => HandleMoveSelection(0, moveIndex, useTera),
                        () => ToggleTerastallize(0),
                        () => TransitionToState(MainBattlePhaseState.MainMenuFirstPokemon)
                    );
                    Console.WriteLine(
                        $"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for MoveSelectionFirstPokemon");
                }

                break;

            case MainBattlePhaseState.SwitchSelectionFirstPokemon:
            {
                Console.WriteLine(
                    "[ChoiceInputManager.SetupMainBattleUi] Creating SwitchSelectionFirstPokemon buttons");
                var availablePokemon = request.Side.Pokemon
                    .Select((p, index) => (Pokemon: p, OriginalIndex: index))
                    .Where(x => !x.Pokemon.Active && !IsFainted(x.OriginalIndex))
                    .ToList();
                _buttons = MainBattleUiHelper.CreateSwitchSelectionButtons(
                    availablePokemon,
                    (switchIndex) => HandleSwitchSelection(0, switchIndex),
                    () => TransitionToState(MainBattlePhaseState.MainMenuFirstPokemon),
                    _perspective,
                    showBackButton: true
                );
                Console.WriteLine(
                    $"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for SwitchSelectionFirstPokemon");
            }
                break;

            case MainBattlePhaseState.MainMenuSecondPokemon:
                Console.WriteLine(
                    "[ChoiceInputManager.SetupMainBattleUi] Creating MainMenuSecondPokemon buttons");
                _buttons = MainBattleUiHelper.CreateMainMenuSecondPokemon(
                    request,
                    () => TransitionToState(MainBattlePhaseState.MoveSelectionSecondPokemon),
                    () => TransitionToState(MainBattlePhaseState.SwitchSelectionSecondPokemon),
                    HandleBackFromSecondPokemon
                );
                Console.WriteLine(
                    $"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for MainMenuSecondPokemon");
                break;

            case MainBattlePhaseState.MoveSelectionSecondPokemon:
                Console.WriteLine(
                    "[ChoiceInputManager.SetupMainBattleUi] Creating MoveSelectionSecondPokemon buttons");
                if (request.Active.Count > 1)
                {
                    PokemonMoveRequestData pokemonData = request.Active[1];
                    // Only allow tera if first Pokemon hasn't already selected it
                    bool canTera = pokemonData.CanTerastallize != null &&
                                   !TurnSelection.FirstPokemonTerastallize;
                    _buttons = MainBattleUiHelper.CreateMoveSelectionButtons(
                        pokemonData,
                        canTera,
                        TurnSelection.SecondPokemonTerastallize,
                        (moveIndex, useTera) => HandleMoveSelection(1, moveIndex, useTera),
                        () => ToggleTerastallize(1),
                        () => TransitionToState(MainBattlePhaseState.MainMenuSecondPokemon)
                    );
                    Console.WriteLine(
                        $"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for MoveSelectionSecondPokemon");
                }

                break;

            case MainBattlePhaseState.SwitchSelectionSecondPokemon:
            {
                Console.WriteLine(
                    "[ChoiceInputManager.SetupMainBattleUi] Creating SwitchSelectionSecondPokemon buttons");
                var availablePokemon = request.Side.Pokemon
                    .Select((p, index) => (Pokemon: p, OriginalIndex: index))
                    .Where(x => !x.Pokemon.Active && !IsFainted(x.OriginalIndex))
                    .ToList();
                _buttons = MainBattleUiHelper.CreateSwitchSelectionButtons(
                    availablePokemon,
                    (switchIndex) => HandleSwitchSelection(1, switchIndex),
                    () => TransitionToState(MainBattlePhaseState.MainMenuSecondPokemon),
                    _perspective,
                    showBackButton: true
                );
                Console.WriteLine(
                    $"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for SwitchSelectionSecondPokemon");
            }
                break;

            case MainBattlePhaseState.ForceSwitch:
            {
                Console.WriteLine(
                    "[ChoiceInputManager.SetupMainBattleUi] Creating ForceSwitch buttons");
                var availablePokemon = request.Side.Pokemon
                    .Select((p, index) => (Pokemon: p, OriginalIndex: index))
                    .Where(x => !x.Pokemon.Active && !IsFainted(x.OriginalIndex))
                    .ToList();
                _buttons = MainBattleUiHelper.CreateSwitchSelectionButtons(
                    availablePokemon,
                    HandleForceSwitchSelection,
                    () => { }, // No back button for force switch
                    _perspective,
                    showBackButton: false
                );
                Console.WriteLine(
                    $"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for ForceSwitch");
            }
                break;
        }

        Console.WriteLine(
            $"[ChoiceInputManager.SetupMainBattleUi] Setup complete. Final button count: {_buttons.Count}");
    }

    /// <summary>
    /// Helper method to check if a Pokemon is fainted based on its position in the perspective
    /// </summary>
    private bool IsFainted(int pokemonIndex)
    {
        if (_perspective == null)
            return false;

        // Find matching Pokemon in perspective by position
        var perspectivePokemon = _perspective.PlayerSide.Pokemon
            .FirstOrDefault(pp => pp.Position == pokemonIndex);

        return perspectivePokemon?.Fainted ?? false;
    }

    private void RenderMainBattleUi()
    {
        // Get Pokemon names from perspective for instruction text
        string? firstPokemonName = null;
        string? secondPokemonName = null;
        if (_perspective != null)
        {
            if (_perspective.PlayerSide.Active.Count > 0 &&
                _perspective.PlayerSide.Active[0] != null)
            {
                firstPokemonName = _perspective.PlayerSide.Active[0].Name;
            }

            if (_perspective.PlayerSide.Active.Count > 1 &&
                _perspective.PlayerSide.Active[1] != null)
            {
                secondPokemonName = _perspective.PlayerSide.Active[1].Name;
            }
        }

        // Draw instruction text based on current state
        string instructionText =
            MainBattleUiHelper.GetInstructionText(MainBattleState, firstPokemonName,
                secondPokemonName);
        // Position instruction text above the main battle UI buttons
        var instructionPosition = MainBattleUiHelper.GetInstructionTextPosition();
        spriteBatch.DrawString(font, instructionText, instructionPosition, Color.White);

        // Draw buttons with selection highlight
        for (int i = 0; i < _buttons.Count; i++)
        {
            bool isSelected = (i == _selectedButtonIndex);
            _buttons[i].Draw(spriteBatch, font, graphicsDevice, isSelected);
        }

        // Draw current selections status
        RenderSelectionStatus();
    }

    private void RenderSelectionStatus()
    {
        var statusLines = new List<string>();

        // First Pokemon selection
        if (TurnSelection.FirstPokemonMoveIndex.HasValue)
        {
            statusLines.Add($"P1: Move {TurnSelection.FirstPokemonMoveIndex.Value + 1}" +
                            (TurnSelection.FirstPokemonTerastallize ? " (Tera)" : ""));
        }
        else if (TurnSelection.FirstPokemonSwitchIndex.HasValue)
        {
            statusLines.Add($"P1: Switch to #{TurnSelection.FirstPokemonSwitchIndex.Value + 1}");
        }

        // Second Pokemon selection (if applicable)
        if (TurnSelection.SecondPokemonMoveIndex.HasValue)
        {
            statusLines.Add($"P2: Move {TurnSelection.SecondPokemonMoveIndex.Value + 1}" +
                            (TurnSelection.SecondPokemonTerastallize ? " (Tera)" : ""));
        }
        else if (TurnSelection.SecondPokemonSwitchIndex.HasValue)
        {
            statusLines.Add($"P2: Switch to #{TurnSelection.SecondPokemonSwitchIndex.Value + 1}");
        }

        // Draw status text aligned with buttons
        if (statusLines.Count > 0)
        {
            string statusText = string.Join(" | ", statusLines);
            var statusPosition = MainBattleUiHelper.GetSelectionStatusPosition();
            spriteBatch.DrawString(font, statusText, statusPosition, Color.Lime);
        }
    }

    private void ProcessMainBattleKeyboardInput(KeyboardState keyboardState)
    {
        if (_buttons.Count == 0) return;

        // Check if we're in move selection state (uses grid navigation)
        bool isMoveSelection = MainBattleState == MainBattlePhaseState.MoveSelectionFirstPokemon ||
                               MainBattleState == MainBattlePhaseState.MoveSelectionSecondPokemon;

        if (isMoveSelection)
        {
            // Grid-based navigation for move selection
            ProcessMoveSelectionGridNavigation(keyboardState);
        }
        else
        {
            // Linear navigation for other states
            ProcessLinearNavigation(keyboardState);
        }

        // Enter to select the highlighted button
        if (IsKeyPressed(keyboardState, Keys.Enter))
        {
            Console.WriteLine(
                $"[ProcessMainBattleKeyboardInput] ENTER key pressed, selecting button {_selectedButtonIndex}");
            if (_selectedButtonIndex >= 0 && _selectedButtonIndex < _buttons.Count)
            {
                _buttons[_selectedButtonIndex].OnClick();
            }
        }

        // ESC key for back
        if (IsKeyPressed(keyboardState, Keys.Escape))
        {
            Console.WriteLine($"[ProcessMainBattleKeyboardInput] ESC key pressed, navigating back");
            HandleEscapeKeyNavigation();
        }
    }

    private void ProcessLinearNavigation(KeyboardState keyboardState)
    {
        // Arrow key navigation (up/down only)
        if (IsKeyPressed(keyboardState, Keys.Up))
        {
            Console.WriteLine($"[ProcessLinearNavigation] UP key pressed");
            _selectedButtonIndex--;
            if (_selectedButtonIndex < 0)
                _selectedButtonIndex = _buttons.Count - 1; // Wrap to bottom
        }
        else if (IsKeyPressed(keyboardState, Keys.Down))
        {
            Console.WriteLine($"[ProcessLinearNavigation] DOWN key pressed");
            _selectedButtonIndex++;
            if (_selectedButtonIndex >= _buttons.Count)
                _selectedButtonIndex = 0; // Wrap to top
        }
    }

    private void ProcessMoveSelectionGridNavigation(KeyboardState keyboardState)
    {
        // Determine how many move buttons we have (exclude Tera button if present)
        int moveButtonCount = _buttons.Count;
        bool hasTeraButton = false;

        // Check if last button is a Tera button (it will have "TERA" in its text)
        if (_buttons.Count > 0)
        {
            // Tera button is always added last in CreateMoveSelectionButtons
            // It will have wider width than move buttons
            var lastButton = _buttons[_buttons.Count - 1];
            // We can identify Tera button by checking if button count is 5 (4 moves + tera)
            // or 4 (3 moves + tera), etc.
            // For simplicity, check button bounds - tera spans full width
            hasTeraButton = _buttons.Count > 4 ||
                            (_buttons.Count > 0 && lastButton.Bounds.Width > 200);
        }

        if (hasTeraButton)
        {
            moveButtonCount = _buttons.Count - 1;
        }

        // Determine current position in grid
        bool isOnTeraButton = hasTeraButton && _selectedButtonIndex == _buttons.Count - 1;
        int currentMoveIndex = isOnTeraButton ? -1 : _selectedButtonIndex;

        // Handle navigation
        if (IsKeyPressed(keyboardState, Keys.Up))
        {
            Console.WriteLine($"[ProcessMoveSelectionGridNavigation] UP key pressed");
            if (isOnTeraButton)
            {
                // From Tera, go to move 3 (index 2) if it exists, otherwise move 1 (index 0)
                _selectedButtonIndex = moveButtonCount >= 3 ? 2 : 0;
            }
            else
            {
                // From move buttons
                if (currentMoveIndex < 2)
                {
                    // From move 1 or 2 (top row), go to Tera if available, otherwise wrap to bottom row
                    if (hasTeraButton)
                    {
                        _selectedButtonIndex = _buttons.Count - 1; // Tera button
                    }
                    else
                    {
                        // Wrap to bottom row
                        _selectedButtonIndex = currentMoveIndex + 2;
                        if (_selectedButtonIndex >= moveButtonCount)
                            _selectedButtonIndex = moveButtonCount - 1;
                    }
                }
                else
                {
                    // From move 3 or 4 (bottom row), go to move 1 or 2 (top row)
                    _selectedButtonIndex = currentMoveIndex - 2;
                }
            }
        }
        else if (IsKeyPressed(keyboardState, Keys.Down))
        {
            Console.WriteLine($"[ProcessMoveSelectionGridNavigation] DOWN key pressed");
            if (isOnTeraButton)
            {
                // From Tera, go to move 1 (index 0)
                _selectedButtonIndex = 0;
            }
            else
            {
                // From move buttons
                if (currentMoveIndex < 2)
                {
                    // From move 1 or 2 (top row), go to move 3 or 4 (bottom row)
                    _selectedButtonIndex = currentMoveIndex + 2;
                    if (_selectedButtonIndex >= moveButtonCount)
                    {
                        // If move 3/4 doesn't exist, go to Tera or wrap
                        _selectedButtonIndex =
                            hasTeraButton ? _buttons.Count - 1 : currentMoveIndex;
                    }
                }
                else
                {
                    // From move 3 or 4 (bottom row), go to Tera if available, otherwise wrap to top
                    _selectedButtonIndex =
                        hasTeraButton ? _buttons.Count - 1 : currentMoveIndex - 2;
                }
            }
        }
        else if (IsKeyPressed(keyboardState, Keys.Left))
        {
            Console.WriteLine($"[ProcessMoveSelectionGridNavigation] LEFT key pressed");
            if (!isOnTeraButton)
            {
                // Swap within row: move 1 <-> 2, move 3 <-> 4
                if (currentMoveIndex == 0 && moveButtonCount >= 2)
                {
                    _selectedButtonIndex = 1; // Move 1 left goes to 2
                }
                else if (currentMoveIndex == 1)
                {
                    _selectedButtonIndex = 0; // Move 2 left goes to 1
                }
                else if (currentMoveIndex == 2 && moveButtonCount >= 4)
                {
                    _selectedButtonIndex = 3; // Move 3 left goes to 4
                }
                else if (currentMoveIndex == 3 && moveButtonCount >= 3)
                {
                    _selectedButtonIndex = 2; // Move 4 left goes to 3
                }
            }
            // Tera button doesn't respond to left/right
        }
        else if (IsKeyPressed(keyboardState, Keys.Right))
        {
            Console.WriteLine($"[ProcessMoveSelectionGridNavigation] RIGHT key pressed");
            if (!isOnTeraButton)
            {
                // Swap within row: move 2 <-> 1, move 4 <-> 3
                if (currentMoveIndex == 1)
                {
                    _selectedButtonIndex = 0; // Move 2 right goes to 1
                }
                else if (currentMoveIndex == 0 && moveButtonCount >= 2)
                {
                    _selectedButtonIndex = 1; // Move 1 right goes to 2
                }
                else if (currentMoveIndex == 3 && moveButtonCount >= 3)
                {
                    _selectedButtonIndex = 2; // Move 4 right goes to 3
                }
                else if (currentMoveIndex == 2 && moveButtonCount >= 4)
                {
                    _selectedButtonIndex = 3; // Move 3 right goes to 4
                }
            }
            // Tera button doesn't respond to left/right
        }
    }

    // Main battle state machine navigation methods
    private void TransitionToState(MainBattlePhaseState newState)
    {
        MainBattleState = newState;
        SetupMainBattleUi((MoveRequest)_currentRequest!);
    }

    private void HandleEscapeKeyNavigation()
    {
        // Handle back navigation based on current state
        switch (MainBattleState)
        {
            case MainBattlePhaseState.MoveSelectionFirstPokemon:
            case MainBattlePhaseState.SwitchSelectionFirstPokemon:
                // Go back to first Pokemon main menu
                TransitionToState(MainBattlePhaseState.MainMenuFirstPokemon);
                break;

            case MainBattlePhaseState.MoveSelectionSecondPokemon:
            case MainBattlePhaseState.SwitchSelectionSecondPokemon:
                // Go back to second Pokemon main menu
                TransitionToState(MainBattlePhaseState.MainMenuSecondPokemon);
                break;

            case MainBattlePhaseState.MainMenuSecondPokemon:
                // Go back to first Pokemon menu
                HandleBackFromSecondPokemon();
                break;

            // MainMenuFirstPokemon and ForceSwitch have no back action
            default:
                break;
        }
    }

    private void HandleMoveSelection(int pokemonIndex, int moveIndex, bool useTera)
    {
        if (pokemonIndex == 0)
        {
            TurnSelection.FirstPokemonMoveIndex = moveIndex;
            TurnSelection.FirstPokemonTarget = 0; // Default target
            TurnSelection.FirstPokemonSwitchIndex = null;
            TurnSelection.FirstPokemonTerastallize = useTera;

            // In singles, submit immediately. In doubles, move to second Pokemon
            if (_currentRequest is MoveRequest request && request.Active.Count > 1)
            {
                TransitionToState(MainBattlePhaseState.MainMenuSecondPokemon);
            }
            else
            {
                SubmitMainBattleTurnChoice();
            }
        }
        else if (pokemonIndex == 1)
        {
            TurnSelection.SecondPokemonMoveIndex = moveIndex;
            TurnSelection.SecondPokemonTarget = 0; // Default target
            TurnSelection.SecondPokemonSwitchIndex = null;
            TurnSelection.SecondPokemonTerastallize = useTera;

            // Both Pokemon have selections, submit
            SubmitMainBattleTurnChoice();
        }
    }

    private void HandleSwitchSelection(int pokemonIndex, int switchIndex)
    {
        if (pokemonIndex == 0)
        {
            TurnSelection.FirstPokemonSwitchIndex = switchIndex;
            TurnSelection.FirstPokemonMoveIndex = null;
            TurnSelection.FirstPokemonTarget = null;

            // In singles, submit immediately. In doubles, move to second Pokemon
            if (_currentRequest is MoveRequest request && request.Active.Count > 1)
            {
                TransitionToState(MainBattlePhaseState.MainMenuSecondPokemon);
            }
            else
            {
                SubmitMainBattleTurnChoice();
            }
        }
        else if (pokemonIndex == 1)
        {
            TurnSelection.SecondPokemonSwitchIndex = switchIndex;
            TurnSelection.SecondPokemonMoveIndex = null;
            TurnSelection.SecondPokemonTarget = null;

            // Both Pokemon have selections, submit
            SubmitMainBattleTurnChoice();
        }
    }

    private void HandleForceSwitchSelection(int switchIndex)
    {
        // For force switch, create switch action and submit immediately
        if (_pendingChoice == null || _currentRequest is not SwitchRequest) return;

        var action = new ChosenAction
        {
            Choice = ChoiceType.Switch,
            Pokemon = null,
            MoveId = MoveId.None,
            Index = switchIndex,
        };

        var actions = _pendingChoice.Actions.ToList();
        actions.Add(action);
        _pendingChoice.Actions = actions;

        SubmitChoice();
    }

    private void ToggleTerastallize(int pokemonIndex)
    {
        Console.WriteLine($"[ToggleTerastallize] Called for pokemon {pokemonIndex}");

        if (pokemonIndex == 0)
        {
            bool oldValue = TurnSelection.FirstPokemonTerastallize;
            TurnSelection.FirstPokemonTerastallize = !TurnSelection.FirstPokemonTerastallize;
            Console.WriteLine(
                $"[ToggleTerastallize] FirstPokemon Tera: {oldValue} -> {TurnSelection.FirstPokemonTerastallize}");
        }
        else if (pokemonIndex == 1)
        {
            bool oldValue = TurnSelection.SecondPokemonTerastallize;
            TurnSelection.SecondPokemonTerastallize = !TurnSelection.SecondPokemonTerastallize;
            Console.WriteLine(
                $"[ToggleTerastallize] SecondPokemon Tera: {oldValue} -> {TurnSelection.SecondPokemonTerastallize}");
        }

        // Refresh buttons to update Tera button appearance
        // BUT preserve the current selection index so user stays on the Tera button
        if (_currentRequest is MoveRequest request)
        {
            int previousSelection = _selectedButtonIndex;
            Console.WriteLine(
                $"[ToggleTerastallize] Refreshing UI, preserving selection index: {previousSelection}");
            SetupMainBattleUi(request);
            _selectedButtonIndex = previousSelection; // Restore selection to Tera button
            Console.WriteLine(
                $"[ToggleTerastallize] UI refreshed, restored selection to: {_selectedButtonIndex}");
        }
        else
        {
            Console.WriteLine(
                $"[ToggleTerastallize] WARNING: _currentRequest is not a MoveRequest!");
        }
    }

    private void HandleForfeit()
    {
        TurnSelection.Forfeit = true;
        // TODO: Implement forfeit choice submission
        Console.WriteLine("[ChoiceInputManager] Forfeit requested (not yet implemented)");
    }

    private void HandleBackFromSecondPokemon()
    {
        // Clear second Pokemon selections and return to first Pokemon menu
        TurnSelection.SecondPokemonMoveIndex = null;
        TurnSelection.SecondPokemonTarget = null;
        TurnSelection.SecondPokemonSwitchIndex = null;
        TurnSelection.SecondPokemonTerastallize = false;

        TransitionToState(MainBattlePhaseState.MainMenuFirstPokemon);
    }

    private void SubmitMainBattleTurnChoice()
    {
        if (_pendingChoice == null || _currentRequest is not MoveRequest request) return;

        var actions = new List<ChosenAction>();

        // Add first Pokemon action
        if (TurnSelection.FirstPokemonMoveIndex.HasValue)
        {
            PokemonMoveRequestData pokemonData = request.Active[0];
            PokemonMoveData moveData =
                pokemonData.Moves[TurnSelection.FirstPokemonMoveIndex.Value];

            // Get Tera type if terastallizing
            MoveType? teraType = null;
            if (TurnSelection.FirstPokemonTerastallize && pokemonData.CanTerastallize != null)
            {
                teraType = pokemonData.CanTerastallize switch
                {
                    MoveTypeMoveTypeFalseUnion mtfu => mtfu.MoveType,
                    _ => null,
                };
            }

            actions.Add(new ChosenAction
            {
                Choice = ChoiceType.Move,
                Pokemon = null,
                MoveId = moveData.Id,
                TargetLoc = TurnSelection.FirstPokemonTarget ?? 0,
                Terastallize = teraType,
            });
        }
        else if (TurnSelection.FirstPokemonSwitchIndex.HasValue)
        {
            actions.Add(new ChosenAction
            {
                Choice = ChoiceType.Switch,
                Pokemon = null,
                MoveId = MoveId.None,
                Index = TurnSelection.FirstPokemonSwitchIndex.Value,
            });
        }

        // Add second Pokemon action (if doubles)
        if (request.Active.Count > 1)
        {
            if (TurnSelection.SecondPokemonMoveIndex.HasValue)
            {
                PokemonMoveRequestData pokemonData = request.Active[1];
                PokemonMoveData moveData =
                    pokemonData.Moves[TurnSelection.SecondPokemonMoveIndex.Value];

                // Get Tera type if terastallizing
                MoveType? teraType = null;
                if (TurnSelection.SecondPokemonTerastallize && pokemonData.CanTerastallize != null)
                {
                    teraType = pokemonData.CanTerastallize switch
                    {
                        MoveTypeMoveTypeFalseUnion mtfu => mtfu.MoveType,
                        _ => null,
                    };
                }

                actions.Add(new ChosenAction
                {
                    Choice = ChoiceType.Move,
                    Pokemon = null,
                    MoveId = moveData.Id,
                    TargetLoc = TurnSelection.SecondPokemonTarget ?? 0,
                    Terastallize = teraType,
                });
            }
            else if (TurnSelection.SecondPokemonSwitchIndex.HasValue)
            {
                actions.Add(new ChosenAction
                {
                    Choice = ChoiceType.Switch,
                    Pokemon = null,
                    MoveId = MoveId.None,
                    Index = TurnSelection.SecondPokemonSwitchIndex.Value,
                });
            }
        }

        _pendingChoice.Actions = actions;

        // Submit the choice
        SubmitChoice();
    }
}