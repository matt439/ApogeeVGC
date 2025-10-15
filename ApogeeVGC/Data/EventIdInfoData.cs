using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Events;

namespace ApogeeVGC.Data;

public record EventIdInfoData
{
    public IReadOnlyDictionary<EventId, EventIdInfo> EventData { get; }

    public EventIdInfoData()
    {
        EventData = new ReadOnlyDictionary<EventId, EventIdInfo>(_eventInfos);
    }

    private readonly Dictionary<EventId, EventIdInfo> _eventInfos = new()
    {
        [EventId.Accuracy] = new EventIdInfo
        {
            Id = EventId.Accuracy,
        },
        [EventId.SwitchIn] = new EventIdInfo
        {
            Id = EventId.SwitchIn,
            Suffix = EventSuffix.SwitchIn,
            UsesEffectOrder = true,
            UsesSpeed = true,
            UsesFractionalSpeed = true,
        },
        [EventId.RedirectTarget] = new EventIdInfo
        {
            Id = EventId.RedirectTarget,
            Suffix = EventSuffix.RedirectTarget,
            UsesEffectOrder = true,
            UsesSpeed = true,
        },
        [EventId.AnySwitchIn] = new EventIdInfo
        {
            Id = EventId.AnySwitchIn,
            Prefix = EventPrefix.Any,
            Suffix = EventSuffix.SwitchIn,
            BaseEventId = EventId.SwitchIn,
            UsesEffectOrder = true,
            UsesSpeed = true,
            UsesFractionalSpeed = true,
        },
        // TODO: Fill in the rest of the event infos
    };
}