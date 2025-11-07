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

    private BattlePerspective? _currentBattlePerspective;
    private readonly object _stateLock = new();
    private bool _shouldExit;

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
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load default font
        _defaultFont = Content.Load<SpriteFont>("Fonts/DefaultFont");

        // Initialize sprite manager and load sprites
        _spriteManager = new SpriteManager();
        _spriteManager.LoadSprites(Content, GraphicsDevice);

        // Initialize battle renderer with sprite manager
        _battleRenderer = new BattleRenderer(_spriteBatch, _defaultFont, GraphicsDevice, _spriteManager);

        // Initialize choice input manager
        _choiceInputManager = new ChoiceInputManager(_spriteBatch, _defaultFont, GraphicsDevice);
    }

    /// <summary>
    /// Starts a battle with the given options.
    /// The battle runs on a background thread and communicates with the GUI via async mechanisms.
    /// </summary>
    public void StartBattle(Library library, BattleOptions battleOptions, IPlayerController playerController)
    {
        if (_battleRunner != null && _battleRunner.IsRunning)
        {
            Console.WriteLine("Battle is already running");
            return;
        }

        _battleRunner = new BattleRunner(library, battleOptions, playerController);
        _battleRunner.StartBattle();
    }

    protected override void Update(GameTime gameTime)
    {
        // Exit on Escape key
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

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
            Console.WriteLine($"Battle complete! Winner: {_battleRunner.Result}");
            // TODO: Show battle results screen
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
    }

    /// <summary>
    /// Request a choice from the user via the GUI.
    /// This is called from the battle simulation thread and returns a Task that completes
    /// when the user makes their choice on the GUI thread.
    /// </summary>
    public Task<Choice> RequestChoiceAsync(IChoiceRequest request, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        if (_choiceInputManager == null)
        {
            throw new InvalidOperationException("Choice input manager not initialized");
        }

        // The battle thread calls this and awaits the result
        // The GUI thread will complete the TaskCompletionSource when the user makes a choice
        return _choiceInputManager.RequestChoiceAsync(request, requestType, perspective, cancellationToken);
    }

    /// <summary>
    /// Request the GUI window to exit gracefully
    /// </summary>
    public void RequestExit()
    {
        _shouldExit = true;
    }
}