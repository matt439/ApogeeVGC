using ApogeeVGC.Data;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils;

public static class PokemonValidator
{
    public static bool IsValid(Library library, Pokemon pokemon)
    {
        // Implement validation logic for the Pokemon object
        // For example, check if the species, moves, item, ability, etc. are valid
        // This is a placeholder implementation

        if (!pokemon.Evs.IsValidEvs())
        {
            throw new ArgumentException("Invalid EVs in Pokemon.");
        }
        if (!pokemon.Ivs.IsValidIvs())
        {
            throw new ArgumentException("Invalid IVs in Pokemon.");
        }

        return true;
    }
}