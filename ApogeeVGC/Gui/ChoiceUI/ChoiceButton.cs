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

    // Shared pixel texture for all buttons (lazy initialized)
    private static Texture2D? _sharedPixelTexture;

    private static Texture2D GetPixelTexture(GraphicsDevice graphicsDevice)
    {
        if (_sharedPixelTexture == null || _sharedPixelTexture.IsDisposed)
        {
            _sharedPixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _sharedPixelTexture.SetData([Color.White]);
        }
        return _sharedPixelTexture;
    }

    /// <summary>
    /// Get an appropriate shadow color that contrasts with the text color
    /// </summary>
    private static Color GetShadowColor(Color textColor)
    {
        // Calculate perceived luminance of the text color
        double luminance = (0.299 * textColor.R + 
                           0.587 * textColor.G + 
                           0.114 * textColor.B) / 255.0;
        
        // If text is light (white), use black shadow
        // If text is dark (black), use white shadow
        return luminance > 0.5 ? Color.Black : Color.White;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice, bool isSelected = false)
    {
        // Get the shared pixel texture (don't create/dispose every frame!)
        var texture = GetPixelTexture(graphicsDevice);

        // Draw button background
        // If selected, brighten the background color instead of replacing it
        // This way the underlying color (like Tera active state) is still visible
        Color bgColor;
        if (isSelected)
        {
            // Selected: Brighten the background color by adding white
            // This makes the button clearly selected while preserving the base color
            int r = Math.Min(255, BackgroundColor.R + 100);
            int g = Math.Min(255, BackgroundColor.G + 100);
            int b = Math.Min(255, BackgroundColor.B + 100);
            bgColor = new Color(r, g, b);
        }
        else
        {
            bgColor = BackgroundColor;
        }
        
        spriteBatch.Draw(texture, Bounds, bgColor);

        // Draw button border (much thicker if selected)
        int borderThickness = isSelected ? 6 : 2;
        
        // Calculate border color that contrasts with background
        Color borderColor;
        if (isSelected)
        {
            // For selected buttons, use a color that contrasts with the background
            // Calculate perceived luminance to determine if background is light or dark
            double luminance = (0.299 * BackgroundColor.R + 
                               0.587 * BackgroundColor.G + 
                               0.114 * BackgroundColor.B) / 255.0;
            
            // For yellow/light backgrounds, use a dark cyan border
            // For dark backgrounds, use a bright cyan border
            // Cyan contrasts well with both yellow and most type colors
            borderColor = luminance > 0.5 ? new Color(0, 100, 150) : Color.Cyan;
        }
        else
        {
            borderColor = Color.White;
        }

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

            // Draw shadows if selected - use contrasting color based on text color
            if (isSelected)
            {
                // Use white shadow for dark text, black shadow for light text
                Color primaryShadowColor = GetShadowColor(TextColor);
                Color secondaryShadowColor = GetShadowColor(SecondaryTextColor.Value);
                
                spriteBatch.DrawString(font, Text, primaryPos + new Vector2(2, 2), primaryShadowColor);
                spriteBatch.DrawString(font, SecondaryText, secondaryPos + new Vector2(2, 2), secondaryShadowColor);
            }

            // Draw primary text
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
                // Use contrasting shadow color based on text color
                Color shadowColor = GetShadowColor(TextColor);
                spriteBatch.DrawString(font, Text, textPos + new Vector2(2, 2), shadowColor);
            }

            spriteBatch.DrawString(font, Text, textPos, TextColor);
        }

        // Don't dispose - we're using a shared texture now
    }
}