using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Core functionality for ChoiceInputManager - fields, properties, and request handling.
/// </summary>
public partial class ChoiceInputManager(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
{
    private IChoiceRequest? _currentRequest;
    private BattlePerspective? _perspective;
    private Choice? _pendingChoice;

    // Thread-safe completion source that the battle thread will await
    private TaskCompletionSource<Choice>? _choiceCompletionSource;

    // Thread-safe flag to indicate if a request is active
    private volatile bool _isRequestActive;
  
    // Flag to indicate UI setup is needed (set by battle thread, read by GUI thread)
    private volatile bool _needsUiSetup;

    // UI state
    private List<ChoiceButton> _buttons = [];
    private KeyboardState _previousKeyboardState = Keyboard.GetState();
    private MouseState _previousMouseState = Mouse.GetState();
    private string _keyboardInput = "";
    private int _selectedPokemonIndex;

    // Arrow key navigation state
    private int _selectedButtonIndex; // Currently highlighted button for arrow key navigation

    // Team preview keyboard state

    private readonly List<int>
        _lockedInPositions = []; // Tracks which Pokemon have been locked in (in order)

    private readonly HashSet<int> _lockedInIndices = []; // Tracks which indices are already locked

    // Main battle phase state

    // Public properties for renderer access
    public int CurrentHighlightedIndex { get; private set; }

    public IReadOnlyList<int> LockedInPositions => _lockedInPositions.AsReadOnly();
    public IReadOnlySet<int> LockedInIndices => _lockedInIndices;
    public BattleRequestType CurrentRequestType { get; private set; }

    public MainBattlePhaseState MainBattleState { get; private set; } = MainBattlePhaseState.MainMenuFirstPokemon;

    public TurnSelectionState TurnSelection { get; } = new();

    public TimerManager TimerManager { get; } = new();

    // Target selection state (visual box selection)
    public List<int> ValidTargets { get; private set; } = [];
    public int? HighlightedTarget { get; private set; }
    private int _highlightedTargetIndex; // Index in ValidTargets list

    // Reference to battle renderer for Pokemon box hit testing
    private Rendering.BattleRenderer? _battleRenderer;

    // Layout constants
    private const int ButtonWidth = 200;
    private const int ButtonHeight = 30;
    private const int ButtonSpacing = 2;

    // Position buttons inline with player's Pokemon vertically, aligned with opponent's Pokemon horizontally
    private const int LeftMargin = 480; // Aligned with opponent's Pokemon X position (midscreen)
    private const int TopMargin = 400; // Aligned with player's Pokemon Y position

    // Team Preview UI constants
    private const float CenteringDivisor = 2f; // Divisor for centering calculations
    private const int TeamPreviewStatusYOffset = 30; // Y offset for status text below instruction
    private const int TeamPreviewOrderYOffset = 60; // Y offset for order text below instruction
    private const int AutoLockPokemonCount = 4; // Number of Pokemon to auto-lock with 'A' key

    /// <summary>
    /// Set the battle renderer reference for Pokemon box hit testing
    /// </summary>
    public void SetBattleRenderer(Rendering.BattleRenderer battleRenderer)
    {
        _battleRenderer = battleRenderer;
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
        CurrentRequestType = requestType;
        _perspective = perspective;
        _pendingChoice = new Choice
        {
            Actions = new List<ChosenAction>(),
        };

        // Create the completion source that the battle thread will await
        _choiceCompletionSource = new TaskCompletionSource<Choice>();
        _isRequestActive = true;

        // Flag that UI setup is needed (will be processed on GUI thread in Update())
        _needsUiSetup = true;

        // Register cancellation
        cancellationToken.Register(() =>
        {
            _choiceCompletionSource?.TrySetCanceled();
            _isRequestActive = false;
            _needsUiSetup = false;
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
        TimerManager.Update(gameTime);

        // Check if we need to set up UI (deferred from RequestChoiceAsync)
        if (_needsUiSetup)
        {
            _needsUiSetup = false;
            SetupChoiceUi();
        }

        // Only process input if we have an active request
        if (!_isRequestActive || _currentRequest == null || _choiceCompletionSource == null)
        {
            return;
        }

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
        if (CurrentRequestType == BattleRequestType.TeamPreview)
        {
            RenderTeamPreviewUi();
            return;
        }

        // For main battle with move request, show main battle UI
        if (CurrentRequestType == BattleRequestType.TurnStart && _currentRequest is MoveRequest)
        {
            RenderMainBattleUi();

            // Render target selection overlay if in target selection state
            bool isTargetSelection = MainBattleState == MainBattlePhaseState.TargetSelectionFirstPokemon ||
                                     MainBattleState == MainBattlePhaseState.TargetSelectionSecondPokemon;
            if (isTargetSelection && _battleRenderer != null)
            {
                _battleRenderer.RenderTargetSelectionOverlay(ValidTargets, HighlightedTarget);
            }
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
            button.Draw(spriteBatch, font, graphicsDevice, isSelected: false);
        }

        // Draw current choice status
        if (_pendingChoice is not { Actions.Count: > 0 }) return;

        string statusText =
            $"Selected: {string.Join(", ", _pendingChoice.Actions.Select(GetActionDescription))}";
        spriteBatch.DrawString(font, statusText, new Vector2(LeftMargin, 635), Color.Lime);
    }

    private void SetupChoiceUi()
    {
        _buttons.Clear();
        _keyboardInput = "";
        _selectedPokemonIndex = 0;

        // Reset team preview state when setting up new UI
        if (CurrentRequestType != BattleRequestType.TeamPreview)
        {
            CurrentHighlightedIndex = 0;
            _lockedInPositions.Clear();
            _lockedInIndices.Clear();
        }

        // Initialize main battle state for turn start requests
        if (CurrentRequestType == BattleRequestType.TurnStart)
        {
            MainBattleState = MainBattlePhaseState.MainMenuFirstPokemon;
            TurnSelection.Reset();
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
        return CurrentRequestType switch
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