using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Effects;

public interface IEffect
{
    EffectType EffectType { get; }
    string Name { get; }
    EffectStateId EffectStateId { get; }
}