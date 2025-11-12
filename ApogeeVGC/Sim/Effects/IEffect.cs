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
    /// <param name="id">The base event identifier</param>
    /// <param name="prefix">Optional event prefix (Foe, Source, Any, Ally)</param>
    /// <param name="suffix">Optional event suffix (Callback, etc.)</param>
    /// <returns>Event handler information if the event is implemented, null otherwise</returns>
    EventHandlerInfo? GetEventHandlerInfo(EventId id, EventPrefix? prefix = null, EventSuffix? suffix = null);
}