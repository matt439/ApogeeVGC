using System.Collections.Frozen;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Abilities;

public partial record Ability : IEffect, IAbilityEventMethodsV2, IBasicEffect, ICopyable<Ability>
{
    public EffectType EffectType => EffectType.Ability;
    public required AbilityId Id { get; init; }
    public EffectStateId EffectStateId => Id;

    /// <summary>
    /// Rating from -1 Detrimental to +5 Essential
    /// </summary>
    public double Rating
    {
        get;
        init
        {
            if (value is < -1.0 or > 5.0)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Rating must be between -1 and 5.");
            field = value;
        }
    }

    public bool SuppressWeather { get; init; }

    public string FullName => $"ability: {Name}";

    public int Gen
    {
        get
        {
            if (field is >= 1 and <= 9) return field;
            return Num switch
            {
                >= 268 => 9,
                >= 234 => 8,
                >= 192 => 7,
                >= 165 => 6,
                >= 124 => 5,
                >= 77 => 4,
                >= 1 => 3,
                _ => field,
            };
        }
        init;
    }

    public string Name { get; init; } = string.Empty;
    public int Num { get; init; } = 0;
    public AbilityFlags Flags { get; init; } = new();
    public ConditionId? Condition { get; init; }

    public bool AffectsFainted { get; init; }

    public IReadOnlyList<PokemonType>? ImmuneTypes { get; init; }

    public Ability Copy()
    {
        return this with { };
    }

    #region IAbilityEventMethods Implementation

    public OnCheckShowEventInfo? OnCheckShow { get; init; }
    public OnEndEventInfo? OnEnd { get; init; }
    public OnStartEventInfo? OnStart { get; init; }

    #endregion

    private FrozenDictionary<(EventId, EventPrefix, EventSuffix), EventHandlerInfo>? _handlerCache;
    private FrozenDictionary<(EventId, EventPrefix, EventSuffix), EventHandlerInfo> HandlerCache =>
        _handlerCache ??= EventHandlerInfoMapper.BuildHandlerCache(this);

    public bool HasAnyEventHandlers => HandlerCache.Count > 0;

    /// <summary>
    /// Gets event handler information for the specified event.
    /// Uses a pre-computed cache for O(1) lookups.
    /// </summary>
    public EventHandlerInfo? GetEventHandlerInfo(EventId id, EventPrefix prefix = EventPrefix.None, EventSuffix suffix = EventSuffix.None)
    {
        return HandlerCache.TryGetValue((id, prefix, suffix), out var info) ? info : null;
    }
}