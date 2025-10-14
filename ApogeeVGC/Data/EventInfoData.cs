using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Events;

namespace ApogeeVGC.Data;

public record EventInfoData
{
    public IReadOnlyDictionary<EventId, EventIdInfo> EventData { get; }

    public EventInfoData()
    {
        EventData = new ReadOnlyDictionary<EventId, EventIdInfo>(_eventInfos);
    }

    private readonly Dictionary<EventId, EventIdInfo> _eventInfos = new()
    {
        [EventId.Accuracy] = new EventIdInfo
        {
            Id = EventId.Accuracy,
        },
        // TODO: Fill in the rest of the event infos
    };
}