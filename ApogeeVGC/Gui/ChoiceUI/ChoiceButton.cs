using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Represents a clickable button in the choice UI
/// </summary>
public class ChoiceButton(Rectangle bounds, string text, Color backgroundColor, Action onClick)
{
    public Rectangle Bounds { get; } = bounds;
    public string Text { get; } = text;
    public Color BackgroundColor { get; } = backgroundColor;
    public Action OnClick { get; } = onClick;

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
    {
        // Create a 1x1 white texture for drawing rectangles
        var texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData([Color.White]);

        // Draw button background
        spriteBatch.Draw(texture, Bounds, BackgroundColor);

        // Draw button border
        const int borderThickness = 2;
        Color borderColor = Color.White;

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

        // Draw text centered in button
        Vector2 textSize = font.MeasureString(Text);
        var textPos = new Vector2(
            Bounds.X + (Bounds.Width - textSize.X) / 2,
            Bounds.Y + (Bounds.Height - textSize.Y) / 2
        );
        spriteBatch.DrawString(font, Text, textPos, Color.White);

        texture.Dispose();
    }
}