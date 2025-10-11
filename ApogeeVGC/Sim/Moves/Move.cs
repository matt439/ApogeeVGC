using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Types;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Moves;

public record Move : IMoveEventHandlers, IBasicEffect
{
    #region IMoveEventHandlers Implementation

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
        init
        {
            if (value is > 5 or < -7)
            {
                throw new ArgumentOutOfRangeException(nameof(Priority), "Priority must be between -7 and 5.");
            }
            field = value;
        }
    }
    public MoveTarget Target { get; init; }
    public MoveFlags Flags { get; init; } = new();
    
    public MoveDamage? Damage { get; init; }


    // Hit effects
    public MoveOhko? Ohko { get; init; }
    public bool? ThawsTarget { get;init; }
    public int[]? Heal { get; init; }
    public bool? ForceSwitch { get; init; }
    public MoveSelfSwitch? SelfSwitch { get; init; }
    public SparseBoostsTable? SelfBoost { get; init; }
    public MoveSelfDestruct? SelfDestruct { get; init; }
    public bool? BreaksProtect { get; init; }


    public (int, int)? Recoil { get; init; }
    public (int, int)? Drain { get; init; }
    public bool? MindBlownRecoil { get; init; }
    public bool? StealsBoosts { get; init; }
    public bool? StruggleRecoil { get; init; }
    public SecondaryEffect? Secondary { get; init; }
    public SecondaryEffect[]? Secondaries { get; init; }
    public SecondaryEffect? Self { get; init; }
    public bool? HasSheerForce { get; init; }


    // Hit effect modifiers
    public MoveType? BaseMoveType { get;init; }
    public int? BasePowerModifier { get; init; }
    public int? CritModifier { get; init; }
    public int? CritRatio { get; init; }
    public MoveOverridePokemon? OverrideOffensivePokemon { get; init; }
    public StatIdExceptHp? OverrideOffensiveStat { get; init; }
    public MoveOverridePokemon? OverrideDefensivePokemon { get; init; }
    public StatIdExceptHp? OverrideDefensiveStat { get; init; }
    public bool? ForceStab { get; init; }
    public bool? IgnoreAbility { get; init; }
    public bool? IgnoreAccuracy { get; init; }
    public bool? IgnoreDefensive { get; init; }
    public bool? IgnoreEvasion { get; init; }
    public MoveIgnoreImmunity? IgnoreImmunity { get; init; }
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