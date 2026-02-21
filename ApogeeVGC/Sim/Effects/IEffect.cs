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
    /// Whether this effect has any event handlers registered.
    /// Used for fast-path skipping in event handler resolution.
    /// </summary>
    bool HasAnyEventHandlers { get; }

    /// <summary>
    /// Whether this effect has any event handlers with a non-None prefix (Ally, Foe, Source, Any).
    /// Used to skip effects entirely during prefixed event handler discovery passes.
    /// </summary>
    bool HasPrefixedHandlers { get; }

    /// <summary>
    /// Gets comprehensive event handler information including delegate, metadata, and type signature.
    /// Returns null if the event is not implemented by this effect.
    /// Uses a pre-computed cache for O(1) lookups.
    /// </summary>
    /// <param name="id">The base event identifier</param>
    /// <param name="prefix">Event prefix (None, Foe, Source, Any, Ally)</param>
    /// <param name="suffix">Event suffix (None, SwitchIn, RedirectTarget)</param>
    /// <returns>Event handler information if the event is implemented, null otherwise</returns>
    EventHandlerInfo? GetEventHandlerInfo(EventId id, EventPrefix prefix = EventPrefix.None, EventSuffix suffix = EventSuffix.None);
}