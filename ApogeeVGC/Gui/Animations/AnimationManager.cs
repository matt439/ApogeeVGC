using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Manages all active battle animations and coordinates their lifecycle
/// </summary>
public class AnimationManager
{
    private readonly List<BattleAnimation> _activeAnimations = [];
    private readonly List<DamageIndicator> _damageIndicators = [];
    private readonly AnimationQueue _animationQueue = new();

    // Temporary storage for sprite position offsets during contact animations
    private readonly Dictionary<int, Vector2> _playerSpriteOffsets = new();
    private readonly Dictionary<int, Vector2> _opponentSpriteOffsets = new();
    
    // HP bar animations - track animated HP values for each Pokemon
    private readonly Dictionary<string, HpBarAnimation> _hpBarAnimations = new();

    /// <summary>
    /// Create a new animation manager
    /// </summary>
    public AnimationManager(GraphicsDevice graphicsDevice, SpriteFont font)
    {
        GraphicsDevice = graphicsDevice;
        Font = font;
    }

    /// <summary>
    /// Get the graphics device (for creating projectile textures)
    /// </summary>
    public GraphicsDevice GraphicsDevice { get; }

    /// <summary>
    /// Get the font (for creating damage indicators)
    /// </summary>
    public SpriteFont Font { get; }

    /// <summary>
    /// Update all active animations
    /// </summary>
    public void Update(GameTime gameTime)
    {
        // Update the animation queue
        _animationQueue.Update(gameTime);

        // If an animation completed in the queue, move it to active lists for rendering
        if (_animationQueue.CurrentAnimation is { IsComplete: false })
        {
            BattleAnimation? currentAnim = _animationQueue.CurrentAnimation;
            
            // Add to appropriate list if not already present
            if (currentAnim is DamageIndicator indicator && !_damageIndicators.Contains(indicator))
            {
                _damageIndicators.Add(indicator);
            }
            else if (currentAnim is ProjectileAnimation or ContactMoveAnimation or MultiTargetProjectileAnimation)
            {
                if (!_activeAnimations.Contains(currentAnim))
                {
                    _activeAnimations.Add(currentAnim);
                }
            }
        }

        // Update all animations
        foreach (BattleAnimation animation in _activeAnimations.ToList())
        {
            animation.Update(gameTime);

            // Remove completed animations
            if (animation.IsComplete)
            {
                _activeAnimations.Remove(animation);
                
                // Dispose projectiles
                if (animation is ProjectileAnimation projectile)
                {
                    projectile.Dispose();
                }
                else if (animation is MultiTargetProjectileAnimation multiProjectile)
                {
                    multiProjectile.Dispose();
                }
            }
        }

        // Update damage indicators separately
        foreach (DamageIndicator indicator in _damageIndicators.ToList())
        {
            indicator.Update(gameTime);

            if (indicator.IsComplete)
            {
                _damageIndicators.Remove(indicator);
            }
        }
        
        // Update HP bar animations
        foreach (HpBarAnimation hpAnimation in _hpBarAnimations.Values.ToList())
        {
            hpAnimation.Update(gameTime);
            
            if (hpAnimation.IsComplete)
            {
                _hpBarAnimations.Remove(hpAnimation.PokemonKey);
            }
        }
    }

    /// <summary>
    /// Render all active animations
    /// </summary>
    public void Render(SpriteBatch spriteBatch, GameTime gameTime)
    {
        // Render the queued animation
        _animationQueue.Render(spriteBatch, gameTime);

        // Render projectiles (both single and multi-target)
        foreach (ProjectileAnimation animation in _activeAnimations.OfType<ProjectileAnimation>())
        {
            animation.Render(spriteBatch, gameTime);
        }
        
        foreach (MultiTargetProjectileAnimation animation in _activeAnimations.OfType<MultiTargetProjectileAnimation>())
        {
            animation.Render(spriteBatch, gameTime);
        }

        // Render damage indicators on top
        foreach (DamageIndicator indicator in _damageIndicators)
        {
            indicator.Render(spriteBatch, gameTime);
        }
    }

    /// <summary>
    /// Queue an attack animation to play sequentially
    /// </summary>
    public void QueueAttackAnimation(
        Vector2 attackerPosition,
        List<Vector2> defenderPositions,
        bool isContactMove,
        int attackerSlot,
        bool isPlayerAttacker,
        string moveName)
    {
        var queuedAnimation = new QueuedAttackAnimation
        {
            AttackerPosition = attackerPosition,
            DefenderPositions = defenderPositions,
            IsContactMove = isContactMove,
            AttackerSlot = attackerSlot,
            IsPlayerAttacker = isPlayerAttacker,
            MoveName = moveName,
            AnimationManager = this
        };

        _animationQueue.Enqueue(queuedAnimation);
    }

    /// <summary>
    /// Trigger an attack animation based on move and battle context (DEPRECATED - use QueueAttackAnimation)
    /// </summary>
    /// <param name="moveUsedMessage">Move used message from battle</param>
    /// <param name="attackerPosition">Position of the attacker sprite</param>
    /// <param name="defenderPositions">Positions of defender sprite(s)</param>
    /// <param name="isContactMove">Whether this is a contact move</param>
    /// <param name="attackerSlot">Slot index of attacker (for position offset tracking)</param>
    /// <param name="isPlayerAttacker">Whether attacker is on player's side</param>
    public void TriggerAttackAnimation(
        MoveUsedMessage moveUsedMessage,
        Vector2 attackerPosition,
        List<Vector2> defenderPositions,
        bool isContactMove,
        int attackerSlot,
        bool isPlayerAttacker)
    {
        QueueAttackAnimation(
            attackerPosition,
            defenderPositions,
            isContactMove,
            attackerSlot,
            isPlayerAttacker,
            moveUsedMessage.MoveName);
    }

    /// <summary>
    /// Queue a damage indicator to play sequentially
    /// </summary>
    public void QueueDamageIndicator(int damageAmount, int maxHp, Vector2 pokemonPosition)
    {
        var queuedIndicator = new QueuedDamageIndicator
        {
            DamageAmount = damageAmount,
            MaxHp = maxHp,
            PokemonPosition = pokemonPosition,
            AnimationManager = this
        };

        _animationQueue.Enqueue(queuedIndicator);
    }

    /// <summary>
    /// Queue a miss indicator to play sequentially
    /// </summary>
    public void QueueMissIndicator(Vector2 pokemonPosition)
    {
        var queuedIndicator = new QueuedMissIndicator
        {
            PokemonPosition = pokemonPosition,
            AnimationManager = this
        };

        _animationQueue.Enqueue(queuedIndicator);
    }

    /// <summary>
    /// Queue a custom text indicator to play sequentially
    /// </summary>
    public void QueueCustomIndicator(string text, Vector2 pokemonPosition, Color color)
    {
        var queuedIndicator = new QueuedCustomIndicator
        {
            Text = text,
            PokemonPosition = pokemonPosition,
            Color = color,
            AnimationManager = this
        };

        _animationQueue.Enqueue(queuedIndicator);
    }

    /// <summary>
    /// Start an HP bar animation for a Pokemon
    /// This should be called when damage/healing occurs
    /// </summary>
    /// <param name="pokemonKey">Unique key for the Pokemon (e.g., "Miraidon|0")</param>
    /// <param name="oldHp">Previous HP value</param>
    /// <param name="newHp">New HP value</param>
    /// <param name="maxHp">Maximum HP value</param>
    public void StartHpBarAnimation(string pokemonKey, int oldHp, int newHp, int maxHp)
    {
        // If there's already an animation for this Pokemon, chain from its current HP
        if (_hpBarAnimations.TryGetValue(pokemonKey, out HpBarAnimation? existingAnimation))
        {
            // Chain from the target HP of the existing animation
            oldHp = existingAnimation.TargetHp;
        }
        
        // Remove the existing animation (it will be replaced)
        _hpBarAnimations.Remove(pokemonKey);
        
        // Create and start new animation from the correct starting HP
        var hpAnimation = new HpBarAnimation(pokemonKey, oldHp, newHp, maxHp);
        hpAnimation.Start();
        _hpBarAnimations[pokemonKey] = hpAnimation;
    }

    /// <summary>
    /// Get the animated HP value for a Pokemon, or null if no animation is active
    /// </summary>
    /// <param name="pokemonKey">Unique key for the Pokemon</param>
    /// <returns>Animated HP value, or null if no animation</returns>
    public int? GetAnimatedHp(string pokemonKey)
    {
        if (_hpBarAnimations.TryGetValue(pokemonKey, out HpBarAnimation? animation))
        {
            return animation.CurrentHp;
        }
        return null;
    }

    /// <summary>
    /// Trigger a damage indicator over a Pokemon (DEPRECATED - use QueueDamageIndicator)
    /// </summary>
    public void TriggerDamageIndicator(int damageAmount, int maxHp, Vector2 pokemonPosition)
    {
        QueueDamageIndicator(damageAmount, maxHp, pokemonPosition);
    }

    /// <summary>
    /// Trigger a miss indicator over a Pokemon (DEPRECATED - use QueueMissIndicator)
    /// </summary>
    public void TriggerMissIndicator(Vector2 pokemonPosition)
    {
        QueueMissIndicator(pokemonPosition);
    }

    /// <summary>
    /// Get the position offset for a player Pokemon (for contact animations)
    /// </summary>
    public Vector2 GetPlayerSpriteOffset(int slot)
    {
        return _playerSpriteOffsets.TryGetValue(slot, out Vector2 offset) ? offset : Vector2.Zero;
    }

    /// <summary>
    /// Get the position offset for an opponent Pokemon (for contact animations)
    /// </summary>
    public Vector2 GetOpponentSpriteOffset(int slot)
    {
        return _opponentSpriteOffsets.TryGetValue(slot, out Vector2 offset) ? offset : Vector2.Zero;
    }

    /// <summary>
    /// Clear all animations
    /// </summary>
    public void Clear()
    {
        foreach (ProjectileAnimation animation in _activeAnimations.OfType<ProjectileAnimation>())
        {
            animation.Dispose();
        }
        
        foreach (MultiTargetProjectileAnimation animation in _activeAnimations.OfType<MultiTargetProjectileAnimation>())
        {
            animation.Dispose();
        }
        
        _activeAnimations.Clear();
        _damageIndicators.Clear();
        _playerSpriteOffsets.Clear();
        _opponentSpriteOffsets.Clear();
        _animationQueue.Clear();
        _hpBarAnimations.Clear();
    }

    /// <summary>
    /// Check if any animations are currently active or queued
    /// </summary>
    public bool HasActiveAnimations => _activeAnimations.Count > 0 || _damageIndicators.Count > 0 || _animationQueue.HasAnimations;

    /// <summary>
    /// Create a callback to update sprite offset during contact animations
    /// </summary>
    public Action<Vector2> CreateOffsetCallback(int slot, bool isPlayer)
    {
        return offset =>
        {
            if (isPlayer)
            {
                _playerSpriteOffsets[slot] = offset;
            }
            else
            {
                _opponentSpriteOffsets[slot] = offset;
            }
        };
    }

    /// <summary>
    /// Get projectile color based on move name (simple heuristic for now)
    /// </summary>
    public static Color GetProjectileColorForMove(string moveName)
    {
        // Simple color mapping based on move name keywords
        string lowerName = moveName.ToLower();

        if (lowerName.Contains("fire") || lowerName.Contains("flame") || lowerName.Contains("blaze"))
            return Color.OrangeRed;
        
        if (lowerName.Contains("water") || lowerName.Contains("hydro") || lowerName.Contains("aqua"))
            return Color.DodgerBlue;
        
        if (lowerName.Contains("thunder") || lowerName.Contains("bolt") || lowerName.Contains("electric"))
            return Color.Yellow;
        
        if (lowerName.Contains("ice") || lowerName.Contains("freeze"))
            return Color.Cyan;
        
        if (lowerName.Contains("psychic") || lowerName.Contains("confusion"))
            return Color.Magenta;
        
        if (lowerName.Contains("shadow") || lowerName.Contains("dark"))
            return Color.Purple;
        
        if (lowerName.Contains("energy") || lowerName.Contains("beam"))
            return Color.White;

        // Default: light gray
        return Color.LightGray;
    }
}
