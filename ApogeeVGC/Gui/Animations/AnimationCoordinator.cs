using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Data;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

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
    private readonly Dictionary<string, (Vector2 Position, int Slot, bool IsPlayer)> _pokemonPositions = new();

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
        _pokemonPositions[pokemonName] = (position, slot, isPlayer);
    }

    /// <summary>
    /// Clear all registered Pokemon positions
    /// </summary>
    public void ClearPokemonPositions()
    {
        _pokemonPositions.Clear();
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

            // Clear pending move on turn start
            case TurnStartMessage:
                _pendingMoveAnimation = null;
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

        // Try to trigger attack animation immediately if we know the target
        TriggerAttackAnimationIfReady();
    }

    /// <summary>
    /// Handle damage - queue damage indicator
    /// </summary>
    private void HandleDamage(DamageMessage damageMsg)
    {
        // Find the damaged Pokemon's position
        if (_pokemonPositions.TryGetValue(damageMsg.PokemonName, out var pokemonInfo))
        {
            // Queue damage indicator to play after attack animation
            _animationManager.QueueDamageIndicator(
                damageMsg.DamageAmount,
                damageMsg.MaxHp,
                pokemonInfo.Position);
        }

        // Clear pending move after damage is shown
        _pendingMoveAnimation = null;
    }

    /// <summary>
    /// Handle miss - queue miss indicator
    /// </summary>
    private void HandleMiss(MissMessage missMsg)
    {
        // Find the Pokemon's position
        if (_pokemonPositions.TryGetValue(missMsg.PokemonName, out var pokemonInfo))
        {
            // Queue miss indicator to play after attack animation
            _animationManager.QueueMissIndicator(pokemonInfo.Position);
        }

        // Clear pending move after miss is shown
        _pendingMoveAnimation = null;
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

        Move? move = _library.Moves.TryGetValue(moveId.Value, out var foundMove) ? foundMove : null;
        if (move == null)
            return;

        // Find attacker position
        if (!_pokemonPositions.TryGetValue(_pendingMoveAnimation.PokemonName, out var attackerInfo))
            return;

        // For now, we'll need to infer the defender(s) from context
        // This is a simplified approach - in practice you might need more sophisticated tracking
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
        foreach (var (name, (position, slot, isPlayer)) in _pokemonPositions)
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
}
