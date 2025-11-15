using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | Pokemon? | false | Value
/// </summary>
public abstract record RunEventSource
{
    public static implicit operator RunEventSource(Pokemon pokemon) => new PokemonRunEventSource(pokemon);
    public static RunEventSource? FromNullablePokemon(Pokemon? pokemon)
{
        return pokemon is null ? null : new PokemonRunEventSource(pokemon);
    }
    public static RunEventSource FromFalse() => new FalseRunEventSource();
 public static implicit operator RunEventSource(PokemonType type) => new TypeRunEventSource(type);
}

public record PokemonRunEventSource(Pokemon Pokemon) : RunEventSource;
public record FalseRunEventSource : RunEventSource;
public record TypeRunEventSource(PokemonType Type) : RunEventSource;
