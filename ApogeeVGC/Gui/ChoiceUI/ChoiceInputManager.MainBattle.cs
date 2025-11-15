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
        Console.WriteLine($"[ChoiceInputManager.SetupMoveRequestUi] Called with CurrentRequestType={CurrentRequestType}");

        // For TurnStart requests, use the main battle state machine
        if (CurrentRequestType == BattleRequestType.TurnStart)
        {
            Console.WriteLine("[ChoiceInputManager.SetupMoveRequestUi] Routing to SetupMainBattleUi");
            SetupMainBattleUi(request);
            Console.WriteLine($"[ChoiceInputManager.SetupMoveRequestUi] SetupMainBattleUi complete. Button count: {_buttons.Count}");
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
        Console.WriteLine($"[ChoiceInputManager.SetupMainBattleUi] Called with MainBattleState={MainBattleState}");

        _buttons.Clear();
        _selectedButtonIndex = 0; // Reset selection to first button

        Console.WriteLine("[ChoiceInputManager.SetupMainBattleUi] Cleared buttons and reset selection");

        // Set up buttons based on current state
        switch (MainBattleState)
        {
            case MainBattlePhaseState.MainMenuFirstPokemon:
                Console.WriteLine("[ChoiceInputManager.SetupMainBattleUi] Creating MainMenuFirstPokemon buttons");
                _buttons = MainBattleUiHelper.CreateMainMenuFirstPokemon(
                    request,
                    () => TransitionToState(MainBattlePhaseState.MoveSelectionFirstPokemon),
                    () => TransitionToState(MainBattlePhaseState.SwitchSelectionFirstPokemon),
                    () => HandleForfeit()
                );
                Console.WriteLine($"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for MainMenuFirstPokemon");
                break;

            case MainBattlePhaseState.MoveSelectionFirstPokemon:
                Console.WriteLine("[ChoiceInputManager.SetupMainBattleUi] Creating MoveSelectionFirstPokemon buttons");
                if (request.Active.Count > 0)
                {
                    PokemonMoveRequestData pokemonData = request.Active[0];
                    bool canTera = pokemonData.CanTerastallize != null;
                    _buttons = MainBattleUiHelper.CreateMoveSelectionButtons(
                        pokemonData,
                        canTera,
                        TurnSelection.FirstPokemonTerastallize,
                        (moveIndex) => HandleMoveSelection(0, moveIndex),
                        () => ToggleTerastallize(0),
                        () => TransitionToState(MainBattlePhaseState.MainMenuFirstPokemon)
                    );
                    Console.WriteLine($"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for MoveSelectionFirstPokemon");
                }

                break;

            case MainBattlePhaseState.SwitchSelectionFirstPokemon:
            {
                Console.WriteLine("[ChoiceInputManager.SetupMainBattleUi] Creating SwitchSelectionFirstPokemon buttons");
                var availablePokemon = request.Side.Pokemon
                    .Where(p => !p.Active)
                    .ToList();
                _buttons = MainBattleUiHelper.CreateSwitchSelectionButtons(
                    availablePokemon,
                    (switchIndex) => HandleSwitchSelection(0, switchIndex),
                    () => TransitionToState(MainBattlePhaseState.MainMenuFirstPokemon),
                    showBackButton: true
                );
                Console.WriteLine($"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for SwitchSelectionFirstPokemon");
            }
                break;

            case MainBattlePhaseState.MainMenuSecondPokemon:
                Console.WriteLine("[ChoiceInputManager.SetupMainBattleUi] Creating MainMenuSecondPokemon buttons");
                _buttons = MainBattleUiHelper.CreateMainMenuSecondPokemon(
                    request,
                    () => TransitionToState(MainBattlePhaseState.MoveSelectionSecondPokemon),
                    () => TransitionToState(MainBattlePhaseState.SwitchSelectionSecondPokemon),
                    () => HandleBackFromSecondPokemon()
                );
                Console.WriteLine($"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for MainMenuSecondPokemon");
                break;

            case MainBattlePhaseState.MoveSelectionSecondPokemon:
                Console.WriteLine("[ChoiceInputManager.SetupMainBattleUi] Creating MoveSelectionSecondPokemon buttons");
                if (request.Active.Count > 1)
                {
                    PokemonMoveRequestData pokemonData = request.Active[1];
                    bool canTera = pokemonData.CanTerastallize != null;
                    _buttons = MainBattleUiHelper.CreateMoveSelectionButtons(
                        pokemonData,
                        canTera,
                        TurnSelection.SecondPokemonTerastallize,
                        (moveIndex) => HandleMoveSelection(1, moveIndex),
                        () => ToggleTerastallize(1),
                        () => TransitionToState(MainBattlePhaseState.MainMenuSecondPokemon)
                    );
                    Console.WriteLine($"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for MoveSelectionSecondPokemon");
                }

                break;

            case MainBattlePhaseState.SwitchSelectionSecondPokemon:
            {
                Console.WriteLine("[ChoiceInputManager.SetupMainBattleUi] Creating SwitchSelectionSecondPokemon buttons");
                var availablePokemon = request.Side.Pokemon
                    .Where(p => !p.Active)
                    .ToList();
                _buttons = MainBattleUiHelper.CreateSwitchSelectionButtons(
                    availablePokemon,
                    (switchIndex) => HandleSwitchSelection(1, switchIndex),
                    () => TransitionToState(MainBattlePhaseState.MainMenuSecondPokemon),
                    showBackButton: true
                );
                Console.WriteLine($"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for SwitchSelectionSecondPokemon");
            }
                break;

            case MainBattlePhaseState.ForceSwitch:
            {
                Console.WriteLine("[ChoiceInputManager.SetupMainBattleUi] Creating ForceSwitch buttons");
                var availablePokemon = request.Side.Pokemon
                    .Where(p => !p.Active)
                    .ToList();
                _buttons = MainBattleUiHelper.CreateSwitchSelectionButtons(
                    availablePokemon,
                    (switchIndex) => HandleForceSwitchSelection(switchIndex),
                    () => { }, // No back button for force switch
                    showBackButton: false
                );
                Console.WriteLine($"[ChoiceInputManager.SetupMainBattleUi] Created {_buttons.Count} buttons for ForceSwitch");
            }
                break;
        }

        Console.WriteLine($"[ChoiceInputManager.SetupMainBattleUi] Setup complete. Final button count: {_buttons.Count}");
    }

    private void RenderMainBattleUi()
    {
        // Draw instruction text based on current state
        string instructionText = MainBattleUiHelper.GetInstructionText(MainBattleState);
        // Position instruction text to align with the main battle UI buttons (700, 300)
        // Move it 80 pixels above the buttons instead of 60
        spriteBatch.DrawString(font, instructionText,
            new Vector2(700, 220), Color.White);

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

        // Draw status - move to align with buttons at X=700
      if (statusLines.Count > 0)
        {
      string statusText = string.Join(" | ", statusLines);
     spriteBatch.DrawString(font, statusText, new Vector2(700, 650), Color.Lime);
        }
    }

    private void ProcessMainBattleKeyboardInput(KeyboardState keyboardState)
    {
        // Log every 100 updates to see if this method is being called
      if (_updateCallCount % 100 == 0)
        {
   Console.WriteLine($"[ProcessMainBattleKeyboardInput #{_updateCallCount}] Called. Buttons={_buttons.Count}, _selectedButtonIndex={_selectedButtonIndex}");
        }
        
        if (_buttons.Count == 0) return;

        // Arrow key navigation
        if (IsKeyPressed(keyboardState, Keys.Up))
   {
Console.WriteLine($"[ProcessMainBattleKeyboardInput] UP key pressed");
    _selectedButtonIndex--;
            if (_selectedButtonIndex < 0)
       _selectedButtonIndex = _buttons.Count - 1; // Wrap to bottom
      }
        else if (IsKeyPressed(keyboardState, Keys.Down))
        {
         Console.WriteLine($"[ProcessMainBattleKeyboardInput] DOWN key pressed");
         _selectedButtonIndex++;
 if (_selectedButtonIndex >= _buttons.Count)
        _selectedButtonIndex = 0; // Wrap to top
        }

        // Enter to select the highlighted button
        if (IsKeyPressed(keyboardState, Keys.Enter))
        {
   Console.WriteLine($"[ProcessMainBattleKeyboardInput] ENTER key pressed, selecting button {_selectedButtonIndex}");
       if (_selectedButtonIndex >= 0 && _selectedButtonIndex < _buttons.Count)
   {
      _buttons[_selectedButtonIndex].OnClick();
    }
        }

     // ESC key for back (changed from B)
        if (IsKeyPressed(keyboardState, Keys.Escape))
        {
            Console.WriteLine($"[ProcessMainBattleKeyboardInput] ESC key pressed, looking for back button");
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
        MainBattleState = newState;
        SetupMainBattleUi((MoveRequest)_currentRequest!);
    }

    private void HandleMoveSelection(int pokemonIndex, int moveIndex)
    {
        if (pokemonIndex == 0)
        {
            TurnSelection.FirstPokemonMoveIndex = moveIndex;
            TurnSelection.FirstPokemonTarget = 0; // Default target
            TurnSelection.FirstPokemonSwitchIndex = null;

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
        if (pokemonIndex == 0)
        {
            TurnSelection.FirstPokemonTerastallize = !TurnSelection.FirstPokemonTerastallize;
        }
        else if (pokemonIndex == 1)
        {
            TurnSelection.SecondPokemonTerastallize = !TurnSelection.SecondPokemonTerastallize;
        }

        // Refresh buttons to update Tera button appearance
        if (_currentRequest is MoveRequest request)
        {
            SetupMainBattleUi(request);
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

            actions.Add(new ChosenAction
            {
                Choice = ChoiceType.Move,
                Pokemon = null,
                MoveId = moveData.Id,
                TargetLoc = TurnSelection.FirstPokemonTarget ?? 0,
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

                actions.Add(new ChosenAction
                {
                    Choice = ChoiceType.Move,
                    Pokemon = null,
                    MoveId = moveData.Id,
                    TargetLoc = TurnSelection.SecondPokemonTarget ?? 0,
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

        // Set Terastallize flag
        _pendingChoice.Terastallize = TurnSelection.FirstPokemonTerastallize ||
                                      TurnSelection.SecondPokemonTerastallize;

        _pendingChoice.Actions = actions;

        // Submit the choice
        SubmitChoice();
    }
}