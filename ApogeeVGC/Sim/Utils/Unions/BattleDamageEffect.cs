using ApogeeVGC.Sim.Effects;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Effect | 'drain' | 'recoil'
/// </summary>
public abstract record BattleDamageEffect
{
    public static BattleDamageEffect FromIEffect(IEffect effect) => new EffectBattleDamageEffect(effect);
    public static BattleDamageEffect FromDrain() => new DrainBattleDamageEffect();
 public static BattleDamageEffect FromRecoil() => new RecoilBattleDamageEffect();
}

public record EffectBattleDamageEffect(IEffect Effect) : BattleDamageEffect;
public record DrainBattleDamageEffect : BattleDamageEffect;
public record RecoilBattleDamageEffect : BattleDamageEffect;
