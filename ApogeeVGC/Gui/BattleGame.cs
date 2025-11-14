using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    private readonly Lock _stateLock = new();
    private bool _shouldExit;
    private bool _battleCompleteShown;

    // Message queue for battle events
    private readonly List<BattleMessage> _messageQueue = [];
    private const int MaxMessagesDisplayed = 50; // Limit message history

    // Thread-safe coordinator for choice requests
    private readonly GuiChoiceCoordinator _choiceCoordinator = new();

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
                Console.WriteLine(
                    "[BattleGame] ChoiceInputManager was already initialized before error");
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
            throw new InvalidOperationException(
                "PlayerController must be a Simulator instance for GUI battles");
        }

        _battleRunner = new BattleRunner(library, battleOptions, simulator);
        _battleRunner.StartBattle();
        Console.WriteLine("[BattleGame] Battle runner started");
    }

    private int _battleGameUpdateCount;

    protected override void Update(GameTime gameTime)
    {
        // CRITICAL DIAGNOSTIC: Log FIRST before anything else
        bool shouldLogVerbose = _battleGameUpdateCount % 60 == 0;
        if (shouldLogVerbose)
        {
            Console.WriteLine(
                $"[BattleGame.Update] START OF UPDATE #{_battleGameUpdateCount}, thread {Thread.CurrentThread.ManagedThreadId}");
        }

        // Log IMMEDIATELY at the start, before try block
        if (shouldLogVerbose)
        {
            Console.WriteLine(
                $"[BattleGame.Update] HEARTBEAT #{_battleGameUpdateCount}, Queue size: {_choiceCoordinator.QueueSize}");
        }

        try
        {
            _battleGameUpdateCount++;

            // Log periodically to confirm Update is being called
            if (_battleGameUpdateCount <= 10 || _battleGameUpdateCount % 100 == 0)
            {
                Console.WriteLine(
                    $"[BattleGame.Update #{_battleGameUpdateCount}] Called on thread {Thread.CurrentThread.ManagedThreadId}, Queue size: {_choiceCoordinator.QueueSize}");
            }

            // Process queued perspective updates from battle thread
            while (_choiceCoordinator.TryDequeuePerspective(out var perspective) &&
                   perspective != null)
            {
                ApplyPerspectiveUpdate(perspective);
            }

            // Process queued messages from battle thread
            while (_choiceCoordinator.TryDequeueMessages(out var messages) && messages != null)
            {
                ApplyMessageUpdates(messages);
            }

            // Process any pending choice requests from the battle thread
            int processedCount = 0;
            while (_choiceCoordinator.TryDequeueRequest(out var pendingRequest) &&
                   pendingRequest != null)
            {
                processedCount++;
                Console.WriteLine(
                    $"[BattleGame.Update #{_battleGameUpdateCount}] Processing queued choice request #{processedCount} on GUI thread");

                if (_choiceInputManager != null)
                {
                    try
                    {
                        // Now we're on the GUI thread, safe to call ChoiceInputManager
                        var task = _choiceInputManager.RequestChoiceAsync(
                            pendingRequest.Request,
                            pendingRequest.RequestType,
                            pendingRequest.Perspective,
                            pendingRequest.CancellationToken);

                        Console.WriteLine(
                            $"[BattleGame.Update #{_battleGameUpdateCount}] ChoiceInputManager.RequestChoiceAsync returned, setting up continuation");

                        // When the task completes, forward the result to the battle thread
                        PendingChoiceRequest request = pendingRequest;
                        task.ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                Console.WriteLine(
                                    $"[BattleGame.ContinueWith] Task faulted: {t.Exception?.GetBaseException().Message}");
                                request.CompletionSource.SetException(t.Exception!);
                            }
                            else if (t.IsCanceled)
                            {
                                Console.WriteLine($"[BattleGame.ContinueWith] Task canceled");
                                request.CompletionSource.SetCanceled();
                            }
                            else
                            {
                                Console.WriteLine(
                                    $"[BattleGame.ContinueWith] Task completed successfully");
                                request.CompletionSource.SetResult(t.Result);
                            }
                        }, TaskScheduler.Default);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"[BattleGame.Update] EXCEPTION processing choice request: {ex.Message}");
                        Console.WriteLine($"[BattleGame.Update] Stack trace: {ex.StackTrace}");
                        pendingRequest.CompletionSource.SetException(ex);
                    }
                }
                else
                {
                    Console.WriteLine(
                        $"[BattleGame.Update] _choiceInputManager is null, cannot process request");
                }
            }

            if (processedCount > 0)
            {
                Console.WriteLine(
                    $"[BattleGame.Update #{_battleGameUpdateCount}] Processed {processedCount} queued requests");
            }

            // Check if external code requested exit
            if (_shouldExit)
            {
                Exit();
            }

            // Update choice input manager (runs on GUI thread)
            if (_choiceInputManager != null)
            {
                _choiceInputManager.Update(gameTime);
            }
            else if (_battleGameUpdateCount <= 10 || _battleGameUpdateCount % 100 == 0)
            {
                Console.WriteLine(
                    $"[BattleGame.Update #{_battleGameUpdateCount}] _choiceInputManager is null!");
            }

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
        catch (Exception ex)
        {
            Console.WriteLine($"[BattleGame.Update] EXCEPTION: {ex.Message}");
            Console.WriteLine($"[BattleGame.Update] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Apply a perspective update on the GUI thread (called from Update)
    /// </summary>
    private void ApplyPerspectiveUpdate(BattlePerspective perspective)
    {
        lock (_stateLock)
        {
            _currentBattlePerspective = perspective;
        }
    }

    /// <summary>
    /// Apply message updates on the GUI thread (called from Update)
    /// </summary>
    private void ApplyMessageUpdates(IEnumerable<BattleMessage> messages)
    {
        var battleMessages = messages.ToList();
        lock (_stateLock)
        {
            _messageQueue.AddRange(battleMessages);

            // Trim old messages if we exceed the maximum
            if (_messageQueue.Count > MaxMessagesDisplayed)
            {
                int toRemove = _messageQueue.Count - MaxMessagesDisplayed;
                _messageQueue.RemoveRange(0, toRemove);
            }
        }
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
        Console.WriteLine(
            $"[BattleGame] UpdateBattlePerspective called: Type={battlePerspective.PerspectiveType}");
        Console.WriteLine(
            $"[BattleGame]   PlayerSide.Active.Count={battlePerspective.PlayerSide.Active.Count}");
        for (int i = 0; i < battlePerspective.PlayerSide.Active.Count; i++)
        {
            var p = battlePerspective.PlayerSide.Active[i];
            Console.WriteLine(
                $"[BattleGame]   PlayerSide.Active[{i}] = {(p == null ? "null" : $"{p.Name} (HP: {p.Hp}/{p.MaxHp})")}");
        }

        Console.WriteLine(
            $"[BattleGame]   OpponentSide.Active.Count={battlePerspective.OpponentSide.Active.Count}");
        for (int i = 0; i < battlePerspective.OpponentSide.Active.Count; i++)
        {
            var p = battlePerspective.OpponentSide.Active[i];
            Console.WriteLine(
                $"[BattleGame]   OpponentSide.Active[{i}] = {(p == null ? "null" : $"{p.Name} (HP: {p.Hp}/{p.MaxHp})")}");
        }
    }

    /// <summary>
    /// Update the message log with new battle messages (thread-safe).
    /// Called from the battle simulation thread.
    /// </summary>
    /// <param name="messages">New messages to add to the display</param>
    public void UpdateMessages(IEnumerable<BattleMessage> messages)
    {
        var battleMessages = messages.ToList();
        lock (_stateLock)
        {
            _messageQueue.AddRange(battleMessages);

            // Trim old messages if we exceed the maximum
            if (_messageQueue.Count > MaxMessagesDisplayed)
            {
                int toRemove = _messageQueue.Count - MaxMessagesDisplayed;
                _messageQueue.RemoveRange(0, toRemove);
            }
        }

        // Debug logging
        foreach (BattleMessage message in battleMessages)
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
            int count = _messageQueue.Count;
            _messageQueue.Clear();
            Console.WriteLine($"[BattleGame] Messages cleared ({count} messages removed)");
        }
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
            $"[BattleGame] RequestChoiceAsync called from thread {Thread.CurrentThread.ManagedThreadId}");

        // Delegate to the thread-safe coordinator
        // This avoids touching MonoGame objects from the battle thread
        return _choiceCoordinator.RequestChoiceAsync(request, requestType, perspective,
            cancellationToken);
    }

    /// <summary>
    /// Request the GUI window to exit gracefully
    /// </summary>
    public void RequestExit()
    {
        _shouldExit = true;
    }

    /// <summary>
    /// Get the choice coordinator for this GUI window.
    /// Used by PlayerGui to communicate with the GUI without calling methods on the Game object.
    /// </summary>
    public GuiChoiceCoordinator GetChoiceCoordinator()
    {
        return _choiceCoordinator;
    }
}