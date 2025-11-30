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
        
        // If this damage indicator has associated HP bar animation data, start it immediately
        // This makes the HP bar animation and damage text animation play concurrently
        if (PokemonKey != null && OldHp.HasValue && NewHp.HasValue)
        {
            Console.WriteLine($"[QueuedDamageIndicator] Starting HP bar animation concurrently for {PokemonKey}: {OldHp} -> {NewHp}");
            
            // Get the starting HP value from any existing animation
            int startHp = OldHp.Value;
            var existingAnimation = AnimationManager.GetHpBarAnimation(PokemonKey);
            if (existingAnimation != null)
            {
                // If there's a frozen animation, start from its current HP
                if (!existingAnimation.IsActive)
                {
                    startHp = existingAnimation.CurrentHp;
                    Console.WriteLine($"[QueuedDamageIndicator] Using frozen animation HP: {startHp}");
                }
                else
                {
                    // Animation is active - chain from its target HP
                    startHp = existingAnimation.TargetHp;
                    Console.WriteLine($"[QueuedDamageIndicator] Chaining from active animation target: {startHp}");
                }
            }
            
            // Create and start the HP bar animation immediately (concurrent with damage indicator)
            var hpAnimation = new HpBarAnimation(PokemonKey, startHp, NewHp.Value, MaxHp);
            hpAnimation.Start();
            AnimationManager.RegisterHpBarAnimation(PokemonKey, hpAnimation);
            Console.WriteLine($"[QueuedDamageIndicator] HP bar animation started concurrently");
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
/// Queued multi-target damage indicators that all start concurrently
/// This is used for multi-target moves like Dazzling Gleam
/// </summary>
public class QueuedMultiTargetDamageIndicators : QueuedAnimation
{
    public required List<DamageTargetInfo> Targets { get; init; }
    public required AnimationManager AnimationManager { get; init; }

    public override BattleAnimation Start()
    {
        Console.WriteLine($"[QueuedMultiTargetDamageIndicators] Starting {Targets.Count} damage indicators concurrently");
        
        // Start all damage indicators at the same time
        foreach (var target in Targets)
        {
            Console.WriteLine($"[QueuedMultiTargetDamageIndicators] Starting indicator for {target.PokemonKey}");
            
            var indicator = DamageIndicator.CreateDamageIndicator(
                target.DamageAmount,
                target.MaxHp,
                target.Position,
                AnimationManager.Font);
            
            indicator.Start();
            AnimationManager.AddDamageIndicatorToActiveList(indicator);
            
            // Start HP bar animation concurrently
            int startHp = target.OldHp;
            var existingAnimation = AnimationManager.GetHpBarAnimation(target.PokemonKey);
            if (existingAnimation != null)
            {
                if (!existingAnimation.IsActive)
                {
                    startHp = existingAnimation.CurrentHp;
                    Console.WriteLine($"[QueuedMultiTargetDamageIndicators] Using frozen animation HP: {startHp}");
                }
                else
                {
                    startHp = existingAnimation.TargetHp;
                    Console.WriteLine($"[QueuedMultiTargetDamageIndicators] Chaining from active animation target: {startHp}");
                }
            }
            
            var hpAnimation = new HpBarAnimation(target.PokemonKey, startHp, target.NewHp, target.MaxHp);
            hpAnimation.Start();
            AnimationManager.RegisterHpBarAnimation(target.PokemonKey, hpAnimation);
        }
        
        // Return a dummy completed animation since we've already started everything
        // The actual animations are tracked separately by AnimationManager
        var dummyAnimation = new CompletedAnimation();
        return dummyAnimation;
    }
}

/// <summary>
/// Queued switch animation (withdraw old Pokémon, send out new Pokémon)
/// </summary>
public class QueuedSwitchAnimation : QueuedAnimation
{
    public string? WithdrawPokemonKey { get; init; }
    public string? SendOutPokemonKey { get; init; }
    public required Vector2 Position { get; init; }
    public required bool IsPlayer { get; init; }
    public required int Slot { get; init; }
    public required AnimationManager AnimationManager { get; init; }

    public override BattleAnimation Start()
    {
        Console.WriteLine($"[QueuedSwitchAnimation] Starting switch animation - Withdraw: {WithdrawPokemonKey}, Send out: {SendOutPokemonKey}");
        
        var switchAnimation = new SwitchAnimation(
            WithdrawPokemonKey,
            SendOutPokemonKey,
            Position,
            IsPlayer,
            Slot);
        
        switchAnimation.Start();
        
        // Register the animation with the manager so it can be queried by BattleRenderer
        string slotKey = $"slot_{Slot}_{(IsPlayer ? "player" : "opponent")}";
        AnimationManager.RegisterSwitchAnimation(slotKey, switchAnimation);
        
        return switchAnimation;
    }
}

/// <summary>
/// Information about a single damage target for multi-target moves
/// </summary>
public record DamageTargetInfo
{
    public required string PokemonKey { get; init; }
    public required int DamageAmount { get; init; }
    public required int OldHp { get; init; }
    public required int NewHp { get; init; }
    public required int MaxHp { get; init; }
    public required Vector2 Position { get; init; }
}

/// <summary>
/// A dummy animation that completes immediately
/// Used when the real work is done in the Start() method
/// </summary>
internal class CompletedAnimation : BattleAnimation
{
    public CompletedAnimation()
    {
        IsComplete = true;
        IsActive = false;
    }

    public override void Update(GameTime gameTime)
    {
        // Already complete
    }

    public override void Render(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameTime gameTime)
    {
        // Nothing to render
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
