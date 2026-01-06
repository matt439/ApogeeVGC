using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | int
/// </summary>
public abstract record PokemonIntUnion
{
    public static implicit operator PokemonIntUnion(Pokemon pokemon) => new PokemonPokemonIntUnion(pokemon);
 public static implicit operator PokemonIntUnion(int value) => new IntPokemonIntUnion(value);
}

public record PokemonPokemonIntUnion(Pokemon Pokemon) : PokemonIntUnion;
public record IntPokemonIntUnion(int Value) : PokemonIntUnion;
