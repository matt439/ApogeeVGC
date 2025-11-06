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

        // Create demo battle state
        InitializeDemoBattle();

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

    private void InitializeDemoBattle()
    {
        // Create a simple demo battle for testing
        _currentBattleState = new BattleState
        {
            Turn = 1,
            PlayerActivePokemon =
            [
                new PokemonDisplayInfo
                {
                    Name = "Pikachu", Species = "Pikachu", CurrentHp = 95, MaxHp = 95, Level = 50,
                },

                new PokemonDisplayInfo
                {
                    Name = "Charizard", Species = "Charizard", CurrentHp = 153, MaxHp = 153,
                    Level = 50,
                },
            ],
            OpponentActivePokemon =
            [
                new PokemonDisplayInfo
                {
                    Name = "Blastoise", Species = "Blastoise", CurrentHp = 158, MaxHp = 158,
                    Level = 50,
                },

                new PokemonDisplayInfo
                {
                    Name = "Venusaur", Species = "Venusaur", CurrentHp = 155, MaxHp = 155,
                    Level = 50,
                },
            ],
        };
    }

    protected override void Update(GameTime gameTime)
    {
        // Exit on Escape key
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // TODO: Update battle state here
        // Future: Handle input for move selection, switching, etc.

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch?.Begin();

        // Render battle using the renderer
        _battleRenderer?.Render(gameTime, _currentBattleState);

        _spriteBatch?.End();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        // Clean up resources
        _spriteBatch?.Dispose();
        base.UnloadContent();
    }
}