using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

public enum RunEventTargetKind : byte
{
    None = 0,
    Pokemon,
    PokemonArray,
    Side,
    Battle,
    Field,
}

/// <summary>
/// Pokemon | Pokemon[] | Side | Battle | PokemonSideBattleUnion? | PokemonSideBattleUnion | Field | Pokemon?
/// Zero-allocation struct-based discriminated union.
/// </summary>
public readonly struct RunEventTarget
{
    public RunEventTargetKind Kind { get; }
    private readonly object? _value;

    private RunEventTarget(RunEventTargetKind kind, object? value)
    {
        Kind = kind;
        _value = value;
    }

    public Pokemon Pokemon => (Pokemon)_value!;
    public Pokemon[] PokemonList => (Pokemon[])_value!;
    public Side Side => (Side)_value!;
    public Battle Battle => (Battle)_value!;
    public Field Field => (Field)_value!;

    public bool IsNone => Kind == RunEventTargetKind.None;

    public static implicit operator RunEventTarget(Pokemon pokemon) =>
        new(RunEventTargetKind.Pokemon, pokemon);

    public static RunEventTarget? FromNullablePokemon(Pokemon? pokemon) =>
        pokemon is null ? null : (RunEventTarget?)new RunEventTarget(RunEventTargetKind.Pokemon, pokemon);

    public static implicit operator RunEventTarget(Pokemon[] pokemonList) =>
        new(RunEventTargetKind.PokemonArray, pokemonList);

    public static implicit operator RunEventTarget(Side side) =>
        new(RunEventTargetKind.Side, side);

    public static RunEventTarget FromBattle(Battle battle) =>
        new(RunEventTargetKind.Battle, battle);

    public static RunEventTarget? FromNullablePokemonSideBattleUnion(PokemonSideBattleUnion? target)
    {
        return target switch
        {
            null => null,
            PokemonSideBattlePokemon pokemon => (RunEventTarget?)new RunEventTarget(RunEventTargetKind.Pokemon, pokemon.Pokemon),
            PokemonSideBattleNullablePokemon nullablePokemon => nullablePokemon.Pokemon is null
                ? null
                : (RunEventTarget?)new RunEventTarget(RunEventTargetKind.Pokemon, nullablePokemon.Pokemon),
            PokemonSideBattleSide side => (RunEventTarget?)new RunEventTarget(RunEventTargetKind.Side, side.Side),
            PokemonSideBattleBattle battle => (RunEventTarget?)new RunEventTarget(RunEventTargetKind.Battle, battle.Battle),
            _ => throw new InvalidOperationException("Cannot convert to RunEventTarget"),
        };
    }

    public static RunEventTarget FromPokemonSideBattleUnion(PokemonSideBattleUnion target)
    {
        return target switch
        {
            PokemonSideBattlePokemon pokemon => new RunEventTarget(RunEventTargetKind.Pokemon, pokemon.Pokemon),
            PokemonSideBattleSide side => new RunEventTarget(RunEventTargetKind.Side, side.Side),
            PokemonSideBattleBattle battle => new RunEventTarget(RunEventTargetKind.Battle, battle.Battle),
            _ => throw new InvalidOperationException("Cannot convert to RunEventTarget"),
        };
    }

    public static implicit operator RunEventTarget(Field field) =>
        new(RunEventTargetKind.Field, field);

    /// <summary>
    /// Converts to a SingleEventTarget if the kind is Pokemon; null otherwise.
    /// </summary>
    public SingleEventTarget? ToSingleEventTarget()
    {
        return Kind == RunEventTargetKind.Pokemon
            ? (SingleEventTarget?)Pokemon
            : null;
    }
}
