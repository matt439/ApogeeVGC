using ApogeeVGC.Sim.Events;
using System.Reflection;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class EventIdExtensions
{
    public static EventInfoAttribute? GetEventInfo(this EventId eventId)
    {
        FieldInfo? field = typeof(EventId).GetField(eventId.ToString());
        return field?.GetCustomAttribute<EventInfoAttribute>();
    }

    public static EventScope GetScope(this EventId eventId)
    {
        return eventId.GetEventInfo()?.Scope ?? EventScope.Global;
    }

    public static bool HasOnPrefix(this EventId eventId)
    {
        return eventId.GetEventInfo()?.HasOnPrefix ?? false;
    }

    public static bool HasFoePrefix(this EventId eventId)
    {
        return eventId.GetEventInfo()?.HasFoePrefix ?? false;
    }
}