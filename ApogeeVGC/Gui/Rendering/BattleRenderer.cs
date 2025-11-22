using ApogeeVGC.Gui.ChoiceUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Conditions;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace ApogeeVGC.Gui.Rendering;

/// <summary>
/// Handles rendering of the battle scene including field, Pokémon, and UI
/// </summary>
public class BattleRenderer(
    SpriteBatch spriteBatch,
    SpriteFont font,
    GraphicsDevice graphicsDevice,
    SpriteManager spriteManager)
{
    // ===== LAYOUT CONSTANTS =====

    // General spacing and padding
    private const int Padding = 10;
    private const int PokemonSpriteSize = 128;
    private const int InfoTextHeight = 75; // Height reserved for Pokemon info text below sprite
    private const int PokemonSpacing = 60; // Horizontal spacing between Pokemon sprites (increased to prevent clipping)

    // Team Preview layout
    private const int TeamPreviewOpponentYOffset = 60; // Offset from top padding for opponent team
    private const int TeamPreviewLabelYOffset = 30; // Distance of label above Pokemon sprites

    // In-Battle layout - all Pokemon in left half only
    private const int InBattleOpponentXOffset = 480; // X position from left edge for opponent Pokemon (midscreen in left half)
    private const int InBattleOpponentYOffset = 80; // Y position from top for opponent Pokemon (below timers)
    private const int InBattlePlayerXOffset = 20; // X position from left edge for player Pokemon (at screen edge)
    private const int InBattlePlayerYOffset = 400; // Y position from top for player Pokemon (moved down to avoid overlap)

    // Timer display layout - position in upper left corner
    private const int TimerXPosition = 10; // X position in upper left corner
    private const int TimerYPosition = 10; // Y position from top
    private const int TimerLineHeight = 20; // Vertical spacing between timer lines

    // Pokemon info text layout
    private const int InfoTextYOffset = 5; // Distance below sprite to start text

    // Lock order indicator
    private const int LockOrderXPadding = 5; // Padding from right edge of sprite
    private const int LockOrderYPadding = 5; // Padding from top edge of sprite
    private const int LockOrderBackgroundPadding = 2; // Padding around lock order text background

    // Border styling
    private const int BorderThicknessNormal = 2;
    private const int BorderThicknessHighlighted = 4;

    // HP color thresholds
    private const double HpColorThresholdGreen = 0.5; // Above 50% HP shows green
    private const double HpColorThresholdYellow = 0.2; // Above 20% HP shows yellow, below shows red

    // Lock order background transparency
    private const float LockOrderBackgroundAlpha = 0.7f;

    // HP bar layout
    private const int HpBarWidth = 100; // Width of HP bar
    private const int HpBarHeight = 8; // Height of HP bar
    private const int HpBarBorderThickness = 1; // Border thickness around HP bar
    private const int HpBarYSpacing = 2; // Vertical spacing between HP text and HP bar

    // Text and rendering constants
    private const int PixelTextureSize = 1; // Size of 1x1 pixel texture for drawing shapes
    private const int StatusLineSpacingMultiplier = 3; // Multiplier for font.LineSpacing to position status text
    private const float CenteringDivisor = 2f; // Divisor for centering calculations
    private const int MinTruncationLength = 3; // Minimum string length before adding "..."

    // Reference to choice input manager for team preview state
    private ChoiceInputManager? _choiceInputManager;

    // Cached pixel texture for drawing filled rectangles (HP bars)
    private Texture2D? _pixelTexture;

    // Track last perspective type to avoid spam logging
    private BattlePerspectiveType? _lastPerspectiveType;

    /// <summary>
    /// Set the choice input manager to access team preview state
    /// </summary>
    public void SetChoiceInputManager(ChoiceInputManager choiceInputManager)
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

        // Debug output only when perspective type changes
        if (_lastPerspectiveType != battlePerspective.PerspectiveType)
        {
            Console.WriteLine(
                $"[BattleRenderer] Perspective changed to: {battlePerspective.PerspectiveType}, " +
                $"Player Active: {battlePerspective.PlayerSide.Active.Count}, " +
                $"Opponent Active: {battlePerspective.OpponentSide.Active.Count}");
            _lastPerspectiveType = battlePerspective.PerspectiveType;
        }

        // Route to appropriate renderer based on perspective type
        switch (battlePerspective.PerspectiveType)
        {
            case BattlePerspectiveType.TeamPreview:
                RenderTeamPreview(battlePerspective);
                break;
            case BattlePerspectiveType.InBattle:
                RenderInBattle(battlePerspective);
                break;
            default:
                RenderWaitingScreen();
                break;
        }
    }

    /// <summary>
    /// Render the team preview screen showing all Pokemon from both teams
    /// </summary>
    private void RenderTeamPreview(BattlePerspective battlePerspective)
    {
        RenderField();
        RenderTeamPreviewPlayerTeam(battlePerspective);
        RenderTeamPreviewOpponentTeam(battlePerspective);
        RenderTeamPreviewUi();
    }

    /// <summary>
    /// Render the active battle screen showing only active Pokemon
    /// </summary>
    private void RenderInBattle(BattlePerspective battlePerspective)
    {
        RenderField();
        RenderInBattlePlayerPokemon(battlePerspective);
        RenderInBattleOpponentPokemon(battlePerspective);
        RenderInBattleUi();
    }

    private void RenderWaitingScreen()
    {
        var screenCenter = new XnaVector2(
            graphicsDevice.Viewport.Width / CenteringDivisor,
            graphicsDevice.Viewport.Height / CenteringDivisor);

        const string message = "Waiting for battle to start...";
        XnaVector2 messageSize = font.MeasureString(message);
        XnaVector2 position = screenCenter - messageSize / 2f;

        spriteBatch.DrawString(font, message, position, XnaColor.White);
    }

    private void RenderField()
    {
        // TODO: Render weather, terrain, field effects
        // Turn counter is now displayed in message log, so no need to render here
        // Field info can be shown in a better position later if needed
    }

    /// <summary>
    /// Render player's full team during team preview
    /// </summary>
    private void RenderTeamPreviewPlayerTeam(BattlePerspective battlePerspective)
    {
        // Display all Pokemon in a horizontal row at the bottom
        int yPosition = graphicsDevice.Viewport.Height - PokemonSpriteSize - Padding -
                        InfoTextHeight;
        int totalWidth = battlePerspective.PlayerSide.Pokemon.Count *
                         (PokemonSpriteSize + PokemonSpacing);
        int startX = (int)((graphicsDevice.Viewport.Width - totalWidth) / CenteringDivisor);

        for (int i = 0; i < battlePerspective.PlayerSide.Pokemon.Count; i++)
        {
            PokemonPerspective pokemon = battlePerspective.PlayerSide.Pokemon[i];
            int xPosition = startX + (i * (PokemonSpriteSize + PokemonSpacing));

            // Check if this Pokemon is highlighted or locked
            bool isHighlighted =
                _choiceInputManager?.CurrentRequestType == BattleRequestType.TeamPreview &&
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
            (graphicsDevice.Viewport.Width - labelSize.X) / CenteringDivisor,
            yPosition - TeamPreviewLabelYOffset);
        spriteBatch.DrawString(font, label, labelPos, XnaColor.White);
    }

    /// <summary>
    /// Render opponent's full team during team preview
    /// </summary>
    private void RenderTeamPreviewOpponentTeam(BattlePerspective battlePerspective)
    {
        // Display all Pokemon in a horizontal row at the top
        int yPosition = Padding + TeamPreviewOpponentYOffset;
        int totalWidth = battlePerspective.OpponentSide.Pokemon.Count *
                         (PokemonSpriteSize + PokemonSpacing);
        int startX = (int)((graphicsDevice.Viewport.Width - totalWidth) / CenteringDivisor);

        for (int i = 0; i < battlePerspective.OpponentSide.Pokemon.Count; i++)
        {
            PokemonPerspective pokemon = battlePerspective.OpponentSide.Pokemon[i];
            int xPosition = startX + (i * (PokemonSpriteSize + PokemonSpacing));
            RenderOpponentPokemonTeamPreview(pokemon, new XnaVector2(xPosition, yPosition));
        }

        // Draw label
        const string label = "Opponent's Team";
        XnaVector2 labelSize = font.MeasureString(label);
        var labelPos = new XnaVector2(
            (graphicsDevice.Viewport.Width - labelSize.X) / CenteringDivisor,
            yPosition - TeamPreviewLabelYOffset);
        spriteBatch.DrawString(font, label, labelPos, XnaColor.White);
    }

    /// <summary>
    /// Render UI for team preview
    /// </summary>
    private void RenderTeamPreviewUi()
    {
        // Render timers
        RenderTimers();

        const string uiInfo = "Team Preview - Select your lead Pokemon";
        XnaVector2 uiSize = font.MeasureString(uiInfo);
        var uiPosition = new XnaVector2(
            (graphicsDevice.Viewport.Width - uiSize.X) / CenteringDivisor,
            Padding);
        spriteBatch.DrawString(font, uiInfo, uiPosition, XnaColor.Yellow);
    }

    /// <summary>
    /// Render player's active Pokemon during battle
    /// </summary>
    private void RenderInBattlePlayerPokemon(BattlePerspective battlePerspective)
    {
        // Render only active Pokemon
        // Position them in left half, offset to the right of opponent Pokemon

        for (int i = 0; i < battlePerspective.PlayerSide.Active.Count; i++)
        {
            PokemonPerspective? pokemon = battlePerspective.PlayerSide.Active[i];
            if (pokemon == null) continue;

            int xPosition = InBattlePlayerXOffset + (i * (PokemonSpriteSize + PokemonSpacing));
            RenderPlayerPokemonInfo(pokemon, new XnaVector2(xPosition, InBattlePlayerYOffset));
        }
    }

    /// <summary>
    /// Render opponent's active Pokemon during battle
    /// </summary>
    private void RenderInBattleOpponentPokemon(BattlePerspective battlePerspective)
    {
        // Render only active Pokemon
        // Position them in left half at the top

        for (int i = 0; i < battlePerspective.OpponentSide.Active.Count; i++)
        {
            PokemonPerspective? pokemon = battlePerspective.OpponentSide.Active[i];
            if (pokemon == null) continue;

            int xPosition = InBattleOpponentXOffset + (i * (PokemonSpriteSize + PokemonSpacing));

            RenderOpponentPokemonInfo(pokemon, new XnaVector2(xPosition, InBattleOpponentYOffset));
        }
    }

    /// <summary>
    /// Render UI for active battle
    /// </summary>
    private void RenderInBattleUi()
    {
        // Render timers
        RenderTimers();

        // UI rendering is now handled by ChoiceInputManager
        // No need for legacy "Press ESC to exit" text
    }

    /// <summary>
    /// Render the three timers: Battle Time, Your Time, Move Time
    /// </summary>
    private void RenderTimers()
    {
        if (_choiceInputManager == null) return;

        TimerManager timerManager = _choiceInputManager.TimerManager;

        // Timer display position (top right of left half)
        int timerY = TimerYPosition;

        // Battle Timer
        string battleTimeText = $"Battle: {timerManager.GetBattleTimeString()}";
        spriteBatch.DrawString(font, battleTimeText, new XnaVector2(TimerXPosition, timerY),
            XnaColor.White);
        timerY += TimerLineHeight;

        // Player Timer (Your Time)
        string playerTimeText = $"Your Time: {timerManager.GetPlayerTimeString()}";
        spriteBatch.DrawString(font, playerTimeText, new XnaVector2(TimerXPosition, timerY),
            XnaColor.Yellow);
        timerY += TimerLineHeight;

        // Move Timer
        string moveTimeText = $"Move Time: {timerManager.GetMoveTimeString()}";
        spriteBatch.DrawString(font, moveTimeText, new XnaVector2(TimerXPosition, timerY), XnaColor.Lime);
    }

    private void RenderPlayerPokemonInfo(PokemonPerspective pokemon, XnaVector2 position)
    {
        // Draw sprite texture (back sprite for player's Pokemon)
        Texture2D sprite = spriteManager.GetBackSprite(pokemon.Species);

        // Calculate centered position for sprite
        var spriteRect = new XnaRectangle(
            (int)position.X + (int)((PokemonSpriteSize - sprite.Width) / CenteringDivisor),
            (int)position.Y + (int)((PokemonSpriteSize - sprite.Height) / CenteringDivisor),
            sprite.Width,
            sprite.Height);

        spriteBatch.Draw(sprite, spriteRect, XnaColor.White);

        // Draw border around sprite area
        var borderRect = new XnaRectangle((int)position.X, (int)position.Y, PokemonSpriteSize,
            PokemonSpriteSize);
        DrawRectangle(borderRect, XnaColor.Blue, BorderThicknessNormal);

        // Draw name
        XnaVector2 textPosition = position + new XnaVector2(0, PokemonSpriteSize + InfoTextYOffset);
        spriteBatch.DrawString(font, pokemon.Name, textPosition, XnaColor.White);

        // Draw HP text
        string hpText = $"HP: {pokemon.Hp}/{pokemon.MaxHp}";
        XnaVector2 hpTextPosition = textPosition + new XnaVector2(0, font.LineSpacing);
        spriteBatch.DrawString(font, hpText, hpTextPosition, XnaColor.White);

        // Draw HP bar
        XnaVector2 hpBarPosition = hpTextPosition + new XnaVector2(0, font.LineSpacing + HpBarYSpacing);
        DrawHpBar(hpBarPosition, pokemon.Hp, pokemon.MaxHp);

        // Draw status condition if present
        (string statusName, XnaColor statusColor) = GetStatusDisplay(pokemon.Status);
        if (!string.IsNullOrEmpty(statusName))
        {
            XnaVector2 statusPosition = hpBarPosition + new XnaVector2(0, HpBarHeight + HpBarYSpacing);
            spriteBatch.DrawString(font, statusName, statusPosition, statusColor);
        }
    }

    private void RenderPlayerPokemonInfoWithState(PokemonPerspective pokemon,
        XnaVector2 position,
        bool isHighlighted, bool isLocked, int? lockOrder)
    {
        // Draw sprite texture (back sprite for player's Pokemon)
        Texture2D sprite = spriteManager.GetBackSprite(pokemon.Species);

        // Calculate centered position for sprite
        var spriteRect = new XnaRectangle(
            (int)position.X + (int)((PokemonSpriteSize - sprite.Width) / CenteringDivisor),
            (int)position.Y + (int)((PokemonSpriteSize - sprite.Height) / CenteringDivisor),
            sprite.Width,
            sprite.Height);

        spriteBatch.Draw(sprite, spriteRect, XnaColor.White);

        // Determine border color and thickness based on state
        XnaColor borderColor;
        int borderThickness;

        if (isLocked)
        {
            borderColor = XnaColor.Green;
            borderThickness = BorderThicknessHighlighted;
        }
        else if (isHighlighted)
        {
            borderColor = XnaColor.Yellow;
            borderThickness = BorderThicknessHighlighted;
        }
        else
        {
            borderColor = XnaColor.Blue;
            borderThickness = BorderThicknessNormal;
        }

        // Draw border around sprite area
        var borderRect = new XnaRectangle((int)position.X, (int)position.Y, PokemonSpriteSize,
            PokemonSpriteSize);
        DrawRectangle(borderRect, borderColor, borderThickness);

        // Draw lock order indicator if locked
        if (isLocked && lockOrder.HasValue)
        {
            string orderText = $"#{lockOrder.Value + 1}";
            XnaVector2 orderSize = font.MeasureString(orderText);
            XnaVector2 orderPos = new XnaVector2(
                position.X + PokemonSpriteSize - orderSize.X - LockOrderXPadding,
                position.Y + LockOrderYPadding);

            // Draw background for order number
            var orderBg = new XnaRectangle(
                (int)orderPos.X - LockOrderBackgroundPadding,
                (int)orderPos.Y - LockOrderBackgroundPadding,
                (int)orderSize.X + (LockOrderBackgroundPadding * 2),
                (int)orderSize.Y + (LockOrderBackgroundPadding * 2));
            var bgTexture = new Texture2D(graphicsDevice, 1, 1);
            bgTexture.SetData([XnaColor.Black]);
            spriteBatch.Draw(bgTexture, orderBg, XnaColor.Black * LockOrderBackgroundAlpha);

            spriteBatch.DrawString(font, orderText, orderPos, XnaColor.White);
            bgTexture.Dispose();
        }

        // Build info string with name, gender, level
        string genderSymbol = GetGenderSymbol(pokemon.Gender);
        string line1 = $"{pokemon.Name}{genderSymbol}";
        string line2 = $"Lv{pokemon.Level} HP:{pokemon.MaxHp}";
        string line3 = GetItemName(pokemon.Item);

        // Ensure text fits within the Pokemon sprite area
        float maxWidth = PokemonSpriteSize;
        if (font.MeasureString(line1).X > maxWidth)
        {
            // Truncate name if too long
            while (font.MeasureString(line1 + "...").X > maxWidth && line1.Length > MinTruncationLength)
            {
                line1 = line1.Substring(0, line1.Length - 1);
            }

            line1 += "...";
        }

        if (font.MeasureString(line3).X > maxWidth)
        {
            // Truncate item name if too long
            while (font.MeasureString(line3 + "...").X > maxWidth && line3.Length > MinTruncationLength)
            {
                line3 = line3.Substring(0, line3.Length - 1);
            }

            line3 += "...";
        }

        // Draw info text
        string info = $"{line1}\n{line2}\n{line3}";
        XnaVector2 textPosition = position + new XnaVector2(0, PokemonSpriteSize + InfoTextYOffset);
        spriteBatch.DrawString(font, info, textPosition, XnaColor.White);

        // Draw HP bar (not shown during team preview)
        // Team preview shows max HP only, so bar would always be full

        // Add status condition if present
        (string statusName, XnaColor statusColor) = GetStatusDisplay(pokemon.Status);
        if (!string.IsNullOrEmpty(statusName))
        {
            XnaVector2 statusPosition = textPosition + new XnaVector2(0, font.LineSpacing * StatusLineSpacingMultiplier);
            spriteBatch.DrawString(font, statusName, statusPosition, statusColor);
        }
    }

    private void RenderOpponentPokemonInfo(PokemonPerspective pokemon, XnaVector2 position)
    {
        // Draw sprite texture (front sprite for opponent's Pokemon)
        Texture2D sprite = spriteManager.GetFrontSprite(pokemon.Species);

        // Calculate centered position for sprite
        var spriteRect = new XnaRectangle(
            (int)position.X + (int)((PokemonSpriteSize - sprite.Width) / CenteringDivisor),
            (int)position.Y + (int)((PokemonSpriteSize - sprite.Height) / CenteringDivisor),
            sprite.Width,
            sprite.Height);

        spriteBatch.Draw(sprite, spriteRect, XnaColor.White);

        // Draw border around sprite area
        var borderRect = new XnaRectangle((int)position.X, (int)position.Y, PokemonSpriteSize,
            PokemonSpriteSize);
        DrawRectangle(borderRect, XnaColor.Red, BorderThicknessNormal);

        // Draw name
        string nameText = pokemon.Name;
        XnaVector2 textPosition = position + new XnaVector2(0, PokemonSpriteSize + InfoTextYOffset);
        spriteBatch.DrawString(font, nameText, textPosition, XnaColor.White);

        // Draw exact HP values (full observability)
        string hpText = $"HP: {pokemon.Hp}/{pokemon.MaxHp}";
        XnaVector2 hpPosition = textPosition + new XnaVector2(0, font.LineSpacing);

        // Determine HP text color based on percentage
        double hpPercentage = (double)pokemon.Hp / pokemon.MaxHp;
        XnaColor hpColor = hpPercentage switch
        {
            > HpColorThresholdGreen => XnaColor.LimeGreen,
            > HpColorThresholdYellow => XnaColor.Yellow,
            _ => XnaColor.Red
        };

        spriteBatch.DrawString(font, hpText, hpPosition, hpColor);

        // Draw HP bar
        XnaVector2 hpBarPosition = hpPosition + new XnaVector2(0, font.LineSpacing + HpBarYSpacing);
        DrawHpBar(hpBarPosition, pokemon.Hp, pokemon.MaxHp);

        // Draw status condition if present
        (string statusName, XnaColor statusColor) = GetStatusDisplay(pokemon.Status);
        if (!string.IsNullOrEmpty(statusName))
        {
            XnaVector2 statusPosition = hpBarPosition + new XnaVector2(0, HpBarHeight + HpBarYSpacing);
            spriteBatch.DrawString(font, statusName, statusPosition, statusColor);
        }
    }

    private void RenderOpponentPokemonTeamPreview(PokemonPerspective pokemon,
        XnaVector2 position)
    {
        // Draw sprite texture (front sprite for opponent's Pokemon)
        Texture2D sprite = spriteManager.GetFrontSprite(pokemon.Species);

        // Calculate centered position for sprite
        var spriteRect = new XnaRectangle(
            (int)position.X + (int)((PokemonSpriteSize - sprite.Width) / CenteringDivisor),
            (int)position.Y + (int)((PokemonSpriteSize - sprite.Height) / CenteringDivisor),
            sprite.Width,
            sprite.Height);

        spriteBatch.Draw(sprite, spriteRect, XnaColor.White);

        // Draw border around sprite area
        var borderRect = new XnaRectangle((int)position.X, (int)position.Y, PokemonSpriteSize,
            PokemonSpriteSize);
        DrawRectangle(borderRect, XnaColor.Red, BorderThicknessNormal);

        // During team preview, show name, gender, level, and item (full observability)
        string genderSymbol = GetGenderSymbol(pokemon.Gender);
        string line1 = $"{pokemon.Name}{genderSymbol}";
        string line2 = $"Lv{pokemon.Level}";
        string line3 = GetItemName(pokemon.Item);

        // Ensure text fits within the Pokemon sprite area
        float maxWidth = PokemonSpriteSize;
        if (font.MeasureString(line1).X > maxWidth)
        {
            // Truncate name if too long
            while (font.MeasureString(line1 + "...").X > maxWidth && line1.Length > MinTruncationLength)
            {
                line1 = line1.Substring(0, line1.Length - 1);
            }

            line1 += "...";
        }

        if (font.MeasureString(line3).X > maxWidth)
        {
            // Truncate item name if too long
            while (font.MeasureString(line3 + "...").X > maxWidth && line3.Length > MinTruncationLength)
            {
                line3 = line3.Substring(0, line3.Length - 1);
            }

            line3 += "...";
        }

        // Draw info text
        string info = $"{line1}\n{line2}\n{line3}";
        XnaVector2 textPosition = position + new XnaVector2(0, PokemonSpriteSize + InfoTextYOffset);
        spriteBatch.DrawString(font, info, textPosition, XnaColor.White);

        // Draw HP bar (not shown during team preview)
        // Team preview shows max HP only in the info, so bar would always be full

        // Add status condition if present
        (string statusName, XnaColor statusColor) = GetStatusDisplay(pokemon.Status);
        if (!string.IsNullOrEmpty(statusName))
        {
            XnaVector2 statusPosition = textPosition + new XnaVector2(0, font.LineSpacing * StatusLineSpacingMultiplier);
            spriteBatch.DrawString(font, statusName, statusPosition, statusColor);
        }
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
    /// Get gender symbol for display
    /// </summary>
    private string GetGenderSymbol(GenderId gender)
    {
        return gender switch
        {
            GenderId.M => " (M)",
            GenderId.F => " (F)",
            _ => ""
        };
    }

    /// <summary>
    /// Get status condition display name and color
    /// </summary>
    private (string DisplayName, XnaColor Color) GetStatusDisplay(ConditionId status)
    {
        return status switch
        {
            ConditionId.Burn => ("BRN", XnaColor.Orange),
            ConditionId.Paralysis => ("PAR", XnaColor.Yellow),
            ConditionId.Sleep => ("SLP", XnaColor.Purple),
            ConditionId.Freeze => ("FRZ", XnaColor.Cyan),
            ConditionId.Poison => ("PSN", XnaColor.Magenta),
            ConditionId.Toxic => ("TOX", XnaColor.DarkMagenta),
            _ => ("", XnaColor.White)
        };
    }

    /// <summary>
    /// Helper to draw a rectangle outline
    /// </summary>
    private void DrawRectangle(XnaRectangle rect, XnaColor color, int lineWidth)
    {
        // Create a 1x1 white texture if needed (lazy init in real implementation)
        var pixel = new Texture2D(graphicsDevice, PixelTextureSize, PixelTextureSize);
        pixel.SetData([XnaColor.White]);

        // Draw four lines
        spriteBatch.Draw(pixel, new XnaRectangle(rect.X, rect.Y, rect.Width, lineWidth),
            color); // Top
        spriteBatch.Draw(pixel,
            new XnaRectangle(rect.X, rect.Bottom - lineWidth, rect.Width, lineWidth),
            color); // Bottom
        spriteBatch.Draw(pixel, new XnaRectangle(rect.X, rect.Y, lineWidth, rect.Height),
            color); // Left
        spriteBatch.Draw(pixel,
            new XnaRectangle(rect.Right - lineWidth, rect.Y, lineWidth, rect.Height),
            color); // Right
    }

    /// <summary>
    /// Helper to draw a filled rectangle
    /// </summary>
    private void DrawFilledRectangle(XnaRectangle rect, XnaColor color)
    {
        // Lazy init pixel texture
        if (_pixelTexture == null)
        {
            _pixelTexture = new Texture2D(graphicsDevice, PixelTextureSize, PixelTextureSize);
            _pixelTexture.SetData([XnaColor.White]);
        }

        spriteBatch.Draw(_pixelTexture, rect, color);
    }

    /// <summary>
    /// Draw an HP bar for a Pokemon
    /// </summary>
    /// <param name="position">Top-left position for the HP bar</param>
    /// <param name="currentHp">Current HP value</param>
    /// <param name="maxHp">Maximum HP value</param>
    private void DrawHpBar(XnaVector2 position, int currentHp, int maxHp)
    {
        // Calculate HP percentage
        double hpPercentage = maxHp > 0 ? (double)currentHp / maxHp : 0.0;
        
        // Determine HP bar color based on percentage
        XnaColor hpBarColor = hpPercentage switch
        {
            > HpColorThresholdGreen => XnaColor.LimeGreen,
            > HpColorThresholdYellow => XnaColor.Yellow,
            _ => XnaColor.Red
        };

        // Draw HP bar background (dark gray)
        var backgroundRect = new XnaRectangle(
            (int)position.X,
            (int)position.Y,
            HpBarWidth,
            HpBarHeight);
        DrawFilledRectangle(backgroundRect, XnaColor.DarkGray);

        // Draw HP bar fill
        int fillWidth = (int)(HpBarWidth * hpPercentage);
        if (fillWidth > 0)
        {
            var fillRect = new XnaRectangle(
                (int)position.X,
                (int)position.Y,
                fillWidth,
                HpBarHeight);
            DrawFilledRectangle(fillRect, hpBarColor);
        }

        // Draw HP bar border
        DrawRectangle(backgroundRect, XnaColor.Black, HpBarBorderThickness);
    }
}