using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ApogeeVGC.Gui.Rendering;
using ApogeeVGC.Gui.ChoiceUI;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Data;

namespace ApogeeVGC.Gui;

/// <summary>
/// Main MonoGame window for the Pokémon battle GUI.
/// Runs the GUI loop on the main thread while the battle simulation runs on a background thread.
/// </summary>
public class BattleGame : Game
{
    private SpriteBatch? _spriteBatch;
    private SpriteFont? _defaultFont;
    private BattleRenderer? _battleRenderer;
    private SpriteManager? _spriteManager;
    private ChoiceInputManager? _choiceInputManager;
    private BattleRunner? _battleRunner;
    private MessageRenderer? _messageRenderer;

    private BattlePerspective? _currentBattlePerspective;
    private readonly object _stateLock = new();
    private bool _shouldExit;
    private bool _battleCompleteShown = false;
    
    // Message queue for battle events
    private readonly List<BattleMessage> _messageQueue = new();
    private const int MaxMessagesDisplayed = 50; // Limit message history

    // Pending battle start data
    private Library? _pendingLibrary;
    private BattleOptions? _pendingBattleOptions;
    private IPlayerController? _pendingPlayerController;
    private bool _shouldStartBattle;

    // Screen dimensions
    private const int ScreenWidth = 1280;
    private const int ScreenHeight = 720;

    public BattleGame()
    {
        var graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Set window size
        graphics.PreferredBackBufferWidth = ScreenWidth;
        graphics.PreferredBackBufferHeight = ScreenHeight;
    }

    protected override void Initialize()
    {
        Window.Title = "Apogee VGC - Pokémon Battle Simulator";

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Console.WriteLine("[BattleGame] LoadContent() called");

     try
      {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
     Console.WriteLine("[BattleGame] SpriteBatch created");

 // Load default font
 _defaultFont = Content.Load<SpriteFont>("Fonts/DefaultFont");
      Console.WriteLine("[BattleGame] Font loaded");

      // Initialize sprite manager and load sprites
      _spriteManager = new SpriteManager();
    _spriteManager.LoadSprites(Content, GraphicsDevice);
    Console.WriteLine("[BattleGame] SpriteManager initialized");

   // Initialize battle renderer with sprite manager
      _battleRenderer =
     new BattleRenderer(_spriteBatch, _defaultFont, GraphicsDevice, _spriteManager);
 Console.WriteLine("[BattleGame] BattleRenderer initialized");

  // Initialize choice input manager
      _choiceInputManager =
     new ChoiceInputManager(_spriteBatch, _defaultFont, GraphicsDevice);
     Console.WriteLine("[BattleGame] ChoiceInputManager initialized successfully");

      // Initialize message renderer
     _messageRenderer = new MessageRenderer(_spriteBatch, _defaultFont, GraphicsDevice);
            Console.WriteLine("[BattleGame] MessageRenderer initialized successfully");

  // Connect choice input manager to battle renderer
   _battleRenderer.SetChoiceInputManager(_choiceInputManager);
    Console.WriteLine("[BattleGame] Components connected");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BattleGame] ERROR in LoadContent: {ex.Message}");
      Console.WriteLine($"[BattleGame] Stack trace: {ex.StackTrace}");

  // Initialize with minimal setup if content loading fails
    // Only initialize if not already initialized
if (_spriteBatch == null)
            {
     _spriteBatch = new SpriteBatch(GraphicsDevice);
    Console.WriteLine("[BattleGame] Created fallback SpriteBatch");
   }

   if (_choiceInputManager == null)
   {
    _choiceInputManager = new ChoiceInputManager(_spriteBatch, null!, GraphicsDevice);
       Console.WriteLine("[BattleGame] Initialized with minimal setup (no fonts/sprites)");
         }
            else
       {
 Console.WriteLine("[BattleGame] ChoiceInputManager was already initialized before error");
     }
      }

        // If there's a pending battle start, start it now that content is loaded
if (_shouldStartBattle && _pendingLibrary != null && _pendingBattleOptions != null &&
            _pendingPlayerController != null)
   {
     Console.WriteLine("[BattleGame] Starting pending battle...");
      StartBattleInternal(_pendingLibrary, _pendingBattleOptions, _pendingPlayerController);
 _shouldStartBattle = false;
      _pendingLibrary = null;
  _pendingBattleOptions = null;
      _pendingPlayerController = null;
   }
    }

    /// <summary>
    /// Starts a battle with the given options.
    /// The battle runs on a background thread and communicates with the GUI via async mechanisms.
    /// If called before LoadContent(), the battle will be queued and started after content loads.
    /// </summary>
    public void StartBattle(Library library, BattleOptions battleOptions,
        IPlayerController playerController)
    {
        Console.WriteLine(
            $"[BattleGame] StartBattle called. _choiceInputManager null? {_choiceInputManager == null}");

        if (_battleRunner != null && _battleRunner.IsRunning)
        {
            Console.WriteLine("[BattleGame] Battle is already running");
            return;
        }

        // If content is loaded, start immediately
        if (_choiceInputManager != null)
        {
            Console.WriteLine("[BattleGame] Content already loaded, starting battle immediately");
            StartBattleInternal(library, battleOptions, playerController);
        }
        else
        {
            // Queue the battle to start after LoadContent()
            Console.WriteLine("[BattleGame] Content not loaded yet, queueing battle start");
            _pendingLibrary = library;
            _pendingBattleOptions = battleOptions;
            _pendingPlayerController = playerController;
            _shouldStartBattle = true;
        }
    }

    private void StartBattleInternal(Library library, BattleOptions battleOptions,
        IPlayerController playerController)
    {
        Console.WriteLine("[BattleGame] StartBattleInternal called");
        
        // Clear any previous battle messages
        ClearMessages();
        
        // PlayerController should be a Simulator instance
  if (playerController is not Simulator simulator)
        {
      throw new InvalidOperationException("PlayerController must be a Simulator instance for GUI battles");
        }
        
        _battleRunner = new BattleRunner(library, battleOptions, simulator);
        _battleRunner.StartBattle();
        Console.WriteLine("[BattleGame] Battle runner started");
    }

    protected override void Update(GameTime gameTime)
    {
        // Check if external code requested exit
        if (_shouldExit)
        {
            Exit();
        }

        // Update choice input manager (runs on GUI thread)
        _choiceInputManager?.Update(gameTime);

        // Check if battle is complete
    if (_battleRunner is { IsCompleted: true })
        {
     if (!_battleCompleteShown)
     {
  Console.WriteLine($"Battle complete! Winner: {_battleRunner.Result}");
        _battleCompleteShown = true;
    // TODO: Show battle results screen
      }
      }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch?.Begin();

        // Thread-safe read of battle perspective
        BattlePerspective? perspectiveToRender;
        lock (_stateLock)
        {
            perspectiveToRender = _currentBattlePerspective;
        }

        // Render battle using the renderer
        _battleRenderer?.Render(gameTime, perspectiveToRender);

        // Render choice UI on top
        _choiceInputManager?.Render(gameTime);
        
     // Render messages on top of everything
   if (_messageRenderer != null)
        {
    var messages = GetMessages();
   _messageRenderer.Render(gameTime, messages);
  }

   _spriteBatch?.End();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        // Clean up resources
        _spriteBatch?.Dispose();
        _spriteManager?.Unload();
        base.UnloadContent();
    }

    /// <summary>
    /// Update the battle perspective to be rendered (thread-safe).
    /// Called from the battle simulation thread.
    /// </summary>
    /// <param name="battlePerspective">New battle perspective to display</param>
    public void UpdateBattlePerspective(BattlePerspective battlePerspective)
    {
        lock (_stateLock)
   {
     _currentBattlePerspective = battlePerspective;
        }

      // Debug logging to track perspective updates
      Console.WriteLine($"[BattleGame] UpdateBattlePerspective called: Type={battlePerspective.PerspectiveType}");
        Console.WriteLine($"[BattleGame]   PlayerSide.Active.Count={battlePerspective.PlayerSide.Active.Count}");
        for (int i = 0; i < battlePerspective.PlayerSide.Active.Count; i++)
        {
        var p = battlePerspective.PlayerSide.Active[i];
          Console.WriteLine($"[BattleGame]   PlayerSide.Active[{i}] = {(p == null ? "null" : $"{p.Name} (HP: {p.Hp}/{p.MaxHp})")}");
        }
        Console.WriteLine($"[BattleGame]   OpponentSide.Active.Count={battlePerspective.OpponentSide.Active.Count}");
   for (int i = 0; i < battlePerspective.OpponentSide.Active.Count; i++)
        {
      var p = battlePerspective.OpponentSide.Active[i];
     Console.WriteLine($"[BattleGame]   OpponentSide.Active[{i}] = {(p == null ? "null" : $"{p.Name} (HP: {p.HpPercentage:F1}%)")}");
        }
    }
    
    /// <summary>
    /// Update the message log with new battle messages (thread-safe).
    /// Called from the battle simulation thread.
    /// </summary>
    /// <param name="messages">New messages to add to the display</param>
    public void UpdateMessages(IEnumerable<BattleMessage> messages)
{
        lock (_stateLock)
{
  _messageQueue.AddRange(messages);
            
         // Trim old messages if we exceed the maximum
     if (_messageQueue.Count > MaxMessagesDisplayed)
            {
     int toRemove = _messageQueue.Count - MaxMessagesDisplayed;
     _messageQueue.RemoveRange(0, toRemove);
     }
        }
        
        // Debug logging
     foreach (var message in messages)
   {
   Console.WriteLine($"[BattleGame] Message: {message.ToDisplayText()}");
        }
    }
    
  /// <summary>
    /// Get a copy of the current message queue (thread-safe).
    /// Called from the GUI rendering thread.
    /// </summary>
    public List<BattleMessage> GetMessages()
    {
        lock (_stateLock)
   {
            return new List<BattleMessage>(_messageQueue);
        }
    }
    
    /// <summary>
    /// Clear all messages from the queue (thread-safe).
    /// Useful when starting a new battle.
    /// </summary>
    public void ClearMessages()
    {
        lock (_stateLock)
        {
   _messageQueue.Clear();
        }
        Console.WriteLine("[BattleGame] Messages cleared");
    }

    /// <summary>
    /// Request a choice from the user via the GUI.
    /// This is called from the battle simulation thread and returns a Task that completes
    /// when the user makes their choice on the GUI thread.
    /// </summary>
    public Task<Choice> RequestChoiceAsync(IChoiceRequest request, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        Console.WriteLine(
        $"[BattleGame] RequestChoiceAsync called. _choiceInputManager null? {_choiceInputManager == null}");

        // Wait for LoadContent() to complete if needed
if (_choiceInputManager == null)
        {
          Console.WriteLine("[BattleGame] Waiting for LoadContent() to complete...");

  // Wait up to 10 seconds for LoadContent to initialize _choiceInputManager
  bool initialized = SpinWait.SpinUntil(() => _choiceInputManager != null,
        TimeSpan.FromSeconds(10));

        if (!initialized)
     {
  Console.WriteLine(
       "[BattleGame] ERROR: Choice input manager not initialized after 10 second wait!");
          Console.WriteLine($"[BattleGame] _spriteBatch null? {_spriteBatch == null}");
                Console.WriteLine($"[BattleGame] _defaultFont null? {_defaultFont == null}");
      Console.WriteLine($"[BattleGame] This BattleGame instance: {this.GetHashCode()}");
      throw new InvalidOperationException(
      "Choice input manager not initialized after waiting 10 seconds. " +
         "LoadContent() may have failed or this is the wrong BattleGame instance.");
      }

            Console.WriteLine("[BattleGame] ChoiceInputManager is now initialized");
        }

        // The battle thread calls this and awaits the result
      // The GUI thread will complete the TaskCompletionSource when the user makes a choice
    if (_choiceInputManager == null)
  {
            throw new InvalidOperationException("Choice input manager is null after initialization wait.");
     }

        return _choiceInputManager.RequestChoiceAsync(request, requestType, perspective,
    cancellationToken);
    }
    
    /// <summary>
    /// Request the GUI window to exit gracefully
    /// </summary>
    public void RequestExit()
    {
        _shouldExit = true;
    }
}