using ApogeeVGC.Sim.Effects;

namespace ApogeeVGC.Sim.GameObjects;

public record Format : IEffect
{
    public EffectType EffectType => EffectType.Format;
}