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
    // Track the actual targets hit by the pending move (collected from damage/miss messages)
    private readonly List<string> _pendingMoveTargets = new();
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
                _pendingMoveTargets.Clear();
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
        
        // Clear target list for the new move
        _pendingMoveTargets.Clear();
        
        // Reset animation trigger flag for this new move
        _animationTriggered = false;

        // Don't trigger animation yet - wait for damage/miss message to know the target(s)
        // This prevents duplicate animations
    }

    /// <summary>
    /// Handle damage - queue damage indicator with HP bar animation
    /// </summary>
    private void HandleDamage(DamageMessage damageMsg)
    {
        // Add this Pokemon to the list of targets first
        string targetKey = CreatePositionKey(damageMsg.PokemonName, damageMsg.SideId);
        if (!_pendingMoveTargets.Contains(targetKey))
        {
            _pendingMoveTargets.Add(targetKey);
        }
        
        // Find the damaged Pokemon's position using name and side
        string key = CreatePositionKey(damageMsg.PokemonName, damageMsg.SideId);
        if (_pokemonPositions.TryGetValue(key, out var pokemonInfo))
        {
            // Calculate old HP (before damage)
            int oldHp = damageMsg.RemainingHp + damageMsg.DamageAmount;
            
            // Queue damage indicator with HP bar animation data
            // The HP bar will start when the damage indicator starts (after attack lands)
            _animationManager.QueueDamageIndicatorWithHpBar(
                damageMsg.DamageAmount,
                damageMsg.MaxHp,
                pokemonInfo.Position,
                key,
                oldHp,
                damageMsg.RemainingHp);
        }

        // Don't trigger animation here - wait until all damage messages are processed
        // Animation will be triggered by the first queued damage indicator when it starts

        // Note: Don't clear _pendingMoveAnimation here yet - it might hit multiple targets
        // Only clear on the next move or turn start
    }

    /// <summary>
    /// Handle miss - queue miss indicator
    /// </summary>
    private void HandleMiss(MissMessage missMsg)
    {
        // Add this Pokemon to the list of targets
        string targetKey = CreatePositionKey(missMsg.PokemonName, missMsg.SideId);
        if (!_pendingMoveTargets.Contains(targetKey))
        {
            _pendingMoveTargets.Add(targetKey);
        }
        
        // Trigger attack animation only once per move (on the first miss message)
        if (_pendingMoveAnimation != null && !_animationTriggered)
        {
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
            // Add this Pokemon to the list of targets
            string targetKey = CreatePositionKey(failMsg.TargetPokemonName, failMsg.TargetSideId);
            if (!_pendingMoveTargets.Contains(targetKey))
            {
                _pendingMoveTargets.Add(targetKey);
            }
            
            // Trigger attack animation only once per move
            if (_pendingMoveAnimation != null && !_animationTriggered)
            {
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
        {
            Console.WriteLine("[AnimationCoordinator] No pending move animation");
            return;
        }

        Console.WriteLine($"[AnimationCoordinator] Trying to trigger animation for {_pendingMoveAnimation.MoveName}");

        // Try to find the move in the library to check if it's a contact move
        MoveId? moveId = GetMoveIdFromName(_pendingMoveAnimation.MoveName);
        if (moveId == null)
        {
            Console.WriteLine($"[AnimationCoordinator] Could not find MoveId for {_pendingMoveAnimation.MoveName}");
            return;
        }

        Move? move = _library.Moves.GetValueOrDefault(moveId.Value);
        if (move == null)
        {
            Console.WriteLine($"[AnimationCoordinator] Could not find Move for MoveId {moveId.Value}");
            return;
        }

        // Skip animations for status moves (Protect, Trick Room, etc.)
        if (move.Category == MoveCategory.Status)
        {
            Console.WriteLine($"[AnimationCoordinator] Skipping status move: {_pendingMoveAnimation.MoveName}");
            return;
        }

        // Find attacker position using name and side
        string attackerKey = CreatePositionKey(_pendingMoveAnimation.PokemonName, _pendingMoveAnimation.SideId);
        Console.WriteLine($"[AnimationCoordinator] Looking for attacker with key: {attackerKey}");
        Console.WriteLine($"[AnimationCoordinator] Registered positions count: {_pokemonPositions.Count}");
        foreach (var kvp in _pokemonPositions)
        {
            Console.WriteLine($"[AnimationCoordinator]   - {kvp.Key}: Position={kvp.Value.Position}, Slot={kvp.Value.Slot}, IsPlayer={kvp.Value.IsPlayer}");
        }
        
        if (!_pokemonPositions.TryGetValue(attackerKey, out var attackerInfo))
        {
            Console.WriteLine($"[AnimationCoordinator] Could not find attacker position for key: {attackerKey}");
            return;
        }

        // Get defender position(s) from the actual targets that were hit
        List<Vector2> defenderPositions = new();
        foreach (string targetKey in _pendingMoveTargets)
        {
            if (_pokemonPositions.TryGetValue(targetKey, out var targetInfo))
            {
                defenderPositions.Add(targetInfo.Position);
            }
        }
        
        Console.WriteLine($"[AnimationCoordinator] Found {defenderPositions.Count} actual target positions from {_pendingMoveTargets.Count} target keys");

        if (defenderPositions.Count == 0)
        {
            Console.WriteLine("[AnimationCoordinator] No defender positions found, falling back to all opponents");
            // Fallback: if no targets collected yet, use all potential defenders
            // This can happen if animation triggers on first damage message before target is added
            defenderPositions = GetPotentialDefenderPositions(attackerInfo.IsPlayer);
        }

        // Check if this is a contact move
        bool isContactMove = move.Flags.Contact ?? false;

        Console.WriteLine($"[AnimationCoordinator] Queueing attack animation: isContactMove={isContactMove}, moveName={_pendingMoveAnimation.MoveName}");

        // Queue the attack animation to play sequentially
        _animationManager.QueueAttackAnimation(
            attackerInfo.Position,
            defenderPositions,
            isContactMove,
            attackerInfo.Slot,
            attackerInfo.IsPlayer,
            _pendingMoveAnimation.MoveName);
            
        Console.WriteLine("[AnimationCoordinator] Attack animation queued successfully");
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

    /// <summary>
    /// Trigger any pending attack animation if one is waiting
    /// Call this after processing a batch of related damage/miss messages
    /// </summary>
    public void TriggerPendingAttackAnimation()
    {
        if (_pendingMoveAnimation != null && !_animationTriggered)
        {
            TriggerAttackAnimationIfReady();
            _animationTriggered = true;
        }
    }
}
