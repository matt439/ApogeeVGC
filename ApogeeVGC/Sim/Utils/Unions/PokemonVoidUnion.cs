using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | void
/// </summary>
public abstract record PokemonVoidUnion
{
    public static implicit operator PokemonVoidUnion(Pokemon pokemon) => new PokemonPokemonVoidUnion(pokemon);
    public static implicit operator PokemonVoidUnion(VoidReturn value) => new VoidPokemonVoidUnion(value);
    public static PokemonVoidUnion FromVoid() => new VoidPokemonVoidUnion(new VoidReturn());
}

public record PokemonPokemonVoidUnion(Pokemon Pokemon) : PokemonVoidUnion;
public record VoidPokemonVoidUnion(VoidReturn Value) : PokemonVoidUnion;
