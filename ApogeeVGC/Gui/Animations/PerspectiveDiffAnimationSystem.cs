using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.Moves;
using Microsoft.Xna.Framework;

namespace ApogeeVGC.Gui.Animations;

/// <summary>
/// Animation system that uses perspective diffs to detect state changes
/// and messages for context. This eliminates target tracking issues.
/// </summary>
public class PerspectiveDiffAnimationSystem
{
    private readonly AnimationManager _animationManager;
    private readonly Library _library;
    
    private BattlePerspective? _previousPerspective;
    private BattlePerspective? _moveStartPerspective; // Perspective when move started (before any damage)
    private MoveUsedMessage? _pendingMove;
    private readonly List<HpChangeInfo> _pendingDamageTargets = new(); // Accumulate targets from same move
    private readonly Dictionary<string, Vector2> _pokemonPositions = new();
    
    /// <summary>
    /// Create a new perspective-diff animation system
    /// </summary>
    public PerspectiveDiffAnimationSystem(AnimationManager animationManager, Library library)
    {
        _animationManager = animationManager;
        _library = library;
    }
    
    /// <summary>
    /// Process a battle event using perspective diff and message context
    /// </summary>
    public void ProcessEvent(BattleEvent evt)
    {
        // 1. Track move context from message
        if (evt.Message is MoveUsedMessage moveMsg)
        {
            // New move started - flush any pending damage from previous move
            if (_pendingMove != null && _pendingDamageTargets.Count > 0)
            {
                QueueAttackAnimation(_pendingMove, _pendingDamageTargets);
                _pendingDamageTargets.Clear();
            }
            
            _pendingMove = moveMsg;
            _moveStartPerspective = _previousPerspective; // Snapshot perspective before damage
            
            // Create frozen HP animations NOW to preserve current HP values
            // This prevents HP bars from jumping when perspectives update
            if (_previousPerspective != null)
            {
                FreezePokemonHpValues(_previousPerspective);
            }
        }
        
        // 2. Process message-based indicators (effectiveness, crits, miss, etc.)
        ProcessMessageIndicators(evt.Message);
        
        // 3. Detect HP changes and accumulate targets (don't queue animations yet)
        if (_previousPerspective != null && evt.Perspective.PerspectiveType == BattlePerspectiveType.InBattle)
        {
            DetectAndAccumulateDamage(_previousPerspective, evt.Perspective);
        }
        
        // 4. If this is NOT a damage/move message, flush pending animations
        //    This handles cases where move sequence ends without another move starting
        if (evt.Message is not (DamageMessage or MoveUsedMessage or EffectivenessMessage))
        {
            if (_pendingMove != null && _pendingDamageTargets.Count > 0)
            {
                QueueAttackAnimation(_pendingMove, _pendingDamageTargets);
                _pendingDamageTargets.Clear();
                _pendingMove = null;
            }
        }
        
        // 5. Store perspective for next comparison
        _previousPerspective = evt.Perspective;
    }
    
    /// <summary>
    /// Register Pokemon positions for animation targeting
    /// </summary>
    public void RegisterPokemonPosition(string pokemonName, Vector2 position, int slot, bool isPlayer)
    {
        string key = CreatePositionKey(pokemonName, isPlayer);
        _pokemonPositions[key] = position;
    }
    
    /// <summary>
    /// Clear all registered Pokemon positions
    /// </summary>
    public void ClearPokemonPositions()
    {
        _pokemonPositions.Clear();
    }
    
    /// <summary>
    /// Check if any animations are currently active
    /// </summary>
    public bool HasActiveAnimations()
    {
        return _animationManager.HasActiveAnimations;
    }
    
    /// <summary>
    /// Process message-based indicators that don't require state diffs
    /// </summary>
    private void ProcessMessageIndicators(BattleMessage? message)
    {
        if (message == null) return;
        
        switch (message)
        {
            case MissMessage missMsg:
                QueueMissIndicator(missMsg);
                break;
                
            case MoveFailMessage failMsg when failMsg.TargetPokemonName != null:
                QueueProtectIndicator(failMsg);
                break;
        }
    }
    
    /// <summary>
    /// Detect HP changes and accumulate them for the pending move
    /// Compare against move start perspective to get total damage from the move
    /// </summary>
    private void DetectAndAccumulateDamage(BattlePerspective prev, BattlePerspective curr)
    {
        // Use move start perspective if available (to get total damage from move, not incremental)
        // Otherwise use previous perspective (for non-damage changes like healing)
        BattlePerspective baselinePerspective = _moveStartPerspective ?? prev;
        
        // Detect HP changes on both sides
        var playerHpChanges = DetectHpChanges(baselinePerspective.PlayerSide.Active, curr.PlayerSide.Active, true);
        var opponentHpChanges = DetectHpChanges(baselinePerspective.OpponentSide.Active, curr.OpponentSide.Active, false);
        var allHpChanges = playerHpChanges.Concat(opponentHpChanges).ToList();
        
        // Accumulate damage targets for the pending move (but don't queue damage indicators yet)
        foreach (var change in allHpChanges)
        {
            if (change.HpDelta < 0 && _pendingMove != null)
            {
                // Check if we already have this target
                if (!_pendingDamageTargets.Any(t => t.Key == change.Key))
                {
                    _pendingDamageTargets.Add(change);
                    // DON'T queue damage indicator yet - wait for attack animation first
                }
            }
            else if (change.HpDelta > 0)
            {
                // Healing - queue immediately (not part of attack animation)
                QueueHealingAnimation(change);
            }
        }
        
        // Detect switches on both sides
        DetectAndQueueSwitches(prev.PlayerSide.Active, curr.PlayerSide.Active, true);
        DetectAndQueueSwitches(prev.OpponentSide.Active, curr.OpponentSide.Active, false);
    }
    
    /// <summary>
    /// Detect HP changes between two active Pokemon lists
    /// </summary>
    private List<HpChangeInfo> DetectHpChanges(
        IReadOnlyList<PokemonPerspective?> prevActive,
        IReadOnlyList<PokemonPerspective?> currActive,
        bool isPlayer)
    {
        var changes = new List<HpChangeInfo>();
        
        for (int slot = 0; slot < Math.Max(prevActive.Count, currActive.Count); slot++)
        {
            var prev = slot < prevActive.Count ? prevActive[slot] : null;
            var curr = slot < currActive.Count ? currActive[slot] : null;
            
            // Same Pokemon, HP changed?
            if (prev != null && curr != null && prev.Name == curr.Name && prev.Hp != curr.Hp)
            {
                string key = CreatePositionKey(curr.Name, isPlayer);
                if (_pokemonPositions.TryGetValue(key, out Vector2 position))
                {
                    changes.Add(new HpChangeInfo
                    {
                        PokemonName = curr.Name,
                        IsPlayer = isPlayer,
                        Slot = slot,
                        PreviousHp = prev.Hp,
                        CurrentHp = curr.Hp,
                        MaxHp = curr.MaxHp,
                        HpDelta = curr.Hp - prev.Hp,
                        Position = position,
                        Key = key
                    });
                }
            }
        }
        
        return changes;
    }
    
    /// <summary>
    /// Detect switches and queue switch animations
    /// </summary>
    private void DetectAndQueueSwitches(
        IReadOnlyList<PokemonPerspective?> prevActive,
        IReadOnlyList<PokemonPerspective?> currActive,
        bool isPlayer)
    {
        for (int slot = 0; slot < Math.Max(prevActive.Count, currActive.Count); slot++)
        {
            var prev = slot < prevActive.Count ? prevActive[slot] : null;
            var curr = slot < currActive.Count ? currActive[slot] : null;
            
            // Different Pokemon in same slot ? switch occurred
            if (prev?.Name != curr?.Name)
            {
                // Note: Switch animations could be queued here if needed
                // For now, switches are instant (no animation)
                // The HP bar system will handle showing the new Pokemon
            }
        }
    }
    
    /// <summary>
    /// Freeze HP values for all Pokemon in the current perspective
    /// This prevents HP bars from jumping when perspectives update during a move sequence
    /// </summary>
    private void FreezePokemonHpValues(BattlePerspective perspective)
    {
        // Freeze player Pokemon HP values
        for (int slot = 0; slot < perspective.PlayerSide.Active.Count; slot++)
        {
            var pokemon = perspective.PlayerSide.Active[slot];
            if (pokemon != null)
            {
                string key = CreatePositionKey(pokemon.Name, true);
                
                // Only create frozen animation if one doesn't already exist
                if (_animationManager.GetAnimatedHp(key) == null)
                {
                    Console.WriteLine($"[PerspectiveDiff] Creating frozen animation for {key}: {pokemon.Hp} HP");
                    _animationManager.StartHpBarAnimation(key, pokemon.Hp, pokemon.Hp, pokemon.MaxHp);
                    // Immediately freeze it at current HP (not Stop, which marks it complete)
                    var frozenAnim = _animationManager.GetHpBarAnimation(key);
                    frozenAnim?.Freeze();
                }
            }
        }
        
        // Freeze opponent Pokemon HP values
        for (int slot = 0; slot < perspective.OpponentSide.Active.Count; slot++)
        {
            var pokemon = perspective.OpponentSide.Active[slot];
            if (pokemon != null)
            {
                string key = CreatePositionKey(pokemon.Name, false);
                
                // Only create frozen animation if one doesn't already exist
                if (_animationManager.GetAnimatedHp(key) == null)
                {
                    Console.WriteLine($"[PerspectiveDiff] Creating frozen animation for {key}: {pokemon.Hp} HP");
                    _animationManager.StartHpBarAnimation(key, pokemon.Hp, pokemon.Hp, pokemon.MaxHp);
                    // Immediately freeze it at current HP (not Stop, which marks it complete)
                    var frozenAnim = _animationManager.GetHpBarAnimation(key);
                    frozenAnim?.Freeze();
                }
            }
        }
    }
    
    /// <summary>
    /// Queue attack animation with targets from HP changes, then queue damage indicators
    /// </summary>
    private void QueueAttackAnimation(MoveUsedMessage moveMsg, List<HpChangeInfo> targets)
    {
        // Find the move to check if it's a contact move
        MoveId? moveId = GetMoveIdFromName(moveMsg.MoveName);
        if (moveId == null) return;
        
        Move? move = _library.Moves.GetValueOrDefault(moveId.Value);
        if (move == null) return;
        
        // Skip animations for status moves
        if (move.Category == MoveCategory.Status) return;
        
        // Find attacker position
        string attackerKey = CreatePositionKey(moveMsg.PokemonName, moveMsg.SideId == SideId.P1);
        if (!_pokemonPositions.TryGetValue(attackerKey, out Vector2 attackerPosition))
        {
            Console.WriteLine($"[PerspectiveDiff] Could not find attacker position for {attackerKey}");
            return;
        }
        
        // Get defender positions from targets that took damage
        List<Vector2> defenderPositions = targets
            .Select(t => t.Position)
            .Distinct()
            .ToList();
        
        if (defenderPositions.Count == 0)
        {
            Console.WriteLine($"[PerspectiveDiff] No defender positions found for {moveMsg.MoveName}");
            return;
        }
        
        bool isContactMove = move.Flags.Contact ?? false;
        bool attackerIsPlayer = moveMsg.SideId == SideId.P1;
        int attackerSlot = GetSlotFromPosition(attackerPosition, attackerIsPlayer);
        
        Console.WriteLine($"[PerspectiveDiff] Queueing attack animation: {moveMsg.MoveName}, targets: {defenderPositions.Count}");
        
        // FIRST: Queue the attack animation
        _animationManager.QueueAttackAnimation(
            attackerPosition,
            defenderPositions,
            isContactMove,
            attackerSlot,
            attackerIsPlayer,
            moveMsg.MoveName);
        
        // SECOND: Queue damage indicators for all targets (these will play after attack animation)
        foreach (var target in targets)
        {
            QueueDamageAnimation(target);
        }
    }
    
    /// <summary>
    /// Queue damage animation from HP change info
    /// </summary>
    private void QueueDamageAnimation(HpChangeInfo change)
    {
        int damageAmount = -change.HpDelta; // Negative delta = damage
        
        Console.WriteLine($"[PerspectiveDiff] Queueing damage: {change.PokemonName} took {damageAmount} damage ({change.PreviousHp} -> {change.CurrentHp})");
        
        _animationManager.QueueDamageIndicatorWithHpBar(
            damageAmount,
            change.MaxHp,
            change.Position,
            change.Key,
            change.PreviousHp,
            change.CurrentHp);
    }
    
    /// <summary>
    /// Queue healing animation from HP change info
    /// </summary>
    private void QueueHealingAnimation(HpChangeInfo change)
    {
        int healAmount = change.HpDelta; // Positive delta = healing
        
        Console.WriteLine($"[PerspectiveDiff] {change.PokemonName} healed {healAmount} HP ({change.PreviousHp} -> {change.CurrentHp})");
        
        // Queue HP bar animation for healing
        _animationManager.StartHpBarAnimation(
            change.Key,
            change.PreviousHp,
            change.CurrentHp,
            change.MaxHp);
    }
    
    /// <summary>
    /// Queue miss indicator
    /// </summary>
    private void QueueMissIndicator(MissMessage missMsg)
    {
        string key = CreatePositionKey(missMsg.PokemonName, missMsg.SideId == SideId.P1);
        if (_pokemonPositions.TryGetValue(key, out Vector2 position))
        {
            _animationManager.QueueMissIndicator(position);
        }
    }
    
    /// <summary>
    /// Queue protect indicator
    /// </summary>
    private void QueueProtectIndicator(MoveFailMessage failMsg)
    {
        if (failMsg.TargetPokemonName == null || failMsg.TargetSideId == null)
            return;
            
        string key = CreatePositionKey(failMsg.TargetPokemonName, failMsg.TargetSideId == SideId.P1);
        if (_pokemonPositions.TryGetValue(key, out Vector2 position))
        {
            _animationManager.QueueCustomIndicator("PROTECTED!", position, Color.LightBlue);
        }
    }
    
    /// <summary>
    /// Create a position key from Pokemon name and side
    /// </summary>
    private static string CreatePositionKey(string pokemonName, bool isPlayer)
    {
        // Player is always SideId.P1 (0), opponent is always SideId.P2 (1)
        int sideId = isPlayer ? 0 : 1;
        return $"{pokemonName}|{sideId}";
    }
    
    /// <summary>
    /// Try to find a MoveId from a move name
    /// </summary>
    private MoveId? GetMoveIdFromName(string moveName)
    {
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
    /// Get slot number from position
    /// </summary>
    private static int GetSlotFromPosition(Vector2 position, bool isPlayer)
    {
        // Calculate slot based on X position
        // Player positions: 20, 208, 396, 584
        // Opponent positions: 480, 668, 856, 1044
        if (isPlayer)
        {
            return (int)((position.X - 20) / 188);
        }
        else
        {
            return (int)((position.X - 480) / 188);
        }
    }
}

/// <summary>
/// Information about an HP change detected from perspective diff
/// </summary>
internal record HpChangeInfo
{
    public required string PokemonName { get; init; }
    public required bool IsPlayer { get; init; }
    public required int Slot { get; init; }
    public required int PreviousHp { get; init; }
    public required int CurrentHp { get; init; }
    public required int MaxHp { get; init; }
    public required int HpDelta { get; init; }
    public required Vector2 Position { get; init; }
    public required string Key { get; init; }
}
