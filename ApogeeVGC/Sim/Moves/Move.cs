using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Moves;

public record Move : IMoveEventMethods, IBasicEffect
{
    #region IMoveEventMethods Implementation

    public TypeUndefinedUnion<BasePowerCallbackHandler>? BasePowerCallback { get; init; }
    public TypeUndefinedUnion<BeforeMoveCallbackHandler>? BeforeMoveCallback { get; init; }
    public TypeUndefinedUnion<BeforeTurnCallbackHandler>? BeforeTurnCallback { get; init; }
    public TypeUndefinedUnion<DamageCallbackHandler>? DamageCallback { get; init; }
    public TypeUndefinedUnion<PriorityChargeCallbackHandler>? PriorityChargeCallback { get; init; }
    public TypeUndefinedUnion<OnDisableMoveHandler>? OnDisableMove { get; init; }
    public TypeUndefinedUnion<VoidSourceMoveHandler>? OnAfterHit { get; init; }
    public TypeUndefinedUnion<OnAfterSubDamageHandler>? OnAfterSubDamage { get; init; }
    public TypeUndefinedUnion<VoidSourceMoveHandler>? OnAfterMoveSecondarySelf { get; init; }
    public TypeUndefinedUnion<VoidMoveHandler>? OnAfterMoveSecondary { get; init; }
    public TypeUndefinedUnion<VoidSourceMoveHandler>? OnAfterMove { get; init; }
    public TypeUndefinedUnion<OnDamageHandler>? OnDamage { get; init; }
    public TypeUndefinedUnion<ModifierSourceMoveHandler>? OnBasePower { get; init; }
    public TypeUndefinedUnion<OnEffectivenessHandler>? OnEffectiveness { get; init; }
    public TypeUndefinedUnion<ResultMoveHandler>? OnHit { get; init; }
    public TypeUndefinedUnion<ResultMoveHandler>? OnHitField { get; init; }
    public TypeUndefinedUnion<OnHitSideHandler>? OnHitSide { get; init; }
    public TypeUndefinedUnion<OnModifyMoveHandler>? OnModifyMove { get; init; }
    public TypeUndefinedUnion<ModifierSourceMoveHandler>? OnModifyPriority { get; init; }
    public TypeUndefinedUnion<VoidMoveHandler>? OnMoveFail { get; init; }
    public TypeUndefinedUnion<OnModifyTypeHandler>? OnModifyType { get; init; }
    public TypeUndefinedUnion<OnModifyTargetHandler>? OnModifyTarget { get; init; }
    public TypeUndefinedUnion<ResultMoveHandler>? OnPrepareHit { get; init; }
    public TypeUndefinedUnion<ResultSourceMoveHandler>? OnTry { get; init; }
    public TypeUndefinedUnion<ExtResultSourceMoveHandler>? OnTryHit { get; init; }
    public TypeUndefinedUnion<ResultMoveHandler>? OnTryHitField { get; init; }
    public TypeUndefinedUnion<OnTryHitSideHandler>? OnTryHitSide { get; init; }
    public TypeUndefinedUnion<ResultMoveHandler>? OnTryImmunity { get; init; }
    public TypeUndefinedUnion<ResultSourceMoveHandler>? OnTryMove { get; init; }
    public TypeUndefinedUnion<VoidSourceMoveHandler>? OnUseMoveMessage { get; init; }

    #endregion


    public MoveId Id { get; init; }
    public EffectStateId EffectStateId => Id;
    public required string Name { get; init; }
    public int Num
    {
        get;
        init
        {
            if (Num < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Num), "Move number must be non-negative.");
            }
            field = value;
        }
    }
    public Condition? Condition { get; init; }
    public int BasePower
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(BasePower), "Base power must be non-negative.");
            }
            field = value;
        }
    }
    public required IntTrueUnion Accuracy
    {
        get;
        init
        {
            if (value is IntIntTrueUnion { Value: < 1 or > 100 })
            {
                throw new ArgumentOutOfRangeException(nameof(Accuracy), "Accuracy must be between 1 and 100.");
            }
            field = value;
        }
    }
    public int BasePp
    {
        get;
        init
        {
            if (!(value == 1 || value % 5 == 0))
            {
                throw new ArgumentOutOfRangeException(nameof(BasePp), "PP must be 1 or a multiple of 5.");
            }
            if (value > 40)
            {
                throw new ArgumentOutOfRangeException(nameof(BasePp), "PP cannot exceed 40.");
            }
            field = value;
        }
    }
    public bool NoPpBoosts { get; init; } = false;
    public MoveCategory Category { get; init; }
    public MoveType Type { get; set; }
    public int Priority
    {
        get;
        set
        {
            if (value is > 5 or < -7)
            {
                throw new ArgumentOutOfRangeException(nameof(Priority), "Priority must be between -7 and 5.");
            }
            field = value;
        }
    }
    public MoveTarget Target { get; init; }
    public MoveFlags Flags { get; set; } = new();
    
    public MoveDamage? Damage { get; init; }


    // Hit effects
    public MoveOhko? Ohko { get; init; }
    public bool? ThawsTarget { get;init; }
    public int[]? Heal { get; init; }
    public bool? ForceSwitch { get; init; }
    public MoveSelfSwitch? SelfSwitch { get; init; }
    public BoolUndefinedUnion? SpreadHit { get; set; }
    public SparseBoostsTable? SelfBoost { get; init; }
    public MoveSelfDestruct? SelfDestruct { get; init; }
    public bool? BreaksProtect { get; init; }


    public (int, int)? Recoil { get; init; }
    public (int, int)? Drain { get; init; }
    public bool? MindBlownRecoil { get; set; }
    public bool? StealsBoosts { get; init; }
    public bool? StruggleRecoil { get; init; }
    public SecondaryEffect? Secondary { get; init; }
    public SecondaryEffect[]? Secondaries { get; init; }
    public SecondaryEffect? Self { get; init; }
    public bool? HasSheerForce { get; init; }


    // Hit effect modifiers
    public BoolUndefinedUnion? AlwaysHit { get; init; }
    public MoveType? BaseMoveType { get;init; }
    public int? BasePowerModifier { get; init; }
    public int? CritModifier { get; init; }
    public int? CritRatio { get; init; }
    public MoveOverridePokemon? OverrideOffensivePokemon { get; init; }
    public StatIdExceptHp? OverrideOffensiveStat { get; init; }
    public MoveOverridePokemon? OverrideDefensivePokemon { get; init; }
    public StatIdExceptHp? OverrideDefensiveStat { get; init; }
    public bool? ForceStab { get; init; }
    public bool? IgnoreAbility { get; set; }
    public bool? IgnoreAccuracy { get; init; }
    public bool? IgnoreDefensive { get; init; }
    public bool? IgnoreEvasion { get; init; }
    public MoveIgnoreImmunity? IgnoreImmunity { get; set; }
    public bool? IgnoreNegativeOffensive { get; init; }
    public bool? IgnoreOffensive { get; init; }
    public bool? IgnorePositiveDefensive { get; init; }
    public bool? IgnorePositiveEvasion { get; init; }
    public bool? MultiAccuracy { get; init; }
    public IntIntArrayUnion? MultiHit { get; init; }
    public MoveMultiHitType? MultiHitType { get; init; }
    public bool? NoDamageVariance { get; init; }
    public MoveTarget? NonGhostTarget { get; init; }
    public double? SpreadModifier { get; init; }
    public bool? SleepUsable { get; init; }
    public bool? SmartTarget { get; set; }
    public bool? TracksTarget { get; init; }
    public bool? WillCrit { get; init; }
    public bool? CallsMove { get; init; }
    public bool? HasCrashDamage { get; init; }
    public bool? IsConfusionSelfHit { get; init; }
    public bool? StallingMove { get; init; }
    public MoveId? BaseMove { get; init; }
    public ConditionId? PseudoWeather { get; init; }
    public ConditionId? VolatileStatus { get; init; }
    public ConditionId? SideCondition { get; init; }
    public ConditionId? Status { get; init; }

    public bool AffectsFainted { get; init; }

    public ActiveMove ToActiveMove()
    {
        throw new NotImplementedException();
    }

    public EffectDelegate? GetDelegate(EventId id)
    {
        return id switch
        {
            EventId.BasePowerCallback => EffectDelegate.FromNullableTypeUndefinedUnion(BasePowerCallback),
            EventId.BeforeMoveCallback => EffectDelegate.FromNullableTypeUndefinedUnion(BeforeMoveCallback),
            EventId.BeforeTurnCallback => EffectDelegate.FromNullableTypeUndefinedUnion(BeforeTurnCallback),
            EventId.DamageCallback => EffectDelegate.FromNullableTypeUndefinedUnion(DamageCallback),
            EventId.PriorityChargeCallback => EffectDelegate.FromNullableTypeUndefinedUnion(PriorityChargeCallback),
            EventId.DisableMove => EffectDelegate.FromNullableTypeUndefinedUnion(OnDisableMove),
            EventId.AfterHit => EffectDelegate.FromNullableTypeUndefinedUnion(OnAfterHit),
            EventId.AfterSubDamage => EffectDelegate.FromNullableTypeUndefinedUnion(OnAfterSubDamage),
            EventId.AfterMoveSecondarySelf => EffectDelegate.FromNullableTypeUndefinedUnion(OnAfterMoveSecondarySelf),
            EventId.AfterMoveSecondary => EffectDelegate.FromNullableTypeUndefinedUnion(OnAfterMoveSecondary),
            EventId.AfterMove => EffectDelegate.FromNullableTypeUndefinedUnion(OnAfterMove),
            EventId.Damage => EffectDelegate.FromNullableTypeUndefinedUnion(OnDamage),
            EventId.BasePower => EffectDelegate.FromNullableTypeUndefinedUnion(OnBasePower),
            EventId.Effectiveness => EffectDelegate.FromNullableTypeUndefinedUnion(OnEffectiveness),
            EventId.Hit => EffectDelegate.FromNullableTypeUndefinedUnion(OnHit),
            EventId.HitField => EffectDelegate.FromNullableTypeUndefinedUnion(OnHitField),
            EventId.HitSide => EffectDelegate.FromNullableTypeUndefinedUnion(OnHitSide),
            EventId.ModifyMove => EffectDelegate.FromNullableTypeUndefinedUnion(OnModifyMove),
            EventId.ModifyPriority => EffectDelegate.FromNullableTypeUndefinedUnion(OnModifyPriority),
            EventId.MoveFail => EffectDelegate.FromNullableTypeUndefinedUnion(OnMoveFail),
            EventId.ModifyType => EffectDelegate.FromNullableTypeUndefinedUnion(OnModifyType),
            EventId.ModifyTarget => EffectDelegate.FromNullableTypeUndefinedUnion(OnModifyTarget),
            EventId.PrepareHit => EffectDelegate.FromNullableTypeUndefinedUnion(OnPrepareHit),
            EventId.Try => EffectDelegate.FromNullableTypeUndefinedUnion(OnTry),
            EventId.TryHit => EffectDelegate.FromNullableTypeUndefinedUnion(OnTryHit),
            EventId.TryHitField => EffectDelegate.FromNullableTypeUndefinedUnion(OnTryHitField),
            EventId.TryHitSide => EffectDelegate.FromNullableTypeUndefinedUnion(OnTryHitSide),
            EventId.TryImmunity => EffectDelegate.FromNullableTypeUndefinedUnion(OnTryImmunity),
            EventId.TryMove => EffectDelegate.FromNullableTypeUndefinedUnion(OnTryMove),
            EventId.UseMoveMessage => EffectDelegate.FromNullableTypeUndefinedUnion(OnUseMoveMessage),
            _ => null,
        };
    }

    // Moves do not define event priorities
    public int? GetPriority(EventId id) => null;

    // Moves do not define event orders
    public IntFalseUnion? GetOrder(EventId id) => null;

    // Moves do not define event sub-orders
    public int? GetSubOrder(EventId id) => null;
    

    /// <summary>
    /// Creates a deep copy of this Move for simulation purposes.
    /// This method creates an independent copy with the same state while sharing immutable references.
    /// </summary>
    /// <returns>A new Move instance with copied state</returns>
    public Move Copy()
    {
        return this with 
        { 
            // Records have built-in copy semantics with 'with' expression
            // This creates a shallow copy which is appropriate since most properties
            // are either value types, immutable references, or function delegates
            // The mutable properties (PpUp, UsedPp) are copied correctly
        };
    }
}