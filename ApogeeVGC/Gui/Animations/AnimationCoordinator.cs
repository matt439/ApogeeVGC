using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Data;
using Microsoft.Xna.Framework;

namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Coordinates animation triggering based on battle messages and state
/// </summary>
public class AnimationCoordinator
{
    private readonly AnimationManager _animationManager;
    private readonly Library _library;

    // Track pending move animations to link with damage/miss messages
    private MoveUsedMessage? _pendingMoveAnimation;
    // Track the target of the pending move (if known)
    private string? _pendingMoveTarget;
    // Track if we've already triggered the animation for the current move
    private bool _animationTriggered;
    // Key format: "PokemonName|SideId" to handle duplicate names on different sides
    private readonly Dictionary<string, (Vector2 Position, int Slot, bool IsPlayer)> _pokemonPositions = new();
    
    // Track which SideId represents the player in the current perspective
    // This is needed to map absolute SideId values from messages to the perspective's player/opponent concept
    private SideId? _playerSideId;
    
    // Track fainted Pokemon that should still be shown until switch animation
    private readonly HashSet<string> _faintedPokemonKeys = [];

    /// <summary>
    /// Create a new animation coordinator
    /// </summary>
    public AnimationCoordinator(AnimationManager animationManager, Library library)
    {
        _animationManager = animationManager;
        _library = library;
    }

    /// <summary>
    /// Register Pokemon positions for animation targeting
    /// Call this each frame to keep positions updated
    /// </summary>
    public void RegisterPokemonPosition(string pokemonName, Vector2 position, int slot, bool isPlayer)
    {
        string key = CreatePositionKey(pokemonName, isPlayer);
        _pokemonPositions[key] = (position, slot, isPlayer);
    }

    /// <summary>
    /// Clear all registered Pokemon positions
    /// </summary>
    public void ClearPokemonPositions()
    {
        _pokemonPositions.Clear();
    }

    /// <summary>
    /// Set which SideId represents the player in the current perspective
    /// This must be called before registering positions or processing messages
    /// </summary>
    public void SetPlayerSideId(SideId playerSideId)
    {
        _playerSideId = playerSideId;
    }

    /// <summary>
    /// Process a battle message and trigger appropriate animations
    /// </summary>
    public void ProcessMessage(BattleMessage message)
    {
        switch (message)
        {
            case MoveUsedMessage moveMsg:
                HandleMoveUsed(moveMsg);
                break;

            case DamageMessage damageMsg:
                HandleDamage(damageMsg);
                break;

            case MissMessage missMsg:
                HandleMiss(missMsg);
                break;

            case MoveFailMessage failMsg:
                HandleMoveFail(failMsg);
                break;

            // Clear pending move on turn start
            case TurnStartMessage:
                _pendingMoveAnimation = null;
                _pendingMoveTarget = null;
                _animationTriggered = false;
                break;
        }
    }

    /// <summary>
    /// Handle a move being used - prepare animation
    /// </summary>
    private void HandleMoveUsed(MoveUsedMessage moveMsg)
    {
        // Store this move for linking with subsequent damage/miss messages
        _pendingMoveAnimation = moveMsg;
        
        // Extract target information if available
        // MoveUsedMessage may contain target info in the future, for now we'll track it from damage messages
        _pendingMoveTarget = null;
        
        // Reset animation trigger flag for this new move
        _animationTriggered = false;

        // Don't trigger animation yet - wait for damage/miss message to know the target
        // This prevents duplicate animations
    }

    /// <summary>
    /// Handle damage - queue damage indicator and HP bar animation
    /// </summary>
    private void HandleDamage(DamageMessage damageMsg)
    {
        // Trigger attack animation only once per move (on the first damage message)
        if (_pendingMoveAnimation != null && !_animationTriggered)
        {
            // Don't set _pendingMoveTarget - let TriggerAttackAnimationIfReady use GetPotentialDefenderPositions
            // This allows spread moves to target all opponents
            TriggerAttackAnimationIfReady();
            _animationTriggered = true; // Mark as triggered to prevent duplicate animations
        }
        
        // Find the damaged Pokemon's position using name and side
        string key = CreatePositionKey(damageMsg.PokemonName, damageMsg.SideId);
        if (_pokemonPositions.TryGetValue(key, out var pokemonInfo))
        {
            // Queue damage indicator to play after attack animation
            _animationManager.QueueDamageIndicator(
                damageMsg.DamageAmount,
                damageMsg.MaxHp,
                pokemonInfo.Position);
            
            // Start HP bar animation (oldHp = remainingHp + damage, newHp = remainingHp)
            int oldHp = damageMsg.RemainingHp + damageMsg.DamageAmount;
            _animationManager.StartHpBarAnimation(key, oldHp, damageMsg.RemainingHp, damageMsg.MaxHp);
        }

        // Note: Don't clear _pendingMoveAnimation here yet - it might hit multiple targets
        // Only clear on the next move or turn start
    }

    /// <summary>
    /// Handle miss - queue miss indicator
    /// </summary>
    private void HandleMiss(MissMessage missMsg)
    {
        // Trigger attack animation only once per move (on the first miss message)
        if (_pendingMoveAnimation != null && !_animationTriggered)
        {
            // Don't set _pendingMoveTarget - let TriggerAttackAnimationIfReady use GetPotentialDefenderPositions
            // This allows spread moves to target all opponents
            TriggerAttackAnimationIfReady();
            _animationTriggered = true; // Mark as triggered to prevent duplicate animations
        }
        
        // Find the Pokemon's position using name and side
        string key = CreatePositionKey(missMsg.PokemonName, missMsg.SideId);
        if (_pokemonPositions.TryGetValue(key, out var pokemonInfo))
        {
            // Queue miss indicator to play after attack animation
            _animationManager.QueueMissIndicator(pokemonInfo.Position);
        }

        // Note: Don't clear _pendingMoveAnimation here yet - it might target multiple Pokemon
        // Only clear on the next move or turn start
    }

    /// <summary>
    /// Handle move fail - queue custom indicator (e.g., "Protected!")
    /// </summary>
    private void HandleMoveFail(MoveFailMessage failMsg)
    {
        // If there's a target (e.g., Protect blocked the move), trigger animation and show indicator
        if (failMsg is { TargetPokemonName: not null, TargetSideId: not null })
        {
            // Trigger attack animation only once per move
            if (_pendingMoveAnimation != null && !_animationTriggered)
            {
                _pendingMoveTarget = CreatePositionKey(failMsg.TargetPokemonName, failMsg.TargetSideId);
                TriggerAttackAnimationIfReady();
                _animationTriggered = true;
            }
            
            // Find the target Pokemon's position
            string key = CreatePositionKey(failMsg.TargetPokemonName, failMsg.TargetSideId);
            if (_pokemonPositions.TryGetValue(key, out var pokemonInfo))
            {
                // Determine the indicator text and color based on the reason
                string indicatorText = "PROTECTED!";
                Color indicatorColor = Color.LightBlue;
                
                // Could add more specific handling based on the reason if needed
                // For now, assume all failures with targets are protection-related
                
                // Queue protected indicator to play after attack animation
                _animationManager.QueueCustomIndicator(indicatorText, pokemonInfo.Position, indicatorColor);
            }
        }

        // Note: Don't clear _pendingMoveAnimation here yet
        // Only clear on the next move or turn start
    }

    /// <summary>
    /// Try to trigger the attack animation if we have all needed information
    /// </summary>
    private void TriggerAttackAnimationIfReady()
    {
        if (_pendingMoveAnimation == null)
            return;

        // Try to find the move in the library to check if it's a contact move
        MoveId? moveId = GetMoveIdFromName(_pendingMoveAnimation.MoveName);
        if (moveId == null)
            return;

        Move? move = _library.Moves.GetValueOrDefault(moveId.Value);
        if (move == null)
            return;

        // Skip animations for status moves (Protect, Trick Room, etc.)
        if (move.Category == MoveCategory.Status)
            return;

        // Find attacker position using name and side
        string attackerKey = CreatePositionKey(_pendingMoveAnimation.PokemonName, _pendingMoveAnimation.SideId);
        if (!_pokemonPositions.TryGetValue(attackerKey, out var attackerInfo))
            return;

        // Get defender position(s) - always use all opponents for proper multi-target animations
        List<Vector2> defenderPositions = GetPotentialDefenderPositions(attackerInfo.IsPlayer);

        if (defenderPositions.Count == 0)
            return;

        // Check if this is a contact move
        bool isContactMove = move.Flags.Contact ?? false;

        // Queue the attack animation to play sequentially
        _animationManager.QueueAttackAnimation(
            attackerInfo.Position,
            defenderPositions,
            isContactMove,
            attackerInfo.Slot,
            attackerInfo.IsPlayer,
            _pendingMoveAnimation.MoveName);
    }

    /// <summary>
    /// Get potential defender positions based on attacker's side
    /// </summary>
    private List<Vector2> GetPotentialDefenderPositions(bool attackerIsPlayer)
    {
        var positions = new List<Vector2>();

        // Get positions of Pokemon on the opposite side
        foreach ((string _, (Vector2 position, int _, bool isPlayer)) in _pokemonPositions)
        {
            if (isPlayer != attackerIsPlayer)
            {
                positions.Add(position);
            }
        }

        return positions;
    }

    /// <summary>
    /// Try to find a MoveId from a move name
    /// </summary>
    private MoveId? GetMoveIdFromName(string moveName)
    {
        // Search through all moves to find matching name
        foreach (var (id, move) in _library.Moves)
        {
            if (move.Name.Equals(moveName, StringComparison.OrdinalIgnoreCase))
            {
                return id;
            }
        }

        return null;
    }

    /// <summary>
    /// Create a unique key for position lookup that includes both name and side
    /// </summary>
    private string CreatePositionKey(string pokemonName, SideId? sideId)
    {
        // If no side ID, fall back to just name (shouldn't happen in normal battles)
        if (!sideId.HasValue)
            return pokemonName;

        // Create composite key: "Name|SideId"
        return $"{pokemonName}|{(int)sideId.Value}";
    }

    /// <summary>
    /// Create a unique key for position lookup using isPlayer boolean
    /// </summary>
    private string CreatePositionKey(string pokemonName, bool isPlayer)
    {
        // Map isPlayer to absolute SideId using the tracked player side
        // If no player side is set, fall back to simple name-based key
        if (_playerSideId == null)
            return pokemonName;

        // isPlayer=true means PlayerSide in perspective, which maps to _playerSideId
        // isPlayer=false means OpponentSide in perspective, which maps to the other side
        SideId sideId = isPlayer ? _playerSideId.Value : (_playerSideId.Value == SideId.P1 ? SideId.P2 : SideId.P1);
        return CreatePositionKey(pokemonName, sideId);
    }

    /// <summary>
    /// Check if any animations are currently active
    /// </summary>
    public bool HasActiveAnimations()
    {
        return _animationManager.HasActiveAnimations;
    }
}
