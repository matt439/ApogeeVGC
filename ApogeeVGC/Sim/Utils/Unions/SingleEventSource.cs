using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | Effect | false | PokemonType
/// </summary>
public abstract record SingleEventSource
{
    public static implicit operator SingleEventSource(Pokemon pokemon) =>
        new PokemonSingleEventSource(pokemon);

    public static SingleEventSource? FromNullablePokemon(Pokemon? pokemon)
    {
        return pokemon is null ? null : new PokemonSingleEventSource(pokemon);
    }

    public static implicit operator SingleEventSource(Ability ability) =>
     EffectUnionFactory.ToSingleEventSource(ability);

    public static implicit operator SingleEventSource(Item item) =>
        EffectUnionFactory.ToSingleEventSource(item);

public static implicit operator SingleEventSource(ActiveMove activeMove) =>
   EffectUnionFactory.ToSingleEventSource(activeMove);

    public static implicit operator SingleEventSource(Species species) =>
 EffectUnionFactory.ToSingleEventSource(species);

    public static implicit operator SingleEventSource(Condition condition) =>
EffectUnionFactory.ToSingleEventSource(condition);

    public static implicit operator SingleEventSource(Format format) =>
        EffectUnionFactory.ToSingleEventSource(format);

    public static SingleEventSource FromFalse() => new FalseSingleEventSource();
    public static implicit operator SingleEventSource(PokemonType type) => new PokemonTypeSingleEventSource(type);
}

public record PokemonSingleEventSource(Pokemon Pokemon) : SingleEventSource;
public record EffectSingleEventSource(IEffect Effect) : SingleEventSource;
public record FalseSingleEventSource : SingleEventSource;
public record PokemonTypeSingleEventSource(PokemonType Type) : SingleEventSource;
