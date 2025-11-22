using Microsoft.Xna.Framework;
using System;

namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Animates HP bar changes to synchronize with damage indicators
/// </summary>
public class HpBarAnimation : BattleAnimation
{
    private readonly string _pokemonKey;  // Unique identifier for the Pokemon
    private readonly int _startHp;
    private readonly int _endHp;
    private readonly int _maxHp;
    private readonly float _duration;
    
    private int _currentHp;

    /// <summary>
    /// Create a new HP bar animation
    /// </summary>
    /// <param name="pokemonKey">Unique key identifying the Pokemon (e.g., "Miraidon|0")</param>
    /// <param name="startHp">Starting HP value</param>
    /// <param name="endHp">Ending HP value</param>
    /// <param name="maxHp">Maximum HP value</param>
    public HpBarAnimation(string pokemonKey, int startHp, int endHp, int maxHp)
    {
        _pokemonKey = pokemonKey;
        _startHp = startHp;
        _endHp = endHp;
        _maxHp = maxHp;
        _duration = AnimationSettings.DamageIndicatorDuration; // Same duration as damage indicator
        _currentHp = startHp;
    }

    /// <summary>
    /// Get the unique key for this Pokemon
    /// </summary>
    public string PokemonKey => _pokemonKey;

    /// <summary>
    /// Get the current animated HP value
    /// </summary>
    public int CurrentHp => _currentHp;

    public override void Start()
    {
        base.Start();
        _currentHp = _startHp;
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsActive || IsComplete)
            return;

        ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Calculate progress (0.0 to 1.0)
        float progress = Math.Min(ElapsedTime / _duration, 1.0f);
        
        // Ease out for smoother HP bar animation
        float easedProgress = 1f - (float)Math.Pow(1f - progress, 2);
        
        // Interpolate HP value
        _currentHp = (int)MathHelper.Lerp(_startHp, _endHp, easedProgress);
        
        // Complete animation when done
        if (progress >= 1.0f)
        {
            _currentHp = _endHp; // Ensure we end at exact value
            Stop();
        }
    }

    public override void Render(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameTime gameTime)
    {
        // HP bar animations don't render anything - they just provide animated values
    }
}
