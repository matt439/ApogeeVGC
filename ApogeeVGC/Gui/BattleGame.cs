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
    private readonly Lock _stateLock = new();
    private bool _shouldExit;
    private bool _battleCompleteShown;
    
    // Cache Active Pokemon to show fainting process before replacement
    private readonly Dictionary<int, PokemonPerspective> _cachedPlayerActive = new();
    private readonly Dictionary<int, PokemonPerspective> _cachedOpponentActive = new();
    private readonly HashSet<int> _pendingFaintPlayer = new();
    private readonly HashSet<int> _pendingFaintOpponent = new();

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

    protected override void Update(GameTime gameTime)
    {
        try
        {

            // Process queued perspective updates from battle thread
            while (_choiceCoordinator.TryDequeuePerspective(out BattlePerspective? perspective) &&
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

            // Note: processedCount tracking kept for debugging but not logged every frame

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
    /// </summary>
    private void ApplyPerspectiveUpdate(BattlePerspective perspective)
    {
        lock (_stateLock)
        {
            // Initialize cache on first InBattle perspective if empty
            if (perspective.PerspectiveType == BattlePerspectiveType.InBattle &&
                _cachedPlayerActive.Count == 0 && _cachedOpponentActive.Count == 0)
            {
                for (int i = 0; i < perspective.PlayerSide.Active.Count; i++)
                {
                    var pokemon = perspective.PlayerSide.Active[i];
                    if (pokemon != null)
                    {
                        _cachedPlayerActive[i] = pokemon;
                        Console.WriteLine($"[BattleGame] Initial cache player slot {i}: {pokemon.Name}");
                    }
                }
                
                for (int i = 0; i < perspective.OpponentSide.Active.Count; i++)
                {
                    var pokemon = perspective.OpponentSide.Active[i];
                    if (pokemon != null)
                    {
                        _cachedOpponentActive[i] = pokemon;
                        Console.WriteLine($"[BattleGame] Initial cache opponent slot {i}: {pokemon.Name}");
                    }
                }
            }
            
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

        // Process messages through animation coordinator and handle special messages
        if (_animationCoordinator != null)
        {
            foreach (var message in battleMessages)
            {
                // Handle DamageMessage - update cached Pokemon HP
                if (message is DamageMessage damageMsg)
                {
                    HandleDamageMessage(damageMsg);
                }
                // Handle FaintMessage - mark slot as pending faint
                else if (message is FaintMessage faintMsg)
                {
                    HandleFaintMessage(faintMsg);
                }
                // Handle SwitchMessage - update cached Pokemon
                else if (message is SwitchMessage switchMsg)
                {
                    HandleSwitchMessage(switchMsg);
                }
                // Handle TurnStartMessage - refresh cache HP values while preserving sprites
                else if (message is TurnStartMessage turnStartMsg)
                {
                    if (_currentBattlePerspective != null && 
                        _currentBattlePerspective.PerspectiveType == BattlePerspectiveType.InBattle)
                    {
                        if (turnStartMsg.TurnNumber > 1)
                        {
                            // Update cache: For slots NOT pending faint, replace entirely
                            // For slots pending faint, only update HP values (keep the fainted Pokemon's sprite)
                            for (int i = 0; i < _currentBattlePerspective.PlayerSide.Active.Count; i++)
                            {
                                var pokemon = _currentBattlePerspective.PlayerSide.Active[i];
                                if (pokemon != null)
                                {
                                    if (!_pendingFaintPlayer.Contains(i))
                                    {
                                        _cachedPlayerActive[i] = pokemon;
                                        Console.WriteLine($"[BattleGame] Turn {turnStartMsg.TurnNumber} start: Updated player slot {i}: {pokemon.Name}");
                                    }
                                    else if (_cachedPlayerActive.TryGetValue(i, out var cached))
                                    {
                                        // Update HP but keep the cached Pokemon (for sprite continuity)
                                        // Don't update - the perspective shows the REPLACEMENT Pokemon here
                                        Console.WriteLine($"[BattleGame] Turn {turnStartMsg.TurnNumber} start: Kept cached player slot {i}: {cached.Name} (pending faint)");
                                    }
                                }
                            }
                            
                            for (int i = 0; i < _currentBattlePerspective.OpponentSide.Active.Count; i++)
                            {
                                var pokemon = _currentBattlePerspective.OpponentSide.Active[i];
                                if (pokemon != null)
                                {
                                    if (!_pendingFaintOpponent.Contains(i))
                                    {
                                        _cachedOpponentActive[i] = pokemon;
                                        Console.WriteLine($"[BattleGame] Turn {turnStartMsg.TurnNumber} start: Updated opponent slot {i}: {pokemon.Name}");
                                    }
                                    else if (_cachedOpponentActive.TryGetValue(i, out var cached))
                                    {
                                        // Keep cached Pokemon (for sprite continuity)
                                        Console.WriteLine($"[BattleGame] Turn {turnStartMsg.TurnNumber} start: Kept cached opponent slot {i}: {cached.Name} (pending faint)");
                                    }
                                }
                            }
                        }
                        else if (turnStartMsg.TurnNumber == 1)
                        {
                            // First turn: initialize cache
                            _cachedPlayerActive.Clear();
                            _cachedOpponentActive.Clear();
                            
                            for (int i = 0; i < _currentBattlePerspective.PlayerSide.Active.Count; i++)
                            {
                                var pokemon = _currentBattlePerspective.PlayerSide.Active[i];
                                if (pokemon != null)
                                {
                                    _cachedPlayerActive[i] = pokemon;
                                    Console.WriteLine($"[BattleGame] Turn 1 start: Initial cache player slot {i}: {pokemon.Name}");
                                }
                            }
                            
                            for (int i = 0; i < _currentBattlePerspective.OpponentSide.Active.Count; i++)
                            {
                                var pokemon = _currentBattlePerspective.OpponentSide.Active[i];
                                if (pokemon != null)
                                {
                                    _cachedOpponentActive[i] = pokemon;
                                    Console.WriteLine($"[BattleGame] Turn 1 start: Initial cache opponent slot {i}: {pokemon.Name}");
                                }
                            }
                        }
                    }
                    
                    // DON'T clear pending faints here!
                    // They should only be cleared when SwitchMessage arrives
                    // This ensures the fainted Pokemon remains visible until replaced
                }
                
                _animationCoordinator.ProcessMessage(message);
            }
        }
    }
    
    /// <summary>
    /// Handle a DamageMessage by updating the cached Pokemon's HP
    /// </summary>
    private void HandleDamageMessage(DamageMessage damageMsg)
    {
        // Update the cached Pokemon's HP so it reflects the damage taken
        bool isPlayer = damageMsg.SideId == SideId.P1;
        
        if (isPlayer)
        {
            foreach (var kvp in _cachedPlayerActive.ToList())
            {
                if (kvp.Value.Name == damageMsg.PokemonName)
                {
                    // Create updated Pokemon with new HP
                    var updated = kvp.Value with { Hp = damageMsg.RemainingHp };
                    _cachedPlayerActive[kvp.Key] = updated;
                    Console.WriteLine($"[BattleGame] Updated cached player {damageMsg.PokemonName} HP: {damageMsg.RemainingHp}/{damageMsg.MaxHp}");
                    break;
                }
            }
        }
        else
        {
            foreach (var kvp in _cachedOpponentActive.ToList())
            {
                if (kvp.Value.Name == damageMsg.PokemonName)
                {
                    // Create updated Pokemon with new HP
                    var updated = kvp.Value with { Hp = damageMsg.RemainingHp };
                    _cachedOpponentActive[kvp.Key] = updated;
                    Console.WriteLine($"[BattleGame] Updated cached opponent {damageMsg.PokemonName} HP: {damageMsg.RemainingHp}/{damageMsg.MaxHp}");
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Handle a FaintMessage by marking the slot as pending faint
    /// </summary>
    private void HandleFaintMessage(FaintMessage faintMsg)
    {
        if (_currentBattlePerspective == null) return;
        
        // Determine if this is a player or opponent Pokemon
        bool isPlayer = faintMsg.SideId == SideId.P1;
        
        Console.WriteLine($"[BattleGame] HandleFaintMessage: {faintMsg.PokemonName} (Side: {faintMsg.SideId})");
        
        // Find which slot the fainted Pokemon was in by checking our cache
        // (The perspective may already show the replacement Pokemon)
        if (isPlayer)
        {
            Console.WriteLine($"[BattleGame] Player cache contains {_cachedPlayerActive.Count} entries:");
            foreach (var kvp in _cachedPlayerActive)
            {
                Console.WriteLine($"  Slot {kvp.Key}: {kvp.Value.Name}");
            }
            
            for (int slot = 0; slot < _cachedPlayerActive.Count; slot++)
            {
                if (_cachedPlayerActive.TryGetValue(slot, out var cached) && 
                    cached.Name == faintMsg.PokemonName)
                {
                    _pendingFaintPlayer.Add(slot);
                    Console.WriteLine($"[BattleGame] ? Marked player slot {slot} as pending faint: {cached.Name}");
                    break;
                }
            }
        }
        else
        {
            Console.WriteLine($"[BattleGame] Opponent cache contains {_cachedOpponentActive.Count} entries:");
            foreach (var kvp in _cachedOpponentActive)
            {
                Console.WriteLine($"  Slot {kvp.Key}: {kvp.Value.Name}");
            }
            
            for (int slot = 0; slot < _cachedOpponentActive.Count; slot++)
            {
                if (_cachedOpponentActive.TryGetValue(slot, out var cached) && 
                    cached.Name == faintMsg.PokemonName)
                {
                    _pendingFaintOpponent.Add(slot);
                    Console.WriteLine($"[BattleGame] ? Marked opponent slot {slot} as pending faint: {cached.Name}");
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Handle a SwitchMessage by clearing the pending faint for that slot
    /// </summary>
    private void HandleSwitchMessage(SwitchMessage switchMsg)
    {
        if (_currentBattlePerspective == null) return;
        
        // Determine which side is switching
        // The trainer name tells us which side
        bool isPlayer = switchMsg.TrainerName == _currentBattlePerspective.PlayerSide.Pokemon[0].Name 
            || _currentBattlePerspective.PlayerSide.Pokemon.Any(p => p.Name == switchMsg.PokemonName);
        
        // Find the slot where this Pokemon appears in the new perspective
        var activeList = isPlayer 
            ? _currentBattlePerspective.PlayerSide.Active 
            : _currentBattlePerspective.OpponentSide.Active;
            
        for (int slot = 0; slot < activeList.Count; slot++)
        {
            var pokemon = activeList[slot];
            if (pokemon != null && pokemon.Name == switchMsg.PokemonName)
            {
                // Clear pending faint and cache for this slot
                if (isPlayer)
                {
                    _pendingFaintPlayer.Remove(slot);
                    _cachedPlayerActive.Remove(slot);
                    Console.WriteLine($"[BattleGame] Cleared player slot {slot} cache after switch to {pokemon.Name}");
                }
                else
                {
                    _pendingFaintOpponent.Remove(slot);
                    _cachedOpponentActive.Remove(slot);
                    Console.WriteLine($"[BattleGame] Cleared opponent slot {slot} cache after switch to {pokemon.Name}");
                }
                break;
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
            _animationCoordinator.ClearPokemonPositions();
            
            // Set which SideId is the player in this perspective
            // In GUI battles, Player 1 is always the GUI player and is always SideId.P1
            _animationCoordinator.SetPlayerSideId(SideId.P1);
            
            // Register player Pokemon (ALWAYS use cached if available to maintain position consistency)
            for (int i = 0; i < perspectiveToRender.PlayerSide.Active.Count; i++)
            {
                PokemonPerspective? pokemon;
                // Use cached Pokemon if it exists (regardless of pending faint status)
                // This ensures animations target the correct Pokemon name
                if (_cachedPlayerActive.TryGetValue(i, out var cached))
                {
                    pokemon = cached;
                }
                else
                {
                    pokemon = perspectiveToRender.PlayerSide.Active[i];
                }
                
                if (pokemon != null)
                {
                    // Use same position calculation as BattleRenderer
                    int xPosition = 20 + (i * 188); // InBattlePlayerXOffset + (i * (PokemonSpriteSize + PokemonSpacing))
                    var position = new Vector2(xPosition + 64, 400 + 64); // Center of sprite
                    _animationCoordinator.RegisterPokemonPosition(pokemon.Name, position, i, true);
                    Console.WriteLine($"[BattleGame.Draw] Registered player slot {i}: {pokemon.Name} at position {position}");
                }
            }
            
            // Register opponent Pokemon (ALWAYS use cached if available)
            for (int i = 0; i < perspectiveToRender.OpponentSide.Active.Count; i++)
            {
                PokemonPerspective? pokemon;
                // Use cached Pokemon if it exists (regardless of pending faint status)
                // This ensures animations target the correct Pokemon name
                if (_cachedOpponentActive.TryGetValue(i, out var cached))
                {
                    pokemon = cached;
                }
                else
                {
                    pokemon = perspectiveToRender.OpponentSide.Active[i];
                }
                
                if (pokemon != null)
                {
                    // Use same position calculation as BattleRenderer
                    int xPosition = 480 + (i * 188); // InBattleOpponentXOffset + (i * (PokemonSpriteSize + PokemonSpacing))
                    var position = new Vector2(xPosition + 64, 80 + 64); // Center of sprite
                    _animationCoordinator.RegisterPokemonPosition(pokemon.Name, position, i, false);
                    Console.WriteLine($"[BattleGame.Draw] Registered opponent slot {i}: {pokemon.Name} at position {position}");
                }
            }
        }

        // Pass cached Pokemon data to renderer
        _battleRenderer?.SetCachedPokemon(
            _cachedPlayerActive,
            _cachedOpponentActive,
            _pendingFaintPlayer,
            _pendingFaintOpponent);

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