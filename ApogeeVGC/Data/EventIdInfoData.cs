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
            UsesSpeed = true,
        },
        [EventId.FieldResidual] = new EventIdInfo
        {
            Id = EventId.FieldResidual,
            FieldEventId = EventId.FieldResidual,
            BaseEventId = EventId.Residual,
            UsesSpeed = true,
        },
        [EventId.SideResidual] = new EventIdInfo
        {
            Id = EventId.SideResidual,
            SideEventId = EventId.SideResidual,
            BaseEventId = EventId.Residual,
            UsesSpeed = true,
        },
        [EventId.BeforeTurn] = new EventIdInfo
        {
            Id = EventId.BeforeTurn,
            UsesSpeed = true,
        },
        [EventId.Update] = new EventIdInfo
        {
            Id = EventId.Update,
            UsesSpeed = true,
        },
        [EventId.EmergencyExit] = new EventIdInfo
        {
            Id = EventId.EmergencyExit,
            UsesSpeed = true,
        },
        [EventId.DisableMove] = new EventIdInfo
        {
            Id = EventId.DisableMove,
            UsesSpeed = true,
        },
        [EventId.TrapPokemon] = new EventIdInfo
        {
            Id = EventId.TrapPokemon,
            UsesSpeed = true,
        },
        [EventId.MaybeTrapPokemon] = new EventIdInfo
        {
            Id = EventId.MaybeTrapPokemon,
            UsesSpeed = true,
        },
        [EventId.FoeMaybeTrapPokemon] = new EventIdInfo
        {
            Id = EventId.FoeMaybeTrapPokemon,
            Prefix = EventPrefix.Foe,
            BaseEventId = EventId.MaybeTrapPokemon,
            UsesSpeed = true,
        },
        [EventId.BeforeSwitchOut] = new EventIdInfo
        {
            Id = EventId.BeforeSwitchOut,
            UsesSpeed = true,
        },
        [EventId.ModifyPriority] = new EventIdInfo
        {
            Id = EventId.ModifyPriority,
            UsesSpeed = true,
        },
        [EventId.Start] = new EventIdInfo
        {
            Id = EventId.Start,
            UsesSpeed = true,
        },
        [EventId.CheckShow] = new EventIdInfo
        {
            Id = EventId.CheckShow,
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
        [EventId.AllyTryHitSide] = new EventIdInfo
        {
            Id = EventId.AllyTryHitSide,
            Prefix = EventPrefix.Ally,
            BaseEventId = EventId.TryHitSide,
            UsesSpeed = true,
        },
        [EventId.TryHitSide] = new EventIdInfo
        {
            Id = EventId.TryHitSide,
            UsesSpeed = true,
        },
        // Add more events as needed
    };
}