using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Animation for projectile moves - shows a projectile traveling from attacker to defender(s)
/// </summary>
public class ProjectileAnimation : BattleAnimation
{
    private readonly Vector2 _startPosition;
    private readonly Vector2 _targetPosition;
    private readonly Color _projectileColor;
    private readonly float _projectileSize;
    private readonly float _duration;

    private Vector2 _currentPosition;
    private Texture2D? _projectileTexture;
    private GraphicsDevice? _graphicsDevice;

    /// <summary>
    /// Create a new projectile animation
    /// </summary>
    /// <param name="startPosition">Starting position (attacker)</param>
    /// <param name="targetPosition">Target position (defender)</param>
    /// <param name="projectileColor">Color of the projectile</param>
    /// <param name="projectileSize">Size of the projectile in pixels</param>
    /// <param name="graphicsDevice">Graphics device for texture creation</param>
    public ProjectileAnimation(Vector2 startPosition, Vector2 targetPosition, 
        Color projectileColor, float projectileSize = 16f, GraphicsDevice? graphicsDevice = null)
    {
        _startPosition = startPosition;
        _targetPosition = targetPosition;
        _projectileColor = projectileColor;
        _projectileSize = projectileSize;
        _duration = AnimationSettings.ProjectileMoveDuration;
        _currentPosition = startPosition;
        _graphicsDevice = graphicsDevice;
    }

    public override void Start()
    {
        base.Start();
        _currentPosition = _startPosition;

        // Create projectile texture if we have a graphics device
        if (_graphicsDevice != null && _projectileTexture == null)
        {
            _projectileTexture = CreateCircleTexture(_graphicsDevice, (int)_projectileSize, _projectileColor);
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsActive || IsComplete)
            return;

        ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Calculate progress (0.0 to 1.0)
        float progress = Math.Min(ElapsedTime / _duration, 1.0f);

        // Ease out for smooth deceleration
        float easedProgress = 1f - (float)Math.Pow(1f - progress, 2);

        // Interpolate position
        _currentPosition = Vector2.Lerp(_startPosition, _targetPosition, easedProgress);

        // Complete animation when done
        if (progress >= 1.0f)
        {
            Stop();
        }
    }

    public override void Render(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!IsActive || IsComplete || _projectileTexture == null)
            return;

        // Draw the projectile at its current position
        var destRect = new Rectangle(
            (int)(_currentPosition.X - _projectileSize / 2),
            (int)(_currentPosition.Y - _projectileSize / 2),
            (int)_projectileSize,
            (int)_projectileSize
        );

        spriteBatch.Draw(_projectileTexture, destRect, Color.White);
    }

    public override void Stop()
    {
        base.Stop();
    }

    /// <summary>
    /// Clean up resources
    /// </summary>
    public void Dispose()
    {
        _projectileTexture?.Dispose();
        _projectileTexture = null;
    }

    /// <summary>
    /// Create a circular texture for the projectile
    /// </summary>
    private static Texture2D CreateCircleTexture(GraphicsDevice device, int diameter, Color color)
    {
        Texture2D texture = new Texture2D(device, diameter, diameter);
        Color[] colorData = new Color[diameter * diameter];

        float radius = diameter / 2f;
        float radiusSquared = radius * radius;

        for (int x = 0; x < diameter; x++)
        {
            for (int y = 0; y < diameter; y++)
            {
                int index = x * diameter + y;
                Vector2 pos = new Vector2(x - radius, y - radius);
                
                float distanceSquared = pos.LengthSquared();
                
                if (distanceSquared <= radiusSquared)
                {
                    // Inside circle - apply anti-aliasing at edges
                    float distance = (float)Math.Sqrt(distanceSquared);
                    float alpha = 1.0f;
                    
                    if (distance > radius - 1)
                    {
                        alpha = radius - distance;
                    }

                    colorData[index] = color * alpha;
                }
                else
                {
                    colorData[index] = Color.Transparent;
                }
            }
        }

        texture.SetData(colorData);
        return texture;
    }

    /// <summary>
    /// Check if the projectile has reached its target
    /// </summary>
    public bool HasReachedTarget => Vector2.Distance(_currentPosition, _targetPosition) < 5f;
}
