namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Global settings for battle animations
/// </summary>
public static class AnimationSettings
{
    /// <summary>
    /// Speed multiplier for all animations (1.0 = normal speed, 2.0 = double speed, 0.5 = half speed)
    /// </summary>
    public static float AnimationSpeedMultiplier { get; set; } = 0.25f;

    /// <summary>
    /// Duration in seconds for contact move animations (default: 0.6 seconds)
    /// </summary>
    public static float ContactMoveDuration => 0.6f / AnimationSpeedMultiplier;

    /// <summary>
    /// Duration in seconds for projectile move animations (default: 0.5 seconds)
    /// </summary>
    public static float ProjectileMoveDuration => 0.5f / AnimationSpeedMultiplier;

    /// <summary>
    /// Duration in seconds for damage indicator display (default: 1.0 second)
    /// </summary>
    public static float DamageIndicatorDuration => 1.0f / AnimationSpeedMultiplier;

    /// <summary>
    /// Speed of sprite shift for contact moves in pixels per second (default: 800 pixels/sec)
    /// </summary>
    public static float ContactMoveSpeed => 800f * AnimationSpeedMultiplier;

    /// <summary>
    /// Speed of projectile movement in pixels per second (default: 1000 pixels/sec)
    /// </summary>
    public static float ProjectileSpeed => 1000f * AnimationSpeedMultiplier;
}
