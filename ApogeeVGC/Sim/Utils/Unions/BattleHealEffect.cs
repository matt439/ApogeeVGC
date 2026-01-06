using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Effect | 'drain'
/// </summary>
public abstract record BattleHealEffect
{
    public static BattleHealEffect FromIEffect(IEffect effect) => new EffectBattleHealEffect(effect);
 public static BattleHealEffect FromDrain() => new DrainBattleHealEffect();
}

public record EffectBattleHealEffect(IEffect Effect) : BattleHealEffect;
public record DrainBattleHealEffect : BattleHealEffect;
