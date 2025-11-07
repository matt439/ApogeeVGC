using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Items;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace ApogeeVGC.Gui.Rendering;

/// <summary>
/// Handles rendering of the battle scene including field, Pokémon, and UI
/// </summary>
public class BattleRenderer(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice, SpriteManager spriteManager)
{
    // Layout constants
    private const int Padding = 20;
    private const int PokemonSpriteSize = 128;

    // Reference to choice input manager for team preview state
    private ChoiceUI.ChoiceInputManager? _choiceInputManager;

    /// <summary>
    /// Set the choice input manager to access team preview state
    /// </summary>
    public void SetChoiceInputManager(ChoiceUI.ChoiceInputManager choiceInputManager)
    {
        _choiceInputManager = choiceInputManager;
    }

    /// <summary>
    /// Render the entire battle scene
    /// </summary>
    public void Render(GameTime gameTime, BattlePerspective? battlePerspective)
    {
        if (battlePerspective == null)
        {
            RenderWaitingScreen();
            return;
        }

        // Route to appropriate renderer based on perspective type
        switch (battlePerspective.PerspectiveType)
        {
            case BattlePerspectiveType.TeamPreview:
                RenderTeamPreview(gameTime, battlePerspective);
                break;
            case BattlePerspectiveType.InBattle:
                RenderInBattle(gameTime, battlePerspective);
                break;
            default:
                RenderWaitingScreen();
                break;
        }
    }

    /// <summary>
    /// Render the team preview screen showing all Pokemon from both teams
    /// </summary>
    private void RenderTeamPreview(GameTime gameTime, BattlePerspective battlePerspective)
    {
        RenderField(battlePerspective);
        RenderTeamPreviewPlayerTeam(battlePerspective);
        RenderTeamPreviewOpponentTeam(battlePerspective);
        RenderTeamPreviewUi(battlePerspective);
    }

    /// <summary>
    /// Render the active battle screen showing only active Pokemon
    /// </summary>
    private void RenderInBattle(GameTime gameTime, BattlePerspective battlePerspective)
    {
        RenderField(battlePerspective);
        RenderInBattlePlayerPokemon(battlePerspective);
        RenderInBattleOpponentPokemon(battlePerspective);
        RenderInBattleUi(battlePerspective);
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

    private void RenderField(BattlePerspective battlePerspective)
    {
        // TODO: Render weather, terrain, field effects
        // For now, just show field status text
        string fieldInfo = $"Turn: {battlePerspective.TurnCounter}";
        spriteBatch.DrawString(font, fieldInfo, new XnaVector2(Padding, Padding), XnaColor.White);
    }

    /// <summary>
    /// Render player's full team during team preview
    /// </summary>
    private void RenderTeamPreviewPlayerTeam(BattlePerspective battlePerspective)
    {
        // Display all Pokemon in a horizontal row at the bottom
        int yPosition = graphicsDevice.Viewport.Height - PokemonSpriteSize - Padding - 60;
        int totalWidth = battlePerspective.PlayerSide.Pokemon.Count * (PokemonSpriteSize + Padding);
        int startX = (graphicsDevice.Viewport.Width - totalWidth) / 2;

        for (int i = 0; i < battlePerspective.PlayerSide.Pokemon.Count; i++)
        {
            var pokemon = battlePerspective.PlayerSide.Pokemon[i];
            int xPosition = startX + (i * (PokemonSpriteSize + Padding));

            // Check if this Pokemon is highlighted or locked
            bool isHighlighted = _choiceInputManager?.CurrentRequestType == BattleRequestType.TeamPreview &&
                                 _choiceInputManager?.CurrentHighlightedIndex == i;
            bool isLocked = _choiceInputManager?.LockedInIndices.Contains(i) ?? false;
            int? lockOrder = null;
            if (isLocked)
            {
                // Find the index in the locked positions list
                var lockedPositions = _choiceInputManager?.LockedInPositions;
                if (lockedPositions != null)
                {
                    for (int j = 0; j < lockedPositions.Count; j++)
                    {
                        if (lockedPositions[j] == i)
                        {
                            lockOrder = j;
                            break;
                        }
                    }
                }
            }

            RenderPlayerPokemonInfoWithState(pokemon, new XnaVector2(xPosition, yPosition),
                isHighlighted, isLocked, lockOrder);
        }

        // Draw label
        string label = "Your Team";
        XnaVector2 labelSize = font.MeasureString(label);
        XnaVector2 labelPos = new XnaVector2(
            (graphicsDevice.Viewport.Width - labelSize.X) / 2,
            yPosition - 30);
        spriteBatch.DrawString(font, label, labelPos, XnaColor.White);
    }

    /// <summary>
    /// Render opponent's full team during team preview
    /// </summary>
    private void RenderTeamPreviewOpponentTeam(BattlePerspective battlePerspective)
    {
        // Display all Pokemon in a horizontal row at the top
        const int yPosition = Padding + 90;
        int totalWidth = battlePerspective.OpponentSide.Pokemon.Count * (PokemonSpriteSize + Padding);
        int startX = (graphicsDevice.Viewport.Width - totalWidth) / 2;

        for (int i = 0; i < battlePerspective.OpponentSide.Pokemon.Count; i++)
        {
            var pokemon = battlePerspective.OpponentSide.Pokemon[i];
            int xPosition = startX + (i * (PokemonSpriteSize + Padding));
            RenderOpponentPokemonTeamPreview(pokemon, new XnaVector2(xPosition, yPosition));
        }

        // Draw label
        string label = "Opponent's Team";
        XnaVector2 labelSize = font.MeasureString(label);
        XnaVector2 labelPos = new XnaVector2(
            (graphicsDevice.Viewport.Width - labelSize.X) / 2,
            yPosition - 30);
        spriteBatch.DrawString(font, label, labelPos, XnaColor.White);
    }

    /// <summary>
    /// Render UI for team preview
    /// </summary>
    private void RenderTeamPreviewUi(BattlePerspective battlePerspective)
    {
        const string uiInfo = "Team Preview - Select your lead Pokemon";
        var uiPosition = new XnaVector2(
            graphicsDevice.Viewport.Width / 2f - 200,
            graphicsDevice.Viewport.Height / 2f);
        spriteBatch.DrawString(font, uiInfo, uiPosition, XnaColor.Yellow);
    }

    /// <summary>
    /// Render player's active Pokemon during battle
    /// </summary>
    private void RenderInBattlePlayerPokemon(BattlePerspective battlePerspective)
    {
        // Render only active Pokemon
        int yPosition = graphicsDevice.Viewport.Height - PokemonSpriteSize - Padding;

        for (int i = 0; i < battlePerspective.PlayerSide.Active.Count; i++)
        {
            var pokemon = battlePerspective.PlayerSide.Active[i];
            if (pokemon == null) continue;

            int xPosition = Padding + (i * (PokemonSpriteSize + Padding));
            RenderPlayerPokemonInfo(pokemon, new XnaVector2(xPosition, yPosition));
        }
    }

    /// <summary>
    /// Render opponent's active Pokemon during battle
    /// </summary>
    private void RenderInBattleOpponentPokemon(BattlePerspective battlePerspective)
    {
        // Render only active Pokemon
        const int yPosition = Padding + 60;

        for (int i = 0; i < battlePerspective.OpponentSide.Active.Count; i++)
        {
            var pokemon = battlePerspective.OpponentSide.Active[i];
            if (pokemon == null) continue;

            int xPosition = graphicsDevice.Viewport.Width - PokemonSpriteSize - Padding -
                            (i * (PokemonSpriteSize + Padding));

            RenderOpponentPokemonInfo(pokemon, new XnaVector2(xPosition, yPosition));
        }
    }

    /// <summary>
    /// Render UI for active battle
    /// </summary>
    private void RenderInBattleUi(BattlePerspective battlePerspective)
    {
        // TODO: Render action buttons, move selection, etc.
        // For now, show available actions
        const string uiInfo = "Press ESC to exit";
        var uiPosition = new XnaVector2(
            graphicsDevice.Viewport.Width / 2f - 100,
            graphicsDevice.Viewport.Height - 40);

        spriteBatch.DrawString(font, uiInfo, uiPosition, XnaColor.Yellow);
    }

    private void RenderPlayerPokemonInfo(PokemonPlayerPerspective pokemon, XnaVector2 position)
    {
        // Draw sprite texture (back sprite for player's Pokemon)
        Texture2D sprite = spriteManager.GetBackSprite(pokemon.Species);

        // Calculate centered position for sprite
        var spriteRect = new XnaRectangle(
            (int)position.X + (PokemonSpriteSize - sprite.Width) / 2,
            (int)position.Y + (PokemonSpriteSize - sprite.Height) / 2,
            sprite.Width,
            sprite.Height);

        spriteBatch.Draw(sprite, spriteRect, XnaColor.White);

        // Draw border around sprite area
        var borderRect = new XnaRectangle((int)position.X, (int)position.Y, PokemonSpriteSize, PokemonSpriteSize);
        DrawRectangle(borderRect, XnaColor.Blue, 2);

        // Draw name and HP
        string info = $"{pokemon.Name}\nHP: {pokemon.Hp}/{pokemon.MaxHp}";
        XnaVector2 textPosition = position + new XnaVector2(0, PokemonSpriteSize + 5);
        spriteBatch.DrawString(font, info, textPosition, XnaColor.White);
    }

    private void RenderPlayerPokemonInfoWithState(PokemonPlayerPerspective pokemon, XnaVector2 position,
        bool isHighlighted, bool isLocked, int? lockOrder)
    {
        // Draw sprite texture (back sprite for player's Pokemon)
        Texture2D sprite = spriteManager.GetBackSprite(pokemon.Species);

        // Calculate centered position for sprite
        var spriteRect = new XnaRectangle(
            (int)position.X + (PokemonSpriteSize - sprite.Width) / 2,
            (int)position.Y + (PokemonSpriteSize - sprite.Height) / 2,
            sprite.Width,
            sprite.Height);

        spriteBatch.Draw(sprite, spriteRect, XnaColor.White);

        // Determine border color and thickness based on state
        XnaColor borderColor;
        int borderThickness;

        if (isLocked)
        {
            borderColor = XnaColor.Green;
            borderThickness = 4;
        }
        else if (isHighlighted)
        {
            borderColor = XnaColor.Yellow;
            borderThickness = 4;
        }
        else
        {
            borderColor = XnaColor.Blue;
            borderThickness = 2;
        }

        // Draw border around sprite area
        var borderRect = new XnaRectangle((int)position.X, (int)position.Y, PokemonSpriteSize, PokemonSpriteSize);
        DrawRectangle(borderRect, borderColor, borderThickness);

        // Draw lock order indicator if locked
        if (isLocked && lockOrder.HasValue)
        {
            string orderText = $"#{lockOrder.Value + 1}";
            XnaVector2 orderSize = font.MeasureString(orderText);
            XnaVector2 orderPos = new XnaVector2(
                position.X + PokemonSpriteSize - orderSize.X - 5,
                position.Y + 5);

            // Draw background for order number
            var orderBg = new XnaRectangle((int)orderPos.X - 2, (int)orderPos.Y - 2,
                (int)orderSize.X + 4, (int)orderSize.Y + 4);
            var bgTexture = new Texture2D(graphicsDevice, 1, 1);
            bgTexture.SetData([XnaColor.Black]);
            spriteBatch.Draw(bgTexture, orderBg, XnaColor.Black * 0.7f);

            spriteBatch.DrawString(font, orderText, orderPos, XnaColor.White);
            bgTexture.Dispose();
        }

        // Draw name and item instead of HP during team preview
        string itemName = GetItemName(pokemon.Item);
        string info = $"{pokemon.Name}\n{itemName}";
        XnaVector2 textPosition = position + new XnaVector2(0, PokemonSpriteSize + 5);
        spriteBatch.DrawString(font, info, textPosition, XnaColor.White);
    }

    private void RenderOpponentPokemonInfo(PokemonOpponentPerspective pokemon, XnaVector2 position)
    {
        // Draw sprite texture (front sprite for opponent's Pokemon)
        Texture2D sprite = spriteManager.GetFrontSprite(pokemon.Species);

        // Calculate centered position for sprite
        var spriteRect = new XnaRectangle(
            (int)position.X + (PokemonSpriteSize - sprite.Width) / 2,
            (int)position.Y + (PokemonSpriteSize - sprite.Height) / 2,
            sprite.Width,
            sprite.Height);

        spriteBatch.Draw(sprite, spriteRect, XnaColor.White);

        // Draw border around sprite area
        var borderRect = new XnaRectangle((int)position.X, (int)position.Y, PokemonSpriteSize, PokemonSpriteSize);
        DrawRectangle(borderRect, XnaColor.Red, 2);

        // Draw name and HP percentage (for active battle)
        string info = $"{pokemon.Name}\nHP: {pokemon.HpPercentage:P0}";
        XnaVector2 textPosition = position + new XnaVector2(0, PokemonSpriteSize + 5);
        spriteBatch.DrawString(font, info, textPosition, XnaColor.White);
    }

    private void RenderOpponentPokemonTeamPreview(PokemonOpponentPerspective pokemon, XnaVector2 position)
    {
        // Draw sprite texture (front sprite for opponent's Pokemon)
        Texture2D sprite = spriteManager.GetFrontSprite(pokemon.Species);

        // Calculate centered position for sprite
        var spriteRect = new XnaRectangle(
      (int)position.X + (PokemonSpriteSize - sprite.Width) / 2,
        (int)position.Y + (PokemonSpriteSize - sprite.Height) / 2,
        sprite.Width,
            sprite.Height);

        spriteBatch.Draw(sprite, spriteRect, XnaColor.White);

 // Draw border around sprite area
        var borderRect = new XnaRectangle((int)position.X, (int)position.Y, PokemonSpriteSize, PokemonSpriteSize);
        DrawRectangle(borderRect, XnaColor.Red, 2);

        // During team preview, show item name (revealed items are shown in team preview)
        string itemInfo = pokemon.RevealedItem.HasValue ? GetItemName(pokemon.RevealedItem.Value) : "(No Item)";
        string info = $"{pokemon.Name}\n{itemInfo}";
        XnaVector2 textPosition = position + new XnaVector2(0, PokemonSpriteSize + 5);
spriteBatch.DrawString(font, info, textPosition, XnaColor.White);
    }

    /// <summary>
    /// Convert ItemId to display name
    /// </summary>
    private string GetItemName(ItemId itemId)
    {
        // Convert enum to readable name
        // For now, use the enum name directly and format it
        if (itemId == ItemId.None)
        {
            return "(No Item)";
        }

        // Convert PascalCase to space-separated words
        string name = itemId.ToString();
        return System.Text.RegularExpressions.Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
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
