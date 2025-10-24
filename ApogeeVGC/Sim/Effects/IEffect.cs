using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Effects;

public interface IEffect
{
    EffectType EffectType { get; }
    string Name { get; }
    string FullName { get; }
    EffectStateId EffectStateId { get; }
    EffectDelegate? GetDelegate(EventId id);

    IntFalseUnion? GetOrder(EventId id);
    int? GetPriority(EventId id);
    int? GetSubOrder(EventId id);
}