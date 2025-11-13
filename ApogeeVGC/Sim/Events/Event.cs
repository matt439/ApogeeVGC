using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Represents the current event context during battle execution.
/// Also serves as a container for dynamically registered event handlers (this.events in TypeScript).
/// </summary>
public class Event
{
    /// <summary>
    /// The current event ID being processed
    /// </summary>
    public EventId Id { get; set; }

    /// <summary>
    /// The source of the current event
    /// </summary>
    public SingleEventSource? Source { get; set; }

    /// <summary>
    /// The target of the current event
    /// </summary>
    public SingleEventTarget? Target { get; set; }

    /// <summary>
    /// The effect associated with the current event
    /// </summary>
    public IEffect? Effect { get; set; }

    /// <summary>
    /// Modifier value for the current event (used in damage calculations, etc.)
    /// </summary>
    public double? Modifier { get; set; }

    /// <summary>
    /// Storage for dynamically registered event handlers.
    /// Maps event IDs to lists of handlers with their priority/order metadata.
    /// Corresponds to this.events in TypeScript.
    /// </summary>
    private Dictionary<EventId, List<EventHandlerData>> RegisteredHandlers { get; } = new();

    /// <summary>
    /// Gets the delegate for a specific event, or null if no handler is registered.
    /// In TypeScript: this.events[callbackName]
    /// Returns the first handler's delegate for backward compatibility.
    /// For full handler list, use GetHandlers().
    /// </summary>
    public EffectDelegate? GetDelegate(EventId id)
    {
        if (RegisteredHandlers.TryGetValue(id, out var handlers) && handlers.Count > 0)
        {
            return handlers[0].Callback;
        }
        return null;
    }

    /// <summary>
    /// Gets all registered handlers for a specific event.
    /// Corresponds to iterating through this.events[callbackName] in TypeScript.
    /// </summary>
    public List<EventHandlerData> GetHandlers(EventId id)
    {
        return RegisteredHandlers.TryGetValue(id, out var handlers)
            ? handlers
            : [];
    }

    /// <summary>
    /// Registers a new event handler.
    /// Corresponds to this.onEvent() in TypeScript.
    /// </summary>
    public void RegisterHandler(EventId eventId, EventHandlerData handler)
    {
        if (!RegisteredHandlers.ContainsKey(eventId))
        {
            RegisteredHandlers[eventId] = [];
        }
        RegisteredHandlers[eventId].Add(handler);
    }

    /// <summary>
    /// Checks if any handlers are registered for a specific event.
    /// </summary>
    public bool HasHandlers(EventId id)
    {
        return RegisteredHandlers.ContainsKey(id) && RegisteredHandlers[id].Count > 0;
    }
}