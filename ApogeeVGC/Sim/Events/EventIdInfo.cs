namespace ApogeeVGC.Sim.Events;

public record EventIdInfo
{
    public EventId Id { get; init; }
    public EventPrefix Prefix { get; init; } = EventPrefix.None;
    public EventSuffix Suffix { get; init; } = EventSuffix.None;
    public EventType Type { get; init; } = EventType.None;

    /// <summary>
    /// Indicates whether this event uses effect order for tie-breaking.
    /// True for events like SwitchIn and RedirectTarget where creation order matters.
    /// </summary>
    public bool UsesEffectOrder { get; init; }

    /// <summary>
    /// Indicates whether this event requires speed calculation for Pokemon holders.
    /// True for most events that involve Pokemon actions.
    /// </summary>
    public bool UsesSpeed { get; init; } = true;

    /// <summary>
    /// Indicates whether this event uses fractional speed adjustments for switch-ins.
    /// Only true for events ending with "SwitchIn".
    /// </summary>
    public bool UsesFractionalSpeed { get; init; }

    /// <summary>
    /// Indicates whether this event uses left-to-right ordering instead of priority.
    /// True for events like Invulnerability, TryHit, DamagingHit, EntryHazard.
    /// </summary>
    public bool UsesLeftToRightOrder { get; init; }
}