using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// pokemon | false
/// </summary>
public abstract record PokemonFalseUnion
{
public static implicit operator PokemonFalseUnion(Pokemon pokemon) => new PokemonPokemonUnion(pokemon);
    public static PokemonFalseUnion FromFalse() => new FalsePokemonUnion();

    public static PokemonFalseUnion? FromNullablePokemon(Pokemon? pokemon)
    {
        return pokemon is null ? null : new PokemonPokemonUnion(pokemon);
    }
}

public record PokemonPokemonUnion(Pokemon Pokemon) : PokemonFalseUnion;
public record FalsePokemonUnion : PokemonFalseUnion;
