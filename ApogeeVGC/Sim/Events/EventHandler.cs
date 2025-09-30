using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.Events;

public interface IEventHandler
{
    EventId EventId { get; }
    string Name { get; }
    Delegate Callback { get; }
    EventFilters Filters { get; }
}


public record EventFilters
{
    public bool PrefixOn { get; init; }
    public bool PrefixOnFoe { get; init; }
    public EventScope Scope { get; init; }
    public string Description { get; init; }

    public EventFilters(EventId eventId)
    {
        EventInfoAttribute? eventInfo = eventId.GetEventInfo();

        PrefixOn = eventInfo?.HasOnPrefix ?? false;
        PrefixOnFoe = eventInfo?.HasFoePrefix ?? false;
        Scope = eventInfo?.Scope ?? EventScope.Global;
        Description = eventInfo?.Description ?? string.Empty;
    }
}

public record AbilityEventHandler(EventId EventId, string Name, Delegate Callback) : IEventHandler
{
    public EventId EventId { get; } = EventId;
    public string Name { get; } = Name ?? throw new ArgumentNullException(nameof(Name), "name");
    public Delegate Callback { get; } = Callback ?? throw new ArgumentNullException(nameof(Callback), "callback");
    public EventFilters Filters { get; } = new(EventId);
}