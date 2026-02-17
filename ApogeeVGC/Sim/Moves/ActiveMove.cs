using System.Collections.Frozen;
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
                    .ToFrozenDictionary();
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

    internal FrozenDictionary<(EventId, EventPrefix, EventSuffix), EventHandlerInfo>? _handlerCache { get; set; }
    private FrozenDictionary<(EventId, EventPrefix, EventSuffix), EventHandlerInfo> HandlerCache =>
        _handlerCache ??= EventHandlerInfoMapper.BuildMoveHandlerCache(this);

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


// Element of MoveHitData
public record MoveHitResult
{
    public bool Crit { get; set; }
    public int TypeMod { get; set; }
    public bool ZBrokeProtect { get; set; }
}

public class MoveHitData : Dictionary<PokemonSlot, MoveHitResult>;
