using System.Collections.Frozen;
using ApogeeVGC.Sim.Effects;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// High-performance mapper for retrieving EventHandlerInfo from IEffect implementations.
/// Uses frozen dictionaries for O(1) lookups without reflection.
/// </summary>
public static class EventHandlerInfoMapper
{
    /// <summary>
    /// Maps EventId to property accessor functions for IEventMethodsV2.
    /// Each function takes an IEventMethodsV2 and returns the corresponding EventHandlerInfo.
    /// </summary>
    private static readonly FrozenDictionary<EventId, Func<IEventMethods, EventHandlerInfo?>>
        EventMethodsMap =
            new Dictionary<EventId, Func<IEventMethods, EventHandlerInfo?>>
            {
                // Base Events (no prefix)
                [EventId.Accuracy] = e => e.OnAccuracy,
                [EventId.AfterBoost] = e => e.OnAfterBoost,
                [EventId.AfterEachBoost] = e => e.OnAfterEachBoost,
                [EventId.AfterFaint] = e => e.OnAfterFaint,
                [EventId.AfterHit] = e => e.OnAfterHit,
                [EventId.AfterMega] = e => e.OnAfterMega,
                [EventId.AfterMove] = e => e.OnAfterMove,
                [EventId.AfterMoveSecondary] = e => e.OnAfterMoveSecondary,
                [EventId.AfterMoveSecondarySelf] = e => e.OnAfterMoveSecondarySelf,
                [EventId.AfterMoveSelf] = e => e.OnAfterMoveSelf,
                [EventId.AfterSetStatus] = e => e.OnAfterSetStatus,
                [EventId.AfterSubDamage] = e => e.OnAfterSubDamage,
                [EventId.AfterSwitchInSelf] = e => e.OnAfterSwitchInSelf,
                [EventId.AfterTakeItem] = e => e.OnAfterTakeItem,
                [EventId.AfterTerastallization] = e => e.OnAfterTerastallization,
                [EventId.AfterUseItem] = e => e.OnAfterUseItem,
                [EventId.Attract] = e => e.OnAttract,
                [EventId.BasePower] = e => e.OnBasePower,
                [EventId.BeforeFaint] = e => e.OnBeforeFaint,
                [EventId.BeforeMove] = e => e.OnBeforeMove,
                [EventId.BeforeSwitchIn] = e => e.OnBeforeSwitchIn,
                [EventId.BeforeSwitchOut] = e => e.OnBeforeSwitchOut,
                [EventId.BeforeTurn] = e => e.OnBeforeTurn,
                [EventId.ChangeBoost] = e => e.OnChangeBoost,
                [EventId.ChargeMove] = e => e.OnChargeMove,
                [EventId.CriticalHit] = e => e.OnCriticalHit,
                [EventId.Damage] = e => e.OnDamage,
                [EventId.DamagingHit] = e => e.OnDamagingHit,
                [EventId.DeductPp] = e => e.OnDeductPp,
                [EventId.DisableMove] = e => e.OnDisableMove,
                [EventId.DragOut] = e => e.OnDragOut,
                [EventId.EatItem] = e => e.OnEatItem,
                [EventId.Effectiveness] = e => e.OnEffectiveness,
                [EventId.EmergencyExit] = e => e.OnEmergencyExit,
                [EventId.EntryHazard] = e => e.OnEntryHazard,
                [EventId.Faint] = e => e.OnFaint,
                [EventId.Flinch] = e => e.OnFlinch,
                [EventId.FractionalPriority] = e => e.OnFractionalPriority,
                [EventId.Hit] = e => e.OnHit,
                [EventId.Immunity] = e => e.OnImmunity,
                [EventId.Invulnerability] = e => e.OnInvulnerability,
                [EventId.LockMove] = e => e.OnLockMove,
                [EventId.MaybeTrapPokemon] = e => e.OnMaybeTrapPokemon,
                [EventId.ModifyAccuracy] = e => e.OnModifyAccuracy,
                [EventId.ModifyAtk] = e => e.OnModifyAtk,
                [EventId.ModifyBoost] = e => e.OnModifyBoost,
                [EventId.ModifyCritRatio] = e => e.OnModifyCritRatio,
                [EventId.ModifyDamage] = e => e.OnModifyDamage,
                [EventId.ModifyDamagePhase1] = e => e.OnModifyDamagePhase1,
                [EventId.ModifyDamagePhase2] = e => e.OnModifyDamagePhase2,
                [EventId.ModifyDef] = e => e.OnModifyDef,
                [EventId.ModifyMove] = e => e.OnModifyMove,
                [EventId.ModifyPriority] = e => e.OnModifyPriority,
                [EventId.ModifySecondaries] = e => e.OnModifySecondaries,
                [EventId.ModifySpA] = e => e.OnModifySpA,
                [EventId.ModifySpD] = e => e.OnModifySpD,
                [EventId.ModifySpe] = e => e.OnModifySpe,
                [EventId.ModifyStab] = e => e.OnModifyStab,
                [EventId.ModifyTarget] = e => e.OnModifyTarget,
                [EventId.ModifyType] = e => e.OnModifyType,
                [EventId.ModifyWeight] = e => e.OnModifyWeight,
                [EventId.MoveAborted] = e => e.OnMoveAborted,
                [EventId.NegateImmunity] = e => e.OnNegateImmunity,
                [EventId.OverrideAction] = e => e.OnOverrideAction,
                [EventId.PrepareHit] = e => e.OnPrepareHit,
                [EventId.PseudoWeatherChange] = e => e.OnPseudoWeatherChange,
                [EventId.RedirectTarget] = e => e.OnRedirectTarget,
                [EventId.Residual] = e => e.OnResidual,
                [EventId.SetAbility] = e => e.OnSetAbility,
                [EventId.SetStatus] = e => e.OnSetStatus,
                [EventId.SetWeather] = e => e.OnSetWeather,
                [EventId.SideConditionStart] = e => e.OnSideConditionStart,
                [EventId.StallMove] = e => e.OnStallMove,
                [EventId.SwitchIn] = e => e.OnSwitchIn,
                [EventId.SwitchOut] = e => e.OnSwitchOut,
                [EventId.Swap] = e => e.OnSwap,
                [EventId.TakeItem] = e => e.OnTakeItem,
                [EventId.TerrainChange] = e => e.OnTerrainChange,
                [EventId.TrapPokemon] = e => e.OnTrapPokemon,
                [EventId.TryAddVolatile] = e => e.OnTryAddVolatile,
                [EventId.TryBoost] = e => e.OnTryBoost,
                [EventId.TryEatItem] = e => e.OnTryEatItem,
                [EventId.TryHeal] = e => e.OnTryHeal,
                [EventId.TryHit] = e => e.OnTryHit,
                [EventId.TryHitField] = e => e.OnTryHitField,
                [EventId.TryHitSide] = e => e.OnTryHitSide,
                [EventId.TryMove] = e => e.OnTryMove,
                [EventId.TryPrimaryHit] = e => e.OnTryPrimaryHit,
                [EventId.Type] = e => e.OnType,
                [EventId.Update] = e => e.OnUpdate,
                [EventId.UseItem] = e => e.OnUseItem,
                [EventId.Weather] = e => e.OnWeather,
                [EventId.WeatherChange] = e => e.OnWeatherChange,
                [EventId.WeatherModifyDamage] = e => e.OnWeatherModifyDamage,

                // Field event handlers
                [EventId.FieldStart] = e => e is IFieldEventMethods f ? f.OnFieldStart : null,
                [EventId.FieldRestart] = e => e is IFieldEventMethods f ? f.OnFieldRestart : null,
                [EventId.FieldResidual] = e => e is IFieldEventMethods f ? f.OnFieldResidual : null,
                [EventId.FieldEnd] = e => e is IFieldEventMethods f ? f.OnFieldEnd : null,

                // Side event handlers
                [EventId.SideStart] = e => e is ISideEventMethods s ? s.OnSideStart : null,
                [EventId.SideRestart] = e => e is ISideEventMethods s ? s.OnSideRestart : null,
                [EventId.SideResidual] = e => e is ISideEventMethods s ? s.OnSideResidual : null,
                [EventId.SideEnd] = e => e is ISideEventMethods s ? s.OnSideEnd : null,
            }.ToFrozenDictionary();

    /// <summary>
    /// Maps EventId + EventPrefix.Foe to property accessor functions.
    /// </summary>
    private static readonly FrozenDictionary<EventId, Func<IEventMethods, EventHandlerInfo?>>
        FoeEventMethodsMap =
            new Dictionary<EventId, Func<IEventMethods, EventHandlerInfo?>>
            {
                [EventId.Accuracy] = e => e.OnFoeAccuracy,
                [EventId.AfterBoost] = e => e.OnFoeAfterBoost,
                [EventId.AfterEachBoost] = e => e.OnFoeAfterEachBoost,
                [EventId.AfterFaint] = e => e.OnFoeAfterFaint,
                [EventId.AfterHit] = e => e.OnFoeAfterHit,
                [EventId.AfterMove] = e => e.OnFoeAfterMove,
                [EventId.AfterMoveSecondary] = e => e.OnFoeAfterMoveSecondary,
                [EventId.AfterMoveSecondarySelf] = e => e.OnFoeAfterMoveSecondarySelf,
                [EventId.AfterMoveSelf] = e => e.OnFoeAfterMoveSelf,
                [EventId.AfterSetStatus] = e => e.OnFoeAfterSetStatus,
                [EventId.AfterSubDamage] = e => e.OnFoeAfterSubDamage,
                [EventId.AfterSwitchInSelf] = e => e.OnFoeAfterSwitchInSelf,
                [EventId.AfterUseItem] = e => e.OnFoeAfterUseItem,
                [EventId.Attract] = e => e.OnFoeAttract,
                [EventId.BasePower] = e => e.OnFoeBasePower,
                [EventId.BeforeFaint] = e => e.OnFoeBeforeFaint,
                [EventId.BeforeMove] = e => e.OnFoeBeforeMove,
                [EventId.BeforeSwitchIn] = e => e.OnFoeBeforeSwitchIn,
                [EventId.BeforeSwitchOut] = e => e.OnFoeBeforeSwitchOut,
                [EventId.ChargeMove] = e => e.OnFoeChargeMove,
                [EventId.CriticalHit] = e => e.OnFoeCriticalHit,
                [EventId.Damage] = e => e.OnFoeDamage,
                [EventId.DamagingHit] = e => e.OnFoeDamagingHit,
                [EventId.DeductPp] = e => e.OnFoeDeductPp,
                [EventId.DisableMove] = e => e.OnFoeDisableMove,
                [EventId.DragOut] = e => e.OnFoeDragOut,
                [EventId.EatItem] = e => e.OnFoeEatItem,
                [EventId.Effectiveness] = e => e.OnFoeEffectiveness,
                [EventId.Faint] = e => e.OnFoeFaint,
                [EventId.Flinch] = e => e.OnFoeFlinch,
                [EventId.Hit] = e => e.OnFoeHit,
                [EventId.Immunity] = e => e.OnFoeImmunity,
                [EventId.Invulnerability] = e => e.OnFoeInvulnerability,
                [EventId.LockMove] = e => e.OnFoeLockMove,
                [EventId.MaybeTrapPokemon] = e => e.OnFoeMaybeTrapPokemon,
                [EventId.ModifyAccuracy] = e => e.OnFoeModifyAccuracy,
                [EventId.ModifyAtk] = e => e.OnFoeModifyAtk,
                [EventId.ModifyBoost] = e => e.OnFoeModifyBoost,
                [EventId.ModifyCritRatio] = e => e.OnFoeModifyCritRatio,
                [EventId.ModifyDamage] = e => e.OnFoeModifyDamage,
                [EventId.ModifyDamagePhase1] = e => e.OnFoeModifyDamagePhase1,
                [EventId.ModifyDamagePhase2] = e => e.OnFoeModifyDamagePhase2,
                [EventId.ModifyDef] = e => e.OnFoeModifyDef,
                [EventId.ModifyMove] = e => e.OnFoeModifyMove,
                [EventId.ModifyPriority] = e => e.OnFoeModifyPriority,
                [EventId.ModifySecondaries] = e => e.OnFoeModifySecondaries,
                [EventId.ModifySpA] = e => e.OnFoeModifySpA,
                [EventId.ModifySpD] = e => e.OnFoeModifySpD,
                [EventId.ModifySpe] = e => e.OnFoeModifySpe,
                [EventId.ModifyStab] = e => e.OnFoeModifyStab,
                [EventId.ModifyTarget] = e => e.OnFoeModifyTarget,
                [EventId.ModifyType] = e => e.OnFoeModifyType,
                [EventId.ModifyWeight] = e => e.OnFoeModifyWeight,
                [EventId.MoveAborted] = e => e.OnFoeMoveAborted,
                [EventId.NegateImmunity] = e => e.OnFoeNegateImmunity,
                [EventId.OverrideAction] = e => e.OnFoeOverrideAction,
                [EventId.PrepareHit] = e => e.OnFoePrepareHit,
                [EventId.RedirectTarget] = e => e.OnFoeRedirectTarget,
                [EventId.Residual] = e => e.OnFoeResidual,
                [EventId.SetAbility] = e => e.OnFoeSetAbility,
                [EventId.SetStatus] = e => e.OnFoeSetStatus,
                [EventId.SetWeather] = e => e.OnFoeSetWeather,
                [EventId.StallMove] = e => e.OnFoeStallMove,
                [EventId.SwitchOut] = e => e.OnFoeSwitchOut,
                [EventId.TakeItem] = e => e.OnFoeTakeItem,
                [EventId.TerrainChange] = e => e.OnFoeTerrain,
                [EventId.TrapPokemon] = e => e.OnFoeTrapPokemon,
                [EventId.TryAddVolatile] = e => e.OnFoeTryAddVolatile,
                [EventId.TryBoost] = e => e.OnFoeTryBoost,
                [EventId.TryEatItem] = e => e.OnFoeTryEatItem,
                [EventId.TryHeal] = e => e.OnFoeTryHeal,
                [EventId.TryHit] = e => e.OnFoeTryHit,
                [EventId.TryHitField] = e => e.OnFoeTryHitField,
                [EventId.TryHitSide] = e => e.OnFoeTryHitSide,
                [EventId.TryMove] = e => e.OnFoeTryMove,
                [EventId.TryPrimaryHit] = e => e.OnFoeTryPrimaryHit,
                [EventId.Type] = e => e.OnFoeType,
                [EventId.WeatherModifyDamage] = e => e.OnFoeWeatherModifyDamage,
            }.ToFrozenDictionary();

    /// <summary>
    /// Maps EventId + EventPrefix.Source to property accessor functions.
    /// </summary>
    private static readonly FrozenDictionary<EventId, Func<IEventMethods, EventHandlerInfo?>>
        SourceEventMethodsMap =
            new Dictionary<EventId, Func<IEventMethods, EventHandlerInfo?>>
            {
                [EventId.Accuracy] = e => e.OnSourceAccuracy,
                [EventId.AfterBoost] = e => e.OnSourceAfterBoost,
                [EventId.AfterEachBoost] = e => e.OnSourceAfterEachBoost,
                [EventId.AfterFaint] = e => e.OnSourceAfterFaint,
                [EventId.AfterHit] = e => e.OnSourceAfterHit,
                [EventId.AfterMove] = e => e.OnSourceAfterMove,
                [EventId.AfterMoveSecondary] = e => e.OnSourceAfterMoveSecondary,
                [EventId.AfterMoveSecondarySelf] = e => e.OnSourceAfterMoveSecondarySelf,
                [EventId.AfterMoveSelf] = e => e.OnSourceAfterMoveSelf,
                [EventId.AfterSetStatus] = e => e.OnSourceAfterSetStatus,
                [EventId.AfterSubDamage] = e => e.OnSourceAfterSubDamage,
                [EventId.AfterSwitchInSelf] = e => e.OnSourceAfterSwitchInSelf,
                [EventId.AfterUseItem] = e => e.OnSourceAfterUseItem,
                [EventId.Attract] = e => e.OnSourceAttract,
                [EventId.BasePower] = e => e.OnSourceBasePower,
                [EventId.BeforeFaint] = e => e.OnSourceBeforeFaint,
                [EventId.BeforeMove] = e => e.OnSourceBeforeMove,
                [EventId.BeforeSwitchIn] = e => e.OnSourceBeforeSwitchIn,
                [EventId.BeforeSwitchOut] = e => e.OnSourceBeforeSwitchOut,
                [EventId.ChargeMove] = e => e.OnSourceChargeMove,
                [EventId.CriticalHit] = e => e.OnSourceCriticalHit,
                [EventId.Damage] = e => e.OnSourceDamage,
                [EventId.DamagingHit] = e => e.OnSourceDamagingHit,
                [EventId.DeductPp] = e => e.OnSourceDeductPp,
                [EventId.DisableMove] = e => e.OnSourceDisableMove,
                [EventId.DragOut] = e => e.OnSourceDragOut,
                [EventId.EatItem] = e => e.OnSourceEatItem,
                [EventId.Effectiveness] = e => e.OnSourceEffectiveness,
                [EventId.Faint] = e => e.OnSourceFaint,
                [EventId.Flinch] = e => e.OnSourceFlinch,
                [EventId.Hit] = e => e.OnSourceHit,
                [EventId.Immunity] = e => e.OnSourceImmunity,
                [EventId.Invulnerability] = e => e.OnSourceInvulnerability,
                [EventId.LockMove] = e => e.OnSourceLockMove,
                [EventId.MaybeTrapPokemon] = e => e.OnSourceMaybeTrapPokemon,
                [EventId.ModifyAccuracy] = e => e.OnSourceModifyAccuracy,
                [EventId.ModifyAtk] = e => e.OnSourceModifyAtk,
                [EventId.ModifyBoost] = e => e.OnSourceModifyBoost,
                [EventId.ModifyCritRatio] = e => e.OnSourceModifyCritRatio,
                [EventId.ModifyDamage] = e => e.OnSourceModifyDamage,
                [EventId.ModifyDamagePhase1] = e => e.OnSourceModifyDamagePhase1,
                [EventId.ModifyDamagePhase2] = e => e.OnSourceModifyDamagePhase2,
                [EventId.ModifyDef] = e => e.OnSourceModifyDef,
                [EventId.ModifyMove] = e => e.OnSourceModifyMove,
                [EventId.ModifyPriority] = e => e.OnSourceModifyPriority,
                [EventId.ModifySecondaries] = e => e.OnSourceModifySecondaries,
                [EventId.ModifySpA] = e => e.OnSourceModifySpA,
                [EventId.ModifySpD] = e => e.OnSourceModifySpD,
                [EventId.ModifySpe] = e => e.OnSourceModifySpe,
                [EventId.ModifyStab] = e => e.OnSourceModifyStab,
                [EventId.ModifyTarget] = e => e.OnSourceModifyTarget,
                [EventId.ModifyType] = e => e.OnSourceModifyType,
                [EventId.ModifyWeight] = e => e.OnSourceModifyWeight,
                [EventId.MoveAborted] = e => e.OnSourceMoveAborted,
                [EventId.NegateImmunity] = e => e.OnSourceNegateImmunity,
                [EventId.OverrideAction] = e => e.OnSourceOverrideAction,
                [EventId.PrepareHit] = e => e.OnSourcePrepareHit,
                [EventId.RedirectTarget] = e => e.OnSourceRedirectTarget,
                [EventId.Residual] = e => e.OnSourceResidual,
                [EventId.SetAbility] = e => e.OnSourceSetAbility,
                [EventId.SetStatus] = e => e.OnSourceSetStatus,
                [EventId.SetWeather] = e => e.OnSourceSetWeather,
                [EventId.StallMove] = e => e.OnSourceStallMove,
                [EventId.SwitchOut] = e => e.OnSourceSwitchOut,
                [EventId.TakeItem] = e => e.OnSourceTakeItem,
                [EventId.TerrainChange] = e => e.OnSourceTerrain,
                [EventId.TrapPokemon] = e => e.OnSourceTrapPokemon,
                [EventId.TryAddVolatile] = e => e.OnSourceTryAddVolatile,
                [EventId.TryBoost] = e => e.OnSourceTryBoost,
                [EventId.TryEatItem] = e => e.OnSourceTryEatItem,
                [EventId.TryHeal] = e => e.OnSourceTryHeal,
                [EventId.TryHit] = e => e.OnSourceTryHit,
                [EventId.TryHitField] = e => e.OnSourceTryHitField,
                [EventId.TryHitSide] = e => e.OnSourceTryHitSide,
                [EventId.TryMove] = e => e.OnSourceTryMove,
                [EventId.TryPrimaryHit] = e => e.OnSourceTryPrimaryHit,
                [EventId.Type] = e => e.OnSourceType,
                [EventId.WeatherModifyDamage] = e => e.OnSourceWeatherModifyDamage,
            }.ToFrozenDictionary();

    /// <summary>
    /// Maps EventId + EventPrefix.Any to property accessor functions.
    /// </summary>
    private static readonly FrozenDictionary<EventId, Func<IEventMethods, EventHandlerInfo?>>
        AnyEventMethodsMap =
            new Dictionary<EventId, Func<IEventMethods, EventHandlerInfo?>>
            {
                [EventId.Accuracy] = e => e.OnAnyAccuracy,
                [EventId.AfterBoost] = e => e.OnAnyAfterBoost,
                [EventId.AfterEachBoost] = e => e.OnAnyAfterEachBoost,
                [EventId.AfterFaint] = e => e.OnAnyAfterFaint,
                [EventId.AfterHit] = e => e.OnAnyAfterHit,
                [EventId.AfterMega] = e => e.OnAnyAfterMega,
                [EventId.AfterMove] = e => e.OnAnyAfterMove,
                [EventId.AfterMoveSecondary] = e => e.OnAnyAfterMoveSecondary,
                [EventId.AfterMoveSecondarySelf] = e => e.OnAnyAfterMoveSecondarySelf,
                [EventId.AfterMoveSelf] = e => e.OnAnyAfterMoveSelf,
                [EventId.AfterSetStatus] = e => e.OnAnyAfterSetStatus,
                [EventId.AfterSubDamage] = e => e.OnAnyAfterSubDamage,
                [EventId.AfterSwitchInSelf] = e => e.OnAnyAfterSwitchInSelf,
                [EventId.AfterTerastallization] = e => e.OnAnyAfterTerastallization,
                [EventId.AfterUseItem] = e => e.OnAnyAfterUseItem,
                [EventId.Attract] = e => e.OnAnyAttract,
                [EventId.BasePower] = e => e.OnAnyBasePower,
                [EventId.BeforeFaint] = e => e.OnAnyBeforeFaint,
                [EventId.BeforeMove] = e => e.OnAnyBeforeMove,
                [EventId.BeforeSwitchIn] = e => e.OnAnyBeforeSwitchIn,
                [EventId.BeforeSwitchOut] = e => e.OnAnyBeforeSwitchOut,
                [EventId.ChargeMove] = e => e.OnAnyChargeMove,
                [EventId.CriticalHit] = e => e.OnAnyCriticalHit,
                [EventId.Damage] = e => e.OnAnyDamage,
                [EventId.DamagingHit] = e => e.OnAnyDamagingHit,
                [EventId.DeductPp] = e => e.OnAnyDeductPp,
                [EventId.DisableMove] = e => e.OnAnyDisableMove,
                [EventId.DragOut] = e => e.OnAnyDragOut,
                [EventId.EatItem] = e => e.OnAnyEatItem,
                [EventId.Effectiveness] = e => e.OnAnyEffectiveness,
                [EventId.Faint] = e => e.OnAnyFaint,
                [EventId.Flinch] = e => e.OnAnyFlinch,
                [EventId.Hit] = e => e.OnAnyHit,
                [EventId.Immunity] = e => e.OnAnyImmunity,
                [EventId.Invulnerability] = e => e.OnAnyInvulnerability,
                [EventId.LockMove] = e => e.OnAnyLockMove,
                [EventId.MaybeTrapPokemon] = e => e.OnAnyMaybeTrapPokemon,
                [EventId.ModifyAccuracy] = e => e.OnAnyModifyAccuracy,
                [EventId.ModifyAtk] = e => e.OnAnyModifyAtk,
                [EventId.ModifyBoost] = e => e.OnAnyModifyBoost,
                [EventId.ModifyCritRatio] = e => e.OnAnyModifyCritRatio,
                [EventId.ModifyDamage] = e => e.OnAnyModifyDamage,
                [EventId.ModifyDamagePhase1] = e => e.OnAnyModifyDamagePhase1,
                [EventId.ModifyDamagePhase2] = e => e.OnAnyModifyDamagePhase2,
                [EventId.ModifyDef] = e => e.OnAnyModifyDef,
                [EventId.ModifyMove] = e => e.OnAnyModifyMove,
                [EventId.ModifyPriority] = e => e.OnAnyModifyPriority,
                [EventId.ModifySecondaries] = e => e.OnAnyModifySecondaries,
                [EventId.ModifySpA] = e => e.OnAnyModifySpA,
                [EventId.ModifySpD] = e => e.OnAnyModifySpD,
                [EventId.ModifySpe] = e => e.OnAnyModifySpe,
                [EventId.ModifyStab] = e => e.OnAnyModifyStab,
                [EventId.ModifyTarget] = e => e.OnAnyModifyTarget,
                [EventId.ModifyType] = e => e.OnAnyModifyType,
                [EventId.ModifyWeight] = e => e.OnAnyModifyWeight,
                [EventId.MoveAborted] = e => e.OnAnyMoveAborted,
                [EventId.NegateImmunity] = e => e.OnAnyNegateImmunity,
                [EventId.OverrideAction] = e => e.OnAnyOverrideAction,
                [EventId.PrepareHit] = e => e.OnAnyPrepareHit,
                [EventId.PseudoWeatherChange] = e => e.OnAnyPseudoWeatherChange,
                [EventId.RedirectTarget] = e => e.OnAnyRedirectTarget,
                [EventId.Residual] = e => e.OnAnyResidual,
                [EventId.SetAbility] = e => e.OnAnySetAbility,
                [EventId.SetStatus] = e => e.OnAnySetStatus,
                [EventId.SetWeather] = e => e.OnAnySetWeather,
                [EventId.StallMove] = e => e.OnAnyStallMove,
                [EventId.SwitchIn] = e => e.OnAnySwitchIn,
                [EventId.SwitchOut] = e => e.OnAnySwitchOut,
                [EventId.TakeItem] = e => e.OnAnyTakeItem,
                [EventId.TerrainChange] = e => e.OnAnyTerrain,
                [EventId.TrapPokemon] = e => e.OnAnyTrapPokemon,
                [EventId.TryAddVolatile] = e => e.OnAnyTryAddVolatile,
                [EventId.TryBoost] = e => e.OnAnyTryBoost,
                [EventId.TryEatItem] = e => e.OnAnyTryEatItem,
                [EventId.TryHeal] = e => e.OnAnyTryHeal,
                [EventId.TryHit] = e => e.OnAnyTryHit,
                [EventId.TryHitField] = e => e.OnAnyTryHitField,
                [EventId.TryHitSide] = e => e.OnAnyTryHitSide,
                [EventId.TryMove] = e => e.OnAnyTryMove,
                [EventId.TryPrimaryHit] = e => e.OnAnyTryPrimaryHit,
                [EventId.Type] = e => e.OnAnyType,
                [EventId.WeatherModifyDamage] = e => e.OnAnyWeatherModifyDamage,
            }.ToFrozenDictionary();

    /// <summary>
    /// Maps EventId + EventPrefix.Ally to property accessor functions (IPokemonEventMethodsV2).
    /// </summary>
    private static readonly
        FrozenDictionary<EventId, Func<IPokemonEventMethods, EventHandlerInfo?>>
        AllyEventMethodsMap =
            new Dictionary<EventId, Func<IPokemonEventMethods, EventHandlerInfo?>>
            {
                [EventId.Accuracy] = e => e.OnAllyAccuracy,
                [EventId.AfterBoost] = e => e.OnAllyAfterBoost,
                [EventId.AfterEachBoost] = e => e.OnAllyAfterEachBoost,
                [EventId.AfterFaint] = e => e.OnAllyAfterFaint,
                [EventId.AfterHit] = e => e.OnAllyAfterHit,
                [EventId.AfterMove] = e => e.OnAllyAfterMove,
                [EventId.AfterMoveSecondary] = e => e.OnAllyAfterMoveSecondary,
                [EventId.AfterMoveSecondarySelf] = e => e.OnAllyAfterMoveSecondarySelf,
                [EventId.AfterMoveSelf] = e => e.OnAllyAfterMoveSelf,
                [EventId.AfterSetStatus] = e => e.OnAllyAfterSetStatus,
                [EventId.AfterSubDamage] = e => e.OnAllyAfterSubDamage,
                [EventId.AfterSwitchInSelf] = e => e.OnAllyAfterSwitchInSelf,
                [EventId.AfterUseItem] = e => e.OnAllyAfterUseItem,
                [EventId.Attract] = e => e.OnAllyAttract,
                [EventId.BasePower] = e => e.OnAllyBasePower,
                [EventId.BeforeFaint] = e => e.OnAllyBeforeFaint,
                [EventId.BeforeMove] = e => e.OnAllyBeforeMove,
                [EventId.BeforeSwitchIn] = e => e.OnAllyBeforeSwitchIn,
                [EventId.BeforeSwitchOut] = e => e.OnAllyBeforeSwitchOut,
                [EventId.ChargeMove] = e => e.OnAllyChargeMove,
                [EventId.CriticalHit] = e => e.OnAllyCriticalHit,
                [EventId.Damage] = e => e.OnAllyDamage,
                [EventId.DamagingHit] = e => e.OnAllyDamagingHit,
                [EventId.DeductPp] = e => e.OnAllyDeductPp,
                [EventId.DisableMove] = e => e.OnAllyDisableMove,
                [EventId.DragOut] = e => e.OnAllyDragOut,
                [EventId.EatItem] = e => e.OnAllyEatItem,
                [EventId.Effectiveness] = e => e.OnAllyEffectiveness,
                [EventId.Faint] = e => e.OnAllyFaint,
                [EventId.Flinch] = e => e.OnAllyFlinch,
                [EventId.Hit] = e => e.OnAllyHit,
                [EventId.Immunity] = e => e.OnAllyImmunity,
                [EventId.Invulnerability] = e => e.OnAllyInvulnerability,
                [EventId.LockMove] = e => e.OnAllyLockMove,
                [EventId.MaybeTrapPokemon] = e => e.OnAllyMaybeTrapPokemon,
                [EventId.ModifyAccuracy] = e => e.OnAllyModifyAccuracy,
                [EventId.ModifyAtk] = e => e.OnAllyModifyAtk,
                [EventId.ModifyBoost] = e => e.OnAllyModifyBoost,
                [EventId.ModifyCritRatio] = e => e.OnAllyModifyCritRatio,
                [EventId.ModifyDamage] = e => e.OnAllyModifyDamage,
                [EventId.ModifyDamagePhase1] = e => e.OnAllyModifyDamagePhase1,
                [EventId.ModifyDamagePhase2] = e => e.OnAllyModifyDamagePhase2,
                [EventId.ModifyDef] = e => e.OnAllyModifyDef,
                [EventId.ModifyMove] = e => e.OnAllyModifyMove,
                [EventId.ModifyPriority] = e => e.OnAllyModifyPriority,
                [EventId.ModifySecondaries] = e => e.OnAllyModifySecondaries,
                [EventId.ModifySpA] = e => e.OnAllyModifySpA,
                [EventId.ModifySpD] = e => e.OnAllyModifySpD,
                [EventId.ModifySpe] = e => e.OnAllyModifySpe,
                [EventId.ModifyStab] = e => e.OnAllyModifyStab,
                [EventId.ModifyTarget] = e => e.OnAllyModifyTarget,
                [EventId.ModifyType] = e => e.OnAllyModifyType,
                [EventId.ModifyWeight] = e => e.OnAllyModifyWeight,
                [EventId.MoveAborted] = e => e.OnAllyMoveAborted,
                [EventId.NegateImmunity] = e => e.OnAllyNegateImmunity,
                [EventId.OverrideAction] = e => e.OnAllyOverrideAction,
                [EventId.PrepareHit] = e => e.OnAllyPrepareHit,
                [EventId.RedirectTarget] = e => e.OnAllyRedirectTarget,
                [EventId.Residual] = e => e.OnAllyResidual,
                [EventId.SetAbility] = e => e.OnAllySetAbility,
                [EventId.SetStatus] = e => e.OnAllySetStatus,
                [EventId.SetWeather] = e => e.OnAllySetWeather,
                [EventId.StallMove] = e => e.OnAllyStallMove,
                [EventId.SwitchOut] = e => e.OnAllySwitchOut,
                [EventId.TakeItem] = e => e.OnAllyTakeItem,
                [EventId.TerrainChange] = e => e.OnAllyTerrain,
                [EventId.TrapPokemon] = e => e.OnAllyTrapPokemon,
                [EventId.TryAddVolatile] = e => e.OnAllyTryAddVolatile,
                [EventId.TryBoost] = e => e.OnAllyTryBoost,
                [EventId.TryEatItem] = e => e.OnAllyTryEatItem,
                [EventId.TryHeal] = e => e.OnAllyTryHeal,
                [EventId.TryHit] = e => e.OnAllyTryHit,
                [EventId.TryHitField] = e => e.OnAllyTryHitField,
                [EventId.TryHitSide] = e => e.OnAllyTryHitSide,
                [EventId.TryMove] = e => e.OnAllyTryMove,
                [EventId.TryPrimaryHit] = e => e.OnAllyTryPrimaryHit,
                [EventId.Type] = e => e.OnAllyType,
                [EventId.WeatherModifyDamage] = e => e.OnAllyWeatherModifyDamage,
            }.ToFrozenDictionary();

    /// <summary>
    /// Maps EventId to property accessor functions for IAbilityEventMethodsV2.
    /// These are ability-specific events like OnCheckShow, OnStart, OnEnd.
    /// </summary>
    private static readonly
        FrozenDictionary<EventId, Func<IAbilityEventMethodsV2, EventHandlerInfo?>>
        AbilityEventMethodsMap =
            new Dictionary<EventId, Func<IAbilityEventMethodsV2, EventHandlerInfo?>>
            {
                [EventId.CheckShow] = e => e.OnCheckShow,
                [EventId.End] = e => e.OnEnd,
                [EventId.Start] = e => e.OnStart,
            }.ToFrozenDictionary();

    /// <summary>
    /// Maps EventId to property accessor functions for IMoveEventMethods.
    /// These are move-specific events like OnPrepareHit, OnTry, OnHit, OnTryHit, etc.
    /// </summary>
    private static readonly FrozenDictionary<EventId, Func<IMoveEventMethods, EventHandlerInfo?>>
        MoveEventMethodsMap =
            new Dictionary<EventId, Func<IMoveEventMethods, EventHandlerInfo?>>
            {
                [EventId.AfterHit] = e => e.OnAfterHit,
                [EventId.AfterMoveSecondary] = e => e.OnAfterMoveSecondary,
                [EventId.AfterMoveSecondarySelf] = e => e.OnAfterMoveSecondarySelf,
                [EventId.AfterMove] = e => e.OnAfterMove,
                [EventId.AfterSubDamage] = e => e.OnAfterSubDamage,
                [EventId.BasePower] = e => e.OnBasePower,
                [EventId.BasePowerCallback] = e => e.BasePowerCallback,
                [EventId.BeforeMoveCallback] = e => e.BeforeMoveCallback,
                [EventId.BeforeTurnCallback] = e => e.BeforeTurnCallback,
                [EventId.Damage] = e => e.OnDamage,
                [EventId.DamageCallback] = e => e.DamageCallback,
                [EventId.DisableMove] = e => e.OnDisableMove,
                [EventId.Effectiveness] = e => e.OnEffectiveness,
                [EventId.Hit] = e => e.OnHit,
                [EventId.HitField] = e => e.OnHitField,
                [EventId.HitSide] = e => e.OnHitSide,
                [EventId.ModifyMove] = e => e.OnModifyMove,
                [EventId.ModifyPriority] = e => e.OnModifyPriority,
                [EventId.ModifyTarget] = e => e.OnModifyTarget,
                [EventId.ModifyType] = e => e.OnModifyType,
                [EventId.MoveFail] = e => e.OnMoveFail,
                [EventId.PrepareHit] = e => e.OnPrepareHit,
                [EventId.PriorityChargeCallback] = e => e.PriorityChargeCallback,
                [EventId.Try] = e => e.OnTry,
                [EventId.TryHit] = e => e.OnTryHit,
                [EventId.TryHitField] = e => e.OnTryHitField,
                [EventId.TryHitSide] = e => e.OnTryHitSide,
                [EventId.TryImmunity] = e => e.OnTryImmunity,
                [EventId.TryMove] = e => e.OnTryMove,
                [EventId.UseMoveMessage] = e => e.OnUseMoveMessage,
            }.ToFrozenDictionary();

    /// <summary>
    /// Gets the EventHandlerInfo for a given EventId from an IEffect.
    /// Uses static dictionaries for O(1) lookups without reflection.
    /// </summary>
    /// <param name="effect">The effect to query</param>
    /// <param name="id">The base EventId (without prefix/suffix)</param>
    /// <param name="prefix">Optional event prefix (Foe, Source, Any, Ally)</param>
    /// <param name="suffix">Optional event suffix</param>
    /// <returns>The EventHandlerInfo if found, null otherwise</returns>
    public static EventHandlerInfo? GetEventHandlerInfo(
        IEffect effect,
        EventId id,
        EventPrefix? prefix = null,
        EventSuffix? suffix = null)
    {
        // Try move-specific events first (if applicable)
        if (effect is IMoveEventMethods moveMethods &&
            MoveEventMethodsMap.TryGetValue(id, out var moveAccessor))
        {
            EventHandlerInfo? info = moveAccessor(moveMethods);
            if (info != null && MatchesPrefixAndSuffix(info, prefix, suffix))
                return info;
        }

        // Try ability-specific events first (if applicable)
        if (effect is IAbilityEventMethodsV2 abilityMethods &&
            AbilityEventMethodsMap.TryGetValue(id, out var abilityAccessor))
        {
            EventHandlerInfo? info = abilityAccessor(abilityMethods);
            if (info != null && MatchesPrefixAndSuffix(info, prefix, suffix))
                return info;
        }

        // Handle prefixed events
        if (prefix.HasValue && effect is IEventMethods eventMethods)
        {
            EventHandlerInfo? info = prefix.Value switch
            {
                EventPrefix.Foe when FoeEventMethodsMap.TryGetValue(id, out var accessor) =>
                    accessor(eventMethods),
                EventPrefix.Source when SourceEventMethodsMap.TryGetValue(id, out var accessor) =>
                    accessor(eventMethods),
                EventPrefix.Any when AnyEventMethodsMap.TryGetValue(id, out var accessor) =>
                    accessor(eventMethods),
                EventPrefix.Ally when effect is IPokemonEventMethods pokemonMethods &&
                                      AllyEventMethodsMap.TryGetValue(id, out var allyAccessor) =>
                    allyAccessor(pokemonMethods),
                _ => null,
            };

            if (info != null && MatchesPrefixAndSuffix(info, prefix, suffix))
                return info;
        }

        // Handle base events (no prefix)
        if (effect is IEventMethods baseMethods &&
            EventMethodsMap.TryGetValue(id, out var baseAccessor))
        {
            EventHandlerInfo? info = baseAccessor(baseMethods);
            if (info != null && MatchesPrefixAndSuffix(info, prefix, suffix))
                return info;
        }

        return null;
    }

    /// <summary>
    /// Checks if an EventHandlerInfo matches the requested prefix and suffix.
    /// </summary>
    private static bool MatchesPrefixAndSuffix(EventHandlerInfo info, EventPrefix? prefix,
        EventSuffix? suffix)
    {
        if (prefix.HasValue && info.Prefix != prefix.Value)
            return false;

        if (suffix.HasValue && info.Suffix != suffix.Value)
            return false;

        return true;
    }
}