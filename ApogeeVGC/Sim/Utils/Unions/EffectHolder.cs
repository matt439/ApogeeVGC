using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | Side | Field | Battle
/// </summary>
public abstract record EffectHolder
{
    public static implicit operator EffectHolder(Pokemon pokemon) => pokemon.CachedEffectHolder;
    public static implicit operator EffectHolder(Side side) => new SideEffectHolder(side);
    public static implicit operator EffectHolder(Field field) => new FieldEffectHolder(field);
    public static EffectHolder FromBattle(Battle battle) => new BattleEffectHolder(battle);
}

public record PokemonEffectHolder(Pokemon Pokemon) : EffectHolder;
public record SideEffectHolder(Side Side) : EffectHolder;
public record FieldEffectHolder(Field Field) : EffectHolder;
public record BattleEffectHolder(Battle Battle) : EffectHolder;
