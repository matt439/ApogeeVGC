using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace ApogeeVGC.Gui.Rendering;

/// <summary>
/// Handles rendering of the battle scene including field, Pokémon, and UI
/// </summary>
public class BattleRenderer(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
{
    // Layout constants
    private const int Padding = 20;
    private const int PokemonSpriteSize = 128;

    /// <summary>
    /// Render the entire battle scene
    /// </summary>
    public void Render(GameTime gameTime, BattleState? battleState)
    {
        if (battleState == null)
        {
            RenderWaitingScreen();
            return;
        }

        RenderField(battleState);
        RenderPlayerPokemon(battleState);
        RenderOpponentPokemon(battleState);
        RenderUi(battleState);
    }

    private void RenderWaitingScreen()
    {
        var screenCenter = new XnaVector2(
            graphicsDevice.Viewport.Width / 2f,
            graphicsDevice.Viewport.Height / 2f);

        const string message = "Waiting for battle to start...";
        XnaVector2 messageSize = font.MeasureString(message);
        XnaVector2 position = screenCenter - messageSize / 2f;

        spriteBatch.DrawString(font, message, position, XnaColor.White);
    }

    private void RenderField(BattleState battleState)
    {
        // TODO: Render weather, terrain, field effects
        // For now, just show field status text
        string fieldInfo = $"Turn: {battleState.Turn}";
        spriteBatch.DrawString(font, fieldInfo, new XnaVector2(Padding, Padding), XnaColor.White);
    }

    private void RenderPlayerPokemon(BattleState battleState)
    {
        // TODO: Render player's active Pokémon sprites and info
        // Position in bottom-left area
        int yPosition = graphicsDevice.Viewport.Height - PokemonSpriteSize - Padding;

        for (int i = 0; i < battleState.PlayerActivePokemon.Count; i++)
        {
            PokemonDisplayInfo pokemon = battleState.PlayerActivePokemon[i];
            int xPosition = Padding + (i * (PokemonSpriteSize + Padding));

            RenderPokemonInfo(pokemon, new XnaVector2(xPosition, yPosition), true);
        }
    }

    private void RenderOpponentPokemon(BattleState battleState)
    {
        // TODO: Render opponent's active Pokémon sprites and info
        // Position in top-right area
        const int yPosition = Padding + 60;

        for (int i = 0; i < battleState.OpponentActivePokemon.Count; i++)
        {
            PokemonDisplayInfo pokemon = battleState.OpponentActivePokemon[i];
            int xPosition = graphicsDevice.Viewport.Width - PokemonSpriteSize - Padding -
                            (i * (PokemonSpriteSize + Padding));

            RenderPokemonInfo(pokemon, new XnaVector2(xPosition, yPosition), false);
        }
    }

    private void RenderPokemonInfo(PokemonDisplayInfo pokemon, XnaVector2 position, bool isPlayer)
    {
        // TODO: Draw actual sprite texture
        // For now, draw a placeholder rectangle and text
        var rect = new XnaRectangle((int)position.X, (int)position.Y, PokemonSpriteSize,
            PokemonSpriteSize);
        DrawRectangle(rect, isPlayer ? XnaColor.Blue : XnaColor.Red, 2);

        // Draw name and HP
        string info = $"{pokemon.Name}\nHP: {pokemon.CurrentHp}/{pokemon.MaxHp}";
        XnaVector2 textPosition = position + new XnaVector2(0, PokemonSpriteSize + 5);
        spriteBatch.DrawString(font, info, textPosition, XnaColor.White);
    }

    private void RenderUi(BattleState battleState)
    {
        // TODO: Render action buttons, move selection, etc.
        // For now, show available actions
        const string uiInfo = "Press ESC to exit";
        var uiPosition = new XnaVector2(
            graphicsDevice.Viewport.Width / 2f - 100,
            graphicsDevice.Viewport.Height - 40);

        spriteBatch.DrawString(font, uiInfo, uiPosition, XnaColor.Yellow);
    }

    /// <summary>
    /// Helper to draw a rectangle outline
    /// </summary>
    private void DrawRectangle(XnaRectangle rect, XnaColor color, int lineWidth)
    {
        // Create a 1x1 white texture if needed (lazy init in real implementation)
        var pixel = new Texture2D(graphicsDevice, 1, 1);
        pixel.SetData([XnaColor.White]);

        // Draw four lines
        spriteBatch.Draw(pixel, new XnaRectangle(rect.X, rect.Y, rect.Width, lineWidth),
            color); // Top
        spriteBatch.Draw(pixel,
            new XnaRectangle(rect.X, rect.Bottom - lineWidth, rect.Width, lineWidth), color); // Bottom
        spriteBatch.Draw(pixel, new XnaRectangle(rect.X, rect.Y, lineWidth, rect.Height),
            color); // Left
        spriteBatch.Draw(pixel,
            new XnaRectangle(rect.Right - lineWidth, rect.Y, lineWidth, rect.Height), color); // Right
    }
}

/// <summary>
/// Simplified battle state for rendering
/// </summary>
public class BattleState
{
    public int Turn { get; set; }
    public List<PokemonDisplayInfo> PlayerActivePokemon { get; set; } = [];
    public List<PokemonDisplayInfo> OpponentActivePokemon { get; set; } = [];
}

/// <summary>
/// Display information for a single Pokémon
/// </summary>
public class PokemonDisplayInfo
{
    public string Name { get; set; } = "";
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public string Species { get; set; } = "";
    public int Level { get; set; } = 50;
}