using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | false | void
/// </summary>
public abstract record PokemonFalseVoidUnion
{
    public static implicit operator PokemonFalseVoidUnion(Pokemon pokemon) => new PokemonPokemonFalseVoidUnion(pokemon);
    public static implicit operator PokemonFalseVoidUnion(VoidReturn value) => new VoidPokemonFalseVoidUnion(value);
    public static implicit operator PokemonFalseVoidUnion(bool value) => value ? throw new ArgumentException("Only false is supported") : new FalsePokemonFalseVoidUnion();
    
    public static PokemonFalseVoidUnion FromVoid() => new VoidPokemonFalseVoidUnion(new VoidReturn());
    public static PokemonFalseVoidUnion FromFalse() => new FalsePokemonFalseVoidUnion();
    
    public static PokemonFalseVoidUnion? FromNullablePokemon(Pokemon? pokemon)
    {
        return pokemon is null ? null : new PokemonPokemonFalseVoidUnion(pokemon);
    }
}

public record PokemonPokemonFalseVoidUnion(Pokemon Pokemon) : PokemonFalseVoidUnion;
public record FalsePokemonFalseVoidUnion : PokemonFalseVoidUnion;
public record VoidPokemonFalseVoidUnion(VoidReturn Value) : PokemonFalseVoidUnion;
