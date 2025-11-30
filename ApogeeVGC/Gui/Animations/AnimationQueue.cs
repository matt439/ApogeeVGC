using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Represents a queued animation with all the data needed to start it
/// </summary>
public abstract class QueuedAnimation
{
    /// <summary>
    /// Start the animation and return the animation instance
    /// </summary>
    public abstract BattleAnimation Start();
}

/// <summary>
/// Queued attack animation (contact or projectile move)
/// </summary>
public class QueuedAttackAnimation : QueuedAnimation
{
    public required Vector2 AttackerPosition { get; init; }
    public required List<Vector2> DefenderPositions { get; init; }
    public required bool IsContactMove { get; init; }
    public required int AttackerSlot { get; init; }
    public required bool IsPlayerAttacker { get; init; }
    public required string MoveName { get; init; }
    public required AnimationManager AnimationManager { get; init; }

    public override BattleAnimation Start()
    {
        if (DefenderPositions.Count == 0)
            throw new InvalidOperationException("No defender positions provided");

        Vector2 defenderPosition = DefenderPositions[0];

        if (IsContactMove)
        {
            // Contact move: shift attacker sprite toward first target
            var offsetCallback = AnimationManager.CreateOffsetCallback(AttackerSlot, IsPlayerAttacker);
            var contactAnimation = new ContactMoveAnimation(
                AttackerPosition,
                defenderPosition,
                offsetCallback);

            contactAnimation.Start();
            return contactAnimation;
        }
        else
        {
            // Projectile move
            Color projectileColor = AnimationManager.GetProjectileColorForMove(MoveName);
            
            if (DefenderPositions.Count > 1)
            {
                // Multi-target: use expanding projectile animation
                var multiProjectileAnimation = new MultiTargetProjectileAnimation(
                    AttackerPosition,
                    DefenderPositions,
                    projectileColor,
                    16f,
                    AnimationManager.GraphicsDevice);

                multiProjectileAnimation.Start();
                return multiProjectileAnimation;
            }
            else
            {
                // Single target: use regular projectile animation
                var projectileAnimation = new ProjectileAnimation(
                    AttackerPosition,
                    defenderPosition,
                    projectileColor,
                    16f,
                    AnimationManager.GraphicsDevice);

                projectileAnimation.Start();
                return projectileAnimation;
            }
        }
    }
}

/// <summary>
/// Queued damage indicator animation
/// </summary>
public class QueuedDamageIndicator : QueuedAnimation
{
    public required int DamageAmount { get; init; }
    public required int MaxHp { get; init; }
    public required Vector2 PokemonPosition { get; init; }
    public required AnimationManager AnimationManager { get; init; }
    public string? PokemonKey { get; init; } // Optional: for triggering HP bar animation
    public int? OldHp { get; init; } // Optional: for HP bar animation
    public int? NewHp { get; init; } // Optional: for HP bar animation

    public override BattleAnimation Start()
    {
        Console.WriteLine($"[QueuedDamageIndicator] Starting damage indicator for {PokemonKey}");
        
        var indicator = DamageIndicator.CreateDamageIndicator(
            DamageAmount,
            MaxHp,
            PokemonPosition,
            AnimationManager.Font);

        indicator.Start();
        
        // If this damage indicator has associated HP bar animation data, queue it to start
        // after this damage indicator completes
        if (PokemonKey != null && OldHp.HasValue && NewHp.HasValue)
        {
            Console.WriteLine($"[QueuedDamageIndicator] Queueing HP bar animation for {PokemonKey}: {OldHp} -> {NewHp}");
            // Queue HP bar animation to start after damage indicator
            // Note: Don't create frozen animation here - that's already handled by PreservePokemonHpBeforePerspectiveUpdate
            AnimationManager.QueueHpBarAnimation(PokemonKey, OldHp.Value, NewHp.Value, MaxHp);
        }
        
        return indicator;
    }
}

/// <summary>
/// Queued miss indicator animation
/// </summary>
public class QueuedMissIndicator : QueuedAnimation
{
    public required Vector2 PokemonPosition { get; init; }
    public required AnimationManager AnimationManager { get; init; }

    public override BattleAnimation Start()
    {
        var indicator = DamageIndicator.CreateMissIndicator(
            PokemonPosition,
            AnimationManager.Font);

        indicator.Start();
        return indicator;
    }
}

/// <summary>
/// Queued custom text indicator animation
/// </summary>
public class QueuedCustomIndicator : QueuedAnimation
{
    public required string Text { get; init; }
    public required Vector2 PokemonPosition { get; init; }
    public required Color Color { get; init; }
    public required AnimationManager AnimationManager { get; init; }

    public override BattleAnimation Start()
    {
        var indicator = DamageIndicator.CreateCustomIndicator(
            Text,
            PokemonPosition,
            AnimationManager.Font,
            Color);

        indicator.Start();
        return indicator;
    }
}

/// <summary>
/// Queued HP bar animation
/// </summary>
public class QueuedHpBarAnimation : QueuedAnimation
{
    public required string PokemonKey { get; init; }
    public required int OldHp { get; init; }
    public required int NewHp { get; init; }
    public required int MaxHp { get; init; }
    public required AnimationManager AnimationManager { get; init; }

    public override BattleAnimation Start()
    {
        Console.WriteLine($"[QueuedHpBarAnimation] Starting HP animation for {PokemonKey}: {OldHp} -> {NewHp}");
        
        var hpAnimation = new HpBarAnimation(PokemonKey, OldHp, NewHp, MaxHp);
        hpAnimation.Start();
        
        // Immediately register the animation - frozen animations are preserved by PreservePokemonHpBeforePerspectiveUpdate
        AnimationManager.RegisterHpBarAnimation(PokemonKey, hpAnimation);
        Console.WriteLine($"[QueuedHpBarAnimation] Registered HP animation for {PokemonKey}");
        
        return hpAnimation;
    }
}

/// <summary>
/// Manages a queue of animations to play sequentially
/// </summary>
public class AnimationQueue
{
    private readonly Queue<QueuedAnimation> _animationQueue = new();
    private BattleAnimation? _currentAnimation;

    /// <summary>
    /// Add an animation to the queue
    /// </summary>
    public void Enqueue(QueuedAnimation animation)
    {
        _animationQueue.Enqueue(animation);
    }

    /// <summary>
    /// Update the current animation and start the next one when ready
    /// </summary>
    public void Update(GameTime gameTime)
    {
        // If no current animation, start the next one in queue
        if (_currentAnimation == null || _currentAnimation.IsComplete)
        {
            if (_animationQueue.Count > 0)
            {
                var nextAnimation = _animationQueue.Dequeue();
                _currentAnimation = nextAnimation.Start();
            }
            else
            {
                _currentAnimation = null;
            }
        }

        // Update current animation
        _currentAnimation?.Update(gameTime);
    }

    /// <summary>
    /// Render the current animation
    /// </summary>
    public void Render(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameTime gameTime)
    {
        _currentAnimation?.Render(spriteBatch, gameTime);
    }

    /// <summary>
    /// Check if animations are currently playing or queued
    /// </summary>
    public bool HasAnimations => _currentAnimation != null || _animationQueue.Count > 0;

    /// <summary>
    /// Get the current animation (if any)
    /// </summary>
    public BattleAnimation? CurrentAnimation => _currentAnimation;

    /// <summary>
    /// Clear all queued and current animations
    /// </summary>
    public void Clear()
    {
        _animationQueue.Clear();
        _currentAnimation?.Stop();
        _currentAnimation = null;
    }

    /// <summary>
    /// Get the number of queued animations (not including current)
    /// </summary>
    public int QueueCount => _animationQueue.Count;
}
