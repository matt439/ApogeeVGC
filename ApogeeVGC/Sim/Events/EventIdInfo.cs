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
            string baseEventName = idString[prefixString.Length..];
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
    /// <param name="prefix">The prefix to add to the base event</param>
    /// <param name="baseEventId">The base event to prefix</param>
    /// <param name="eventLookup">Optional dictionary of EventIdInfo for metadata-based lookup</param>
    /// <returns>The combined EventId, or the base event if combination doesn't exist</returns>
    public static EventId CombinePrefixWithEvent(EventPrefix prefix, EventId baseEventId,
        IReadOnlyDictionary<EventId, EventIdInfo>? eventLookup = null)
    {
        if (prefix == EventPrefix.None)
        {
            return baseEventId;
        }

        // If we have event metadata, use it for a more efficient lookup
        if (eventLookup != null)
        {
            // Get the suffix from the base event (if it has one)
            EventSuffix suffix = eventLookup.TryGetValue(baseEventId, out EventIdInfo? baseInfo)
                ? baseInfo.Suffix
                : EventSuffix.None;

            // Search for an event with matching prefix, base event, and suffix
            foreach ((EventId candidateId, EventIdInfo candidateInfo) in eventLookup)
            {
                // Check if this event has the right prefix and suffix
                if (candidateInfo.Prefix == prefix && candidateInfo.Suffix == suffix)
                {
                    // Get the base event of this candidate
                    EventId candidateBase = candidateInfo.GetBaseEventId();
                    
                    // If the base matches, we found our prefixed event
                    if (candidateBase == baseEventId)
                    {
                        return candidateId;
                    }
                }
            }
            
            // No match found with metadata lookup, return base event
            return baseEventId;
        }

        // Fallback to string manipulation if no metadata available
        string combinedName = $"{prefix}{baseEventId}";
        if (Enum.TryParse(combinedName, out EventId combinedId))
        {
            return combinedId;
        }

        // If the combination doesn't exist, return the base event
        return baseEventId;
    }

    /// <summary>
    /// Checks if this event has a specific prefix.
    /// </summary>
    public bool HasPrefix(EventPrefix prefix) => Prefix == prefix;

    /// <summary>
    /// Checks if this event has a specific suffix.
    /// </summary>
    public bool HasSuffix(EventSuffix suffix) => Suffix == suffix;

    /// <summary>
    /// Gets the base event name without prefix or suffix information.
    /// For events like "AnySwitchIn", this would conceptually be "SwitchIn" without the "Any" prefix.
    /// This is useful for finding related events with different prefixes.
    /// </summary>
    public EventId GetCoreEventId()
    {
        // If no prefix, just return the base event
        if (Prefix == EventPrefix.None)
        {
            return GetBaseEventId();
        }

        // For prefixed events, return the base without the prefix
        return GetBaseEventId();
    }
}