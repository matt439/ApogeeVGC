using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Manages user input for battle choices, supporting both mouse clicks and keyboard input.
/// Uses a thread-safe queue to communicate choices back to the async battle simulation.
/// </summary>
public class ChoiceInputManager(
    SpriteBatch spriteBatch,
    SpriteFont font,
    GraphicsDevice graphicsDevice)
{
    private IChoiceRequest? _currentRequest;
    private BattleRequestType _requestType;
    private BattlePerspective? _perspective;
    private Choice? _pendingChoice;

    // Thread-safe completion source that the battle thread will await
    private TaskCompletionSource<Choice>? _choiceCompletionSource;

    // Thread-safe flag to indicate if a request is active
    private volatile bool _isRequestActive;

    // UI state
    private List<ChoiceButton> _buttons = [];
    private KeyboardState _previousKeyboardState = Keyboard.GetState();
    private MouseState _previousMouseState = Mouse.GetState();
    private string _keyboardInput = "";
    private int _selectedPokemonIndex;

    // Team preview keyboard state
    private int _currentHighlightedIndex = 0;

    private readonly List<int>
        _lockedInPositions = []; // Tracks which Pokemon have been locked in (in order)

    private readonly HashSet<int> _lockedInIndices = []; // Tracks which indices are already locked

    // Main battle phase state
    private MainBattlePhaseState _mainBattleState = MainBattlePhaseState.MainMenuFirstPokemon;
    private readonly TurnSelectionState _turnSelection = new();
    private readonly TimerManager _timerManager = new();

    // Public properties for renderer access
    public int CurrentHighlightedIndex => _currentHighlightedIndex;
    public IReadOnlyList<int> LockedInPositions => _lockedInPositions.AsReadOnly();
    public IReadOnlySet<int> LockedInIndices => _lockedInIndices;
    public BattleRequestType CurrentRequestType => _requestType;
    public MainBattlePhaseState MainBattleState => _mainBattleState;
    public TurnSelectionState TurnSelection => _turnSelection;
    public TimerManager TimerManager => _timerManager;

    // Layout constants
    private const int ButtonWidth = 200;
    private const int ButtonHeight = 50;
    private const int ButtonSpacing = 10;
    private const int LeftMargin = 50;
    private const int TopMargin = 400;

    /// <summary>
    /// Request a choice from the user and return it asynchronously.
    /// This is called from the battle simulation thread, NOT the GUI thread.
    /// </summary>
    public Task<Choice> RequestChoiceAsync(IChoiceRequest request, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        if (_isRequestActive)
        {
            throw new InvalidOperationException("A choice request is already active");
        }

        // Set up the request state (thread-safe assignment)
        _currentRequest = request;
        _requestType = requestType;
        _perspective = perspective;
        _pendingChoice = new Choice
        {
            Actions = new List<ChosenAction>(),
        };

        // Create the completion source that the battle thread will await
        _choiceCompletionSource = new TaskCompletionSource<Choice>();
        _isRequestActive = true;

        // Setup UI (this will be processed on the next Update() call on the GUI thread)
        SetupChoiceUi();

        // Register cancellation
        cancellationToken.Register(() =>
        {
            _choiceCompletionSource?.TrySetCanceled();
            _isRequestActive = false;
        });

        return _choiceCompletionSource.Task;
    }

    /// <summary>
    /// Update input state and process user choices.
    /// This runs on the MonoGame update thread.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        // Update timers
        _timerManager.Update(gameTime);

        // Only process input if we have an active request
        if (!_isRequestActive || _currentRequest == null || _choiceCompletionSource == null)
            return;

        KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();

        // Process keyboard input
        ProcessKeyboardInput(keyboardState);

        // Process mouse input
        ProcessMouseInput(mouseState);

        _previousKeyboardState = keyboardState;
        _previousMouseState = mouseState;
    }

    /// <summary>
    /// Render the choice UI
    /// </summary>
    public void Render(GameTime gameTime)
    {
        if (_currentRequest == null)
            return;

        // For team preview, render different UI
        if (_requestType == BattleRequestType.TeamPreview)
        {
            RenderTeamPreviewUi();
            return;
        }

        // For main battle with move request, show main battle UI
        if (_requestType == BattleRequestType.TurnStart && _currentRequest is MoveRequest)
        {
            RenderMainBattleUi();
            return;
        }

        // Draw instruction text (legacy UI for other request types)
        string instructionText = GetInstructionText();
        spriteBatch.DrawString(font, instructionText, new Vector2(LeftMargin, TopMargin - 60),
            Color.White);

        // Draw keyboard input display
        if (!string.IsNullOrEmpty(_keyboardInput))
        {
            spriteBatch.DrawString(font, $"Input: {_keyboardInput}",
                new Vector2(LeftMargin, TopMargin - 30),
                Color.Yellow);
        }

        // Draw buttons
        foreach (ChoiceButton button in _buttons)
        {
            button.Draw(spriteBatch, font, graphicsDevice);
        }

        // Draw current choice status
        if (_pendingChoice is not { Actions.Count: > 0 }) return;

        string statusText =
            $"Selected: {string.Join(", ", _pendingChoice.Actions.Select(GetActionDescription))}";
        spriteBatch.DrawString(font, statusText, new Vector2(LeftMargin, 650), Color.Lime);
    }

    private void RenderMainBattleUi()
    {
        // Draw instruction text based on current state
        string instructionText = MainBattleUiHelper.GetInstructionText(_mainBattleState);
        spriteBatch.DrawString(font, instructionText,
            new Vector2(LeftMargin, TopMargin - 60), Color.White);

        // Draw buttons (they should already be set up based on state)
        foreach (ChoiceButton button in _buttons)
        {
            button.Draw(spriteBatch, font, graphicsDevice);
        }

        // Draw current selections status
        RenderSelectionStatus();
    }

    private void RenderSelectionStatus()
    {
        if (_turnSelection == null) return;

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
            spriteBatch.DrawString(font, statusText, new Vector2(LeftMargin, 650), Color.Lime);
        }
    }

    private void RenderTeamPreviewUi()
    {
        // Draw instruction text
        string instructionText =
            "Use LEFT/RIGHT arrows to navigate. Press ENTER to lock in. Lock in all 6 Pokemon.";
        Vector2 instructionPos = new Vector2(
            (graphicsDevice.Viewport.Width - font.MeasureString(instructionText).X) / 2,
            graphicsDevice.Viewport.Height / 2);
        spriteBatch.DrawString(font, instructionText, instructionPos, Color.White);

        // Draw locked-in status
        if (_lockedInPositions.Count > 0)
        {
            string statusText = $"Locked in {_lockedInPositions.Count}/6 Pokemon";
            Vector2 statusPos = new Vector2(
                (graphicsDevice.Viewport.Width - font.MeasureString(statusText).X) / 2,
                graphicsDevice.Viewport.Height / 2 + 30);
            spriteBatch.DrawString(font, statusText, statusPos, Color.Lime);

            // Show the order
            string orderText =
                $"Order: {string.Join(" -> ", _lockedInPositions.Select(i => $"#{i + 1}"))}";
            Vector2 orderPos = new Vector2(
                (graphicsDevice.Viewport.Width - font.MeasureString(orderText).X) / 2,
                graphicsDevice.Viewport.Height / 2 + 60);
            spriteBatch.DrawString(font, orderText, orderPos, Color.Yellow);
        }
    }

    private void SetupChoiceUi()
    {
        _buttons.Clear();
        _keyboardInput = "";
        _selectedPokemonIndex = 0;

        // Reset team preview state when setting up new UI
        if (_requestType != BattleRequestType.TeamPreview)
        {
            _currentHighlightedIndex = 0;
            _lockedInPositions.Clear();
            _lockedInIndices.Clear();
        }

        // Initialize main battle state for turn start requests
        if (_requestType == BattleRequestType.TurnStart)
        {
            _mainBattleState = MainBattlePhaseState.MainMenuFirstPokemon;
            _turnSelection.Reset();
        }

        if (_currentRequest is MoveRequest moveRequest)
        {
            SetupMoveRequestUi(moveRequest);
        }
        else if (_currentRequest is SwitchRequest switchRequest)
        {
            SetupSwitchRequestUi(switchRequest);
        }
        else if (_currentRequest is TeamPreviewRequest teamPreviewRequest)
        {
            SetupTeamPreviewUi(teamPreviewRequest);
        }
    }

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

    private bool IsMouseClicked(MouseState currentState)
    {
        return currentState.LeftButton == ButtonState.Pressed &&
               _previousMouseState.LeftButton == ButtonState.Released;
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
        if (_pendingChoice == null || _currentRequest is not SwitchRequest request) return;

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

    // Legacy methods still needed for other UI parts
    private void ProcessKeyboardInput(KeyboardState keyboardState)
    {
        // Handle team preview keyboard controls separately
        if (_requestType == BattleRequestType.TeamPreview && _currentRequest is TeamPreviewRequest)
        {
            ProcessTeamPreviewKeyboardInput(keyboardState);
            return;
        }

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

    private void ProcessTeamPreviewKeyboardInput(KeyboardState keyboardState)
    {
        if (_currentRequest is not TeamPreviewRequest request) return;

        int teamSize = request.Side.Pokemon.Count;

        // Left arrow - move highlight left
        if (IsKeyPressed(keyboardState, Keys.Left))
        {
            _currentHighlightedIndex--;
            if (_currentHighlightedIndex < 0)
                _currentHighlightedIndex = teamSize - 1; // Wrap around
        }

        // Right arrow - move highlight right
        if (IsKeyPressed(keyboardState, Keys.Right))
        {
            _currentHighlightedIndex++;
            if (_currentHighlightedIndex >= teamSize)
                _currentHighlightedIndex = 0; // Wrap around
        }

        // Enter - lock in the currently highlighted Pokemon
        if (IsKeyPressed(keyboardState, Keys.Enter))
        {
            // Check if this Pokemon is already locked in
            if (!_lockedInIndices.Contains(_currentHighlightedIndex))
            {
                _lockedInPositions.Add(_currentHighlightedIndex);
                _lockedInIndices.Add(_currentHighlightedIndex);

                // If all Pokemon are locked in, submit the choice
                if (_lockedInPositions.Count == teamSize)
                {
                    SubmitTeamPreviewChoice();
                }
            }
        }
    }

    private void ProcessMouseInput(MouseState mouseState)
    {
        // Disable mouse input for team preview
        if (_requestType == BattleRequestType.TeamPreview)
            return;

        // Check button clicks
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

    private void SubmitChoice()
    {
        if (_pendingChoice == null || _choiceCompletionSource == null)
            return;

        // Complete the task with the choice (thread-safe)
        // This will unblock the battle simulation thread
        _choiceCompletionSource.TrySetResult(_pendingChoice);

// Clear state
        _currentRequest = null;
        _pendingChoice = null;
        _buttons.Clear();
        _keyboardInput = "";
        _isRequestActive = false;
    }

    private void SubmitTeamPreviewChoice()
    {
        if (_pendingChoice == null || _choiceCompletionSource == null)
            return;

        // Build the team preview choice with the locked-in order
        _pendingChoice.Actions = _lockedInPositions.Select((pokemonIndex, orderPosition) =>
            new ChosenAction
            {
                Choice = ChoiceType.Team,
                Pokemon = null,
                MoveId = MoveId.None,
                Index = pokemonIndex,
                Priority = -orderPosition, // Earlier picks have higher priority
            }).ToList();

        // Submit the choice
        SubmitChoice();
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

    private void SetupTeamPreviewUi(TeamPreviewRequest request)
    {
        // Reset team preview state
        _currentHighlightedIndex = 0;
        _lockedInPositions.Clear();
        _lockedInIndices.Clear();

        // No buttons needed for keyboard-only input
        // Pokemon display and highlighting will be handled in BattleRenderer
    }

    private string GetInstructionText()
    {
        return _requestType switch
        {
            BattleRequestType.TurnStart =>
                "Select a move (1-4) or Switch (S). Press Enter to confirm.",
            BattleRequestType.ForceSwitch =>
                "Select a Pokemon to switch in (1-6). Press Enter to confirm.",
            BattleRequestType.TeamPreview =>
                "Use LEFT/RIGHT arrows to navigate. Press ENTER to lock in each Pokemon.",
            _ => "Make your choice and press Enter.",
        };
    }

    private string GetActionDescription(ChosenAction action)
    {
        return action.Choice switch
        {
            ChoiceType.Move => $"Move: {action.MoveId}",
            ChoiceType.Switch => $"Switch to slot {action.Index}",
            ChoiceType.Team => $"Position {action.Index}",
            _ => action.Choice.ToString(),
        };
    }

    private bool IsKeyPressed(KeyboardState currentState, Keys key)
    {
        return currentState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    }
}
