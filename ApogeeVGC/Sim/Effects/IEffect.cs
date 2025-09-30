using ApogeeVGC.Sim.Events;

namespace ApogeeVGC.Sim.Effects;

public interface IEffect
{
    EffectType EffectType { get; }
    // IReadOnlyList<IEventHandler> EventHandlers { get; }
}