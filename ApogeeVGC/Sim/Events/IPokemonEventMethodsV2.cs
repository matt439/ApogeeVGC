using ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Modern interface for Pokemon/Ally-specific event methods using strongly-typed EventHandlerInfo records.
/// This replaces IPokemonEventMethods with a type-safe approach that validates delegate signatures at compile-time.
/// Each EventHandlerInfo record contains its own Priority, Order, and SubOrder properties.
/// All events in this interface use the "Ally" prefix to indicate they apply to allied Pokemon.
/// </summary>
public interface IPokemonEventMethodsV2 : IEventMethodsV2
{
    // Ally-Prefixed Events (78 total)
    
    /// <summary>
    /// Triggered when an ally deals damaging hit.
    /// </summary>
    OnAllyDamagingHitEventInfo? OnAllyDamagingHit { get; }
    
    /// <summary>
    /// Triggered after each boost to an ally.
/// </summary>
    OnAllyAfterEachBoostEventInfo? OnAllyAfterEachBoost { get; }
    
    /// <summary>
    /// Triggered after ally hits.
    /// </summary>
    OnAllyAfterHitEventInfo? OnAllyAfterHit { get; }
 
    /// <summary>
    /// Triggered after status is set on ally.
    /// </summary>
    OnAllyAfterSetStatusEventInfo? OnAllyAfterSetStatus { get; }
    
    /// <summary>
    /// Triggered after substitute damage to ally.
    /// </summary>
    OnAllyAfterSubDamageEventInfo? OnAllyAfterSubDamage { get; }
    
    /// <summary>
    /// Triggered after ally switches in.
/// </summary>
    OnAllyAfterSwitchInSelfEventInfo? OnAllyAfterSwitchInSelf { get; }
    
    /// <summary>
    /// Triggered after ally uses item.
    /// </summary>
  OnAllyAfterUseItemEventInfo? OnAllyAfterUseItem { get; }
    
    /// <summary>
    /// Triggered after ally is boosted.
    /// </summary>
    OnAllyAfterBoostEventInfo? OnAllyAfterBoost { get; }
    
    /// <summary>
    /// Triggered after ally faints.
/// </summary>
    OnAllyAfterFaintEventInfo? OnAllyAfterFaint { get; }
    
    /// <summary>
 /// Triggered after ally's move secondary effects on self.
    /// </summary>
    OnAllyAfterMoveSecondarySelfEventInfo? OnAllyAfterMoveSecondarySelf { get; }
    
    /// <summary>
    /// Triggered after ally's move secondary effects.
    /// </summary>
    OnAllyAfterMoveSecondaryEventInfo? OnAllyAfterMoveSecondary { get; }
    
    /// <summary>
    /// Triggered after ally's move.
    /// </summary>
    OnAllyAfterMoveEventInfo? OnAllyAfterMove { get; }
  
    /// <summary>
    /// Triggered after ally's move (self).
    /// </summary>
    OnAllyAfterMoveSelfEventInfo? OnAllyAfterMoveSelf { get; }
    
    /// <summary>
    /// Triggered when ally is attracted.
    /// </summary>
    OnAllyAttractEventInfo? OnAllyAttract { get; }
    
    /// <summary>
    /// Triggered to modify ally's accuracy.
    /// </summary>
    OnAllyAccuracyEventInfo? OnAllyAccuracy { get; }
    
    /// <summary>
    /// Triggered to modify ally's base power.
  /// </summary>
    OnAllyBasePowerEventInfo? OnAllyBasePower { get; }
 
    /// <summary>
    /// Triggered before ally faints.
    /// </summary>
OnAllyBeforeFaintEventInfo? OnAllyBeforeFaint { get; }
    
    /// <summary>
    /// Triggered before ally moves.
    /// </summary>
  OnAllyBeforeMoveEventInfo? OnAllyBeforeMove { get; }
    
    /// <summary>
    /// Triggered before ally switches in.
    /// </summary>
    OnAllyBeforeSwitchInEventInfo? OnAllyBeforeSwitchIn { get; }
    
    /// <summary>
    /// Triggered before ally switches out.
  /// </summary>
    OnAllyBeforeSwitchOutEventInfo? OnAllyBeforeSwitchOut { get; }
    
    /// <summary>
    /// Triggered when trying to boost ally.
    /// </summary>
    OnAllyTryBoostEventInfo? OnAllyTryBoost { get; }
    
    /// <summary>
    /// Triggered when ally charges move.
    /// </summary>
    OnAllyChargeMoveEventInfo? OnAllyChargeMove { get; }
    
    /// <summary>
    /// Triggered when ally gets critical hit.
    /// </summary>
    OnAllyCriticalHitEventInfo? OnAllyCriticalHit { get; }
    
    /// <summary>
    /// Triggered to modify damage to ally.
    /// </summary>
    OnAllyDamageEventInfo? OnAllyDamage { get; }
    
/// <summary>
    /// Triggered when deducting ally's PP.
 /// </summary>
    OnAllyDeductPpEventInfo? OnAllyDeductPp { get; }
    
    /// <summary>
  /// Triggered to disable ally's move.
    /// </summary>
    OnAllyDisableMoveEventInfo? OnAllyDisableMove { get; }
 
    /// <summary>
    /// Triggered when ally is dragged out.
    /// </summary>
    OnAllyDragOutEventInfo? OnAllyDragOut { get; }
    
 /// <summary>
    /// Triggered when ally eats item.
    /// </summary>
    OnAllyEatItemEventInfo? OnAllyEatItem { get; }
    
    /// <summary>
    /// Triggered to modify type effectiveness against ally.
    /// </summary>
    OnAllyEffectivenessEventInfo? OnAllyEffectiveness { get; }
    
    /// <summary>
    /// Triggered when ally faints.
    /// </summary>
    OnAllyFaintEventInfo? OnAllyFaint { get; }
    
    /// <summary>
    /// Triggered when ally flinches.
    /// </summary>
    OnAllyFlinchEventInfo? OnAllyFlinch { get; }
    
    /// <summary>
    /// Triggered when ally is hit.
    /// </summary>
  OnAllyHitEventInfo? OnAllyHit { get; }
    
    /// <summary>
    /// Triggered for ally immunity check.
    /// </summary>
    OnAllyImmunityEventInfo? OnAllyImmunity { get; }
  
    /// <summary>
    /// Triggered to lock ally's move.
/// </summary>
    OnAllyLockMoveEventInfo? OnAllyLockMove { get; }
    
    /// <summary>
/// Triggered to maybe trap ally.
    /// </summary>
    OnAllyMaybeTrapPokemonEventInfo? OnAllyMaybeTrapPokemon { get; }
    
    /// <summary>
    /// Triggered to modify ally's accuracy.
    /// </summary>
    OnAllyModifyAccuracyEventInfo? OnAllyModifyAccuracy { get; }
    
/// <summary>
    /// Triggered to modify ally's attack.
    /// </summary>
    OnAllyModifyAtkEventInfo? OnAllyModifyAtk { get; }
    
    /// <summary>
    /// Triggered to modify ally's boosts.
    /// </summary>
    OnAllyModifyBoostEventInfo? OnAllyModifyBoost { get; }
    
    /// <summary>
    /// Triggered to modify ally's crit ratio.
    /// </summary>
    OnAllyModifyCritRatioEventInfo? OnAllyModifyCritRatio { get; }
    
    /// <summary>
    /// Triggered to modify damage to ally.
    /// </summary>
    OnAllyModifyDamageEventInfo? OnAllyModifyDamage { get; }
    
    /// <summary>
    /// Triggered to modify ally's defense.
  /// </summary>
    OnAllyModifyDefEventInfo? OnAllyModifyDef { get; }
    
    /// <summary>
    /// Triggered to modify ally's move.
    /// </summary>
    OnAllyModifyMoveEventInfo? OnAllyModifyMove { get; }
    
    /// <summary>
    /// Triggered to modify ally's move priority.
    /// </summary>
    OnAllyModifyPriorityEventInfo? OnAllyModifyPriority { get; }
    
    /// <summary>
    /// Triggered to modify ally's move secondary effects.
    /// </summary>
    OnAllyModifySecondariesEventInfo? OnAllyModifySecondaries { get; }
    
    /// <summary>
    /// Triggered to modify ally's special attack.
    /// </summary>
    OnAllyModifySpAEventInfo? OnAllyModifySpA { get; }
    
    /// <summary>
    /// Triggered to modify ally's special defense.
    /// </summary>
    OnAllyModifySpDEventInfo? OnAllyModifySpD { get; }
    
    /// <summary>
    /// Triggered to modify ally's speed.
    /// </summary>
    OnAllyModifySpeEventInfo? OnAllyModifySpe { get; }
    
/// <summary>
    /// Triggered to modify ally's STAB.
    /// </summary>
    OnAllyModifyStabEventInfo? OnAllyModifyStab { get; }
    
    /// <summary>
    /// Triggered to modify ally's move type.
    /// </summary>
    OnAllyModifyTypeEventInfo? OnAllyModifyType { get; }
    
    /// <summary>
    /// Triggered to modify ally's move target.
    /// </summary>
    OnAllyModifyTargetEventInfo? OnAllyModifyTarget { get; }
    
    /// <summary>
    /// Triggered to modify ally's weight.
    /// </summary>
    OnAllyModifyWeightEventInfo? OnAllyModifyWeight { get; }
 
    /// <summary>
    /// Triggered when ally's move is aborted.
    /// </summary>
    OnAllyMoveAbortedEventInfo? OnAllyMoveAborted { get; }
    
    /// <summary>
    /// Triggered to negate ally immunity.
    /// </summary>
  OnAllyNegateImmunityEventInfo? OnAllyNegateImmunity { get; }
    
    /// <summary>
    /// Triggered to override ally action.
    /// </summary>
    OnAllyOverrideActionEventInfo? OnAllyOverrideAction { get; }
    
    /// <summary>
    /// Triggered to prepare ally hit.
    /// </summary>
    OnAllyPrepareHitEventInfo? OnAllyPrepareHit { get; }
    
    /// <summary>
    /// Triggered to redirect target from ally.
    /// </summary>
    OnAllyRedirectTargetEventInfo? OnAllyRedirectTarget { get; }
    
    /// <summary>
    /// Triggered for ally residual effects.
    /// </summary>
    OnAllyResidualEventInfo? OnAllyResidual { get; }
    
    /// <summary>
    /// Triggered when setting ally ability.
    /// </summary>
    OnAllySetAbilityEventInfo? OnAllySetAbility { get; }
    
  /// <summary>
    /// Triggered when setting ally status.
    /// </summary>
    OnAllySetStatusEventInfo? OnAllySetStatus { get; }
    
  /// <summary>
    /// Triggered when setting weather affecting ally.
    /// </summary>
    OnAllySetWeatherEventInfo? OnAllySetWeather { get; }
    
    /// <summary>
/// Triggered for ally stall move.
    /// </summary>
    OnAllyStallMoveEventInfo? OnAllyStallMove { get; }
    
    /// <summary>
    /// Triggered when ally switches out.
    /// </summary>
    OnAllySwitchOutEventInfo? OnAllySwitchOut { get; }
    
    /// <summary>
    /// Triggered when taking ally's item.
    /// </summary>
    OnAllyTakeItemEventInfo? OnAllyTakeItem { get; }
    
    /// <summary>
    /// Triggered for terrain affecting ally.
    /// </summary>
    OnAllyTerrainEventInfo? OnAllyTerrain { get; }
    
    /// <summary>
    /// Triggered when trapping ally.
    /// </summary>
    OnAllyTrapPokemonEventInfo? OnAllyTrapPokemon { get; }
    
  /// <summary>
    /// Triggered when trying to add volatile to ally.
    /// </summary>
    OnAllyTryAddVolatileEventInfo? OnAllyTryAddVolatile { get; }
    
    /// <summary>
    /// Triggered when ally tries to eat item.
    /// </summary>
    OnAllyTryEatItemEventInfo? OnAllyTryEatItem { get; }
    
    /// <summary>
    /// Triggered when trying to heal ally.
    /// </summary>
  OnAllyTryHealEventInfo? OnAllyTryHeal { get; }
    
    /// <summary>
    /// Triggered when trying to hit ally.
    /// </summary>
    OnAllyTryHitEventInfo? OnAllyTryHit { get; }
    
    /// <summary>
    /// Triggered when trying to hit field affecting ally.
  /// </summary>
    OnAllyTryHitFieldEventInfo? OnAllyTryHitField { get; }
    
    /// <summary>
    /// Triggered when trying to hit side with ally.
    /// </summary>
    OnAllyTryHitSideEventInfo? OnAllyTryHitSide { get; }
    
    /// <summary>
  /// Triggered for ally invulnerability check.
    /// </summary>
    OnAllyInvulnerabilityEventInfo? OnAllyInvulnerability { get; }
  
    /// <summary>
    /// Triggered when ally tries to move.
    /// </summary>
    OnAllyTryMoveEventInfo? OnAllyTryMove { get; }
    
    /// <summary>
    /// Triggered when trying primary hit on ally.
    /// </summary>
    OnAllyTryPrimaryHitEventInfo? OnAllyTryPrimaryHit { get; }
  
    /// <summary>
    /// Triggered to get ally's type.
    /// </summary>
    OnAllyTypeEventInfo? OnAllyType { get; }
    
    /// <summary>
    /// Triggered when weather modifies damage to ally.
    /// </summary>
    OnAllyWeatherModifyDamageEventInfo? OnAllyWeatherModifyDamage { get; }
    
    /// <summary>
/// Triggered to modify damage to ally (phase 1).
    /// </summary>
    OnAllyModifyDamagePhase1EventInfo? OnAllyModifyDamagePhase1 { get; }
    
    /// <summary>
    /// Triggered to modify damage to ally (phase 2).
  /// </summary>
    OnAllyModifyDamagePhase2EventInfo? OnAllyModifyDamagePhase2 { get; }
}
