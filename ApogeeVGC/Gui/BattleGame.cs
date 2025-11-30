using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ApogeeVGC.Gui.Rendering;
using ApogeeVGC.Gui.ChoiceUI;
using ApogeeVGC.Gui.Animations;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.PokemonClasses;

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
    private AnimationManager? _animationManager;
    private AnimationCoordinator? _animationCoordinator;

    private BattlePerspective? _currentBattlePerspective;
    private BattlePerspective? _hpSnapshotPerspective; // Snapshot for HP preservation across event batches
    private readonly Lock _stateLock = new();
    private bool _shouldExit;
    private bool _battleCompleteShown;
    
    // Deferred events that should wait for animations to complete
    private readonly Queue<BattleEvent> _deferredEvents = new();

    // Message queue for battle events
    private readonly List<BattleMessage> _messageQueue = [];
    private const int MaxMessagesDisplayed = 50; // Limit message history

    // Thread-safe coordinator for choice requests
    private readonly GuiChoiceCoordinator _choiceCoordinator = new();

    // Pending battle start data
    private Library? _pendingLibrary;
    private BattleOptions? _pendingBattleOptions;
    private Simulator? _pendingPlayerController;
    private bool _shouldStartBattle;

    // Screen dimensions
    private const int ScreenWidth = 1920;
    private const int ScreenHeight = 1080;

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
            // Connect battle renderer to choice input manager (for Pokemon box hit testing)
            _choiceInputManager.SetBattleRenderer(_battleRenderer);
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
        Simulator simulator)
    {
        Console.WriteLine(
            $"[BattleGame] StartBattle called. _choiceInputManager null? {_choiceInputManager == null}");

        if (_battleRunner is { IsRunning: true })
        {
            Console.WriteLine("[BattleGame] Battle is already running");
            return;
        }

        // If content is loaded, start immediately
        if (_choiceInputManager != null)
        {
            Console.WriteLine("[BattleGame] Content already loaded, starting battle immediately");
            StartBattleInternal(library, battleOptions, simulator);
        }
        else
        {
            // Queue the battle to start after LoadContent()
            Console.WriteLine("[BattleGame] Content not loaded yet, queueing battle start");
            _pendingLibrary = library;
            _pendingBattleOptions = battleOptions;
            _pendingPlayerController = simulator;
            _shouldStartBattle = true;
        }
    }

    private void StartBattleInternal(Library library, BattleOptions battleOptions,
        Simulator simulator)
    {
        Console.WriteLine("[BattleGame] StartBattleInternal called");

        // Clear any previous battle messages
        ClearMessages();

        // Initialize animation system
        if (_defaultFont != null)
        {
            _animationManager = new AnimationManager(GraphicsDevice, _defaultFont);
            _animationCoordinator = new AnimationCoordinator(_animationManager, library);
            
            // Connect animation manager to battle renderer
            _battleRenderer?.SetAnimationManager(_animationManager);
            
            Console.WriteLine("[BattleGame] Animation system initialized");
        }

        _battleRunner = new BattleRunner(library, battleOptions, simulator);
        _battleRunner.StartBattle();
        Console.WriteLine("[BattleGame] Battle runner started");
    }

    /// <summary>
    /// Process a battle event: store perspective and trigger animations
    /// Returns true if the event should stop further event processing (to wait for animations)
    /// </summary>
    private bool ProcessBattleEvent(BattleEvent evt)
    {
        Console.WriteLine($"[BattleGame.ProcessBattleEvent] Processing event with message type: {evt.Message?.GetType().Name ?? "null"}");
        Console.WriteLine($"[BattleGame.ProcessBattleEvent] _animationCoordinator is null? {_animationCoordinator == null}");
        
        // Check if this is a faint/switch message that should wait for animations
        bool hasActiveAnimations = _animationCoordinator?.HasActiveAnimations() ?? false;
        bool shouldDefer = evt.Message is FaintMessage or SwitchMessage && hasActiveAnimations;
        
        if (shouldDefer)
        {
            // Defer this event until animations complete
            Console.WriteLine($"[BattleGame.ProcessBattleEvent] Deferring {evt.Message?.GetType().Name} until animations complete");
            _deferredEvents.Enqueue(evt);
            return true; // Stop processing more events
        }
        
        // Before processing any messages, ensure all Pokemon have frozen HP animations if perspective changed
        // Use the HP snapshot (not _currentBattlePerspective) to detect changes
        // This prevents the "jump" effect when perspective updates with new HP before animations are queued
        PreservePokemonHpBeforePerspectiveUpdate(evt.Perspective);
        
        // Add message to display queue (if present)
        if (evt.Message != null)
        {
            lock (_stateLock)
            {
                _messageQueue.Add(evt.Message);
                if (_messageQueue.Count > MaxMessagesDisplayed)
                {
                    _messageQueue.RemoveRange(0, _messageQueue.Count - MaxMessagesDisplayed);
                }
            }

            Console.WriteLine($"[BattleGame.ProcessBattleEvent] About to call ProcessMessage for {evt.Message.GetType().Name}");
            
            // Process message for animations BEFORE storing perspective
            // This ensures HP bar animations are queued with the correct old HP values
            _animationCoordinator?.ProcessMessage(evt.Message);
            
            Console.WriteLine($"[BattleGame.ProcessBattleEvent] ProcessMessage called");
        }
        
        // Store perspective for rendering AFTER processing animations
        lock (_stateLock)
        {
            _currentBattlePerspective = evt.Perspective;
        }
        
        return false; // Continue processing events
    }
    
    /// <summary>
    /// Preserve Pokemon HP values before perspective update by creating frozen animations
    /// This prevents HP bars from jumping to new values before animations are queued
    /// Uses _hpSnapshotPerspective instead of _currentBattlePerspective to detect changes
    /// </summary>
    private void PreservePokemonHpBeforePerspectiveUpdate(BattlePerspective newPerspective)
    {
        if (_animationManager == null)
            return;
            
        // Only preserve HP during active battle (not team preview)
        if (newPerspective.PerspectiveType != BattlePerspectiveType.InBattle)
            return;
        
        // Use HP snapshot as the baseline (if it exists), otherwise use current perspective
        BattlePerspective? baselinePerspective = _hpSnapshotPerspective ?? _currentBattlePerspective;
        
        if (baselinePerspective == null)
        {
            // First event - create snapshot
            _hpSnapshotPerspective = newPerspective;
            return;
        }
        
        // Check each Pokemon in the new perspective and create frozen animations if HP changed from baseline
        for (int slot = 0; slot < newPerspective.PlayerSide.Active.Count; slot++)
        {
            var newPokemon = newPerspective.PlayerSide.Active[slot];
            var oldPokemon = slot < baselinePerspective.PlayerSide.Active.Count 
                ? baselinePerspective.PlayerSide.Active[slot] 
                : null;
                
            if (newPokemon != null && oldPokemon != null && newPokemon.Name == oldPokemon.Name)
            {
                // Same Pokemon in same slot - check if HP changed
                if (newPokemon.Hp != oldPokemon.Hp)
                {
                    string key = $"{newPokemon.Name}|0"; // Player is always SideId.P1 (0)
                    
                    // Only create frozen animation if one doesn't already exist
                    if (_animationManager.GetAnimatedHp(key) == null)
                    {
                        Console.WriteLine($"[BattleGame] Creating frozen animation for {key}: {oldPokemon.Hp} HP (new perspective shows {newPokemon.Hp})");
                        _animationManager.StartHpBarAnimation(key, oldPokemon.Hp, oldPokemon.Hp, oldPokemon.MaxHp);
                        // Immediately freeze it at old HP (not Stop, which marks it complete)
                        var frozenAnim = _animationManager.GetHpBarAnimation(key);
                        frozenAnim?.Freeze();
                    }
                }
            }
        }
        
        for (int slot = 0; slot < newPerspective.OpponentSide.Active.Count; slot++)
        {
            var newPokemon = newPerspective.OpponentSide.Active[slot];
            var oldPokemon = slot < baselinePerspective.OpponentSide.Active.Count 
                ? baselinePerspective.OpponentSide.Active[slot] 
                : null;
                
            if (newPokemon != null && oldPokemon != null && newPokemon.Name == oldPokemon.Name)
            {
                // Same Pokemon in same slot - check if HP changed
                if (newPokemon.Hp != oldPokemon.Hp)
                {
                    string key = $"{newPokemon.Name}|1"; // Opponent is always SideId.P2 (1)
                    
                    // Only create frozen animation if one doesn't already exist
                    if (_animationManager.GetAnimatedHp(key) == null)
                    {
                        Console.WriteLine($"[BattleGame] Creating frozen animation for {key}: {oldPokemon.Hp} HP (new perspective shows {newPokemon.Hp})");
                        _animationManager.StartHpBarAnimation(key, oldPokemon.Hp, oldPokemon.Hp, oldPokemon.MaxHp);
                        // Immediately freeze it at old HP (not Stop, which marks it complete)
                        var frozenAnim = _animationManager.GetHpBarAnimation(key);
                        frozenAnim?.Freeze();
                    }
                }
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        try
        {
            // Process events from battle thread
            // Process events until an animation is triggered, then wait for completion
            bool hasActiveAnimations = _animationCoordinator?.HasActiveAnimations() ?? false;
            
            if (!hasActiveAnimations)
            {
                // No animations running - update HP snapshot to current perspective before processing new events
                // This ensures the next batch of events compares against the latest stable HP values
                if (_currentBattlePerspective != null && 
                    _currentBattlePerspective.PerspectiveType == BattlePerspectiveType.InBattle)
                {
                    _hpSnapshotPerspective = _currentBattlePerspective;
                }
                
                // First, process any deferred events that were waiting for animations to complete
                while (_deferredEvents.Count > 0)
                {
                    BattleEvent deferredEvt = _deferredEvents.Dequeue();
                    Console.WriteLine($"[BattleGame.Update] Processing deferred event: {deferredEvt.Message?.GetType().Name ?? "null"}");
                    bool shouldStop = ProcessBattleEvent(deferredEvt);
                    if (shouldStop)
                        break;
                        
                    // After processing deferred event, trigger any pending animations
                    _animationCoordinator?.TriggerPendingAttackAnimation();
                    
                    // Check if animations started
                    if (_animationCoordinator?.HasActiveAnimations() ?? false)
                    {
                        break; // Stop processing, wait for animations
                    }
                }
                
                // If no deferred events or they didn't trigger animations, process new events
                if (!(_animationCoordinator?.HasActiveAnimations() ?? false))
                {
                    // Process events until we trigger an animation
                    // This allows related messages (move used + damage) to be processed together
                    bool hadActiveAnimationsBefore = false;
                    int maxEventsPerFrame = 10; // Safety limit to prevent infinite loops
                    int eventsProcessed = 0;
                    
                    while (eventsProcessed < maxEventsPerFrame && 
                           _choiceCoordinator.TryDequeueEvent(out BattleEvent? evt) && evt != null)
                    {
                        bool shouldStop = ProcessBattleEvent(evt);
                        eventsProcessed++;
                        
                        if (shouldStop)
                        {
                            // Event was deferred, stop processing
                            break;
                        }
                        
                        // After processing each event, trigger any pending attack animations
                        // This ensures all damage messages have been collected before the animation starts
                        _animationCoordinator?.TriggerPendingAttackAnimation();
                        
                        // Check if this event triggered an animation
                        bool hasActiveAnimationsNow = _animationCoordinator?.HasActiveAnimations() ?? false;
                        if (hasActiveAnimationsNow && !hadActiveAnimationsBefore)
                        {
                            // Animation was just triggered - stop processing events
                            break;
                        }
                        hadActiveAnimationsBefore = hasActiveAnimationsNow;
                    }
                }
            }
            // If animations ARE active, don't process any new events until they complete
            
            // Process perspective updates (LEGACY - for team preview)
            while (_choiceCoordinator.TryDequeuePerspective(out BattlePerspective? perspective) &&
                   perspective != null)
            {
                ApplyPerspectiveUpdate(perspective);
            }

            // Process any pending choice requests from the battle thread
            while (_choiceCoordinator.TryDequeueRequest(out PendingChoiceRequest? pendingRequest) &&
                   pendingRequest != null)
            {
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

                        // When the task completes, forward the result to the battle thread
                        PendingChoiceRequest request = pendingRequest;
                        task.ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                request.CompletionSource.SetException(t.Exception!);
                            }
                            else if (t.IsCanceled)
                            {
                                request.CompletionSource.SetCanceled();
                            }
                            else
                            {
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
                        "[BattleGame.Update] _choiceInputManager is null, cannot process request");
                }
            }

            // Check if external code requested exit
            if (_shouldExit)
            {
                Exit();
            }

            // Update choice input manager (runs on GUI thread)
            _choiceInputManager?.Update(gameTime);

            // Update animation manager
            _animationManager?.Update(gameTime);

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
    /// Legacy method for team preview compatibility
    /// </summary>
    private void ApplyPerspectiveUpdate(BattlePerspective perspective)
    {
        lock (_stateLock)
        {
            _currentBattlePerspective = perspective;
        }
    }

    /// <summary>
    /// Register Pokemon positions for animations based on perspective
    /// </summary>
    private void RegisterPokemonPositions(BattlePerspective perspective)
    {
        _animationCoordinator!.ClearPokemonPositions();
        _animationCoordinator.SetPlayerSideId(SideId.P1);

        // Register player active Pokemon
        for (int slot = 0; slot < perspective.PlayerSide.Active.Count; slot++)
        {
            var pokemon = perspective.PlayerSide.Active[slot];
            if (pokemon != null)
            {
                int xPosition = 20 + (slot * 188);
                var position = new Vector2(xPosition + 64, 400 + 64);
                _animationCoordinator.RegisterPokemonPosition(pokemon.Name, position, slot, true);
            }
        }

        // Register opponent active Pokemon
        for (int slot = 0; slot < perspective.OpponentSide.Active.Count; slot++)
        {
            var pokemon = perspective.OpponentSide.Active[slot];
            if (pokemon != null)
            {
                int xPosition = 480 + (slot * 188);
                var position = new Vector2(xPosition + 64, 80 + 64);
                _animationCoordinator.RegisterPokemonPosition(pokemon.Name, position, slot, false);
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

        // Register Pokemon positions for animation coordinator
        if (_animationCoordinator != null && perspectiveToRender != null && 
            perspectiveToRender.PerspectiveType == BattlePerspectiveType.InBattle)
        {
            RegisterPokemonPositions(perspectiveToRender);
        }

        // Always render from perspective - single code path!
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
            PokemonPerspective? p = battlePerspective.PlayerSide.Active[i];
            Console.WriteLine(
                $"[BattleGame]   PlayerSide.Active[{i}] = {(p == null ? "null" : $"{p.Name} (HP: {p.Hp}/{p.MaxHp})")}");
        }

        Console.WriteLine(
            $"[BattleGame]   OpponentSide.Active.Count={battlePerspective.OpponentSide.Active.Count}");
        for (int i = 0; i < battlePerspective.OpponentSide.Active.Count; i++)
        {
            PokemonPerspective? p = battlePerspective.OpponentSide.Active[i];
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
            return [.._messageQueue];
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
        
        // Clear animations as well
        _animationManager?.Clear();
        _animationCoordinator?.ClearPokemonPositions();
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