using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Moves;

public record ActiveMove : Move, IEffect
{
    /// <summary>The MoveSlot this move occupies in a Pokemon's moveset.</summary>
    public required MoveSlot MoveSlot { get; init; }

    /// <summary>
    /// Creates a shallow clone using <see cref="MemberwiseClone"/>,
    /// which is a flat memory copy — faster than the record copy constructor
    /// chain (HitEffect → Move → ActiveMove) for this ~900-byte object.
    /// </summary>
    internal ActiveMove ShallowClone()
    {
        var clone = (ActiveMove)MemberwiseClone();
        // Deep-copy secondaries to prevent mutations from leaking to the library template
        clone.Secondaries = clone.Secondaries?.Select(s => s with { }).ToArray();
        if (clone.Self is not null) clone.Self = clone.Self with { };
        clone.Flags = clone.Flags with { };
        return clone;
    }

    /// <summary>
    /// Resets all mutable fields of this instance to match the given template.
    /// Used by the per-Battle <see cref="ActiveMove"/> pool to recycle instances
    /// instead of allocating via <see cref="MemberwiseClone"/>.
    /// <para>
    /// MAINTENANCE: when adding a new settable property to <see cref="HitEffect"/>,
    /// <see cref="Move"/>, or <see cref="ActiveMove"/>, add a corresponding line here.
    /// </para>
    /// </summary>
    internal void ResetFromTemplate(ActiveMove template)
    {
        // --- HitEffect mutable fields ---
        // Bypass ActiveMove's 'new OnHit' setter (which invalidates _handlerCache)
        // by writing directly to the base HitEffect.OnHit property.
        base.OnHit = template.OnHit;
        Boosts = template.Boosts;
        VolatileStatus = template.VolatileStatus;
        SideCondition = template.SideCondition;

        // --- Move mutable fields (restore to template defaults) ---
        BasePower = template.BasePower;
        Accuracy = template.Accuracy;
        Category = template.Category;
        Type = template.Type;
        Priority = template.Priority;
        Target = template.Target;
        Flags = template.Flags with { };
        SelfSwitch = template.SelfSwitch;
        SpreadHit = template.SpreadHit;
        MindBlownRecoil = template.MindBlownRecoil;
        Secondaries = template.Secondaries?.Select(s => s with { }).ToArray();
        Self = template.Self is not null ? template.Self with { } : null;
        HasSheerForce = template.HasSheerForce;
        ForceStab = template.ForceStab;
        IgnoreAbility = template.IgnoreAbility;
        IgnoreEvasion = template.IgnoreEvasion;
        IgnoreImmunity = template.IgnoreImmunity;
        MultiAccuracy = template.MultiAccuracy;
        MultiHit = template.MultiHit;
        MultiHitType = template.MultiHitType;
        SmartTarget = template.SmartTarget;
        TracksTarget = template.TracksTarget;
        OverrideOffensiveStat = template.OverrideOffensiveStat;

        // --- ActiveMove mutable fields (reset to defaults) ---
        Weather = template.Weather;
        Hit = 0;
        MoveHitData = null;
        HitTargets = null;
        Ability = null;
        Allies = null;
        AuraBooster = null;
        CausedCrashDamage = null;
        ForceStatus = null;
        HasAuraBreak = null;
        HasBounced = null;
        IsExternal = null;
        LastHit = null;
        Magnitude = null;
        PranksterBooster = null;
        PranksterBoosted = null;
        SelfDropped = null;
        StatusRoll = null;
        StellarBoosted = null;
        TotalDamage = null;
        TypeChangerBoosted = null;
        WillChangeForme = null;
        Infiltrates = null;
        RuinedAtk = null;
        RuinedDef = null;
        RuinedSpA = null;
        RuinedSpD = null;
        SourceEffect = null;
        HitEffect = null;

        // Restore shared handler cache from template (must be last)
        _handlerCache = template._handlerCache;
    }

    /// <summary>
    /// Creates an ActiveMove from a base Move, leveraging the record copy constructor
    /// to efficiently copy all Move properties instead of manual property-by-property assignment.
    /// </summary>
    [SetsRequiredMembers]
    public ActiveMove(Move source) : base(source)
    {
        // Deep-copy secondaries to prevent mutations from leaking to the library template
        // (e.g., Serene Grace doubling Chance values in-place)
        Secondaries = Secondaries?.Select(s => s with { }).ToArray()
                      ?? (Secondary != null ? [Secondary with { }] : null);
        Self = Self is not null ? Self with { } : null;

        // Copy Weather from base HitEffect (ActiveMove.Weather hides HitEffect.Weather,
        // so the record copy constructor doesn't propagate it automatically)
        Weather = source.Weather;

        // Share pre-built handler cache from base Move
        _handlerCache = source.MoveHandlerCache;

        // Set ActiveMove-specific required property
        MoveSlot = new MoveSlot
        {
            Move = Id,
            Id = Id,
            Pp = NoPpBoosts ? BasePp : BasePp * 8 / 5,
            MaxPp = NoPpBoosts ? BasePp : BasePp * 8 / 5,
            Target = Target,
            Disabled = false,
            DisabledSource = null,
            Used = false,
        };
    }

    /// <summary>
    /// Parameterless constructor for direct object-initializer construction.
    /// Required members (Name, Accuracy, MoveSlot) must be set in the initializer.
    /// </summary>
    public ActiveMove() { }

    public EffectType EffectType => EffectType.Move;

    /// <summary>
    /// Hides Move.OnHit to invalidate the handler cache when a move handler is mutated at runtime
    /// (e.g. Curse's non-Ghost branch nulls OnHit in OnTryHit).
    /// Instead of a full rebuild, derives a new cache from the shared Move cache minus the stale key.
    /// </summary>
    public new OnHitEventInfo? OnHit
    {
        get => base.OnHit;
        set
        {
            base.OnHit = value;
            var current = _handlerCache;
            if (current is null) return;

            if (value is null)
            {
                // Remove all Hit-event entries from the shared cache (cheap: filters ~1-3 keys out of ~5-30)
                _handlerCache = current
                    .Where(kvp => kvp.Key.Item1 != EventId.Hit)
                    .ToDictionary();
            }
            else
            {
                // Handler replaced — full rebuild required
                _handlerCache = null;
            }
        }
    }

    public ConditionId? Weather { get; set; }

    //public ConditionId? Status { get; set; }
    public int Hit { get; set; }
    public MoveHitData? MoveHitData { get; set; }
    public List<Pokemon>? HitTargets { get; set; }
    public Ability? Ability { get; set; }
    public List<Pokemon>? Allies { get; set; }
    public Pokemon? AuraBooster { get; set; }
    public bool? CausedCrashDamage { get; set; }
    public ConditionId? ForceStatus { get; set; }
    public bool? HasAuraBreak { get; set; }

    public bool? HasBounced { get; set; }

    //public bool? HasSheerForce { get; init; }
    public bool? IsExternal { get; set; }
    public bool? LastHit { get; set; }
    public int? Magnitude { get; set; }
    public bool? PranksterBooster { get; set; }
    public bool? PranksterBoosted { get; set; }

    public bool? SelfDropped { get; set; }

    //public object? SelfSwitch { get; init; } // "copyvolatile", "shedtail", or bool
    public ConditionId? StatusRoll { get; set; }
    public bool? StellarBoosted { get; set; }
    public IntFalseUnion? TotalDamage { get; set; }
    public IEffect? TypeChangerBoosted { get; set; }
    public bool? WillChangeForme { get; set; }
    public bool? Infiltrates { get; set; }
    public Pokemon? RuinedAtk { get; set; }
    public Pokemon? RuinedDef { get; set; }
    public Pokemon? RuinedSpA { get; set; }
    public Pokemon? RuinedSpD { get; set; }


    // additional properties
    public EffectStateId? SourceEffect { get; set; }


    /// PP management
    public int PpUp
    {
        get;
        init
        {
            if (value is < 0 or > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(PpUp), "PP Ups must be between 0 and 3.");
            }

            field = value;
        }
    } = 0;

    public int MaxPp => BasePp + (int)(0.2 * BasePp * PpUp);

    public int UsedPp
    {
        get;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(UsedPp), "Used PP cannot be negative.");
            }

            field = value;
        }
    } = 0;

    public int Pp
    {
        get
        {
            int pp = MaxPp - UsedPp;
            return pp > 0 ? pp : 0;
        }
    }

    /// <summary>
    /// This is used to handle the casting of a HitEffect to an ActiveMove in RunMoveEffects() and SpreadMoveHit().
    /// Store the HitEffect in this property so it can be accessed later.
    /// </summary>
    public HitEffect? HitEffect { get; set; }

    internal Dictionary<(EventId, EventPrefix, EventSuffix), EventHandlerInfo>? _handlerCache { get; set; }
    private Dictionary<(EventId, EventPrefix, EventSuffix), EventHandlerInfo> HandlerCache =>
        _handlerCache ??= EventHandlerInfoMapper.BuildMoveHandlerCache(this);

    public bool HasAnyEventHandlers => HandlerCache.Count > 0;
    public bool HasPrefixedHandlers => false; // Moves don't have prefixed handlers

    /// <summary>
    /// Gets event handler information for the specified event.
    /// Uses a pre-computed cache for O(1) lookups.
    /// </summary>
    public EventHandlerInfo? GetEventHandlerInfo(EventId id, EventPrefix prefix = EventPrefix.None, EventSuffix suffix = EventSuffix.None)
    {
        return HandlerCache.TryGetValue((id, prefix, suffix), out var info) ? info : null;
    }
}


// Element of MoveHitData
public record MoveHitResult
{
    public bool Crit { get; set; }
    public int TypeMod { get; set; }
    public bool ZBrokeProtect { get; set; }
}

public class MoveHitData : Dictionary<PokemonSlot, MoveHitResult>;
