using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

public class SpreadMoveTargets : List<PokemonFalseUnion>
{
    public static SpreadMoveTargets FromPokemonList(List<Pokemon> pokemons)
    {
   var spreadTargets = new SpreadMoveTargets();
        spreadTargets.AddRange(pokemons.Select(pokemon => new PokemonPokemonUnion(pokemon)));
        return spreadTargets;
    }

    public static List<Pokemon> ToPokemonList(SpreadMoveTargets targets)
    {
        return targets
       .OfType<PokemonPokemonUnion>()
            .Select(union => union.Pokemon)
            .ToList();
    }
}
