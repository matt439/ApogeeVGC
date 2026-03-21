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

    private EventHandlerInfo?[]? _handlerArray;
    private bool _hasAnyHandlers;

    private void EnsureHandlerArray()
    {
        if (Volatile.Read(ref _handlerArray) is not null) return;

        var (array, hasAny, hasPrefixed) = EventHandlerInfoMapper.BuildHandlerArray(this);
        Interlocked.CompareExchange(ref _hasPrefixedHandlers, hasPrefixed ? 1 : 0, -1);
        // _hasAnyHandlers is set before _handlerArray is published to avoid a race
        // where another thread sees the array but reads the default false for _hasAnyHandlers.
        _hasAnyHandlers = hasAny;
        Interlocked.CompareExchange(ref _handlerArray, array, null);
    }

    public bool HasAnyEventHandlers
    {
        get { EnsureHandlerArray(); return _hasAnyHandlers; }
    }

    private int _hasPrefixedHandlers = -1; // -1 = unset, 0 = false, 1 = true
    public bool HasPrefixedHandlers
    {
        get
        {
            int cached = Volatile.Read(ref _hasPrefixedHandlers);
            if (cached >= 0) return cached == 1;

            EnsureHandlerArray();
            return Volatile.Read(ref _hasPrefixedHandlers) == 1;
        }
    }

    /// <summary>
    /// Gets event handler information for the specified event.
    /// Uses a pre-computed flat array for O(1) lookups without hashing.
    /// </summary>
    public EventHandlerInfo? GetEventHandlerInfo(EventId id, EventPrefix prefix = EventPrefix.None, EventSuffix suffix = EventSuffix.None)
    {
        return EventHandlerInfoMapper.LookupHandler(_handlerArray ?? InitAndGetArray(), id, prefix, suffix);
    }

    private EventHandlerInfo?[] InitAndGetArray()
    {
        EnsureHandlerArray();
        return _handlerArray!;
    }
}