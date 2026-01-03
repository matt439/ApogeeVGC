using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.ItemSpecific;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Items;

public record FlingData
{
    public int BasePower { get; init; }
    public ConditionId? Status { get; init; }
    public ConditionId? VolatileStatus { get; init; }
    public ResultMoveHandler? Effect { get; init; }
}

public partial record Item : IEffect, IBasicEffect, ICopyable<Item>
{
    public ItemId Id { get; init; }
    public EffectStateId EffectStateId => Id;
    public EffectType EffectType => EffectType.Item;
    public required string Name { get; init; }
    public string FullName => $"Item: {Name}";
    public int SpriteNum { get; init; }

    public FlingData? Fling
    {
        get
        {
            if (IsBerry)
            {
                return new FlingData { BasePower = 10 };
            }

            if (Id.ToString().EndsWith("Plate"))
            {
                return new FlingData { BasePower = 90 };
            }

            if (OnDrive != null)
            {
                return new FlingData { BasePower = 70 };
            }

            if (OnMemory != null)
            {
                return new FlingData { BasePower = 50 };
            }

            return field;
        }
        init;
    }

    public MoveType? OnDrive { get; init; }
    public int Num { get; init; }

    public int Gen
    {
        get
        {
            if (field is >= 1 and <= 9) return field;
            return Num switch
            {
                >= 1124 => 9,
                >= 927 => 8,
                >= 689 => 7,
                >= 577 => 6,
                >= 537 => 5,
                >= 377 => 4,
                >= 1 => 3,
                _ => field,
            };
        }
        init;
    }

    public MoveType? OnMemory { get; init; }
    public bool IsBerry { get; init; }
    public bool IgnoreKlutz { get; init; }
    public PokemonType? OnPlate { get; init; }
    public bool IsGem { get; init; }
    public bool IsPokeball { get; init; }
    public bool IsPrimalOrb { get; init; }
    public Condition? Condition { get; init; }
    public string? ForcedForme { get; init; }
    public bool? IsChoice { get; init; }
    public (int BasePower, string Type)? NaturalGift { get; init; }
    public ItemBoosts? Boosts { get; init; }

    public OnEatEventInfo? OnEat { get; init; }
    public OnUseEventInfo? OnUse { get; init; }
    public OnStartEventInfo? OnStart { get; init; }
    public OnEndEventInfo? OnEnd { get; init; }

    public bool AffectsFainted { get; init; }

    public Item Copy()
    {
        return this with { };
    }

    //public EffectDelegate? GetDelegate(EventId id)
    //{
    //    return id switch
    //    {
    //        EventId.Start => EffectDelegate.FromNullableDelegate(OnStart),
    //        EventId.End => EffectDelegate.FromNullableDelegate(OnEnd),
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
    //    return id switch
    //    {
    //        EventId.DamagingHit => OnDamagingHitOrder is not null ?
    //            IntFalseUnion.FromInt(OnDamagingHitOrder.Value) : null,

    //        EventId.Residual => OnResidualOrder is not null ?
    //            IntFalseUnion.FromInt(OnResidualOrder.Value) : null,

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
}