using System.Collections.Frozen;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Conditions;

public partial record Condition : ISideEventMethods, IFieldEventMethods, IEffect, IBasicEffect,
    ICopyable<Condition>
{
    public required ConditionId Id { get; init; }
    public EffectStateId EffectStateId => Id;
    public EffectType EffectType
    {
        get;
        init
        {
            if (value is not (EffectType.Condition or EffectType.Weather or EffectType.Status or EffectType.Terrain))
            {
                throw new ArgumentException("Condition EffectType must be Condition, Weather, Status, or Terrain.");
            }
            field = value;
        }
    }
    public string Name { get; init; } = string.Empty;
    public string FullName => Name;

    //public ConditionEffectType ConditionEffectType { get; init; }

    ///// <summary>
    ///// Many conditions are defined by a source effect, such as a move or ability.
    ///// This property tracks that source effect, if any.
    ///// </summary>
    //public IEffect? Source { get; init; }

    public AbilityId? AssociatedAbility { get; init; }
    public ItemId? AssociatedItem { get; init; }
    public MoveId? AssociatedMove { get; init; }
    public SpecieId? AssociatedSpecies { get; init; }

    public int? Duration { get; set; }
    public int? CounterMax { get; init; }
    public int? Counter { get; set; }

    public bool NoCopy { get; init; }

    public IReadOnlyList<PokemonType>? ImmuneTypes { get; init; }

    public bool AffectsFainted { get; init; }

    public Condition Copy()
    {
        return this with
        {
            // Records have built-in copy semantics with 'with' expression
            // This creates a shallow copy which is appropriate since most properties
            // are either value types, immutable references (strings), or function delegates
            // The only mutable properties (Duration, Counter) are copied correctly
        };
    }

    ///// <summary>
    ///// battle, target, source, effect -> number
    ///// </summary>
    //public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; init; }

    ///// <summary>
    ///// battle, pokemon
    ///// </summary>
    //public Action<Battle, Pokemon>? OnCopy { get; init; }

    ///// <summary>
    ///// battle, pokemon
    ///// </summary>
    //public Action<Battle, Pokemon>? OnEnd { get; init; }

    ///// <summary>
    ///// battle, target, source, sourceEffect -> boolean | null
    ///// </summary>
    //public Func<Battle, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnRestart { get; init; }

    ///// <summary>
    ///// battle, target, source, sourceEffect -> boolean | null
    ///// </summary>
    //public Func<Battle, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnStart { get; init; }

    public DurationCallbackEventInfo? DurationCallback { get; init; }
    public OnCopyEventInfo? OnCopy { get; init; }
    public OnEndEventInfo? OnEnd { get; init; }
    public OnRestartEventInfo? OnRestart { get; init; }
    public OnStartEventInfo? OnStart { get; init; }

    //public Action<Battle, Side, Pokemon, IEffect>? OnSideStart { get; init; }
    //public Action<Battle, Side, Pokemon, IEffect>? OnSideRestart { get; init; }
    //public Action<Battle, Side, Pokemon, IEffect>? OnSideResidual { get; init; }
    //public Action<Battle, Side>? OnSideEnd { get; init; 

    public OnSideStartEventInfo? OnSideStart { get; init; }
    public OnSideRestartEventInfo? OnSideRestart { get; init; }
    public OnSideResidualEventInfo? OnSideResidual { get; init; }
    public OnSideEndEventInfo? OnSideEnd { get; init; }
    //public int? OnSideResidualOrder { get; init; }
    //public int? OnSideResidualPriority { get; init; }
    //public int? OnSideResidualSubOrder { get; init; }



    //public Action<Battle, Field, Pokemon, IEffect>? OnFieldStart { get; init; }
    //public Action<Battle, Field, Pokemon, IEffect>? OnFieldRestart { get; init; }
    //public Action<Battle, Field, Pokemon, IEffect>? OnFieldResidual { get; init; }
    //public Action<Battle, Field>? OnFieldEnd { get; init; }
    //public int? OnFieldResidualOrder { get; init; }
    //public int? OnFieldResidualPriority { get; init; }
    //public int? OnFieldResidualSubOrder { get; init; }


    //public EffectDelegate? GetDelegate(EventId id)
    //{
    //    return id switch
    //    {
    //        EventId.End => EffectDelegate.FromNullableDelegate(OnEnd),
    //        EventId.Start => EffectDelegate.FromNullableDelegate(OnStart),
    //        EventId.Restart => EffectDelegate.FromNullableDelegate(OnRestart),
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
    //        EventId.WeatherChange => EffectDelegate.FromNullableDelegate(OnWeatherChange),
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
    //        EventId.SideStart => EffectDelegate.FromNullableDelegate(OnSideStart),
    //        EventId.SideRestart => EffectDelegate.FromNullableDelegate(OnSideRestart),
    //        EventId.SideResidual => EffectDelegate.FromNullableDelegate(OnSideResidual),
    //        EventId.SideEnd => EffectDelegate.FromNullableDelegate(OnSideEnd),
    //        EventId.FieldStart => EffectDelegate.FromNullableDelegate(OnFieldStart),
    //        EventId.FieldRestart => EffectDelegate.FromNullableDelegate(OnFieldRestart),
    //        EventId.FieldResidual => EffectDelegate.FromNullableDelegate(OnFieldResidual),
    //        EventId.FieldEnd => EffectDelegate.FromNullableDelegate(OnFieldEnd),
    //        _ => null,
    //    };
    //}

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
    //        EventId.SideResidual => OnSideResidualPriority,
    //        EventId.FieldResidual => OnFieldResidualPriority,
    //        _ => null,
    //    };
    //}

    //public IntFalseUnion? GetOrder(EventId id)
    //{
    //    return id switch
    //    {
    //        EventId.DamagingHit => OnDamagingHitOrder.HasValue ? IntFalseUnion.FromInt(OnDamagingHitOrder.Value) :
    //            null,
    //        EventId.Residual => OnResidualOrder.HasValue ? IntFalseUnion.FromInt(OnResidualOrder.Value) : null,
    //        EventId.SideResidual => OnSideResidualOrder.HasValue ? IntFalseUnion.FromInt(OnSideResidualOrder.Value) :
    //            null,

    //        EventId.FieldResidual => OnFieldResidualOrder.HasValue ?
    //            IntFalseUnion.FromInt(OnFieldResidualOrder.Value) : null,

    //        _ => null,
    //    };
    //}

    //public int? GetSubOrder(EventId id)
    //{
    //    return id switch
    //    {
    //        EventId.AnySwitchIn => OnAnySwitchInSubOrder,
    //        EventId.Residual => OnResidualSubOrder,
    //        EventId.SwitchIn => OnSwitchInSubOrder,
    //        EventId.SideResidual => OnSideResidualSubOrder,
    //        EventId.FieldResidual => OnFieldResidualSubOrder,
    //        _ => null,
    //    };
    //}
}