using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Represents a clickable button in the choice UI
/// </summary>
public class ChoiceButton(
    Rectangle bounds,
    string text,
    Color backgroundColor,
    Action onClick,
    Color? textColor = null,
    string? secondaryText = null,
    Color? secondaryTextColor = null)
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

    // Button rendering constants
    private const int PixelTextureSize = 1; // Size of 1x1 pixel texture for drawing shapes

    private const int
        SelectionBrighteningAmount = 100; // Amount to brighten background when selected

    private const int SelectedBorderThickness = 6; // Border thickness when selected
    private const int NormalBorderThickness = 2; // Border thickness when not selected
    private const int ShadowOffsetX = 2; // X offset for text shadow
    private const int ShadowOffsetY = 2; // Y offset for text shadow
    private const float CenteringDivisor = 2f; // Divisor for centering calculations

    // Luminance calculation constants (ITU-R BT.601 standard)
    private const double LuminanceRedWeight = 0.299;
    private const double LuminanceGreenWeight = 0.587;
    private const double LuminanceBlueWeight = 0.114;
    private const double LuminanceThreshold = 0.5; // Threshold for light vs dark colors
    private const int ColorChannelMax = 255; // Maximum RGB color value

    // Border color for selected buttons
    private const int DarkBorderR = 0;
    private const int DarkBorderG = 100;
    private const int DarkBorderB = 150;

    private static Texture2D GetPixelTexture(GraphicsDevice graphicsDevice)
    {
        if (_sharedPixelTexture == null || _sharedPixelTexture.IsDisposed)
        {
            _sharedPixelTexture = new Texture2D(graphicsDevice, PixelTextureSize, PixelTextureSize);
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
        double luminance = (LuminanceRedWeight * textColor.R +
                            LuminanceGreenWeight * textColor.G +
                            LuminanceBlueWeight * textColor.B) / ColorChannelMax;

        // If text is light (white), use black shadow
        // If text is dark (black), use white shadow
        return luminance > LuminanceThreshold ? Color.Black : Color.White;
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice,
        bool isSelected = false)
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
            int r = Math.Min(ColorChannelMax, BackgroundColor.R + SelectionBrighteningAmount);
            int g = Math.Min(ColorChannelMax, BackgroundColor.G + SelectionBrighteningAmount);
            int b = Math.Min(ColorChannelMax, BackgroundColor.B + SelectionBrighteningAmount);
            bgColor = new Color(r, g, b);
        }
        else
        {
            bgColor = BackgroundColor;
        }

        spriteBatch.Draw(texture, Bounds, bgColor);

        // Draw button border (much thicker if selected)
        int borderThickness = isSelected ? SelectedBorderThickness : NormalBorderThickness;

        // Calculate border color that contrasts with background
        Color borderColor;
        if (isSelected)
        {
            // For selected buttons, use a color that contrasts with the background
            // Calculate perceived luminance to determine if background is light or dark
            double luminance = (LuminanceRedWeight * BackgroundColor.R +
                                LuminanceGreenWeight * BackgroundColor.G +
                                LuminanceBlueWeight * BackgroundColor.B) / ColorChannelMax;

            // For yellow/light backgrounds, use a dark cyan border
            // For dark backgrounds, use a bright cyan border
            // Cyan contrasts well with both yellow and most type colors
            borderColor = luminance > LuminanceThreshold
                ? new Color(DarkBorderR, DarkBorderG, DarkBorderB)
                : Color.Cyan;
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
            float startX = Bounds.X + (Bounds.Width - totalWidth) / CenteringDivisor;
            float startY = Bounds.Y + (Bounds.Height - primarySize.Y) / CenteringDivisor;

            var primaryPos = new Vector2(startX, startY);
            var secondaryPos = new Vector2(startX + primarySize.X, startY);

            // Draw shadows if selected - use contrasting color based on text color
            if (isSelected)
            {
                // Use white shadow for dark text, black shadow for light text
                Color primaryShadowColor = GetShadowColor(TextColor);
                Color secondaryShadowColor = GetShadowColor(SecondaryTextColor.Value);

                spriteBatch.DrawString(font, Text,
                    primaryPos + new Vector2(ShadowOffsetX, ShadowOffsetY), primaryShadowColor);
                spriteBatch.DrawString(font, SecondaryText,
                    secondaryPos + new Vector2(ShadowOffsetX, ShadowOffsetY), secondaryShadowColor);
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
                Bounds.X + (Bounds.Width - textSize.X) / CenteringDivisor,
                Bounds.Y + (Bounds.Height - textSize.Y) / CenteringDivisor
            );

            // Draw text with shadow/outline effect when selected for extra visibility
            if (isSelected)
            {
                // Use contrasting shadow color based on text color
                Color shadowColor = GetShadowColor(TextColor);
                spriteBatch.DrawString(font, Text,
                    textPos + new Vector2(ShadowOffsetX, ShadowOffsetY), shadowColor);
            }

            spriteBatch.DrawString(font, Text, textPos, TextColor);
        }

        // Don't dispose - we're using a shared texture now
    }
}