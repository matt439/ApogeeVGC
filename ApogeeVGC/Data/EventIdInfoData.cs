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
        [EventId.StallMove] = new EventIdInfo
        {
            Id = EventId.StallMove,
            UsesSpeed = true,
        },
        [EventId.TryHit] = new EventIdInfo
        {
            Id = EventId.TryHit,
            UsesSpeed = true,
            UsesLeftToRightOrder = true,
        },
        [EventId.Invulnerability] = new EventIdInfo
        {
            Id = EventId.Invulnerability,
            UsesSpeed = true,
            UsesLeftToRightOrder = true,
        },
        [EventId.DamagingHit] = new EventIdInfo
        {
            Id = EventId.DamagingHit,
            UsesSpeed = true,
            UsesLeftToRightOrder = true,
        },
        [EventId.EntryHazard] = new EventIdInfo
        {
            Id = EventId.EntryHazard,
            UsesSpeed = true,
            UsesLeftToRightOrder = true,
        },
        [EventId.Start] = new EventIdInfo
        {
            Id = EventId.Start,
            UsesSpeed = true,
        },
        [EventId.End] = new EventIdInfo
        {
            Id = EventId.End,
            UsesSpeed = true,
        },
        [EventId.TakeItem] = new EventIdInfo
        {
            Id = EventId.TakeItem,
            UsesSpeed = true,
        },
        [EventId.PrepareHit] = new EventIdInfo
        {
            Id = EventId.PrepareHit,
            UsesSpeed = true,
        },
        [EventId.ModifyPriority] = new EventIdInfo
        {
            Id = EventId.ModifyPriority,
            UsesSpeed = true,
        },
        [EventId.BeforeMove] = new EventIdInfo
        {
            Id = EventId.BeforeMove,
            UsesSpeed = true,
        },
        [EventId.Flinch] = new EventIdInfo
        {
            Id = EventId.Flinch,
            UsesSpeed = true,
        },
        [EventId.BasePower] = new EventIdInfo
        {
            Id = EventId.BasePower,
            UsesSpeed = true,
        },
        [EventId.ModifyAtk] = new EventIdInfo
        {
            Id = EventId.ModifyAtk,
            UsesSpeed = true,
        },
        [EventId.ModifyDef] = new EventIdInfo
        {
            Id = EventId.ModifyDef,
            UsesSpeed = true,
        },
        [EventId.ModifySpA] = new EventIdInfo
        {
            Id = EventId.ModifySpA,
            UsesSpeed = true,
        },
        [EventId.ModifySpD] = new EventIdInfo
        {
            Id = EventId.ModifySpD,
            UsesSpeed = true,
        },
        [EventId.ModifySpe] = new EventIdInfo
        {
            Id = EventId.ModifySpe,
            UsesSpeed = true,
        },
        [EventId.ModifyDamage] = new EventIdInfo
        {
            Id = EventId.ModifyDamage,
            UsesSpeed = true,
        },
        [EventId.Hit] = new EventIdInfo
        {
            Id = EventId.Hit,
            UsesSpeed = true,
        },
        [EventId.AfterMove] = new EventIdInfo
        {
            Id = EventId.AfterMove,
            UsesSpeed = true,
        },
        [EventId.Effectiveness] = new EventIdInfo
        {
            Id = EventId.Effectiveness,
            UsesSpeed = true,
        },
        [EventId.TryMove] = new EventIdInfo
        {
            Id = EventId.TryMove,
            UsesSpeed = true,
        },
        [EventId.Damage] = new EventIdInfo
        {
            Id = EventId.Damage,
            UsesSpeed = true,
        },
        [EventId.Boost] = new EventIdInfo
        {
            Id = EventId.Boost,
            UsesSpeed = true,
        },
        [EventId.SetStatus] = new EventIdInfo
        {
            Id = EventId.SetStatus,
            UsesSpeed = true,
        },
        [EventId.ModifyMove] = new EventIdInfo
        {
            Id = EventId.ModifyMove,
            UsesSpeed = true,
        },
        // TODO: Fill in the rest of the event infos
    };
}