using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Moves;

public record ActiveMove : Move, IEffect
{
    /// <summary>The MoveSlot this move occupies in a Pokemon's moveset.</summary>
    public required MoveSlot MoveSlot { get; init; }

    public EffectType EffectType => EffectType.Move;
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
    public bool? Infiltrates { get; init; }
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

    public Delegate? GetDelegate(EventId id)
    {
        return id switch
        {
            EventId.BasePowerCallback => BasePowerCallback,
            EventId.BeforeMoveCallback => BeforeMoveCallback,
            EventId.BeforeTurnCallback => BeforeTurnCallback,
            EventId.DamageCallback => DamageCallback,
            EventId.PriorityChargeCallback => PriorityChargeCallback,
            EventId.DisableMove => OnDisableMove,
            EventId.AfterHit => OnAfterHit,
            EventId.AfterSubDamage => OnAfterSubDamage,
            EventId.AfterMoveSecondarySelf => OnAfterMoveSecondarySelf,
            EventId.AfterMoveSecondary => OnAfterMoveSecondary,
            EventId.AfterMove => OnAfterMove,
            EventId.Damage => OnDamage,
            EventId.BasePower => OnBasePower,
            EventId.Effectiveness => OnEffectiveness,
            EventId.Hit => OnHit,
            EventId.HitField => OnHitField,
            EventId.HitSide => OnHitSide,
            EventId.ModifyMove => OnModifyMove,
            EventId.ModifyPriority => OnModifyPriority,
            EventId.MoveFail => OnMoveFail,
            EventId.ModifyType => OnModifyType,
            EventId.ModifyTarget => OnModifyTarget,
            EventId.PrepareHit => OnPrepareHit,
            EventId.Try => OnTry,
            EventId.TryHit => OnTryHit,
            EventId.TryHitField => OnTryHitField,
            EventId.TryHitSide => OnTryHitSide,
            EventId.TryImmunity => OnTryImmunity,
            EventId.TryMove => OnTryMove,
            EventId.UseMoveMessage => OnUseMoveMessage,
            _ => null,
        };
    }
}



// Element of MoveHitData
public record MoveHitResult
{
    public bool Crit { get; init; }
    public int TypeMod { get; init; }
    public bool ZBrokeProtect { get; set; }
}

public class MoveHitData : Dictionary<PokemonSlot, MoveHitResult>;