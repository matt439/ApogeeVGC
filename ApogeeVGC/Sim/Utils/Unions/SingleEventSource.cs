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
/// Stack-allocated discriminated union replacing the former abstract record hierarchy.
/// Variants: Pokemon | Effect | False | PokemonType
/// </summary>
public readonly struct SingleEventSource
{
    private enum Tag : byte { None, Pokemon, Effect, False, PokemonType }

    private readonly Tag _tag;
    private readonly object? _reference; // Pokemon or IEffect
    private readonly PokemonType _pokemonType;

    private SingleEventSource(Tag tag, object? reference, PokemonType pokemonType)
    {
        _tag = tag;
        _reference = reference;
        _pokemonType = pokemonType;
    }

    public bool IsPokemon => _tag == Tag.Pokemon;
    public bool IsEffect => _tag == Tag.Effect;
    public bool IsFalse => _tag == Tag.False;
    public bool IsPokemonType => _tag == Tag.PokemonType;

    public Pokemon Pokemon => (Pokemon)_reference!;
    public IEffect Effect => (IEffect)_reference!;
    public PokemonType Type => _pokemonType;

    public static SingleEventSource FromPokemon(Pokemon pokemon) => new(Tag.Pokemon, pokemon, default);
    public static SingleEventSource FromEffect(IEffect effect) => new(Tag.Effect, effect, default);
    public static SingleEventSource FromFalse() => new(Tag.False, null, default);
    public static SingleEventSource FromPokemonType(PokemonType type) => new(Tag.PokemonType, null, type);

    public static SingleEventSource? FromNullablePokemon(Pokemon? pokemon)
        => pokemon is null ? null : (SingleEventSource?)new SingleEventSource(Tag.Pokemon, pokemon, default);

    public static implicit operator SingleEventSource(Pokemon pokemon) => FromPokemon(pokemon);

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

    public static implicit operator SingleEventSource(PokemonType type) => FromPokemonType(type);
}
