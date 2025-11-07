using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ApogeeVGC.Gui.Rendering;

namespace ApogeeVGC.Gui;

/// <summary>
/// Main MonoGame window for the Pokémon battle GUI
/// </summary>
public class BattleGame : Game
{
    private SpriteBatch? _spriteBatch;
    private SpriteFont? _defaultFont;
    private BattleRenderer? _battleRenderer;
    private BattleState? _currentBattleState;
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

        // Initialize battle renderer
        _battleRenderer = new BattleRenderer(_spriteBatch, _defaultFont, GraphicsDevice);
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

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch?.Begin();

        // Thread-safe read of battle state
        BattleState? stateToRender;
        lock (_stateLock)
        {
            stateToRender = _currentBattleState;
        }

        // Render battle using the renderer
        _battleRenderer?.Render(gameTime, stateToRender);

        _spriteBatch?.End();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        // Clean up resources
        _spriteBatch?.Dispose();
        base.UnloadContent();
    }

    /// <summary>
    /// Update the battle state to be rendered (thread-safe)
    /// </summary>
    /// <param name="battleState">New battle state to display</param>
    public void UpdateBattleState(BattleState battleState)
    {
        lock (_stateLock)
        {
            _currentBattleState = battleState;
        }
    }

    /// <summary>
    /// Request the GUI window to exit gracefully
    /// </summary>
    public void RequestExit()
    {
        _shouldExit = true;
    }
}