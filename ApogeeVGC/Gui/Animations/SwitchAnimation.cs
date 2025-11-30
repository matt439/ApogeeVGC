using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Animation for Pokémon switching (withdraw old Pokémon, send out new Pokémon)
/// Simulates the Pokéball effect: shrink + move back for withdraw, move forward + grow for send-out
/// </summary>
public class SwitchAnimation : BattleAnimation
{
    private readonly string? _withdrawPokemonKey;
    private readonly string? _sendOutPokemonKey;
    private readonly Vector2 _position;
    private readonly bool _isPlayer;
    private readonly int _slot;
    
    private SwitchPhase _currentPhase;
    private float _phaseElapsedTime;
    
    // Animation state
    private float _currentScale = 1.0f;
    private Vector2 _currentOffset = Vector2.Zero;
    
    /// <summary>
    /// Current phase of the switch animation
    /// </summary>
    private enum SwitchPhase
    {
        Withdraw,   // Shrinking and moving back
        SendOut,    // Moving forward and growing
        Complete
    }
    
    public SwitchAnimation(
        string? withdrawPokemonKey,
        string? sendOutPokemonKey,
        Vector2 position,
        bool isPlayer,
        int slot)
    {
        _withdrawPokemonKey = withdrawPokemonKey;
        _sendOutPokemonKey = sendOutPokemonKey;
        _position = position;
        _isPlayer = isPlayer;
        _slot = slot;
        
        // Start with withdraw if there's a Pokémon to withdraw, otherwise go straight to send-out
        _currentPhase = withdrawPokemonKey != null ? SwitchPhase.Withdraw : SwitchPhase.SendOut;
    }
    
    /// <summary>
    /// Get the current scale for the withdrawing Pokémon
    /// </summary>
    public float GetWithdrawScale()
    {
        if (_currentPhase != SwitchPhase.Withdraw || _withdrawPokemonKey == null)
            return 1.0f;
        
        return _currentScale;
    }
    
    /// <summary>
    /// Get the current offset for the withdrawing Pokémon
    /// </summary>
    public Vector2 GetWithdrawOffset()
    {
        if (_currentPhase != SwitchPhase.Withdraw || _withdrawPokemonKey == null)
            return Vector2.Zero;
        
        return _currentOffset;
    }
    
    /// <summary>
    /// Get the current scale for the sending out Pokémon
    /// </summary>
    public float GetSendOutScale()
    {
        if (_currentPhase != SwitchPhase.SendOut || _sendOutPokemonKey == null)
            return 1.0f;
        
        return _currentScale;
    }
    
    /// <summary>
    /// Get the current offset for the sending out Pokémon
    /// </summary>
    public Vector2 GetSendOutOffset()
    {
        if (_currentPhase != SwitchPhase.SendOut || _sendOutPokemonKey == null)
            return Vector2.Zero;
        
        return _currentOffset;
    }
    
    /// <summary>
    /// Get the Pokémon key for the withdrawing Pokémon
    /// </summary>
    public string? WithdrawPokemonKey => _withdrawPokemonKey;
    
    /// <summary>
    /// Get the Pokémon key for the sending out Pokémon
    /// </summary>
    public string? SendOutPokemonKey => _sendOutPokemonKey;
    
    /// <summary>
    /// Get the slot this switch is occurring in
    /// </summary>
    public int Slot => _slot;
    
    /// <summary>
    /// Whether this switch is for the player's side
    /// </summary>
    public bool IsPlayer => _isPlayer;
    
    /// <summary>
    /// Check if the animation is currently in the send-out phase
    /// </summary>
    public bool IsInSendOutPhase => _currentPhase == SwitchPhase.SendOut;
    
    public override void Update(GameTime gameTime)
    {
        if (!IsActive) return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _phaseElapsedTime += deltaTime;
        
        switch (_currentPhase)
        {
            case SwitchPhase.Withdraw:
                UpdateWithdrawPhase();
                break;
                
            case SwitchPhase.SendOut:
                UpdateSendOutPhase();
                break;
                
            case SwitchPhase.Complete:
                Stop();
                break;
        }
    }
    
    private void UpdateWithdrawPhase()
    {
        float duration = AnimationSettings.SwitchWithdrawDuration;
        float progress = Math.Clamp(_phaseElapsedTime / duration, 0f, 1f);
        
        // Ease-in effect for smooth acceleration
        float eased = EaseIn(progress);
        
        // Scale: 1.0 -> 0.2
        _currentScale = 1.0f - (eased * 0.8f);
        
        // Offset: move backward (player moves down, opponent moves up)
        float maxOffset = AnimationSettings.SwitchWithdrawDistance;
        float offsetDirection = _isPlayer ? 1f : -1f; // Player moves down (+Y), opponent moves up (-Y)
        _currentOffset = new Vector2(0, offsetDirection * maxOffset * eased);
        
        // Transition to send-out phase when withdraw completes
        if (progress >= 1.0f)
        {
            _currentPhase = _sendOutPokemonKey != null ? SwitchPhase.SendOut : SwitchPhase.Complete;
            _phaseElapsedTime = 0f;
            
            // Reset for send-out phase
            if (_currentPhase == SwitchPhase.SendOut)
            {
                _currentScale = 0.2f;
                float sendOutMaxOffset = AnimationSettings.SwitchSendOutDistance;
                float sendOutDirection = _isPlayer ? 1f : -1f;
                _currentOffset = new Vector2(0, sendOutDirection * sendOutMaxOffset);
            }
        }
    }
    
    private void UpdateSendOutPhase()
    {
        float duration = AnimationSettings.SwitchSendOutDuration;
        float progress = Math.Clamp(_phaseElapsedTime / duration, 0f, 1f);
        
        // Ease-out effect for smooth deceleration
        float eased = EaseOut(progress);
        
        // Scale: 0.2 -> 1.0
        _currentScale = 0.2f + (eased * 0.8f);
        
        // Offset: move forward (player moves up, opponent moves down)
        float maxOffset = AnimationSettings.SwitchSendOutDistance;
        float offsetDirection = _isPlayer ? 1f : -1f; // Player moves up (back to position), opponent moves down
        _currentOffset = new Vector2(0, offsetDirection * maxOffset * (1f - eased));
        
        // Complete when send-out finishes
        if (progress >= 1.0f)
        {
            _currentPhase = SwitchPhase.Complete;
            _currentScale = 1.0f;
            _currentOffset = Vector2.Zero;
        }
    }
    
    /// <summary>
    /// Ease-in function (quadratic) for smooth acceleration
    /// </summary>
    private static float EaseIn(float t)
    {
        return t * t;
    }
    
    /// <summary>
    /// Ease-out function (quadratic) for smooth deceleration
    /// </summary>
    private static float EaseOut(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }
    
    public override void Render(SpriteBatch spriteBatch, GameTime gameTime)
    {
        // Switch animations don't render directly - they modify sprite transforms
        // The BattleRenderer will read the scale/offset values when rendering Pokémon sprites
    }
}
