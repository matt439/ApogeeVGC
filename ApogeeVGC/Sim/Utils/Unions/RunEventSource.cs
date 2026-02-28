using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Stack-allocated discriminated union replacing the former abstract record hierarchy.
/// Variants: Pokemon | False | PokemonType
/// </summary>
public readonly struct RunEventSource
{
    private enum Tag : byte { None, Pokemon, False, Type }

    private readonly Tag _tag;
    private readonly object? _reference; // Pokemon
    private readonly PokemonType _pokemonType;

    private RunEventSource(Tag tag, object? reference, PokemonType pokemonType)
    {
        _tag = tag;
        _reference = reference;
        _pokemonType = pokemonType;
    }

    public bool IsPokemon => _tag == Tag.Pokemon;
    public bool IsFalse => _tag == Tag.False;
    public bool IsType => _tag == Tag.Type;

    public Pokemon Pokemon => (Pokemon)_reference!;
    public PokemonType Type => _pokemonType;

    public static RunEventSource FromPokemon(Pokemon pokemon) => new(Tag.Pokemon, pokemon, default);
    public static RunEventSource FromFalse() => new(Tag.False, null, default);
    public static RunEventSource FromType(PokemonType type) => new(Tag.Type, null, type);

    public static RunEventSource? FromNullablePokemon(Pokemon? pokemon)
        => pokemon is null ? null : (RunEventSource?)new RunEventSource(Tag.Pokemon, pokemon, default);

    public static implicit operator RunEventSource(Pokemon pokemon) => FromPokemon(pokemon);
    public static implicit operator RunEventSource(PokemonType type) => FromType(type);
}
