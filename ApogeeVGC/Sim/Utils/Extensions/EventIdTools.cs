using System.Reflection;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;

namespace ApogeeVGC.Sim.Utils.Extensions;

public enum EventPropertyType
{
    Order,
    Priority,
    SubOrder,
}

public static class EventIdMapper
{
    private static readonly Dictionary<(EventId baseEvent, EventPrefix prefix), EventId> PrefixMap = new()
    {
        // BasePower variants
        [(EventId.BasePower, EventPrefix.Ally)] = EventId.AllyBasePower,
        [(EventId.BasePower, EventPrefix.Foe)] = EventId.FoeBasePower,
        [(EventId.BasePower, EventPrefix.Source)] = EventId.SourceBasePower,
        [(EventId.BasePower, EventPrefix.Any)] = EventId.AnyBasePower,

        // Invulnerability variants
        [(EventId.Invulnerability, EventPrefix.Source)] = EventId.SourceInvulnerability,
        [(EventId.Invulnerability, EventPrefix.Any)] = EventId.AnyInvulnerability,

        // ModifyAccuracy variants
        [(EventId.ModifyAccuracy, EventPrefix.Source)] = EventId.SourceModifyAccuracy,
        [(EventId.ModifyAccuracy, EventPrefix.Any)] = EventId.AnyModifyAccuracy,

        // Faint variants
        [(EventId.Faint, EventPrefix.Any)] = EventId.AnyFaint,

        // PrepareHit variants
        [(EventId.PrepareHit, EventPrefix.Any)] = EventId.AnyPrepareHit,

        // SwitchIn variants
        [(EventId.SwitchIn, EventPrefix.Any)] = EventId.AnySwitchIn,

        // ModifyAtk variants
        [(EventId.ModifyAtk, EventPrefix.Ally)] = EventId.AllyModifyAtk,
        [(EventId.ModifyAtk, EventPrefix.Source)] = EventId.SourceModifyAtk,

        // ModifySpA variants
        [(EventId.ModifySpA, EventPrefix.Ally)] = EventId.AllyModifySpA,
        [(EventId.ModifySpA, EventPrefix.Source)] = EventId.SourceModifySpA,

        // ModifySpD variants
        [(EventId.ModifySpD, EventPrefix.Ally)] = EventId.AllyModifySpD,
        [(EventId.ModifySpD, EventPrefix.Foe)] = EventId.FoeModifySpD,

        // BeforeMove variants
        [(EventId.BeforeMove, EventPrefix.Foe)] = EventId.FoeBeforeMove,

        // ModifyDef variants
        [(EventId.ModifyDef, EventPrefix.Foe)] = EventId.FoeModifyDef,

        // RedirectTarget variants
        [(EventId.RedirectTarget, EventPrefix.Foe)] = EventId.FoeRedirectTarget,

        // TrapPokemon variants
        [(EventId.TrapPokemon, EventPrefix.Foe)] = EventId.FoeTrapPokemon,

        // ModifyDamage variants
        [(EventId.ModifyDamage, EventPrefix.Source)] = EventId.SourceModifyDamage,
    };

    public static EventId GetPrefixedEvent(EventId baseEvent, EventPrefix prefix)
    {
        if (prefix == EventPrefix.None)
            return baseEvent;

        if (PrefixMap.TryGetValue((baseEvent, prefix), out EventId prefixedEvent))
            return prefixedEvent;

        throw new ArgumentException($"No prefixed variant of {baseEvent} with prefix {prefix}");
    }

    public static bool HasPrefixedVariant(EventId baseEvent, EventPrefix prefix)
    {
        return PrefixMap.ContainsKey((baseEvent, prefix));
    }

    /// <summary>
    /// Gets the fallback event ID for Gen 5+ ability/item switch-in behavior.
    /// In Gen 5+, abilities and items use Start instead of SwitchIn.
    /// </summary>
    /// <param name="eventId">The original event ID</param>
    /// <param name="gen">The generation number</param>
    /// <returns>The fallback event ID if applicable, otherwise null</returns>
    public static EventId? GetSwitchInFallback(EventId eventId, int gen)
    {
        // Only applies to Gen 5+ and SwitchIn events
        if (gen < 5 || eventId != EventId.SwitchIn)
            return null;

        return EventId.Start;
    }

    /// <summary>
    /// Checks if an effect is an innate ability or item based on its ID string format.
    /// Innate abilities/items follow the format "ability:name" or "item:name".
    /// </summary>
    /// <param name="effectStateId">The effect state ID to check</param>
    /// <returns>True if the ID represents an innate ability or item</returns>
    public static bool IsInnateAbilityOrItem(EffectStateId effectStateId)
    {
        string effectIdStr = effectStateId.ToString();
        
        if (!effectIdStr.Contains(':'))
            return false;

        string prefix = effectIdStr.Split(':')[0];
        return prefix is "ability" or "item";
    }

    /// <summary>
    /// Checks if an event ID ends with "SwitchIn" or "RedirectTarget".
    /// </summary>
    public static bool EventEndsWithSwitchInOrRedirectTarget(EventId eventId)
    {
        string eventName = eventId.ToString();
        return eventName.EndsWith("SwitchIn", StringComparison.Ordinal) || 
               eventName.EndsWith("RedirectTarget", StringComparison.Ordinal);
    }

    /// <summary>
    /// Checks if an event ID ends with "SwitchIn".
    /// </summary>
    public static bool EventEndsWithSwitchIn(EventId eventId)
    {
        string eventName = eventId.ToString();
        return eventName.EndsWith("SwitchIn", StringComparison.Ordinal);
    }

    /// <summary>
    /// Gets an event-specific property from an effect using reflection.
    /// For example, for EventId.BasePower and EventPropertyType.Order, 
    /// it looks for a property named "BasePowerOrder" on the effect.
    /// </summary>
    public static T? GetEventProperty<T>(IEffect effect, EventId eventId, EventPropertyType propertyType) 
        where T : struct
    {
        // Construct the property name: e.g., "BasePowerOrder", "ModifyAtkPriority", etc.
        string eventName = eventId.ToString();
        string propertyName = $"{eventName}{propertyType}";

        // Use reflection to get the property value
        Type effectType = effect.GetType();
        PropertyInfo? property = effectType.GetProperty(propertyName, 
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (property != null && property.PropertyType == typeof(T?))
        {
            return (T?)property.GetValue(effect);
        }

        return null;
    }
}