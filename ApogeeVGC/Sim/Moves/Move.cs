using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Moves;

public record Move : IMoveEventMethods, IBasicEffect, ICopyable<Move>
{
    #region IMoveEventMethods Implementation

    public BasePowerCallbackHandler? BasePowerCallback { get; init; }
    public BeforeMoveCallbackHandler? BeforeMoveCallback { get; init; }
    public BeforeTurnCallbackHandler? BeforeTurnCallback { get; init; }
    public DamageCallbackHandler? DamageCallback { get; init; }
    public PriorityChargeCallbackHandler? PriorityChargeCallback { get; init; }
    public OnDisableMoveHandler? OnDisableMove { get; init; }
    public VoidSourceMoveHandler? OnAfterHit { get; init; }
    public OnAfterSubDamageHandler? OnAfterSubDamage { get; init; }
    public VoidSourceMoveHandler? OnAfterMoveSecondarySelf { get; init; }
    public VoidMoveHandler? OnAfterMoveSecondary { get; init; }
    public VoidSourceMoveHandler? OnAfterMove { get; init; }
    public OnDamageHandler? OnDamage { get; init; }
    public ModifierSourceMoveHandler? OnBasePower { get; init; }
    public OnEffectivenessHandler? OnEffectiveness { get; init; }
    public ResultMoveHandler? OnHit { get; init; }
    public ResultMoveHandler? OnHitField { get; init; }
    public OnHitSideHandler? OnHitSide { get; init; }
    public OnModifyMoveHandler? OnModifyMove { get; init; }
    public ModifierSourceMoveHandler? OnModifyPriority { get; init; }
    public VoidMoveHandler? OnMoveFail { get; init; }
    public OnModifyTypeHandler? OnModifyType { get; init; }
    public OnModifyTargetHandler? OnModifyTarget { get; init; }
    public ResultMoveHandler? OnPrepareHit { get; init; }
    public ResultSourceMoveHandler? OnTry { get; init; }
    public ExtResultSourceMoveHandler? OnTryHit { get; init; }
    public ResultMoveHandler? OnTryHitField { get; init; }
    public OnTryHitSideHandler? OnTryHitSide { get; init; }
    public ResultMoveHandler? OnTryImmunity { get; init; }
    public ResultSourceMoveHandler? OnTryMove { get; init; }
    public VoidSourceMoveHandler? OnUseMoveMessage { get; init; }

    #endregion


    public MoveId Id { get; init; }
    public EffectStateId EffectStateId => Id;
    public required string Name { get; init; }
    public string FullName => $"move: {Name}";
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
    public bool? SpreadHit { get; set; }
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
    public bool? AlwaysHit { get; init; }
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
        return new ActiveMove
        {
            // Copy all base Move properties
            Id = Id,
            Name = Name,
            Num = Num,
            Condition = Condition,
            BasePower = BasePower,
            Accuracy = Accuracy,
            BasePp = BasePp,
            NoPpBoosts = NoPpBoosts,
            Category = Category,
            Type = Type,
            Priority = Priority,
            Target = Target,
            Flags = Flags,
            Damage = Damage,
            Ohko = Ohko,
            ThawsTarget = ThawsTarget,
            Heal = Heal,
            ForceSwitch = ForceSwitch,
            SelfSwitch = SelfSwitch,
            SpreadHit = SpreadHit,
            SelfBoost = SelfBoost,
            SelfDestruct = SelfDestruct,
            BreaksProtect = BreaksProtect,
            Recoil = Recoil,
            Drain = Drain,
            MindBlownRecoil = MindBlownRecoil,
            StealsBoosts = StealsBoosts,
            StruggleRecoil = StruggleRecoil,
            Secondary = Secondary,
            Secondaries = Secondaries,
            Self = Self,
            HasSheerForce = HasSheerForce,
            AlwaysHit = AlwaysHit,
            BaseMoveType = BaseMoveType,
            BasePowerModifier = BasePowerModifier,
            CritModifier = CritModifier,
            CritRatio = CritRatio,
            OverrideOffensivePokemon = OverrideOffensivePokemon,
            OverrideOffensiveStat = OverrideOffensiveStat,
            OverrideDefensivePokemon = OverrideDefensivePokemon,
            OverrideDefensiveStat = OverrideDefensiveStat,
            ForceStab = ForceStab,
            IgnoreAbility = IgnoreAbility,
            IgnoreAccuracy = IgnoreAccuracy,
            IgnoreDefensive = IgnoreDefensive,
            IgnoreEvasion = IgnoreEvasion,
            IgnoreImmunity = IgnoreImmunity,
            IgnoreNegativeOffensive = IgnoreNegativeOffensive,
            IgnoreOffensive = IgnoreOffensive,
            IgnorePositiveDefensive = IgnorePositiveDefensive,
            IgnorePositiveEvasion = IgnorePositiveEvasion,
            MultiAccuracy = MultiAccuracy,
            MultiHit = MultiHit,
            MultiHitType = MultiHitType,
            NoDamageVariance = NoDamageVariance,
            NonGhostTarget = NonGhostTarget,
            SpreadModifier = SpreadModifier,
            SleepUsable = SleepUsable,
            SmartTarget = SmartTarget,
            TracksTarget = TracksTarget,
            WillCrit = WillCrit,
            CallsMove = CallsMove,
            HasCrashDamage = HasCrashDamage,
            IsConfusionSelfHit = IsConfusionSelfHit,
            StallingMove = StallingMove,
            BaseMove = BaseMove,
            PseudoWeather = PseudoWeather,
            VolatileStatus = VolatileStatus,
            SideCondition = SideCondition,
            Status = Status,
            AffectsFainted = AffectsFainted,
            
            // Copy event handlers
            BasePowerCallback = BasePowerCallback,
            BeforeMoveCallback = BeforeMoveCallback,
            BeforeTurnCallback = BeforeTurnCallback,
            DamageCallback = DamageCallback,
            PriorityChargeCallback = PriorityChargeCallback,
            OnDisableMove = OnDisableMove,
            OnAfterHit = OnAfterHit,
            OnAfterSubDamage = OnAfterSubDamage,
            OnAfterMoveSecondarySelf = OnAfterMoveSecondarySelf,
            OnAfterMoveSecondary = OnAfterMoveSecondary,
            OnAfterMove = OnAfterMove,
            OnDamage = OnDamage,
            OnBasePower = OnBasePower,
            OnEffectiveness = OnEffectiveness,
            OnHit = OnHit,
            OnHitField = OnHitField,
            OnHitSide = OnHitSide,
            OnModifyMove = OnModifyMove,
            OnModifyPriority = OnModifyPriority,
            OnMoveFail = OnMoveFail,
            OnModifyType = OnModifyType,
            OnModifyTarget = OnModifyTarget,
            OnPrepareHit = OnPrepareHit,
            OnTry = OnTry,
            OnTryHit = OnTryHit,
            OnTryHitField = OnTryHitField,
            OnTryHitSide = OnTryHitSide,
            OnTryImmunity = OnTryImmunity,
            OnTryMove = OnTryMove,
            OnUseMoveMessage = OnUseMoveMessage,

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
            },
        };
    }

    public EffectDelegate? GetDelegate(EventId id)
    {
        return id switch
        {
            EventId.BasePowerCallback => EffectDelegate.FromNullableDelegate(BasePowerCallback),
            EventId.BeforeMoveCallback => EffectDelegate.FromNullableDelegate(BeforeMoveCallback),
            EventId.BeforeTurnCallback => EffectDelegate.FromNullableDelegate(BeforeTurnCallback),
            EventId.DamageCallback => EffectDelegate.FromNullableDelegate(DamageCallback),
            EventId.PriorityChargeCallback => EffectDelegate.FromNullableDelegate(PriorityChargeCallback),
            EventId.DisableMove => EffectDelegate.FromNullableDelegate(OnDisableMove),
            EventId.AfterHit => EffectDelegate.FromNullableDelegate(OnAfterHit),
            EventId.AfterSubDamage => EffectDelegate.FromNullableDelegate(OnAfterSubDamage),
            EventId.AfterMoveSecondarySelf => EffectDelegate.FromNullableDelegate(OnAfterMoveSecondarySelf),
            EventId.AfterMoveSecondary => EffectDelegate.FromNullableDelegate(OnAfterMoveSecondary),
            EventId.AfterMove => EffectDelegate.FromNullableDelegate(OnAfterMove),
            EventId.Damage => EffectDelegate.FromNullableDelegate(OnDamage),
            EventId.BasePower => EffectDelegate.FromNullableDelegate(OnBasePower),
            EventId.Effectiveness => EffectDelegate.FromNullableDelegate(OnEffectiveness),
            EventId.Hit => EffectDelegate.FromNullableDelegate(OnHit),
            EventId.HitField => EffectDelegate.FromNullableDelegate(OnHitField),
            EventId.HitSide => EffectDelegate.FromNullableDelegate(OnHitSide),
            EventId.ModifyMove => EffectDelegate.FromNullableDelegate(OnModifyMove),
            EventId.ModifyPriority => EffectDelegate.FromNullableDelegate(OnModifyPriority),
            EventId.MoveFail => EffectDelegate.FromNullableDelegate(OnMoveFail),
            EventId.ModifyType => EffectDelegate.FromNullableDelegate(OnModifyType),
            EventId.ModifyTarget => EffectDelegate.FromNullableDelegate(OnModifyTarget),
            EventId.PrepareHit => EffectDelegate.FromNullableDelegate(OnPrepareHit),
            EventId.Try => EffectDelegate.FromNullableDelegate(OnTry),
            EventId.TryHit => EffectDelegate.FromNullableDelegate(OnTryHit),
            EventId.TryHitField => EffectDelegate.FromNullableDelegate(OnTryHitField),
            EventId.TryHitSide => EffectDelegate.FromNullableDelegate(OnTryHitSide),
            EventId.TryImmunity => EffectDelegate.FromNullableDelegate(OnTryImmunity),
            EventId.TryMove => EffectDelegate.FromNullableDelegate(OnTryMove),
            EventId.UseMoveMessage => EffectDelegate.FromNullableDelegate(OnUseMoveMessage),
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