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
        // For TurnStart requests, use the main battle state machine
        if (_requestType == BattleRequestType.TurnStart)
        {
            SetupMainBattleUi(request);
            return;
        }

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

                int index = moveIndex;
                var button = new ChoiceButton(
                    new Rectangle(LeftMargin, y, ButtonWidth, ButtonHeight),
                    $"{moveIndex}. {moveData.Move.Name}",
                    disabled ? Color.Gray : Color.Blue,
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
        _buttons.Clear();
        _selectedButtonIndex = 0; // Reset selection to first button

        // Set up buttons based on current state
        switch (_mainBattleState)
        {
            case MainBattlePhaseState.MainMenuFirstPokemon:
                _buttons = MainBattleUiHelper.CreateMainMenuFirstPokemon(
                    request,
                    () => TransitionToState(MainBattlePhaseState.MoveSelectionFirstPokemon),
                    () => TransitionToState(MainBattlePhaseState.SwitchSelectionFirstPokemon),
                    () => HandleForfeit()
                );
                break;

            case MainBattlePhaseState.MoveSelectionFirstPokemon:
                if (request.Active.Count > 0)
                {
                    PokemonMoveRequestData pokemonData = request.Active[0];
                    bool canTera = pokemonData.CanTerastallize != null;
                    _buttons = MainBattleUiHelper.CreateMoveSelectionButtons(
                        pokemonData,
                        canTera,
                        _turnSelection.FirstPokemonTerastallize,
                        (moveIndex) => HandleMoveSelection(0, moveIndex),
                        () => ToggleTerastallize(0),
                        () => TransitionToState(MainBattlePhaseState.MainMenuFirstPokemon)
                    );
                }

                break;

            case MainBattlePhaseState.SwitchSelectionFirstPokemon:
            {
                var availablePokemon = request.Side.Pokemon
                    .Where(p => !p.Active)
                    .ToList();
                _buttons = MainBattleUiHelper.CreateSwitchSelectionButtons(
                    availablePokemon,
                    (switchIndex) => HandleSwitchSelection(0, switchIndex),
                    () => TransitionToState(MainBattlePhaseState.MainMenuFirstPokemon),
                    showBackButton: true
                );
            }
                break;

            case MainBattlePhaseState.MainMenuSecondPokemon:
                _buttons = MainBattleUiHelper.CreateMainMenuSecondPokemon(
                    request,
                    () => TransitionToState(MainBattlePhaseState.MoveSelectionSecondPokemon),
                    () => TransitionToState(MainBattlePhaseState.SwitchSelectionSecondPokemon),
                    () => HandleBackFromSecondPokemon()
                );
                break;

            case MainBattlePhaseState.MoveSelectionSecondPokemon:
                if (request.Active.Count > 1)
                {
                    PokemonMoveRequestData pokemonData = request.Active[1];
                    bool canTera = pokemonData.CanTerastallize != null;
                    _buttons = MainBattleUiHelper.CreateMoveSelectionButtons(
                        pokemonData,
                        canTera,
                        _turnSelection.SecondPokemonTerastallize,
                        (moveIndex) => HandleMoveSelection(1, moveIndex),
                        () => ToggleTerastallize(1),
                        () => TransitionToState(MainBattlePhaseState.MainMenuSecondPokemon)
                    );
                }

                break;

            case MainBattlePhaseState.SwitchSelectionSecondPokemon:
            {
                var availablePokemon = request.Side.Pokemon
                    .Where(p => !p.Active)
                    .ToList();
                _buttons = MainBattleUiHelper.CreateSwitchSelectionButtons(
                    availablePokemon,
                    (switchIndex) => HandleSwitchSelection(1, switchIndex),
                    () => TransitionToState(MainBattlePhaseState.MainMenuSecondPokemon),
                    showBackButton: true
                );
            }
                break;

            case MainBattlePhaseState.ForceSwitch:
            {
                var availablePokemon = request.Side.Pokemon
                    .Where(p => !p.Active)
                    .ToList();
                _buttons = MainBattleUiHelper.CreateSwitchSelectionButtons(
                    availablePokemon,
                    (switchIndex) => HandleForceSwitchSelection(switchIndex),
                    () => { }, // No back button for force switch
                    showBackButton: false
                );
            }
                break;
        }
    }

    private void RenderMainBattleUi()
    {
        // Draw instruction text based on current state
        string instructionText = MainBattleUiHelper.GetInstructionText(_mainBattleState);
        _spriteBatch.DrawString(_font, instructionText,
            new Vector2(LeftMargin, TopMargin - 60), Color.White);

        // Draw buttons with selection highlight
        for (int i = 0; i < _buttons.Count; i++)
        {
            bool isSelected = (i == _selectedButtonIndex);
            _buttons[i].Draw(_spriteBatch, _font, _graphicsDevice, isSelected);
        }

        // Draw current selections status
        RenderSelectionStatus();
    }

    private void RenderSelectionStatus()
    {
        var statusLines = new List<string>();

        // First Pokemon selection
        if (_turnSelection.FirstPokemonMoveIndex.HasValue)
        {
            statusLines.Add($"P1: Move {_turnSelection.FirstPokemonMoveIndex.Value + 1}" +
                            (_turnSelection.FirstPokemonTerastallize ? " (Tera)" : ""));
        }
        else if (_turnSelection.FirstPokemonSwitchIndex.HasValue)
        {
            statusLines.Add($"P1: Switch to #{_turnSelection.FirstPokemonSwitchIndex.Value + 1}");
        }

        // Second Pokemon selection (if applicable)
        if (_turnSelection.SecondPokemonMoveIndex.HasValue)
        {
            statusLines.Add($"P2: Move {_turnSelection.SecondPokemonMoveIndex.Value + 1}" +
                            (_turnSelection.SecondPokemonTerastallize ? " (Tera)" : ""));
        }
        else if (_turnSelection.SecondPokemonSwitchIndex.HasValue)
        {
            statusLines.Add($"P2: Switch to #{_turnSelection.SecondPokemonSwitchIndex.Value + 1}");
        }

        // Draw status
        if (statusLines.Count > 0)
        {
            string statusText = string.Join(" | ", statusLines);
            _spriteBatch.DrawString(_font, statusText, new Vector2(LeftMargin, 650), Color.Lime);
        }
    }

    private void ProcessMainBattleKeyboardInput(KeyboardState keyboardState)
    {
        if (_buttons.Count == 0) return;

        // Arrow key navigation
        if (IsKeyPressed(keyboardState, Keys.Up))
        {
            _selectedButtonIndex--;
            if (_selectedButtonIndex < 0)
                _selectedButtonIndex = _buttons.Count - 1; // Wrap to bottom
        }
        else if (IsKeyPressed(keyboardState, Keys.Down))
        {
            _selectedButtonIndex++;
            if (_selectedButtonIndex >= _buttons.Count)
                _selectedButtonIndex = 0; // Wrap to top
        }

        // Enter to select the highlighted button
        if (IsKeyPressed(keyboardState, Keys.Enter))
        {
            if (_selectedButtonIndex >= 0 && _selectedButtonIndex < _buttons.Count)
            {
                _buttons[_selectedButtonIndex].OnClick();
            }
        }

        // ESC key for back (changed from B)
        if (IsKeyPressed(keyboardState, Keys.Escape))
        {
            // Find back button and trigger it
            var backButton = _buttons.FirstOrDefault(b => b.Text.Contains("Back"));
            if (backButton != null)
            {
                backButton.OnClick();
            }
        }
    }

    // Main battle state machine navigation methods
    private void TransitionToState(MainBattlePhaseState newState)
    {
        _mainBattleState = newState;
        SetupMainBattleUi((MoveRequest)_currentRequest!);
    }

    private void HandleMoveSelection(int pokemonIndex, int moveIndex)
    {
        if (pokemonIndex == 0)
        {
            _turnSelection.FirstPokemonMoveIndex = moveIndex;
            _turnSelection.FirstPokemonTarget = 0; // Default target
            _turnSelection.FirstPokemonSwitchIndex = null;

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
            _turnSelection.SecondPokemonMoveIndex = moveIndex;
            _turnSelection.SecondPokemonTarget = 0; // Default target
            _turnSelection.SecondPokemonSwitchIndex = null;

            // Both Pokemon have selections, submit
            SubmitMainBattleTurnChoice();
        }
    }

    private void HandleSwitchSelection(int pokemonIndex, int switchIndex)
    {
        if (pokemonIndex == 0)
        {
            _turnSelection.FirstPokemonSwitchIndex = switchIndex;
            _turnSelection.FirstPokemonMoveIndex = null;
            _turnSelection.FirstPokemonTarget = null;

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
            _turnSelection.SecondPokemonSwitchIndex = switchIndex;
            _turnSelection.SecondPokemonMoveIndex = null;
            _turnSelection.SecondPokemonTarget = null;

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
        if (pokemonIndex == 0)
        {
            _turnSelection.FirstPokemonTerastallize = !_turnSelection.FirstPokemonTerastallize;
        }
        else if (pokemonIndex == 1)
        {
            _turnSelection.SecondPokemonTerastallize = !_turnSelection.SecondPokemonTerastallize;
        }

        // Refresh buttons to update Tera button appearance
        if (_currentRequest is MoveRequest request)
        {
            SetupMainBattleUi(request);
        }
    }

    private void HandleForfeit()
    {
        _turnSelection.Forfeit = true;
        // TODO: Implement forfeit choice submission
        Console.WriteLine("[ChoiceInputManager] Forfeit requested (not yet implemented)");
    }

    private void HandleBackFromSecondPokemon()
    {
        // Clear second Pokemon selections and return to first Pokemon menu
        _turnSelection.SecondPokemonMoveIndex = null;
        _turnSelection.SecondPokemonTarget = null;
        _turnSelection.SecondPokemonSwitchIndex = null;
        _turnSelection.SecondPokemonTerastallize = false;

        TransitionToState(MainBattlePhaseState.MainMenuFirstPokemon);
    }

    private void SubmitMainBattleTurnChoice()
    {
        if (_pendingChoice == null || _currentRequest is not MoveRequest request) return;

        var actions = new List<ChosenAction>();

        // Add first Pokemon action
        if (_turnSelection.FirstPokemonMoveIndex.HasValue)
        {
            PokemonMoveRequestData pokemonData = request.Active[0];
            PokemonMoveData moveData =
                pokemonData.Moves[_turnSelection.FirstPokemonMoveIndex.Value];

            actions.Add(new ChosenAction
            {
                Choice = ChoiceType.Move,
                Pokemon = null,
                MoveId = moveData.Id,
                TargetLoc = _turnSelection.FirstPokemonTarget ?? 0,
            });
        }
        else if (_turnSelection.FirstPokemonSwitchIndex.HasValue)
        {
            actions.Add(new ChosenAction
            {
                Choice = ChoiceType.Switch,
                Pokemon = null,
                MoveId = MoveId.None,
                Index = _turnSelection.FirstPokemonSwitchIndex.Value,
            });
        }

        // Add second Pokemon action (if doubles)
        if (request.Active.Count > 1)
        {
            if (_turnSelection.SecondPokemonMoveIndex.HasValue)
            {
                PokemonMoveRequestData pokemonData = request.Active[1];
                PokemonMoveData moveData =
                    pokemonData.Moves[_turnSelection.SecondPokemonMoveIndex.Value];

                actions.Add(new ChosenAction
                {
                    Choice = ChoiceType.Move,
                    Pokemon = null,
                    MoveId = moveData.Id,
                    TargetLoc = _turnSelection.SecondPokemonTarget ?? 0,
                });
            }
            else if (_turnSelection.SecondPokemonSwitchIndex.HasValue)
            {
                actions.Add(new ChosenAction
                {
                    Choice = ChoiceType.Switch,
                    Pokemon = null,
                    MoveId = MoveId.None,
                    Index = _turnSelection.SecondPokemonSwitchIndex.Value,
                });
            }
        }

        // Set Terastallize flag
        _pendingChoice.Terastallize = _turnSelection.FirstPokemonTerastallize ||
                                      _turnSelection.SecondPokemonTerastallize;

        _pendingChoice.Actions = actions;

        // Submit the choice
        SubmitChoice();
    }
}