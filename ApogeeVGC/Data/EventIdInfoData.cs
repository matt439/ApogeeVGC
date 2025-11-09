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
        [EventId.Residual] = new EventIdInfo
        {
            Id = EventId.Residual,
            FieldEventId = EventId.FieldResidual,
            SideEventId = EventId.SideResidual,
            UsesSpeed = true,
        },
        [EventId.FieldResidual] = new EventIdInfo
        {
            Id = EventId.FieldResidual,
            BaseEventId = EventId.Residual,
            UsesSpeed = true,
        },
        [EventId.SideResidual] = new EventIdInfo
        {
            Id = EventId.SideResidual,
            BaseEventId = EventId.Residual,
            UsesSpeed = true,
        },
        // TODO: Fill in the rest of the event infos
    };
}