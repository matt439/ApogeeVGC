using ApogeeVGC.Sim.Effects;

namespace ApogeeVGC.Sim.GameObjects;

public record Format : EffectMethods, IEffect
{
    public EffectType EffectType => EffectType.Format;
}