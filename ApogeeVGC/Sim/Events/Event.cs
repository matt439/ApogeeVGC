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
    /// Lazy-initialized to avoid allocating the dictionary for the vast majority of
    /// Event instances that never register handlers.
    /// </summary>
    private Dictionary<EventId, List<EventHandlerData>>? _registeredHandlers;

    /// <summary>
    /// Gets all registered handlers for a specific event.
    /// Corresponds to iterating through this.events[callbackName] in TypeScript.
    /// </summary>
    public List<EventHandlerData> GetHandlers(EventId id)
    {
        return _registeredHandlers != null && _registeredHandlers.TryGetValue(id, out var handlers)
            ? handlers
            : [];
    }

    /// <summary>
    /// Registers a new event handler.
    /// Corresponds to this.onEvent() in TypeScript.
    /// </summary>
    public void RegisterHandler(EventId eventId, EventHandlerData handler)
    {
        _registeredHandlers ??= new();
        if (!_registeredHandlers.ContainsKey(eventId))
        {
            _registeredHandlers[eventId] = [];
        }
        _registeredHandlers[eventId].Add(handler);
    }

    /// <summary>
    /// Checks if any handlers are registered for a specific event.
    /// </summary>
    public bool HasHandlers(EventId id)
    {
        return _registeredHandlers != null &&
               _registeredHandlers.TryGetValue(id, out var handlers) &&
               handlers.Count > 0;
    }
}