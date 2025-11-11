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

    // Layout constants
    private const int ButtonWidth = 200;
    private const int ButtonHeight = 30;
    private const int ButtonSpacing = 2;

    // Move buttons to the right side of the screen to avoid overlap with Pokemon
    private const int LeftMargin = 400; // Changed from 50 to 800
    private const int TopMargin = 200;

    /// <summary>
    /// Request a choice from the user and return it asynchronously.
    /// This is called from the battle simulation thread, NOT the GUI thread.
    /// </summary>
    public Task<Choice> RequestChoiceAsync(IChoiceRequest request, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
     Console.WriteLine($"[ChoiceInputManager.RequestChoiceAsync] Called with requestType={requestType}, _isRequestActive={_isRequestActive}");
        
        if (_isRequestActive)
        {
            Console.WriteLine("[ChoiceInputManager.RequestChoiceAsync] ERROR: A choice request is already active!");
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

        Console.WriteLine($"[ChoiceInputManager.RequestChoiceAsync] Set up request: CurrentRequestType={CurrentRequestType}, request type={request.GetType().Name}");

      // Create the completion source that the battle thread will await
  _choiceCompletionSource = new TaskCompletionSource<Choice>();
        _isRequestActive = true;
    
        // Flag that UI setup is needed (will be processed on GUI thread in Update())
      _needsUiSetup = true;

     Console.WriteLine($"[ChoiceInputManager.RequestChoiceAsync] Set _needsUiSetup=true, _isRequestActive={_isRequestActive}");

        // Register cancellation
        cancellationToken.Register(() =>
        {
   _choiceCompletionSource?.TrySetCanceled();
       _isRequestActive = false;
            _needsUiSetup = false;
        });

        Console.WriteLine("[ChoiceInputManager.RequestChoiceAsync] Returning task (UI will be set up on next Update)");
        return _choiceCompletionSource.Task;
    }

    /// <summary>
    /// Update input state and process user choices.
    /// This runs on the MonoGame update thread.
    /// </summary>
    private int _updateCallCount = 0;
    
    public void Update(GameTime gameTime)
    {
  _updateCallCount++;
        
        // Log ALL updates for first 200, then every 100th
        bool shouldLog = _updateCallCount <= 200 || _updateCallCount % 100 == 0;
        
 if (shouldLog)
        {
   Console.WriteLine($"[ChoiceInputManager.Update #{_updateCallCount}] Called. _isRequestActive={_isRequestActive}, _needsUiSetup={_needsUiSetup}");
        }
     
        // Update timers
        TimerManager.Update(gameTime);

        // Check if we need to set up UI (deferred from RequestChoiceAsync)
        if (_needsUiSetup)
  {
Console.WriteLine($"[ChoiceInputManager.Update #{_updateCallCount}] Setting up UI on GUI thread");
  _needsUiSetup = false;
SetupChoiceUi();
       Console.WriteLine($"[ChoiceInputManager.Update #{_updateCallCount}] UI setup complete");
        }

 // Only process input if we have an active request
  if (!_isRequestActive || _currentRequest == null || _choiceCompletionSource == null)
  {
 if (shouldLog)
{
      Console.WriteLine($"[ChoiceInputManager.Update #{_updateCallCount}] Skipping: _isRequestActive={_isRequestActive}, _currentRequest null? {_currentRequest == null}, _choiceCompletionSource null? {_choiceCompletionSource == null}");
 }
            return;
        }

  if (shouldLog)
    {
     Console.WriteLine($"[ChoiceInputManager.Update #{_updateCallCount}] Processing input. CurrentRequestType={CurrentRequestType}, MainBattleState={MainBattleState}, Buttons={_buttons.Count}");
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
        spriteBatch.DrawString(font, statusText, new Vector2(LeftMargin, 650), Color.Lime);
    }

    private void SetupChoiceUi()
    {
     Console.WriteLine($"[ChoiceInputManager.SetupChoiceUi] Called with CurrentRequestType={CurrentRequestType}");
        
        _buttons.Clear();
        _keyboardInput = "";
_selectedPokemonIndex = 0;

     // Reset team preview state when setting up new UI
        if (CurrentRequestType != BattleRequestType.TeamPreview)
    {
       CurrentHighlightedIndex = 0;
       _lockedInPositions.Clear();
_lockedInIndices.Clear();
         Console.WriteLine("[ChoiceInputManager.SetupChoiceUi] Cleared team preview state");
        }

        // Initialize main battle state for turn start requests
    if (CurrentRequestType == BattleRequestType.TurnStart)
 {
   MainBattleState = MainBattlePhaseState.MainMenuFirstPokemon;
      TurnSelection.Reset();
       Console.WriteLine($"[ChoiceInputManager.SetupChoiceUi] Initialized main battle state: {MainBattleState}");
        }

    Console.WriteLine($"[ChoiceInputManager.SetupChoiceUi] Current request type: {_currentRequest?.GetType().Name}");

        if (_currentRequest is MoveRequest moveRequest)
        {
     Console.WriteLine("[ChoiceInputManager.SetupChoiceUi] Setting up MoveRequest UI");
            SetupMoveRequestUi(moveRequest);
        }
        else if (_currentRequest is SwitchRequest switchRequest)
      {
       Console.WriteLine("[ChoiceInputManager.SetupChoiceUi] Setting up SwitchRequest UI");
       SetupSwitchRequestUi(switchRequest);
        }
        else if (_currentRequest is TeamPreviewRequest)
        {
   Console.WriteLine("[ChoiceInputManager.SetupChoiceUi] Setting up TeamPreviewRequest UI");
       SetupTeamPreviewUi();
        }
        
     Console.WriteLine($"[ChoiceInputManager.SetupChoiceUi] Setup complete. Button count: {_buttons.Count}");
    }

    private void SubmitChoice()
    {
      Console.WriteLine($"[ChoiceInputManager.SubmitChoice] Called. _pendingChoice null? {_pendingChoice == null}, _choiceCompletionSource null? {_choiceCompletionSource == null}");
        
        if (_pendingChoice == null || _choiceCompletionSource == null)
       return;

        Console.WriteLine($"[ChoiceInputManager.SubmitChoice] Submitting choice with {_pendingChoice.Actions.Count} actions");

   // Complete the task with the choice (thread-safe)
        // This will unblock the battle simulation thread
        _choiceCompletionSource.TrySetResult(_pendingChoice);

        Console.WriteLine("[ChoiceInputManager.SubmitChoice] Choice submitted to battle thread");

    // Clear state
      _currentRequest = null;
        _pendingChoice = null;
        _buttons.Clear();
       _keyboardInput = "";
        _isRequestActive = false;

        Console.WriteLine($"[ChoiceInputManager.SubmitChoice] State cleared, _isRequestActive={_isRequestActive}");
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