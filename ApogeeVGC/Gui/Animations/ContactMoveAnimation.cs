using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Animation for contact moves - shifts the attacker's sprite toward the defender and back
/// </summary>
public class ContactMoveAnimation : BattleAnimation
{
    private readonly Vector2 _startPosition;
    private readonly Vector2 _targetPosition;
    private readonly Action<Vector2>? _updatePositionCallback;
    private readonly float _duration;

    // Animation phases
    public enum AnimationPhase
    {
        MovingToTarget,
        Striking,
        Returning
    }

    private AnimationPhase _currentPhase;

    // Phase timing (as percentage of total duration)
    private const float MoveToTargetPercent = 0.4f;  // 40% of time moving to target
    private const float StrikingPercent = 0.2f;      // 20% of time at target
    private const float ReturningPercent = 0.4f;     // 40% of time returning

    /// <summary>
    /// Create a new contact move animation
    /// </summary>
    /// <param name="startPosition">Starting position of the attacker</param>
    /// <param name="targetPosition">Position of the defender</param>
    /// <param name="updatePositionCallback">Callback to update the attacker's rendered position</param>
    public ContactMoveAnimation(Vector2 startPosition, Vector2 targetPosition, 
        Action<Vector2>? updatePositionCallback = null)
    {
        _startPosition = startPosition;
        _targetPosition = targetPosition;
        _updatePositionCallback = updatePositionCallback;
        _duration = AnimationSettings.ContactMoveDuration;
        _currentPhase = AnimationPhase.MovingToTarget;
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsActive || IsComplete)
            return;

        ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Calculate progress (0.0 to 1.0)
        float progress = Math.Min(ElapsedTime / _duration, 1.0f);

        // Determine current position based on animation phase
        Vector2 currentPosition;

        if (progress < MoveToTargetPercent)
        {
            // Phase 1: Moving to target
            _currentPhase = AnimationPhase.MovingToTarget;
            float phaseProgress = progress / MoveToTargetPercent;
            // Ease out for smoother deceleration
            float easedProgress = 1f - (float)Math.Pow(1f - phaseProgress, 2);
            currentPosition = Vector2.Lerp(_startPosition, _targetPosition, easedProgress);
        }
        else if (progress < MoveToTargetPercent + StrikingPercent)
        {
            // Phase 2: Striking (stay at target)
            _currentPhase = AnimationPhase.Striking;
            currentPosition = _targetPosition;
        }
        else
        {
            // Phase 3: Returning to start
            _currentPhase = AnimationPhase.Returning;
            float phaseProgress = (progress - MoveToTargetPercent - StrikingPercent) / ReturningPercent;
            // Ease in for smoother acceleration back
            float easedProgress = (float)Math.Pow(phaseProgress, 2);
            currentPosition = Vector2.Lerp(_targetPosition, _startPosition, easedProgress);
        }

        // Calculate offset from start position (not absolute position)
        Vector2 offset = currentPosition - _startPosition;

        // Update offset via callback
        _updatePositionCallback?.Invoke(offset);

        // Complete animation when done
        if (progress >= 1.0f)
        {
            _updatePositionCallback?.Invoke(Vector2.Zero); // Ensure offset returns to zero
            Stop();
        }
    }

    public override void Render(SpriteBatch spriteBatch, GameTime gameTime)
    {
        // Contact animations don't render anything directly - they modify sprite positions
        // The actual sprite rendering is handled by BattleRenderer
    }

    /// <summary>
    /// Get the current animation phase
    /// </summary>
    public AnimationPhase CurrentPhase => _currentPhase;

    /// <summary>
    /// Check if the animation is in the striking phase (when damage should be shown)
    /// </summary>
    public bool IsInStrikingPhase => _currentPhase == AnimationPhase.Striking;
}
