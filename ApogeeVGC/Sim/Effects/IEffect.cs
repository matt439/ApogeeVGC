using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Effects;

public interface IEffect
{
    EffectType EffectType { get; }
    string Name { get; }
    string FullName { get; }
    EffectStateId EffectStateId { get; }
    
    /// <summary>
    /// Gets comprehensive event handler information including delegate, metadata, and type signature.
    /// This replaces the need for separate GetDelegate, GetPriority, GetOrder, and GetSubOrder calls.
    /// Returns null if the event is not implemented by this effect.
    /// </summary>
    EventHandlerInfo? GetEventHandlerInfo(EventId id);
}