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

    /// <summary>
    /// Gets the base EventId by removing any prefix from the current event.
    /// For example, AllyBasePower returns BasePower, SourceModifyAtk returns ModifyAtk.
    /// </summary>
    public EventId GetBaseEventId()
    {
        if (Prefix == EventPrefix.None)
        {
            return Id;
        }

        // The base event name is the Id with the prefix removed
        string idString = Id.ToString();
        string prefixString = Prefix.ToString();

        if (idString.StartsWith(prefixString))
        {
            string baseEventName = idString.Substring(prefixString.Length);
            if (Enum.TryParse<EventId>(baseEventName, out EventId baseId))
            {
                return baseId;
            }
        }

        return Id;
    }

    /// <summary>
    /// Creates an EventId by combining a prefix with a base event.
    /// For example, combining Ally + BasePower returns AllyBasePower.
    /// Returns the original eventId if prefix is None or if the combination doesn't exist.
    /// </summary>
    public static EventId CombinePrefixWithEvent(EventPrefix prefix, EventId baseEventId)
    {
        if (prefix == EventPrefix.None)
        {
            return baseEventId;
        }

        string combinedName = $"{prefix}{baseEventId}";
        if (Enum.TryParse<EventId>(combinedName, out EventId combinedId))
        {
            return combinedId;
        }

        // If the combination doesn't exist, return the base event
        return baseEventId;
    }
}