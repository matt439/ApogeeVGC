using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    /// <summary>
    /// Swaps a Pokémon's position with another Pokémon on the same side.
    /// Used for moves like Ally Switch or game mechanics that change field positions.
    /// </summary>
    /// <param name="pokemon">The Pokémon to swap</param>
    /// <param name="newPosition">The target position index (0-based)</param>
    /// <param name="attributes">Optional attributes for the swap event</param>
    /// <returns>True if swap succeeded, false if invalid</returns>
    public bool SwapPosition(Pokemon pokemon, int newPosition, string? attributes = null)
    {
        // Validate the new position is within the active slots
        if (newPosition >= pokemon.Side.Active.Count)
        {
            throw new ArgumentException("Invalid swap position", nameof(newPosition));
        }

        // Get the Pokémon at the target position (may be null)
        Pokemon? target = pokemon.Side.Active[newPosition];

        // Special check: position 1 can be swapped even if empty/fainted
        // Other positions require a valid, non-fainted target
        if (newPosition != 1 && (target == null || target.Fainted))
        {
            return false;
        }

        // Log the swap event
        if (DisplayUi)
        {
            if (!string.IsNullOrEmpty(attributes))
            {
                Add("swap", pokemon, newPosition, attributes);
            }
            else
            {
                Add("swap", pokemon, newPosition, string.Empty);
            }
        }

        // Perform the swap
        Side side = pokemon.Side;

        // Swap in the Pokemon array (full team roster)
        // Note: target can be null for position 1, handle accordingly
        if (target != null)
        {
            side.Pokemon[pokemon.Position] = target;
            side.Pokemon[newPosition] = pokemon;

            // Swap in the Active array (currently active Pokemon)
            side.Active[pokemon.Position] = side.Pokemon[pokemon.Position];
            side.Active[newPosition] = side.Pokemon[newPosition];

            // Update position properties
            target.Position = pokemon.Position;
            pokemon.Position = newPosition;

            // Trigger swap events for both Pokemon
            RunEvent(EventId.Swap, target, RunEventSource.FromNullablePokemon(pokemon));
            RunEvent(EventId.Swap, pokemon, RunEventSource.FromNullablePokemon(target));
        }
        else
        {
            // Swapping with an empty slot (only valid for position 1)
            side.Pokemon[pokemon.Position] = null;
            side.Pokemon[newPosition] = pokemon;

            side.Active[pokemon.Position] = null;
            side.Active[newPosition] = pokemon;

            pokemon.Position = newPosition;

            // Trigger swap event only for the pokemon (no target)
            RunEvent(EventId.Swap, pokemon, null);
        }

        return true;
    }

    public Pokemon? GetAtSlot(PokemonSlot? slot)
    {
        if (slot is null) return null;
        Side side = GetSide(slot.SideId);
        int position = (int)slot.PositionLetter;
        int positionOffset = (int)Math.Floor(side.N / 2.0) * side.Active.Count;
        return side.Active[position - positionOffset];
    }

    /// <summary>
    /// Determines if the current active move is suppressing abilities.
    /// Returns true if:
    /// - There's an active Pokemon that is active (not fainted)
    /// - The active Pokemon is not the target (or Gen &lt; 8)
    /// - There's an active move that ignores abilities
    /// - The target doesn't have an Ability Shield
    /// Used for abilities like Mold Breaker, Teravolt, Turboblaze and moves like
    /// Sunsteel Strike, Moongeist Beam that ignore target abilities.
    /// </summary>
    public bool SuppressingAbility(Pokemon? target = null)
    {
        // Check if there's an active Pokemon and it's currently active
        if (ActivePokemon is not { IsActive: true })
        {
            return false;
        }

        // In Gen 8+, moves can't suppress their user's own ability
        // In earlier gens, they could
        if (ActivePokemon == target && Gen >= 8)
        {
            return false;
        }

        // Check if there's an active move that ignores abilities
        if (ActiveMove is not { IgnoreAbility: true })
        {
            return false;
        }

        // Ability Shield protects against ability suppression
        return target?.HasItem(ItemId.AbilityShield) != true;
    }

    public void SetActiveMove(ActiveMove? move = null, Pokemon? pokemon = null, Pokemon? target = null)
    {
        ActiveMove = move;
        ActivePokemon = pokemon;
        ActiveTarget = target ?? pokemon;
    }

    public void ClearActiveMove(bool failed = false)
    {
        if (ActiveMove != null)
        {
            if (!failed)
            {
                LastMove = ActiveMove;
            }
            ActiveMove = null;
            ActivePokemon = null;
            ActiveTarget = null;
        }
    }

    public void UpdateSpeed()
    {
        foreach (Pokemon pokemon in EnumerateAllActive())
        {
            pokemon.UpdateSpeed();
        }
    }
}