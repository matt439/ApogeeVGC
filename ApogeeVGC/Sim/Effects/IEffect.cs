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
    /// Gets the delegate for the specified event (legacy method).
    /// Consider using GetEventHandlerInfo for type-safe event information.
    /// </summary>
    EffectDelegate? GetDelegate(EventId id);

  /// <summary>
  /// Gets comprehensive event handler information including delegate, metadata, and type signature.
  /// This replaces the need for separate GetDelegate, GetPriority, GetOrder, and GetSubOrder calls.
  /// Returns null if the event is not implemented by this effect.
  /// </summary>
  EventHandlerInfo? GetEventHandlerInfo(EventId id);

    IntFalseUnion? GetOrder(EventId id);
    int? GetPriority(EventId id);
    int? GetSubOrder(EventId id);
}