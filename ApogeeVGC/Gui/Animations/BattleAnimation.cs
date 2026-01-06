using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Base class for all battle animations
/// </summary>
public abstract class BattleAnimation
{
    /// <summary>
    /// Whether this animation is currently active
    /// </summary>
    public bool IsActive { get; protected set; }

    /// <summary>
    /// Whether this animation has completed
    /// </summary>
    public bool IsComplete { get; protected set; }

    /// <summary>
    /// Elapsed time since animation started
    /// </summary>
    protected float ElapsedTime { get; set; }

    /// <summary>
    /// Start the animation
    /// </summary>
    public virtual void Start()
    {
        IsActive = true;
        IsComplete = false;
        ElapsedTime = 0f;
    }

    /// <summary>
    /// Update the animation state
    /// </summary>
    /// <param name="gameTime">Game time information</param>
    public abstract void Update(GameTime gameTime);

    /// <summary>
    /// Render the animation
    /// </summary>
    /// <param name="spriteBatch">Sprite batch for rendering</param>
    /// <param name="gameTime">Game time information</param>
    public abstract void Render(SpriteBatch spriteBatch, GameTime gameTime);

    /// <summary>
    /// Stop the animation
    /// </summary>
    public virtual void Stop()
    {
        IsActive = false;
        IsComplete = true;
    }

    /// <summary>
    /// Freeze the animation at its current state without marking it complete
    /// Used for holding HP bars at a specific value until a queued animation starts
    /// </summary>
    public virtual void Freeze()
    {
        IsActive = false;
        IsComplete = false;
    }
}
