using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

public class SpreadMoveTargets : List<PokemonFalseUnion>
{
    public static SpreadMoveTargets FromPokemonList(List<Pokemon> pokemons)
    {
        var spreadTargets = new SpreadMoveTargets();
        for (int i = 0; i < pokemons.Count; i++)
        {
            spreadTargets.Add(new PokemonPokemonUnion(pokemons[i]));
        }
        return spreadTargets;
    }

    public static List<Pokemon> ToPokemonList(SpreadMoveTargets targets)
    {
        var result = new List<Pokemon>(targets.Count);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] is PokemonPokemonUnion union)
            {
                result.Add(union.Pokemon);
            }
        }
        return result;
    }

    /// <summary>
    /// Returns the first Pokemon in the targets list without allocating a List.
    /// Used by SpreadMoveHit which only needs the first target.
    /// </summary>
    public static Pokemon FirstPokemon(SpreadMoveTargets targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] is PokemonPokemonUnion union)
            {
                return union.Pokemon;
            }
        }
        throw new InvalidOperationException("No Pokemon found in SpreadMoveTargets");
    }
}
