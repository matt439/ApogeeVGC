using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Core functionality for ChoiceInputManager - fields, properties, and request handling.
/// </summary>
public partial class ChoiceInputManager
{
    private readonly SpriteBatch _spriteBatch;
    private readonly SpriteFont _font;
    private readonly GraphicsDevice _graphicsDevice;

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

    // Arrow key navigation state
    private int _selectedButtonIndex; // Currently highlighted button for arrow key navigation

    // Team preview keyboard state
    private int _currentHighlightedIndex;

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

    // Move buttons to the right side of the screen to avoid overlap with Pokemon
    private const int LeftMargin = 800; // Changed from 50 to 800
    private const int TopMargin = 400;

    public ChoiceInputManager(
        SpriteBatch spriteBatch,
        SpriteFont font,
        GraphicsDevice graphicsDevice)
    {
        _spriteBatch = spriteBatch;
        _font = font;
        _graphicsDevice = graphicsDevice;
    }

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
        _spriteBatch.DrawString(_font, instructionText, new Vector2(LeftMargin, TopMargin - 60),
            Color.White);

        // Draw keyboard input display
        if (!string.IsNullOrEmpty(_keyboardInput))
        {
            _spriteBatch.DrawString(_font, $"Input: {_keyboardInput}",
                new Vector2(LeftMargin, TopMargin - 30),
                Color.Yellow);
        }

        // Draw buttons
        foreach (ChoiceButton button in _buttons)
        {
            button.Draw(_spriteBatch, _font, _graphicsDevice, isSelected: false);
        }

        // Draw current choice status
        if (_pendingChoice is not { Actions.Count: > 0 }) return;

        string statusText =
            $"Selected: {string.Join(", ", _pendingChoice.Actions.Select(GetActionDescription))}";
        _spriteBatch.DrawString(_font, statusText, new Vector2(LeftMargin, 650), Color.Lime);
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
        else if (_currentRequest is TeamPreviewRequest)
        {
            SetupTeamPreviewUi();
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

    private bool IsMouseClicked(MouseState currentState)
    {
        return currentState.LeftButton == ButtonState.Pressed &&
               _previousMouseState.LeftButton == ButtonState.Released;
    }
}