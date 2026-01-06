using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Animation for projectile moves that hit multiple targets - shows expanding/spreading effect
/// </summary>
public class MultiTargetProjectileAnimation : BattleAnimation
{
    private readonly Vector2 _startPosition;
    private readonly List<Vector2> _targetPositions;
    private readonly Color _projectileColor;
    private readonly float _projectileSize;
    private readonly float _duration;
    private readonly GraphicsDevice _graphicsDevice;
    
    // Projectile texture (white circle)
    private Texture2D? _projectileTexture;
    
    // Current projectile state
    private Vector2 _currentPosition;
    private float _currentSize;
    private float _currentAlpha;

    // Animation phases
    private const float TravelPercent = 0.6f;  // 60% of time traveling to center
    private const float ExpandPercent = 0.3f;   // 30% of time expanding
    private const float FadePercent = 0.1f;     // 10% of time fading

    /// <summary>
    /// Create a new multi-target projectile animation
    /// </summary>
    /// <param name="startPosition">Starting position of the projectile</param>
    /// <param name="targetPositions">Positions of all targets</param>
    /// <param name="projectileColor">Color of the projectile</param>
    /// <param name="projectileSize">Initial size of the projectile</param>
    /// <param name="graphicsDevice">Graphics device for creating textures</param>
    public MultiTargetProjectileAnimation(
        Vector2 startPosition,
        List<Vector2> targetPositions,
        Color projectileColor,
        float projectileSize,
        GraphicsDevice graphicsDevice)
    {
        _startPosition = startPosition;
        _targetPositions = targetPositions;
        _projectileColor = projectileColor;
        _projectileSize = projectileSize;
        _duration = AnimationSettings.ProjectileMoveDuration;
        _graphicsDevice = graphicsDevice;
        
        _currentPosition = startPosition;
        _currentSize = projectileSize;
        _currentAlpha = 1.0f;
    }

    public override void Start()
    {
        base.Start();
        
        // Create a circular projectile texture
        int textureSize = (int)(_projectileSize * 4); // Make texture larger for expansion
        _projectileTexture = new Texture2D(_graphicsDevice, textureSize, textureSize);
        
        // Create a circle with anti-aliasing
        var colorData = new Color[textureSize * textureSize];
        var center = new Vector2(textureSize / 2f, textureSize / 2f);
        float radius = textureSize / 2f;
        
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                var pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                // Anti-aliased edge
                if (distance <= radius - 1)
                {
                    colorData[y * textureSize + x] = Color.White;
                }
                else if (distance <= radius)
                {
                    float alpha = radius - distance;
                    colorData[y * textureSize + x] = Color.White * alpha;
                }
                else
                {
                    colorData[y * textureSize + x] = Color.Transparent;
                }
            }
        }
        
        _projectileTexture.SetData(colorData);
        
        _currentPosition = _startPosition;
        _currentSize = _projectileSize;
        _currentAlpha = 1.0f;
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsActive || IsComplete)
            return;

        ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds * AnimationSettings.AnimationSpeedMultiplier;
        
        // Calculate progress (0.0 to 1.0)
        float progress = Math.Min(ElapsedTime / _duration, 1.0f);
        
        // Calculate center point of all targets
        Vector2 centerTarget = CalculateCenterPoint(_targetPositions);
        
        if (progress < TravelPercent)
        {
            // Phase 1: Travel to center of targets
            float travelProgress = progress / TravelPercent;
            // Ease out for smoother deceleration
            float easedProgress = 1f - (float)Math.Pow(1f - travelProgress, 2);
            _currentPosition = Vector2.Lerp(_startPosition, centerTarget, easedProgress);
            _currentSize = _projectileSize;
            _currentAlpha = 1.0f;
        }
        else if (progress < TravelPercent + ExpandPercent)
        {
            // Phase 2: Expand to hit all targets
            float expandProgress = (progress - TravelPercent) / ExpandPercent;
            _currentPosition = centerTarget;
            
            // Calculate maximum distance from center to any target
            float maxDistance = 0f;
            foreach (Vector2 target in _targetPositions)
            {
                float distance = Vector2.Distance(centerTarget, target);
                if (distance > maxDistance)
                    maxDistance = distance;
            }
            
            // Expand size to reach all targets
            float targetSize = Math.Max(_projectileSize * 2, maxDistance + _projectileSize);
            _currentSize = MathHelper.Lerp(_projectileSize, targetSize, expandProgress);
            _currentAlpha = 1.0f;
        }
        else
        {
            // Phase 3: Fade out
            float fadeProgress = (progress - TravelPercent - ExpandPercent) / FadePercent;
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
        if (!IsActive || IsComplete || _projectileTexture == null)
            return;

        // Calculate the scale to apply to the texture
        float textureSize = _projectileTexture.Width;
        float scale = (_currentSize * 2) / textureSize;
        
        // Draw the projectile centered at current position
        var origin = new Vector2(textureSize / 2f, textureSize / 2f);
        Color renderColor = _projectileColor * _currentAlpha;
        
        spriteBatch.Draw(
            _projectileTexture,
            _currentPosition,
            null,
            renderColor,
            0f,
            origin,
            scale,
            SpriteEffects.None,
            0f);
    }

    /// <summary>
    /// Calculate the center point of multiple positions
    /// </summary>
    private Vector2 CalculateCenterPoint(List<Vector2> positions)
    {
        if (positions.Count == 0)
            return Vector2.Zero;
        
        if (positions.Count == 1)
            return positions[0];
        
        Vector2 sum = Vector2.Zero;
        foreach (Vector2 pos in positions)
        {
            sum += pos;
        }
        
        return sum / positions.Count;
    }

    /// <summary>
    /// Dispose of the projectile texture
    /// </summary>
    public void Dispose()
    {
        _projectileTexture?.Dispose();
        _projectileTexture = null;
    }
}
