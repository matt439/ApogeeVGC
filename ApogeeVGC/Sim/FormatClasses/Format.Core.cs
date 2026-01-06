using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.FormatClasses;


public enum FormatId
{
    Gen9Ou,
    CustomSingles,
    CustomSinglesBlind,
    CustomDoubles,
}

public enum FormatEffectType
{
    Format,
    Ruleset,
    Rule,
    ValidatorRule,
}

public enum RuleId
{
    Standard,
    OverflowStatMod,
    EndlessBattleClause,
}

public partial record Format : IEffect, IBasicEffect, ICopyable<Format>
{
    public FormatId FormatId { get; init; }
    public EffectStateId EffectStateId => FormatId;
    public EffectType EffectType => EffectType.Format;
    public required string Name { get; init; }
    public string FullName => $"format: {Name}";
    public string? Desc { get; init; }
    public bool AffectsFainted { get; init; }
    public FormatEffectType FormatEffectType { get; init; }
    public GameType GameType { get; init; }
    public static int PlayerCount => 2;

    public IReadOnlyList<RuleId> Ruleset { get; init; } = [];
    public IReadOnlyList<RuleId> BaseRuleset { get; init; } = [];
    public IReadOnlyList<RuleId> Banlist { get; init; } = [];
    public IReadOnlyList<RuleId> Restricted { get; init; } = [];
    public IReadOnlyList<RuleId> Unbanlist { get; init; } = [];
    public IReadOnlyList<RuleId>? CustomRules { get; init; }
    public RuleTable? RuleTable { get; set; }
    public Action<Battle>? OnBegin { get; init; }
    public bool NoLog { get; init; }

    public FormatHasValue? HasValue { get; init; }
    // OnValidateRule TODO: Implement this event method
    public RuleId? MutuallyExclusiveWith { get; init; }
    // ModdedDex fields here. Possible unnecessary in this context.
    public bool? ChallengeShow { get; init; }
    public bool? SearchShow { get; init; }
    public bool? BestOfDefault { get; init; }
    public bool? TeraPreviewDefault { get; init; }
    // Threads // TODO: Implement thread handling
    public bool? TournamentShow { get; init; }


    public Func<TeamValidator, Move, Species, PokemonSources, PokemonSet, string?>? CheckCanLearn { get; init; }
    public Func<Format, SpecieId, SpecieId>? GetEvoFamily { get; init; }
    public Func<Format, Pokemon, HashSet<string>>? GetSharedPower { get; init; }
    public Func<Format, Pokemon, HashSet<string>>? GetSharedItems { get; init; }
    public Func<TeamValidator, PokemonSet, Format, object?, object?, string[]?>? OnChangeSet { get; init; }
    public int? OnModifySpeciesPriority { get; init; }
    public Func<Battle, Species, Pokemon?, Pokemon?, IEffect?, Species?>? OnModifySpecies { get; init; }
    public Action<Battle>? OnBattleStart { get; init; }
    public Action<Battle>? OnTeamPreview { get; init; }
    public Func<TeamValidator, PokemonSet, Format, object, object, string[]?>? OnValidateSet { get; init; }
    public Func<TeamValidator, PokemonSet[], Format, object, string[]?>? OnValidateTeam { get; init; }
    public Func<TeamValidator, PokemonSet, object, string[]?>? ValidateSet { get; init; }
    public Func<TeamValidator, PokemonSet[], ValidateTeamOptions?, string[]?>? ValidateTeam { get; init; }

    public class ValidateTeamOptions
    {
        public bool RemoveNicknames { get; init; }
        public Dictionary<string, Dictionary<string, bool>>? SkipSets { get; init; }
    }
    public string? Section { get; init; }
    public int? Column { get; init; }

    //public EffectDelegate? GetDelegate(EventId id)
    //{
    //    return id switch
    //    {
    //        EventId.DamagingHit => EffectDelegate.FromNullableDelegate(OnDamagingHit),
    //        EventId.EmergencyExit => EffectDelegate.FromNullableDelegate(OnEmergencyExit),
    //        EventId.AfterEachBoost => EffectDelegate.FromNullableDelegate(OnAfterEachBoost),
    //        EventId.AfterHit => EffectDelegate.FromNullableDelegate(OnAfterHit),
    //        EventId.AfterMega => EffectDelegate.FromNullableDelegate(OnAfterMega),
    //        EventId.AfterSetStatus => EffectDelegate.FromNullableDelegate(OnAfterSetStatus),
    //        EventId.AfterSubDamage => EffectDelegate.FromNullableDelegate(OnAfterSubDamage),
    //        EventId.AfterSwitchInSelf => EffectDelegate.FromNullableDelegate(OnAfterSwitchInSelf),
    //        EventId.AfterTerastallization => EffectDelegate.FromNullableDelegate(OnAfterTerastallization),
    //        EventId.AfterUseItem => EffectDelegate.FromNullableDelegate(OnAfterUseItem),
    //        EventId.AfterTakeItem => EffectDelegate.FromNullableDelegate(OnAfterTakeItem),
    //        EventId.AfterBoost => EffectDelegate.FromNullableDelegate(OnAfterBoost),
    //        EventId.AfterFaint => EffectDelegate.FromNullableDelegate(OnAfterFaint),
    //        EventId.AfterMoveSecondarySelf => EffectDelegate.FromNullableDelegate(OnAfterMoveSecondarySelf),
    //        EventId.AfterMoveSecondary => EffectDelegate.FromNullableDelegate(OnAfterMoveSecondary),
    //        EventId.AfterMove => EffectDelegate.FromNullableDelegate(OnAfterMove),
    //        EventId.AfterMoveSelf => EffectDelegate.FromNullableDelegate(OnAfterMoveSelf),
    //        EventId.Attract => EffectDelegate.FromNullableDelegate(OnAttract),
    //        EventId.Accuracy => EffectDelegate.FromNullableDelegate(OnAccuracy),
    //        EventId.BasePower => EffectDelegate.FromNullableDelegate(OnBasePower),
    //        EventId.BeforeFaint => EffectDelegate.FromNullableDelegate(OnBeforeFaint),
    //        EventId.BeforeMove => EffectDelegate.FromNullableDelegate(OnBeforeMove),
    //        EventId.BeforeSwitchIn => EffectDelegate.FromNullableDelegate(OnBeforeSwitchIn),
    //        EventId.BeforeSwitchOut => EffectDelegate.FromNullableDelegate(OnBeforeSwitchOut),
    //        EventId.BeforeTurn => EffectDelegate.FromNullableDelegate(OnBeforeTurn),
    //        EventId.ChangeBoost => EffectDelegate.FromNullableDelegate(OnChangeBoost),
    //        EventId.TryBoost => EffectDelegate.FromNullableDelegate(OnTryBoost),
    //        EventId.ChargeMove => EffectDelegate.FromNullableDelegate(OnChargeMove),
    //        EventId.CriticalHit => EffectDelegate.FromNullableOnCriticalHit(OnCriticalHit),
    //        EventId.Damage => EffectDelegate.FromNullableDelegate(OnDamage),
    //        EventId.DeductPp => EffectDelegate.FromNullableDelegate(OnDeductPp),
    //        EventId.DisableMove => EffectDelegate.FromNullableDelegate(OnDisableMove),
    //        EventId.DragOut => EffectDelegate.FromNullableDelegate(OnDragOut),
    //        EventId.EatItem => EffectDelegate.FromNullableDelegate(OnEatItem),
    //        EventId.Effectiveness => EffectDelegate.FromNullableDelegate(OnEffectiveness),
    //        EventId.EntryHazard => EffectDelegate.FromNullableDelegate(OnEntryHazard),
    //        EventId.Faint => EffectDelegate.FromNullableDelegate(OnFaint),
    //        EventId.Flinch => EffectDelegate.FromNullableOnFlinch(OnFlinch),
    //        EventId.FractionalPriority => EffectDelegate.FromNullableOnFractionalPriority(OnFractionalPriority),
    //        EventId.Hit => EffectDelegate.FromNullableDelegate(OnHit),
    //        EventId.Immunity => EffectDelegate.FromNullableDelegate(OnImmunity),
    //        EventId.LockMove => EffectDelegate.FromNullableOnLockMove(OnLockMove),
    //        EventId.MaybeTrapPokemon => EffectDelegate.FromNullableDelegate(OnMaybeTrapPokemon),
    //        EventId.ModifyAccuracy => EffectDelegate.FromNullableDelegate(OnModifyAccuracy),
    //        EventId.ModifyAtk => EffectDelegate.FromNullableDelegate(OnModifyAtk),
    //        EventId.ModifyBoost => EffectDelegate.FromNullableDelegate(OnModifyBoost),
    //        EventId.ModifyCritRatio => EffectDelegate.FromNullableDelegate(OnModifyCritRatio),
    //        EventId.ModifyDamage => EffectDelegate.FromNullableDelegate(OnModifyDamage),
    //        EventId.ModifyDef => EffectDelegate.FromNullableDelegate(OnModifyDef),
    //        EventId.ModifyMove => EffectDelegate.FromNullableDelegate(OnModifyMove),
    //        EventId.ModifyPriority => EffectDelegate.FromNullableDelegate(OnModifyPriority),
    //        EventId.ModifySecondaries => EffectDelegate.FromNullableDelegate(OnModifySecondaries),
    //        EventId.ModifyType => EffectDelegate.FromNullableDelegate(OnModifyType),
    //        EventId.ModifyTarget => EffectDelegate.FromNullableDelegate(OnModifyTarget),
    //        EventId.ModifySpA => EffectDelegate.FromNullableDelegate(OnModifySpA),
    //        EventId.ModifySpD => EffectDelegate.FromNullableDelegate(OnModifySpD),
    //        EventId.ModifySpe => EffectDelegate.FromNullableDelegate(OnModifySpe),
    //        EventId.ModifyStab => EffectDelegate.FromNullableDelegate(OnModifyStab),
    //        EventId.ModifyWeight => EffectDelegate.FromNullableDelegate(OnModifyWeight),
    //        EventId.MoveAborted => EffectDelegate.FromNullableDelegate(OnMoveAborted),
    //        EventId.NegateImmunity => EffectDelegate.FromNullableOnNegateImmunity(OnNegateImmunity),
    //        EventId.OverrideAction => EffectDelegate.FromNullableDelegate(OnOverrideAction),
    //        EventId.PrepareHit => EffectDelegate.FromNullableDelegate(OnPrepareHit),
    //        EventId.PseudoWeatherChange => EffectDelegate.FromNullableDelegate(OnPseudoWeatherChange),
    //        EventId.RedirectTarget => EffectDelegate.FromNullableDelegate(OnRedirectTarget),
    //        EventId.Residual => EffectDelegate.FromNullableDelegate(OnResidual),
    //        EventId.SetAbility => EffectDelegate.FromNullableDelegate(OnSetAbility),
    //        EventId.SetStatus => EffectDelegate.FromNullableDelegate(OnSetStatus),
    //        EventId.SetWeather => EffectDelegate.FromNullableDelegate(OnSetWeather),
    //        EventId.SideConditionStart => EffectDelegate.FromNullableDelegate(OnSideConditionStart),
    //        EventId.StallMove => EffectDelegate.FromNullableDelegate(OnStallMove),
    //        EventId.SwitchIn => EffectDelegate.FromNullableDelegate(OnSwitchIn),
    //        EventId.SwitchOut => EffectDelegate.FromNullableDelegate(OnSwitchOut),
    //        EventId.Swap => EffectDelegate.FromNullableDelegate(OnSwap),
    //        EventId.TakeItem => EffectDelegate.FromNullableOnTakeItem(OnTakeItem),
    //        EventId.TerrainChange => EffectDelegate.FromNullableDelegate(OnTerrainChange),
    //        EventId.TrapPokemon => EffectDelegate.FromNullableDelegate(OnTrapPokemon),
    //        EventId.TryAddVolatile => EffectDelegate.FromNullableDelegate(OnTryAddVolatile),
    //        EventId.TryEatItem => EffectDelegate.FromNullableOnTryEatItem(OnTryEatItem),
    //        EventId.TryHeal => EffectDelegate.FromNullableOnTryHeal(OnTryHeal),
    //        EventId.TryHit => EffectDelegate.FromNullableDelegate(OnTryHit),
    //        EventId.TryHitField => EffectDelegate.FromNullableDelegate(OnTryHitField),
    //        EventId.TryHitSide => EffectDelegate.FromNullableDelegate(OnTryHitSide),
    //        EventId.Invulnerability => EffectDelegate.FromNullableDelegate(OnInvulnerability),
    //        EventId.TryMove => EffectDelegate.FromNullableDelegate(OnTryMove),
    //        EventId.TryPrimaryHit => EffectDelegate.FromNullableDelegate(OnTryPrimaryHit),
    //        EventId.Type => EffectDelegate.FromNullableDelegate(OnType),
    //        EventId.UseItem => EffectDelegate.FromNullableDelegate(OnUseItem),
    //        EventId.Update => EffectDelegate.FromNullableDelegate(OnUpdate),
    //        EventId.Weather => EffectDelegate.FromNullableDelegate(OnWeather),
    //        EventId.WeatherModifyDamage => EffectDelegate.FromNullableDelegate(OnWeatherModifyDamage),
    //        EventId.ModifyDamagePhase1 => EffectDelegate.FromNullableDelegate(OnModifyDamagePhase1),
    //        EventId.ModifyDamagePhase2 => EffectDelegate.FromNullableDelegate(OnModifyDamagePhase2),
    //        _ => null,
    //    };
    //}

    //public int? GetPriority(EventId id)
    //{
    //    return id switch
    //    {
    //        EventId.Accuracy => OnAccuracyPriority,
    //        EventId.DamagingHit => OnDamagingHitOrder,
    //        EventId.AfterMoveSecondary => OnAfterMoveSecondaryPriority,
    //        EventId.AfterMoveSecondarySelf => OnAfterMoveSecondarySelfPriority,
    //        EventId.AfterMoveSelf => OnAfterMoveSelfPriority,
    //        EventId.AfterSetStatus => OnAfterSetStatusPriority,
    //        EventId.AnyBasePower => OnAnyBasePowerPriority,
    //        EventId.AnyInvulnerability => OnAnyInvulnerabilityPriority,
    //        EventId.AnyModifyAccuracy => OnAnyModifyAccuracyPriority,
    //        EventId.AnyFaint => OnAnyFaintPriority,
    //        EventId.AnyPrepareHit => OnAnyPrepareHitPriority,
    //        EventId.AnySwitchIn => OnAnySwitchInPriority,
    //        EventId.AllyBasePower => OnAllyBasePowerPriority,
    //        EventId.AllyModifyAtk => OnAllyModifyAtkPriority,
    //        EventId.AllyModifySpA => OnAllyModifySpAPriority,
    //        EventId.AllyModifySpD => OnAllyModifySpDPriority,
    //        EventId.Attract => OnAttractPriority,
    //        EventId.BasePower => OnBasePowerPriority,
    //        EventId.BeforeMove => OnBeforeMovePriority,
    //        EventId.BeforeSwitchOut => OnBeforeSwitchOutPriority,
    //        EventId.ChangeBoost => OnChangeBoostPriority,
    //        EventId.Damage => OnDamagePriority,
    //        EventId.DragOut => OnDragOutPriority,
    //        EventId.Effectiveness => OnEffectivenessPriority,
    //        EventId.FoeBasePower => OnFoeBasePowerPriority,
    //        EventId.FoeBeforeMove => OnFoeBeforeMovePriority,
    //        EventId.FoeModifyDef => OnFoeModifyDefPriority,
    //        EventId.FoeModifySpD => OnFoeModifySpDPriority,
    //        EventId.FoeRedirectTarget => OnFoeRedirectTargetPriority,
    //        EventId.FoeTrapPokemon => OnFoeTrapPokemonPriority,
    //        EventId.FractionalPriority => OnFractionalPriorityPriority,
    //        EventId.Hit => OnHitPriority,
    //        EventId.Invulnerability => OnInvulnerabilityPriority,
    //        EventId.ModifyAccuracy => OnModifyAccuracyPriority,
    //        EventId.ModifyAtk => OnModifyAtkPriority,
    //        EventId.ModifyCritRatio => OnModifyCritRatioPriority,
    //        EventId.ModifyDef => OnModifyDefPriority,
    //        EventId.ModifyMove => OnModifyMovePriority,
    //        EventId.ModifyPriority => OnModifyPriorityPriority,
    //        EventId.ModifySpA => OnModifySpAPriority,
    //        EventId.ModifySpD => OnModifySpDPriority,
    //        EventId.ModifySpe => OnModifySpePriority,
    //        EventId.ModifyStab => OnModifyStabPriority,
    //        EventId.ModifyType => OnModifyTypePriority,
    //        EventId.ModifyWeight => OnModifyWeightPriority,
    //        EventId.RedirectTarget => OnRedirectTargetPriority,
    //        EventId.Residual => OnResidualPriority,
    //        EventId.SourceBasePower => OnSourceBasePowerPriority,
    //        EventId.SourceInvulnerability => OnSourceInvulnerabilityPriority,
    //        EventId.SourceModifyAccuracy => OnSourceModifyAccuracyPriority,
    //        EventId.SourceModifyAtk => OnSourceModifyAtkPriority,
    //        EventId.SourceModifyDamage => OnSourceModifyDamagePriority,
    //        EventId.SourceModifySpA => OnSourceModifySpAPriority,
    //        EventId.SwitchIn => OnSwitchInPriority,
    //        EventId.TrapPokemon => OnTrapPokemonPriority,
    //        EventId.TryBoost => OnTryBoostPriority,
    //        EventId.TryEatItem => OnTryEatItemPriority,
    //        EventId.TryHeal => OnTryHealPriority,
    //        EventId.TryHit => OnTryHitPriority,
    //        EventId.TryMove => OnTryMovePriority,
    //        EventId.TryPrimaryHit => OnTryPrimaryHitPriority,
    //        EventId.Type => OnTypePriority,
    //        _ => null,
    //    };
    //}

    //public IntFalseUnion? GetOrder(EventId id)
    //{
    //    int? order = id switch
    //    {
    //        EventId.DamagingHit => OnDamagingHitOrder,
    //        EventId.Residual => OnResidualOrder,
    //        _ => null,
    //    };
    //    return order.HasValue ? IntFalseUnion.FromInt(order.Value) : null;
    //}

    //public int? GetSubOrder(EventId id)
    //{
    //    return id switch
    //    {
    //        EventId.AnySwitchIn => OnAnySwitchInSubOrder,
    //        EventId.Residual => OnResidualSubOrder,
    //        EventId.SwitchIn => OnSwitchInSubOrder,
    //        _ => null,
    //    };
    //}

    /// <summary>
    /// Gets event handler information for the specified event.
    /// Uses high-performance mapper with O(1) lookups.
    /// </summary>
    public EventHandlerInfo? GetEventHandlerInfo(EventId id, EventPrefix? prefix = null, EventSuffix? suffix = null)
    {
        return EventHandlerInfoMapper.GetEventHandlerInfo(this, id, prefix, suffix);
    }

    /// <summary>
    /// Creates a copy of this Format for simulation purposes.
    /// This method creates an independent copy with the same state while sharing immutable references.
    /// </summary>
    /// <returns>A new Format instance with copied state</returns>
    public Format Copy()
    {
        // Since Format is a record, we can use the with expression to create a shallow copy
        // All delegate properties (event handlers) are immutable references and safe to share
        return this with { };
    }
}