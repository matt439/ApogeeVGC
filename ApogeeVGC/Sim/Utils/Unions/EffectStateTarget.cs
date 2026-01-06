using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Pokemon | Side | Field | Battle
/// </summary>
public abstract record EffectStateTarget
{
    public static implicit operator EffectStateTarget(Pokemon pokemon) => new PokemonEffectStateTarget(pokemon);
    public static implicit operator EffectStateTarget(Side side) => new SideEffectStateTarget(side);
    public static implicit operator EffectStateTarget(Field field) => new FieldEffectStateTarget(field);
    public static EffectStateTarget FromBattle(Battle battle) => new BattleEffectStateTarget(battle);
}

public record PokemonEffectStateTarget(Pokemon Pokemon) : EffectStateTarget;
public record SideEffectStateTarget(Side Side) : EffectStateTarget;
public record FieldEffectStateTarget(Field Field) : EffectStateTarget;
public record BattleEffectStateTarget(Battle Battle) : EffectStateTarget;
