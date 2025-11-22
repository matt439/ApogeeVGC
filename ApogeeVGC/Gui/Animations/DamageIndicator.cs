using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Visual indicator showing damage dealt or miss status over a Pokemon's sprite
/// </summary>
public class DamageIndicator : BattleAnimation
{
    private readonly string _displayText;
    private readonly Vector2 _position;
    private readonly Color _textColor;
    private readonly SpriteFont _font;
    private readonly float _duration;

    // Animation properties
    private float _currentAlpha;
    private float _verticalOffset;

    // Movement and fade parameters
    private const float RiseSpeed = 30f;  // Pixels per second to rise
    private const float FadeStartPercent = 0.6f;  // When to start fading (60% through duration)

    /// <summary>
    /// Create a new damage indicator
    /// </summary>
    /// <param name="displayText">Text to display (e.g., "42%" or "MISS")</param>
    /// <param name="position">Position over the Pokemon sprite</param>
    /// <param name="font">Font to render the text</param>
    /// <param name="textColor">Color of the text</param>
    public DamageIndicator(string displayText, Vector2 position, SpriteFont font, Color textColor)
    {
        _displayText = displayText;
        _position = position;
        _font = font;
        _textColor = textColor;
        _duration = AnimationSettings.DamageIndicatorDuration;
        _currentAlpha = 1.0f;
        _verticalOffset = 0f;
    }

    public override void Start()
    {
        base.Start();
        _currentAlpha = 1.0f;
        _verticalOffset = 0f;
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsActive || IsComplete)
            return;

        ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Calculate progress (0.0 to 1.0)
        float progress = Math.Min(ElapsedTime / _duration, 1.0f);

        // Rise upward
        _verticalOffset -= RiseSpeed * deltaTime * AnimationSettings.AnimationSpeedMultiplier;

        // Fade out in the last portion
        if (progress >= FadeStartPercent)
        {
            float fadeProgress = (progress - FadeStartPercent) / (1.0f - FadeStartPercent);
            _currentAlpha = 1.0f - fadeProgress;
        }

        // Complete animation when done
        if (progress >= 1.0f)
        {
            Stop();
        }
    }

    public override void Render(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!IsActive || IsComplete)
            return;

        // Calculate render position with vertical offset
        Vector2 renderPosition = _position + new Vector2(0, _verticalOffset);

        // Measure text for centering
        Vector2 textSize = _font.MeasureString(_displayText);
        Vector2 centeredPosition = renderPosition - textSize / 2f;

        // Draw text with current alpha
        Color colorWithAlpha = _textColor * _currentAlpha;
        
        // Draw outline for better visibility
        DrawTextWithOutline(spriteBatch, _font, _displayText, centeredPosition, colorWithAlpha, Color.Black * _currentAlpha);
    }

    /// <summary>
    /// Draw text with a black outline for better visibility
    /// </summary>
    private void DrawTextWithOutline(SpriteBatch spriteBatch, SpriteFont font, string text, 
        Vector2 position, Color color, Color outlineColor)
    {
        // Draw outline (4 directions)
        spriteBatch.DrawString(font, text, position + new Vector2(-1, -1), outlineColor);
        spriteBatch.DrawString(font, text, position + new Vector2(1, -1), outlineColor);
        spriteBatch.DrawString(font, text, position + new Vector2(-1, 1), outlineColor);
        spriteBatch.DrawString(font, text, position + new Vector2(1, 1), outlineColor);

        // Draw main text
        spriteBatch.DrawString(font, text, position, color);
    }

    /// <summary>
    /// Create a damage indicator showing percentage damage
    /// </summary>
    public static DamageIndicator CreateDamageIndicator(int damageAmount, int maxHp, Vector2 position, SpriteFont font)
    {
        double percentDamage = (double)damageAmount / maxHp * 100.0;
        string displayText = $"{percentDamage:F1}%";
        
        // Color based on damage severity
        Color textColor = percentDamage switch
        {
            >= 50 => Color.Red,           // Heavy damage
            >= 30 => Color.Orange,        // Moderate damage  
            >= 15 => Color.Yellow,        // Light damage
            _ => Color.White              // Minimal damage
        };

        return new DamageIndicator(displayText, position, font, textColor);
    }

    /// <summary>
    /// Create a miss indicator
    /// </summary>
    public static DamageIndicator CreateMissIndicator(Vector2 position, SpriteFont font)
    {
        return new DamageIndicator("MISS", position, font, Color.Gray);
    }

    /// <summary>
    /// Create a custom text indicator
    /// </summary>
    public static DamageIndicator CreateCustomIndicator(string text, Vector2 position, SpriteFont font, Color color)
    {
        return new DamageIndicator(text, position, font, color);
    }
}
