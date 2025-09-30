using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public interface IEventMethods
{
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnDamagingHit { get; }
    Action<BattleContext, Pokemon>? OnEmergencyExit { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; }
    VoidSourceMoveHandler? OnAfterHit { get; } // MoveEventMethods['onAfterHit']
    Action<BattleContext, Pokemon>? OnAfterMega { get; }
    Action<BattleContext, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; }
    OnAfterSubDamageHandler? OnAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    Action<BattleContext, Pokemon>? OnAfterSwitchInSelf { get; }
    Action<BattleContext, Pokemon>? OnAfterTerastallization { get; }
    Action<BattleContext, Item, Pokemon>? OnAfterUseItem { get; }
    Action<BattleContext, Item, Pokemon>? OnAfterTakeItem { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; }
    Action<BattleContext, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; }
    VoidSourceMoveHandler? OnAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf'] 
    VoidMoveHandler? OnAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    VoidSourceMoveHandler? OnAfterMove { get; } // MoveEventMethods['onAfterMove']
    VoidSourceMoveHandler? OnAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon, Pokemon>? OnAttract { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, object?>? OnAccuracy { get; }
    ModifierSourceMoveHandler? OnBasePower { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Pokemon, IEffect>? OnBeforeFaint { get; }
    VoidSourceMoveHandler? OnBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon>? OnBeforeSwitchIn { get; }
    Action<BattleContext, Pokemon>? OnBeforeSwitchOut { get; }
    Action<BattleContext, Pokemon>? OnBeforeTurn { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; }
    VoidSourceMoveHandler? OnChargeMove { get; } // CommonHandlers['VoidSourceMove']
    Func<BattleContext, Pokemon, object?, Move, bool?>? OnCriticalHit { get; }
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; }
    Func<BattleContext, Pokemon, Pokemon, int?>? OnDeductPp { get; }
    Action<BattleContext, Pokemon>? OnDisableMove { get; }
    Action<BattleContext, Pokemon, Pokemon?, Move?>? OnDragOut { get; }
    Action<BattleContext, Item, Pokemon>? OnEatItem { get; }
    OnEffectivenessHandler? OnEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    Action<BattleContext, Pokemon>? OnEntryHazard { get; }
    VoidEffectHandler? OnFaint { get; } // CommonHandlers['VoidEffect']
    Func<BattleContext, Pokemon, bool?>? OnFlinch { get; }
    OnFractionalPriority? OnFractionalPriority { get; }
    ResultMoveHandler? OnHit { get; } // MoveEventMethods['onHit']
    Action<BattleContext, string, Pokemon>? OnImmunity { get; }
    Func<BattleContext, Pokemon, string?>? OnLockMove { get; }
    Action<BattleContext, Pokemon>? OnMaybeTrapPokemon { get; }
    ModifierMoveHandler? OnModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    ModifierSourceMoveHandler? OnModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; }
    ModifierSourceMoveHandler? OnModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnModifyDef { get; } // CommonHandlers['ModifierMove']
    OnModifyMoveHandler? OnModifyMove { get; } // MoveEventMethods['onModifyMove']
    ModifierSourceMoveHandler? OnModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnModifySecondaries { get; }
    OnModifyTypeHandler? OnModifyType { get; } // MoveEventMethods['onModifyType']
    OnModifyTargetHandler? OnModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    ModifierSourceMoveHandler? OnModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnModifySpD { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, int?>? OnModifySpe { get; }
    ModifierSourceMoveHandler? OnModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, int?>? OnModifyWeight { get; }
    VoidMoveHandler? OnMoveAborted { get; } // CommonHandlers['VoidMove']
    Func<BattleContext, Pokemon, string, bool?>? OnNegateImmunity { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, string?>? OnOverrideAction { get; }
    ResultSourceMoveHandler? OnPrepareHit { get; } // CommonHandlers['ResultSourceMove']
    Action<BattleContext, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; }
    Func<BattleContext, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnRedirectTarget { get; }
    Action<BattleContext, Pokemon, Pokemon, IEffect>? OnResidual { get; }
    Func<BattleContext, string, Pokemon, Pokemon, IEffect, object?>? OnSetAbility { get; }
    Func<BattleContext, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get; }
    Func<BattleContext, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get; }
    Action<BattleContext, Side, Pokemon, Condition>? OnSideConditionStart { get; }
    Func<BattleContext, Pokemon, bool?>? OnStallMove { get; }
    Action<BattleContext, Pokemon>? OnSwitchIn { get; }
    Action<BattleContext, Pokemon>? OnSwitchOut { get; }
    Action<BattleContext, Pokemon, Pokemon>? OnSwap { get; }
    Func<BattleContext, Item, Pokemon, Pokemon, Move?, bool?>? OnTakeItem { get; }
    Action<BattleContext, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; }
    Action<BattleContext, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; }
    Action<BattleContext, Pokemon>? OnTrapPokemon { get; }
    Func<BattleContext, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get; }
    Func<BattleContext, Item, Pokemon, bool?>? OnTryEatItem { get; }
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, object?>? OnTryHeal { get; }



    ExtResultSourceMoveHandler? OnTryHit { get; } // MoveEventMethods['onTryHit']
    ResultMoveHandler? OnTryHitField { get; } // MoveEventMethods['onTryHitField']
    ResultMoveHandler? OnTryHitSide { get; } // CommonHandlers['ResultMove']
    ExtResultMoveHandler? OnInvulnerability { get; } // CommonHandlers['ExtResultMove']
    ResultSourceMoveHandler? OnTryMove { get; } // MoveEventMethods['onTryMove']
    Func<BattleContext, Pokemon, Pokemon, Move, object?>? OnTryPrimaryHit { get; }
    Func<BattleContext, List<string>, Pokemon, List<string>?>? OnType { get; }
    Action<BattleContext, Item, Pokemon>? OnUseItem { get; }
    Action<BattleContext, Pokemon>? OnUpdate { get; }
    Action<BattleContext, Pokemon, object?, Condition>? OnWeather { get; }
    ModifierSourceMoveHandler? OnWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']

    // Foe event handlers (triggered by opponent actions)
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnFoeDamagingHit { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; }
    VoidSourceMoveHandler? OnFoeAfterHit { get; } // MoveEventMethods['onAfterHit']
    Action<BattleContext, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; }
    OnAfterSubDamageHandler? OnFoeAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    Action<BattleContext, Pokemon>? OnFoeAfterSwitchInSelf { get; }
    Action<BattleContext, Item, Pokemon>? OnFoeAfterUseItem { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; }
    Action<BattleContext, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; }
    VoidSourceMoveHandler? OnFoeAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf']
    VoidMoveHandler? OnFoeAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    VoidSourceMoveHandler? OnFoeAfterMove { get; } // MoveEventMethods['onAfterMove']
    VoidSourceMoveHandler? OnFoeAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon, Pokemon>? OnFoeAttract { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, object?>? OnFoeAccuracy { get; }
    ModifierSourceMoveHandler? OnFoeBasePower { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Pokemon, IEffect>? OnFoeBeforeFaint { get; }
    VoidSourceMoveHandler? OnFoeBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon>? OnFoeBeforeSwitchIn { get; }
    Action<BattleContext, Pokemon>? OnFoeBeforeSwitchOut { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; }
    VoidSourceMoveHandler? OnFoeChargeMove { get; } // CommonHandlers['VoidSourceMove']
    Func<BattleContext, Pokemon, object?, Move, bool?>? OnFoeCriticalHit { get; }
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, object?>? OnFoeDamage { get; }
    Func<BattleContext, Pokemon, Pokemon, int?>? OnFoeDeductPp { get; }
    Action<BattleContext, Pokemon>? OnFoeDisableMove { get; }
    Action<BattleContext, Pokemon, Pokemon?, Move?>? OnFoeDragOut { get; }
    Action<BattleContext, Item, Pokemon>? OnFoeEatItem { get; }
    OnEffectivenessHandler? OnFoeEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    VoidEffectHandler? OnFoeFaint { get; } // CommonHandlers['VoidEffect']
    Func<BattleContext, Pokemon, bool?>? OnFoeFlinch { get; }
    ResultMoveHandler? OnFoeHit { get; } // MoveEventMethods['onHit']
    Action<BattleContext, string, Pokemon>? OnFoeImmunity { get; }
    Func<BattleContext, Pokemon, string?>? OnFoeLockMove { get; }
    Action<BattleContext, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; }
    ModifierMoveHandler? OnFoeModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    ModifierSourceMoveHandler? OnFoeModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; }
    ModifierSourceMoveHandler? OnFoeModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnFoeModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnFoeModifyDef { get; } // CommonHandlers['ModifierMove']
    OnModifyMoveHandler? OnFoeModifyMove { get; } // MoveEventMethods['onModifyMove']
    ModifierSourceMoveHandler? OnFoeModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnFoeModifySecondaries { get; }
    ModifierSourceMoveHandler? OnFoeModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnFoeModifySpD { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, int?>? OnFoeModifySpe { get; }
    ModifierSourceMoveHandler? OnFoeModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    OnModifyTypeHandler? OnFoeModifyType { get; } // MoveEventMethods['onModifyType']
    OnModifyTargetHandler? OnFoeModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    Func<BattleContext, int, Pokemon, int?>? OnFoeModifyWeight { get; }
    VoidMoveHandler? OnFoeMoveAborted { get; } // CommonHandlers['VoidMove']
    Func<BattleContext, Pokemon, string, bool?>? OnFoeNegateImmunity { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, string?>? OnFoeOverrideAction { get; }
    ResultSourceMoveHandler? OnFoePrepareHit { get; } // CommonHandlers['ResultSourceMove']
    Func<BattleContext, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnFoeRedirectTarget { get; }
    Action<BattleContext, object, Pokemon, IEffect>? OnFoeResidual { get; }
    Func<BattleContext, string, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get; }
    Func<BattleContext, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get; }
    Func<BattleContext, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get; }
    Func<BattleContext, Pokemon, bool?>? OnFoeStallMove { get; }
    Action<BattleContext, Pokemon>? OnFoeSwitchOut { get; }
    Func<BattleContext, Item, Pokemon, Pokemon, Move?, bool?>? OnFoeTakeItem { get; }
    Action<BattleContext, Pokemon>? OnFoeTerrain { get; }
    Action<BattleContext, Pokemon>? OnFoeTrapPokemon { get; }
    Func<BattleContext, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get; }
    Func<BattleContext, Item, Pokemon, bool?>? OnFoeTryEatItem { get; }
    Func<BattleContext, object, object?, object?, object?, object?>? OnFoeTryHeal { get; }
    ExtResultSourceMoveHandler? OnFoeTryHit { get; } // MoveEventMethods['onTryHit']
    ResultMoveHandler? OnFoeTryHitField { get; } // MoveEventMethods['onTryHitField']
    ResultMoveHandler? OnFoeTryHitSide { get; } // CommonHandlers['ResultMove']
    ExtResultMoveHandler? OnFoeInvulnerability { get; } // CommonHandlers['ExtResultMove']
    ResultSourceMoveHandler? OnFoeTryMove { get; } // MoveEventMethods['onTryMove']
    Func<BattleContext, Pokemon, Pokemon, Move, object?>? OnFoeTryPrimaryHit { get; }


    Func<BattleContext, List<string>, Pokemon, List<string>?>? OnFoeType { get; }
    ModifierSourceMoveHandler? OnFoeWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnFoeModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnFoeModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']

    // Source event handlers (triggered when this Pokémon is the source)
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnSourceDamagingHit { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; }
    VoidSourceMoveHandler? OnSourceAfterHit { get; } // MoveEventMethods['onAfterHit']
    Action<BattleContext, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; }
    OnAfterSubDamageHandler? OnSourceAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    Action<BattleContext, Pokemon>? OnSourceAfterSwitchInSelf { get; }
    Action<BattleContext, Item, Pokemon>? OnSourceAfterUseItem { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; }
    Action<BattleContext, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; }
    VoidSourceMoveHandler? OnSourceAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf']
    VoidMoveHandler? OnSourceAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    VoidSourceMoveHandler? OnSourceAfterMove { get; } // MoveEventMethods['onAfterMove']
    VoidSourceMoveHandler? OnSourceAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon, Pokemon>? OnSourceAttract { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, object?>? OnSourceAccuracy { get; }
    ModifierSourceMoveHandler? OnSourceBasePower { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Pokemon, IEffect>? OnSourceBeforeFaint { get; }
    VoidSourceMoveHandler? OnSourceBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon>? OnSourceBeforeSwitchIn { get; }
    Action<BattleContext, Pokemon>? OnSourceBeforeSwitchOut { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; }
    VoidSourceMoveHandler? OnSourceChargeMove { get; } // CommonHandlers['VoidSourceMove']
    Func<BattleContext, Pokemon, object?, Move, bool?>? OnSourceCriticalHit { get; }
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, object?>? OnSourceDamage { get; }
    Func<BattleContext, Pokemon, Pokemon, int?>? OnSourceDeductPp { get; }
    Action<BattleContext, Pokemon>? OnSourceDisableMove { get; }
    Action<BattleContext, Pokemon, Pokemon?, Move?>? OnSourceDragOut { get; }
    Action<BattleContext, Item, Pokemon>? OnSourceEatItem { get; }
    OnEffectivenessHandler? OnSourceEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    VoidEffectHandler? OnSourceFaint { get; } // CommonHandlers['VoidEffect']
    Func<BattleContext, Pokemon, bool?>? OnSourceFlinch { get; }
    ResultMoveHandler? OnSourceHit { get; } // MoveEventMethods['onHit']
    Action<BattleContext, string, Pokemon>? OnSourceImmunity { get; }
    Func<BattleContext, Pokemon, string?>? OnSourceLockMove { get; }
    Action<BattleContext, Pokemon>? OnSourceMaybeTrapPokemon { get; }
    ModifierMoveHandler? OnSourceModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    ModifierSourceMoveHandler? OnSourceModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; }
    ModifierSourceMoveHandler? OnSourceModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnSourceModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnSourceModifyDef { get; } // CommonHandlers['ModifierMove']
    OnModifyMoveHandler? OnSourceModifyMove { get; } // MoveEventMethods['onModifyMove']
    ModifierSourceMoveHandler? OnSourceModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnSourceModifySecondaries { get; }
    ModifierSourceMoveHandler? OnSourceModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnSourceModifySpD { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, int?>? OnSourceModifySpe { get; }
    ModifierSourceMoveHandler? OnSourceModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    OnModifyTypeHandler? OnSourceModifyType { get; } // MoveEventMethods['onModifyType']
    OnModifyTargetHandler? OnSourceModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    Func<BattleContext, int, Pokemon, int?>? OnSourceModifyWeight { get; }
    VoidMoveHandler? OnSourceMoveAborted { get; } // CommonHandlers['VoidMove']
    Func<BattleContext, Pokemon, string, bool?>? OnSourceNegateImmunity { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, string?>? OnSourceOverrideAction { get; }
    ResultSourceMoveHandler? OnSourcePrepareHit { get; } // CommonHandlers['ResultSourceMove']
    Func<BattleContext, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnSourceRedirectTarget { get; }
    Action<BattleContext, object, Pokemon, IEffect>? OnSourceResidual { get; }
    Func<BattleContext, string, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get; }
    Func<BattleContext, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get; }
    Func<BattleContext, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get; }
    Func<BattleContext, Pokemon, bool?>? OnSourceStallMove { get; }
    Action<BattleContext, Pokemon>? OnSourceSwitchOut { get; }
    Func<BattleContext, Item, Pokemon, Pokemon, Move?, bool?>? OnSourceTakeItem { get; }
    Action<BattleContext, Pokemon>? OnSourceTerrain { get; }
    Action<BattleContext, Pokemon>? OnSourceTrapPokemon { get; }
    Func<BattleContext, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get; }
    Func<BattleContext, Item, Pokemon, bool?>? OnSourceTryEatItem { get; }
    Func<BattleContext, object, object?, object?, object?, object?>? OnSourceTryHeal { get; }
    ExtResultSourceMoveHandler? OnSourceTryHit { get; } // MoveEventMethods['onTryHit']
    ResultMoveHandler? OnSourceTryHitField { get; } // MoveEventMethods['onTryHitField']
    ResultMoveHandler? OnSourceTryHitSide { get; } // CommonHandlers['ResultMove']
    ExtResultMoveHandler? OnSourceInvulnerability { get; } // CommonHandlers['ExtResultMove']
    ResultSourceMoveHandler? OnSourceTryMove { get; } // MoveEventMethods['onTryMove']
    Func<BattleContext, Pokemon, Pokemon, Move, object?>? OnSourceTryPrimaryHit { get; }
    Func<BattleContext, List<string>, Pokemon, List<string>?>? OnSourceType { get; }
    ModifierSourceMoveHandler? OnSourceWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnSourceModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnSourceModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']

    // Any event handlers (triggered for any Pokémon action in BattleContext)
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnAnyDamagingHit { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; }
    VoidSourceMoveHandler? OnAnyAfterHit { get; } // MoveEventMethods['onAfterHit']
    Action<BattleContext, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; }
    OnAfterSubDamageHandler? OnAnyAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    Action<BattleContext, Pokemon>? OnAnyAfterSwitchInSelf { get; }
    Action<BattleContext, Item, Pokemon>? OnAnyAfterUseItem { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; }
    Action<BattleContext, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; }
    Action<BattleContext, Pokemon>? OnAnyAfterMega { get; }
    VoidSourceMoveHandler? OnAnyAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf']
    VoidMoveHandler? OnAnyAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    VoidSourceMoveHandler? OnAnyAfterMove { get; } // MoveEventMethods['onAfterMove']
    VoidSourceMoveHandler? OnAnyAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon>? OnAnyAfterTerastallization { get; }
    Action<BattleContext, Pokemon, Pokemon>? OnAnyAttract { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, object?>? OnAnyAccuracy { get; }
    ModifierSourceMoveHandler? OnAnyBasePower { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Pokemon, IEffect>? OnAnyBeforeFaint { get; }
    VoidSourceMoveHandler? OnAnyBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon>? OnAnyBeforeSwitchIn { get; }
    Action<BattleContext, Pokemon>? OnAnyBeforeSwitchOut { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; }
    VoidSourceMoveHandler? OnAnyChargeMove { get; } // CommonHandlers['VoidSourceMove']
    Func<BattleContext, Pokemon, object?, Move, bool?>? OnAnyCriticalHit { get; }
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, object?>? OnAnyDamage { get; }
    Func<BattleContext, Pokemon, Pokemon, int?>? OnAnyDeductPp { get; }
    Action<BattleContext, Pokemon>? OnAnyDisableMove { get; }
    Action<BattleContext, Pokemon, Pokemon?, Move?>? OnAnyDragOut { get; }
    Action<BattleContext, Item, Pokemon>? OnAnyEatItem { get; }
    OnEffectivenessHandler? OnAnyEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    VoidEffectHandler? OnAnyFaint { get; } // CommonHandlers['VoidEffect']
    Func<BattleContext, Pokemon, bool?>? OnAnyFlinch { get; }
    ResultMoveHandler? OnAnyHit { get; } //  MoveEventMethods['onHit']
    Action<BattleContext, string, Pokemon>? OnAnyImmunity { get; }
    Func<BattleContext, Pokemon, string?>? OnAnyLockMove { get; }
    Action<BattleContext, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; }
    ModifierMoveHandler? OnAnyModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    ModifierSourceMoveHandler? OnAnyModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; }
    ModifierSourceMoveHandler? OnAnyModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnAnyModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnAnyModifyDef { get; } // CommonHandlers['ModifierMove']
    OnModifyMoveHandler? OnAnyModifyMove { get; } // MoveEventMethods['onModifyMove']
    ModifierSourceMoveHandler? OnAnyModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnAnyModifySecondaries { get; }
    ModifierSourceMoveHandler? OnAnyModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnAnyModifySpD { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, int?>? OnAnyModifySpe { get; }
    ModifierSourceMoveHandler? OnAnyModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    OnModifyTypeHandler? OnAnyModifyType { get; } // MoveEventMethods['onModifyType']
    OnModifyTargetHandler? OnAnyModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    Func<BattleContext, int, Pokemon, int?>? OnAnyModifyWeight { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAnyMoveAborted { get; }
    Func<BattleContext, Pokemon, string, bool?>? OnAnyNegateImmunity { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, string?>? OnAnyOverrideAction { get; }
    ResultSourceMoveHandler? OnAnyPrepareHit { get; } // CommonHandlers['ResultSourceMove']
    Action<BattleContext, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; }
    Func<BattleContext, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnAnyRedirectTarget { get; }
    Action<BattleContext, object, Pokemon, IEffect>? OnAnyResidual { get; }
    Func<BattleContext, string, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; }
    Func<BattleContext, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get; }
    Func<BattleContext, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get; }
    Func<BattleContext, Pokemon, bool?>? OnAnyStallMove { get; }
    Action<BattleContext, Pokemon>? OnAnySwitchIn { get; }
    Action<BattleContext, Pokemon>? OnAnySwitchOut { get; }
    Func<BattleContext, Item, Pokemon, Pokemon, Move?, bool?>? OnAnyTakeItem { get; }
    Action<BattleContext, Pokemon>? OnAnyTerrain { get; }
    Action<BattleContext, Pokemon>? OnAnyTrapPokemon { get; }
    Func<BattleContext, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get; }
    Func<BattleContext, Item, Pokemon, bool?>? OnAnyTryEatItem { get; }
    Func<BattleContext, object, object?, object?, object?, object?>? OnAnyTryHeal { get; }
    ExtResultSourceMoveHandler? OnAnyTryHit { get; } // MoveEventMethods['onTryHit']
    ResultMoveHandler? OnAnyTryHitField { get; } // MoveEventMethods['onTryHitField']
    ResultMoveHandler? OnAnyTryHitSide { get; } // CommonHandlers['ResultMove']
    ExtResultMoveHandler? OnAnyInvulnerability { get; } // CommonHandlers['ExtResultMove']
    ResultSourceMoveHandler? OnAnyTryMove { get; } // MoveEventMethods['onTryMove']
    Func<BattleContext, Pokemon, Pokemon, Move, object?>? OnAnyTryPrimaryHit { get; }
    Func<BattleContext, List<string>, Pokemon, List<string>?>? OnAnyType { get; }
    ModifierSourceMoveHandler? OnAnyWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnAnyModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnAnyModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']

    // Priorities (incomplete list)
    int? OnAccuracyPriority { get; }
    int? OnDamagingHitOrder { get; }
    int? OnAfterMoveSecondaryPriority { get; }
    int? OnAfterMoveSecondarySelfPriority { get; }
    int? OnAfterMoveSelfPriority { get; }
    int? OnAfterSetStatusPriority { get; }
    int? OnAnyBasePowerPriority { get; }
    int? OnAnyInvulnerabilityPriority { get; }
    int? OnAnyModifyAccuracyPriority { get; }
    int? OnAnyFaintPriority { get; }
    int? OnAnyPrepareHitPriority { get; }
    int? OnAnySwitchInPriority { get; }
    int? OnAnySwitchInSubOrder { get; }
    int? OnAllyBasePowerPriority { get; }
    int? OnAllyModifyAtkPriority { get; }
    int? OnAllyModifySpAPriority { get; }
    int? OnAllyModifySpDPriority { get; }
    int? OnAttractPriority { get; }
    int? OnBasePowerPriority { get; }
    int? OnBeforeMovePriority { get; }
    int? OnBeforeSwitchOutPriority { get; }
    int? OnChangeBoostPriority { get; }
    int? OnDamagePriority { get; }
    int? OnDragOutPriority { get; }
    int? OnEffectivenessPriority { get; }
    int? OnFoeBasePowerPriority { get; }
    int? OnFoeBeforeMovePriority { get; }
    int? OnFoeModifyDefPriority { get; }
    int? OnFoeModifySpDPriority { get; }
    int? OnFoeRedirectTargetPriority { get; }
    int? OnFoeTrapPokemonPriority { get; }
    int? OnFractionalPriorityPriority { get; }
    int? OnHitPriority { get; }
    int? OnInvulnerabilityPriority { get; }
    int? OnModifyAccuracyPriority { get; }
    int? OnModifyAtkPriority { get; }
    int? OnModifyCritRatioPriority { get; }
    int? OnModifyDefPriority { get; }
    int? OnModifyMovePriority { get; }
    int? OnModifyPriorityPriority { get; }
    int? OnModifySpAPriority { get; }
    int? OnModifySpDPriority { get; }
    int? OnModifySpePriority { get; }
    int? OnModifyStabPriority { get; }
    int? OnModifyTypePriority { get; }
    int? OnModifyWeightPriority { get; }
    int? OnRedirectTargetPriority { get; }
    int? OnResidualOrder { get; }
    int? OnResidualPriority { get; }
    int? OnResidualSubOrder { get; }
    int? OnSourceBasePowerPriority { get; }
    int? OnSourceInvulnerabilityPriority { get; }
    int? OnSourceModifyAccuracyPriority { get; }
    int? OnSourceModifyAtkPriority { get; }
    int? OnSourceModifyDamagePriority { get; }
    int? OnSourceModifySpAPriority { get; }
    int? OnSwitchInPriority { get; }
    int? OnSwitchInSubOrder { get; }
    int? OnTrapPokemonPriority { get; }
    int? OnTryBoostPriority { get; }
    int? OnTryEatItemPriority { get; }
    int? OnTryHealPriority { get; }
    int? OnTryHitPriority { get; }
    int? OnTryMovePriority { get; }
    int? OnTryPrimaryHitPriority { get; }
    int? OnTypePriority { get; }
}