using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    public MoveType? CanTerastallize(IBattle battle, Pokemon pokemon)
    {
        if (Battle.Gen != 9)
        {
            return null;
        }
        return pokemon.TeraType;
    }

    public void Terastallize(Pokemon pokemon)
    {
        // Check for Ogerpon with invalid tera types
        if (pokemon.Species.BaseSpecies == SpecieId.Ogerpon &&
            pokemon.TeraType is not (MoveType.Fire or MoveType.Grass or MoveType.Rock or MoveType.Water) &&
            (pokemon.Illusion == null || pokemon.Illusion.Species.BaseSpecies == SpecieId.Ogerpon))
        {
            Battle.Hint("If Ogerpon Terastallizes into a type other than Fire, Grass, Rock, or Water, the game softlocks.");
            return;
        }

        // End Illusion if disguised as Ogerpon or Terapagos
        if (pokemon.Illusion is { Species.BaseSpecies: SpecieId.Ogerpon or SpecieId.Terapagos })
        {
            Battle.SingleEvent(EventId.End, Battle.Library.Abilities[AbilityId.Illusion],
                pokemon.AbilityState, pokemon);
        }

        // Get the tera type
        MoveType type = pokemon.TeraType;

        // Add terastallize message to battle log
        if (Battle.DisplayUi)
        {
            Battle.Add("-terastallize", pokemon, type.ToString());
        }

        // Mark Pokemon as terastallized
        pokemon.Terastallized = type;

        // Disable terastallization for all allies
        foreach (Pokemon ally in pokemon.Side.Pokemon)
        {
            ally.CanTerastallize = null;
        }

        // Clear added type and update type visibility
        pokemon.AddedType = null;
        pokemon.KnownType = true;
        pokemon.ApparentType = [type.ConvertToPokemonType()];

        // Handle Ogerpon forme change
        if (pokemon.Species.BaseSpecies == SpecieId.Ogerpon)
        {
            // Determine the tera forme name
            string ogerponSpecies = pokemon.Species.BattleOnly?.ToString() ??
                                    pokemon.Species.Id.ToString();

            // Add "tera" suffix or change base to tealtera
            SpecieId teraForme = ogerponSpecies == "Ogerpon"
                ? SpecieId.OgerponTealtera
                : Enum.Parse<SpecieId>(ogerponSpecies + "Tera");

            pokemon.FormeChange(teraForme, null, true);
        }

        // Handle Terapagos forme change to Stellar
        if (pokemon.Species.Id == SpecieId.TerapagosTerastal)
        {
            pokemon.FormeChange(SpecieId.TerapagosStellar, null, true);
        }

        // Handle Morpeko forme regression
        if (pokemon.Species.BaseSpecies == SpecieId.Morpeko &&
            !pokemon.Transformed &&
            pokemon.BaseSpecies.Id != pokemon.Species.Id)
        {
            pokemon.FormeRegression = true;
            pokemon.BaseSpecies = pokemon.Species;
            pokemon.Details = pokemon.GetUpdatedDetails();
        }

        // Trigger AfterTerastallization event
        Battle.RunEvent(EventId.AfterTerastallization, pokemon);
    }
}