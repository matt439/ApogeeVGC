using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    /// <summary>
    /// Determines if a Pokemon can mega evolve. Returns the target mega species ID, or null.
    /// Called during Pokemon initialization and switch-in.
    /// </summary>
    public SpecieId? CanMegaEvo(Pokemon pokemon)
    {
        // Check if the format allows mega evolution
        if (!Battle.RuleTable.AllowMegaEvolution) return null;

        var species = pokemon.BaseSpecies;

        // Rayquaza special case: mega evolves via knowing Dragon Ascent, no item needed.
        // Check if the first OtherForme is a mega with a RequiredMove (instead of a RequiredItem).
        if (species.OtherFormes is { Count: > 0 })
        {
            var firstForme = species.OtherFormes[0];
            if (firstForme.IsMegaForme())
            {
                // Construct the mega forme SpecieId from base species name + forme suffix
                // e.g., Rayquaza + Mega -> RayquazaMega
                var megaName = species.Id.ToString() + firstForme.ToString();
                if (Enum.TryParse<SpecieId>(megaName, out var megaSpeciesId) &&
                    Battle.Library.Species.TryGetValue(megaSpeciesId, out var megaSpecies) &&
                    megaSpecies.RequiredMove is not null)
                {
                    // Must know the required move and not hold a mega stone
                    if (pokemon.HasBaseMove(megaSpecies.RequiredMove.Value))
                    {
                        var item = pokemon.GetItem();
                        if (item.MegaStone == null)
                        {
                            return megaSpeciesId;
                        }
                    }
                    return null;
                }
            }
        }

        // Standard case: mega evolves via held mega stone item
        var heldItem = pokemon.GetItem();
        if (heldItem.MegaStone == null) return null;

        // Check if this mega stone maps from this Pokemon's base species
        if (heldItem.MegaStone.TryGetValue(species.Id, out var megaTarget))
        {
            return megaTarget;
        }

        return null;
    }

    /// <summary>
    /// Executes mega evolution for a Pokemon.
    /// </summary>
    public bool RunMegaEvo(Pokemon pokemon)
    {
        var megaSpecieId = pokemon.CanMegaEvo;
        if (megaSpecieId == null) return false;

        // Perform the forme change (permanent — does not revert on switch-out)
        var item = pokemon.GetItem();
        pokemon.FormeChange(megaSpecieId.Value, item, isPermanent: true);

        // Add mega evolution message to battle log
        if (Battle.DisplayUi)
        {
            Battle.Add("-mega", pokemon, pokemon.GetItem().Name);
        }

        // Disable mega evolution for all allies on the same side
        foreach (Pokemon ally in pokemon.Side.Pokemon)
        {
            ally.CanMegaEvo = null;
        }

        // Fire the AfterMega event
        Battle.RunEvent(EventId.AfterMega, pokemon);
        return true;
    }
}
