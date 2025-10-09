using ApogeeVGC.Sim.Effects;

namespace ApogeeVGC.Sim.GameObjects;

public record Format : IEffect, IBasicEffect
{
    public EffectType EffectType => EffectType.Format;
    public required string Name { get; init; }
    public bool AffectsFainted { get; init; }
}