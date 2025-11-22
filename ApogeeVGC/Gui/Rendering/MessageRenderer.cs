using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ApogeeVGC.Sim.BattleClasses;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace ApogeeVGC.Gui.Rendering;

/// <summary>
/// Handles rendering of battle messages in a scrollable log window
/// </summary>
public class MessageRenderer(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
{
    // Layout constants
    private const int Padding = 10;
    private const int LineHeight = 20;

    // Message box positioning and size
    private const int MessageBoxX = 10; // Position in left corner with small margin
    private const int MessageBoxY = 660; // Position at bottom (below menu at 650)
    private const int MessageBoxWidth = 580; // Width to fit in left half
    private const int MessageBoxHeight = 410; // Height from Y position to bottom minus margin (1080 - 660 - 10)

    // Auto-scroll tracking
    private double _lastMessageTime = 0;

    // Rendering constants
    private const float BackgroundTransparency = 0.7f; // Semi-transparent background alpha
    private const int BorderWidth = 2; // Border line width
    private const int PaddingMultiplier = 2; // Multiplier for padding in calculations
    private const int MinTruncationLength = 3; // Minimum string length before adding "..."
    private const int PixelTextureSize = 1; // Size of 1x1 pixel texture for drawing shapes

    /// <summary>
    /// Render the message log in the right half of the screen
    /// </summary>
    public void Render(GameTime gameTime, List<BattleMessage> messages)
    {
        if (messages.Count == 0) return;

        // Use fixed position in right half of screen

        var messageBox = new XnaRectangle(MessageBoxX, MessageBoxY, MessageBoxWidth, MessageBoxHeight);

        // Draw semi-transparent background
        DrawFilledRectangle(messageBox, XnaColor.Black * BackgroundTransparency);

        // Draw border
        DrawRectangleBorder(messageBox, XnaColor.White, BorderWidth);

        // Calculate how many messages can fit in the box
        const int maxLines = (MessageBoxHeight - (Padding * PaddingMultiplier)) / LineHeight;
        
        // Determine which messages to show (last maxLines messages)
        int startIndex = Math.Max(0, messages.Count - maxLines);

        // Render messages from top to bottom (oldest at top, newest at bottom)
        int yOffset = MessageBoxY + Padding;
        for (int i = startIndex; i < messages.Count; i++)
        {
            BattleMessage message = messages[i];
            string displayText = message.ToDisplayText();

            // Truncate message if it's too long
            float maxWidth = MessageBoxWidth - (Padding * PaddingMultiplier);
            if (font.MeasureString(displayText).X > maxWidth)
            {
                while (font.MeasureString(displayText + "...").X > maxWidth &&
                       displayText.Length > MinTruncationLength)
                {
                    displayText = displayText[..^1];
                }

                displayText += "...";
            }

            // Determine message color based on type
            XnaColor messageColor = GetMessageColor(message);

            // Draw the message text
            var textPosition = new XnaVector2(MessageBoxX + Padding, yOffset);
            spriteBatch.DrawString(font, displayText, textPosition, messageColor);

            yOffset += LineHeight;
        }
    }

    /// <summary>
    /// Get the display color for a message based on its type
    /// </summary>
    private static XnaColor GetMessageColor(BattleMessage message)
    {
        return message switch
        {
            MoveUsedMessage => XnaColor.White,
            EffectivenessMessage em => em.Effectiveness switch
            {
                EffectivenessMessage.EffectivenessType.SuperEffective => XnaColor.LimeGreen,
                EffectivenessMessage.EffectivenessType.NotVeryEffective => XnaColor.Orange,
                EffectivenessMessage.EffectivenessType.NoEffect => XnaColor.Gray,
                _ => XnaColor.White
            },
            DamageMessage => XnaColor.Red,
            FaintMessage => XnaColor.DarkRed,
            SwitchMessage => XnaColor.Cyan,
            CriticalHitMessage => XnaColor.Yellow,
            MissMessage => XnaColor.Gray,
            StatusMessage => XnaColor.Purple,
            WeatherMessage => XnaColor.LightBlue,
            StatChangeMessage => XnaColor.Gold,
            HealMessage => XnaColor.LightGreen,
            ItemMessage => XnaColor.Pink,
            AbilityMessage => XnaColor.Magenta,
            TurnStartMessage => XnaColor.Yellow,
            MoveFailMessage => XnaColor.DarkGray,
            GenericMessage => XnaColor.LightGray,
            _ => XnaColor.White,
        };
    }

    /// <summary>
    /// Draw a filled rectangle
    /// </summary>
    private void DrawFilledRectangle(XnaRectangle rect, XnaColor color)
    {
        var pixel = new Texture2D(graphicsDevice, PixelTextureSize, PixelTextureSize);
        pixel.SetData([XnaColor.White]);
        spriteBatch.Draw(pixel, rect, color);
        pixel.Dispose();
    }

    /// <summary>
    /// Draw a rectangle border
    /// </summary>
    private void DrawRectangleBorder(XnaRectangle rect, XnaColor color, int lineWidth)
    {
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

        pixel.Dispose();
    }
}