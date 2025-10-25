using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// The 'void' type, representing the absence of a value.
/// </summary>
public record VoidReturn;

/// <summary>
/// Represents the 'undefined' type, indicating an uninitialized or absent value.
/// </summary>
public record Undefined;

/// <summary>
/// Represents an empty type with no data. In TypeScript this is "" (empty string).
/// Empty should be used where Battle.NOT_FAIL ("") is used in the original code.
/// </summary>
public record Empty;



/// <summary>
/// (int | bool | undefined)[]
/// </summary>
public class SpreadMoveDamage : List<BoolIntUndefinedUnion>
{
    public SpreadMoveDamage()
    {
    }

    public SpreadMoveDamage(SpreadMoveDamage other)
    {
        foreach (BoolIntUndefinedUnion item in other)
        {
            Add(item);
        }
    }
}

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