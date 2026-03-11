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

/// <summary>
/// Represents null in Showdown's targets array.
/// Used when a Substitute absorbed the hit — the target is "gone" but self effects still apply.
/// In Showdown: targets[i] = null after HIT_SUBSTITUTE, selfDrops checks target === false (not null).
/// </summary>
public record NullPokemonUnion : PokemonFalseUnion;
