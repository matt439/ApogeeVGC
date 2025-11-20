using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Represents a clickable button in the choice UI
/// </summary>
public class ChoiceButton(Rectangle bounds, string text, Color backgroundColor, Action onClick, Color? textColor = null, string? secondaryText = null, Color? secondaryTextColor = null)
{
    public Rectangle Bounds { get; } = bounds;
    public string Text { get; } = text;
    public Color BackgroundColor { get; } = backgroundColor;
    public Color TextColor { get; } = textColor ?? Color.White;
    public string? SecondaryText { get; } = secondaryText;
    public Color? SecondaryTextColor { get; } = secondaryTextColor;
    public Action OnClick { get; } = onClick;

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice, bool isSelected = false)
    {
        // Create a 1x1 white texture for drawing rectangles
        var texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData([Color.White]);

        // Draw button background (MUCH brighter and different color if selected)
        Color bgColor;
        if (isSelected)
        {
            // Selected: Use a bright cyan/blue color that's very obvious
            bgColor = new Color(0, 150, 255); // Bright cyan
        }
        else
        {
            bgColor = BackgroundColor;
        }
        spriteBatch.Draw(texture, Bounds, bgColor);

        // Draw button border (much thicker and brighter if selected)
        int borderThickness = isSelected ? 6 : 2;
        Color borderColor = isSelected ? Color.Yellow : Color.White;

        // Top
        spriteBatch.Draw(texture, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, borderThickness),
            borderColor);
        // Bottom
        spriteBatch.Draw(texture,
            new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - borderThickness, Bounds.Width,
                borderThickness), borderColor);
        // Left
        spriteBatch.Draw(texture, new Rectangle(Bounds.X, Bounds.Y, borderThickness, Bounds.Height),
            borderColor);
        // Right
        spriteBatch.Draw(texture,
            new Rectangle(Bounds.X + Bounds.Width - borderThickness, Bounds.Y, borderThickness,
                Bounds.Height), borderColor);

        // Draw text centered in button (make it white and bold-looking when selected)
        // If we have secondary text, draw both parts with different colors
        if (!string.IsNullOrEmpty(SecondaryText) && SecondaryTextColor.HasValue)
        {
            // Measure both parts
            Vector2 primarySize = font.MeasureString(Text);
            Vector2 secondarySize = font.MeasureString(SecondaryText);
            float totalWidth = primarySize.X + secondarySize.X;

            // Calculate starting position to center the combined text
            float startX = Bounds.X + (Bounds.Width - totalWidth) / 2;
            float startY = Bounds.Y + (Bounds.Height - primarySize.Y) / 2;

            var primaryPos = new Vector2(startX, startY);
            var secondaryPos = new Vector2(startX + primarySize.X, startY);

            // Draw shadows if selected
            if (isSelected)
            {
                spriteBatch.DrawString(font, Text, primaryPos + new Vector2(2, 2), Color.Black);
                spriteBatch.DrawString(font, SecondaryText, secondaryPos + new Vector2(2, 2), Color.Black);
            }

            // Draw primary text (white)
            spriteBatch.DrawString(font, Text, primaryPos, TextColor);
            // Draw secondary text (colored)
            spriteBatch.DrawString(font, SecondaryText, secondaryPos, SecondaryTextColor.Value);
        }
        else
        {
            // Single color text (original behavior)
            Vector2 textSize = font.MeasureString(Text);
            var textPos = new Vector2(
                Bounds.X + (Bounds.Width - textSize.X) / 2,
                Bounds.Y + (Bounds.Height - textSize.Y) / 2
            );

            // Draw text with shadow/outline effect when selected for extra visibility
            if (isSelected)
            {
                // Draw shadow slightly offset for depth
                spriteBatch.DrawString(font, Text, textPos + new Vector2(2, 2), Color.Black);
            }

            spriteBatch.DrawString(font, Text, textPos, TextColor);
        }

        texture.Dispose();
    }
}