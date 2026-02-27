using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

public enum SingleEventTargetKind : byte
{
    None = 0,
    Pokemon,
    Side,
    Field,
    Battle,
}

/// <summary>
/// Pokemon | Side | Field | Battle | Pokemon?
/// Zero-allocation struct-based discriminated union.
/// </summary>
public readonly struct SingleEventTarget
{
    public SingleEventTargetKind Kind { get; }
    private readonly object? _value;

    private SingleEventTarget(SingleEventTargetKind kind, object? value)
    {
        Kind = kind;
        _value = value;
    }

    public Pokemon Pokemon => (Pokemon)_value!;
    public Side Side => (Side)_value!;
    public Field Field => (Field)_value!;
    public Battle Battle => (Battle)_value!;

    public bool IsNone => Kind == SingleEventTargetKind.None;

    public static implicit operator SingleEventTarget(Pokemon pokemon) =>
        new(SingleEventTargetKind.Pokemon, pokemon);

    public static implicit operator SingleEventTarget(Side side) =>
        new(SingleEventTargetKind.Side, side);

    public static implicit operator SingleEventTarget(Field field) =>
        new(SingleEventTargetKind.Field, field);

    public static SingleEventTarget FromBattle(Battle battle) =>
        new(SingleEventTargetKind.Battle, battle);

    public static SingleEventTarget? FromNullablePokemon(Pokemon? pokemon) =>
        pokemon is null ? null : (SingleEventTarget?)new SingleEventTarget(SingleEventTargetKind.Pokemon, pokemon);
}
