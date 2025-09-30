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
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterHit { get; } // MoveEventMethods['onAfterHit']
    Action<BattleContext, Pokemon>? OnAfterMega { get; }
    Action<BattleContext, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; }
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    Action<BattleContext, Pokemon>? OnAfterSwitchInSelf { get; }
    Action<BattleContext, Pokemon>? OnAfterTerastallization { get; }
    Action<BattleContext, Item, Pokemon>? OnAfterUseItem { get; }
    Action<BattleContext, Item, Pokemon>? OnAfterTakeItem { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; }
    Action<BattleContext, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf'] 
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterMove { get; } // MoveEventMethods['onAfterMove']
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon, Pokemon>? OnAttract { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, object?>? OnAccuracy { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int>? OnBasePower { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Pokemon, IEffect>? OnBeforeFaint { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon>? OnBeforeSwitchIn { get; }
    Action<BattleContext, Pokemon>? OnBeforeSwitchOut { get; }
    Action<BattleContext, Pokemon>? OnBeforeTurn { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnChargeMove { get; } // CommonHandlers['VoidSourceMove']
    Func<BattleContext, Pokemon, object?, Move, bool?>? OnCriticalHit { get; }
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, object?>? OnDamage { get; }
    Func<BattleContext, Pokemon, Pokemon, int?>? OnDeductPp { get; }
    Action<BattleContext, Pokemon>? OnDisableMove { get; }
    Action<BattleContext, Pokemon, Pokemon?, Move?>? OnDragOut { get; }
    Action<BattleContext, Item, Pokemon>? OnEatItem { get; }
    Func<BattleContext, int, Pokemon?, string, Move, int?>? OnEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    Action<BattleContext, Pokemon>? OnEntryHazard { get; }
    Action<BattleContext, Pokemon, Pokemon, IEffect>? OnFaint { get; } // CommonHandlers['VoidEffect']
    Func<BattleContext, Pokemon, bool?>? OnFlinch { get; }
    OnFractionalPriority? OnFractionalPriority { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnHit { get; } // MoveEventMethods['onHit']
    Action<BattleContext, string, Pokemon>? OnImmunity { get; }
    Func<BattleContext, Pokemon, string?>? OnLockMove { get; }
    Action<BattleContext, Pokemon>? OnMaybeTrapPokemon { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifyDef { get; } // CommonHandlers['ModifierMove']
    Action<BattleContext, Move, Pokemon, Pokemon?>? OnModifyMove { get; } // MoveEventMethods['onModifyMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnModifySecondaries { get; }
    Action<BattleContext, Move, Pokemon, Pokemon>? OnModifyType { get; } // MoveEventMethods['onModifyType']
    Action<BattleContext, Pokemon, Pokemon, Pokemon, Move>? OnModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifySpD { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, int?>? OnModifySpe { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, int?>? OnModifyWeight { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnMoveAborted { get; } // CommonHandlers['VoidMove']
    Func<BattleContext, Pokemon, string, bool?>? OnNegateImmunity { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, string?>? OnOverrideAction { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnPrepareHit { get; } // CommonHandlers['ResultSourceMove']
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



    Func<BattleContext, Pokemon, Pokemon, Move, object>? OnTryHit { get; } // MoveEventMethods['onTryHit']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnTryHitField { get; } // MoveEventMethods['onTryHitField']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnTryHitSide { get; } // CommonHandlers['ResultMove']
    Func<BattleContext, Pokemon, Pokemon, Move, IntBoolUnion?>? OnInvulnerability { get; } // CommonHandlers['ExtResultMove']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnTryMove { get; } // MoveEventMethods['onTryMove']
    Func<BattleContext, Pokemon, Pokemon, Move, object?>? OnTryPrimaryHit { get; }
    Func<BattleContext, List<string>, Pokemon, List<string>?>? OnType { get; }
    Action<BattleContext, Item, Pokemon>? OnUseItem { get; }
    Action<BattleContext, Pokemon>? OnUpdate { get; }
    Action<BattleContext, Pokemon, object?, Condition>? OnWeather { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']

    // Foe event handlers (triggered by opponent actions)
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnFoeDamagingHit { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnFoeAfterHit { get; } // MoveEventMethods['onAfterHit']
    Action<BattleContext, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; }
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnFoeAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    Action<BattleContext, Pokemon>? OnFoeAfterSwitchInSelf { get; }
    Action<BattleContext, Item, Pokemon>? OnFoeAfterUseItem { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; }
    Action<BattleContext, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnFoeAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf']
    Action<BattleContext, Pokemon, Pokemon, Move>? OnFoeAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    Action<BattleContext, Pokemon, Pokemon, Move>? OnFoeAfterMove { get; } // MoveEventMethods['onAfterMove']
    Action<BattleContext, Pokemon, Pokemon, Move>? OnFoeAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon, Pokemon>? OnFoeAttract { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, object?>? OnFoeAccuracy { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeBasePower { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Pokemon, IEffect>? OnFoeBeforeFaint { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnFoeBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon>? OnFoeBeforeSwitchIn { get; }
    Action<BattleContext, Pokemon>? OnFoeBeforeSwitchOut { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnFoeChargeMove { get; } // CommonHandlers['VoidSourceMove']
    Func<BattleContext, Pokemon, object?, Move, bool?>? OnFoeCriticalHit { get; }
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, object?>? OnFoeDamage { get; }
    Func<BattleContext, Pokemon, Pokemon, int?>? OnFoeDeductPp { get; }
    Action<BattleContext, Pokemon>? OnFoeDisableMove { get; }
    Action<BattleContext, Pokemon, Pokemon?, Move?>? OnFoeDragOut { get; }
    Action<BattleContext, Item, Pokemon>? OnFoeEatItem { get; }
    Func<BattleContext, int, Pokemon?, string, Move, int?>? OnFoeEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    Action<BattleContext, Pokemon, Pokemon, IEffect>? OnFoeFaint { get; } // CommonHandlers['VoidEffect']
    Func<BattleContext, Pokemon, bool?>? OnFoeFlinch { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnFoeHit { get; } // MoveEventMethods['onHit']
    Action<BattleContext, string, Pokemon>? OnFoeImmunity { get; }
    Func<BattleContext, Pokemon, string?>? OnFoeLockMove { get; }
    Action<BattleContext, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeModifyDef { get; } // CommonHandlers['ModifierMove']
    Action<BattleContext, Move, Pokemon, Pokemon?>? OnFoeModifyMove { get; } // MoveEventMethods['onModifyMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnFoeModifySecondaries { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeModifySpD { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, int?>? OnFoeModifySpe { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Move, Pokemon, Pokemon>? OnFoeModifyType { get; } // MoveEventMethods['onModifyType']
    Action<BattleContext, Pokemon, Pokemon, Pokemon, Move>? OnFoeModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    Func<BattleContext, int, Pokemon, int?>? OnFoeModifyWeight { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnFoeMoveAborted { get; } // CommonHandlers['VoidMove']
    Func<BattleContext, Pokemon, string, bool?>? OnFoeNegateImmunity { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, string?>? OnFoeOverrideAction { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnFoePrepareHit { get; } // CommonHandlers['ResultSourceMove']
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
    Func<BattleContext, Pokemon, Pokemon, Move, object>? OnFoeTryHit { get; } // MoveEventMethods['onTryHit']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnFoeTryHitField { get; } // MoveEventMethods['onTryHitField']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnFoeTryHitSide { get; } // CommonHandlers['ResultMove']
    Func<BattleContext, Pokemon, Pokemon, Move, IntBoolUnion?>? OnFoeInvulnerability { get; } // CommonHandlers['ExtResultMove']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnFoeTryMove { get; } // MoveEventMethods['onTryMove']
    Func<BattleContext, Pokemon, Pokemon, Move, object?>? OnFoeTryPrimaryHit { get; }


    Func<BattleContext, List<string>, Pokemon, List<string>?>? OnFoeType { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnFoeModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']

    // Source event handlers (triggered when this Pokémon is the source)
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnSourceDamagingHit { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnSourceAfterHit { get; } // MoveEventMethods['onAfterHit']
    Action<BattleContext, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; }
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnSourceAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    Action<BattleContext, Pokemon>? OnSourceAfterSwitchInSelf { get; }
    Action<BattleContext, Item, Pokemon>? OnSourceAfterUseItem { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; }
    Action<BattleContext, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnSourceAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf']
    Action<BattleContext, Pokemon, Pokemon, Move>? OnSourceAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    Action<BattleContext, Pokemon, Pokemon, Move>? OnSourceAfterMove { get; } // MoveEventMethods['onAfterMove']
    Action<BattleContext, Pokemon, Pokemon, Move>? OnSourceAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon, Pokemon>? OnSourceAttract { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, object?>? OnSourceAccuracy { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceBasePower { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Pokemon, IEffect>? OnSourceBeforeFaint { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnSourceBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon>? OnSourceBeforeSwitchIn { get; }
    Action<BattleContext, Pokemon>? OnSourceBeforeSwitchOut { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnSourceChargeMove { get; } // CommonHandlers['VoidSourceMove']
    Func<BattleContext, Pokemon, object?, Move, bool?>? OnSourceCriticalHit { get; }
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, object?>? OnSourceDamage { get; }
    Func<BattleContext, Pokemon, Pokemon, int?>? OnSourceDeductPp { get; }
    Action<BattleContext, Pokemon>? OnSourceDisableMove { get; }
    Action<BattleContext, Pokemon, Pokemon?, Move?>? OnSourceDragOut { get; }
    Action<BattleContext, Item, Pokemon>? OnSourceEatItem { get; }
    Func<BattleContext, int, Pokemon?, string, Move, int?>? OnSourceEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    Action<BattleContext, Pokemon, Pokemon, IEffect>? OnSourceFaint { get; } // CommonHandlers['VoidEffect']
    Func<BattleContext, Pokemon, bool?>? OnSourceFlinch { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnSourceHit { get; } // MoveEventMethods['onHit']
    Action<BattleContext, string, Pokemon>? OnSourceImmunity { get; }
    Func<BattleContext, Pokemon, string?>? OnSourceLockMove { get; }
    Action<BattleContext, Pokemon>? OnSourceMaybeTrapPokemon { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceModifyDef { get; } // CommonHandlers['ModifierMove']
    Action<BattleContext, Move, Pokemon, Pokemon?>? OnSourceModifyMove { get; } // MoveEventMethods['onModifyMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnSourceModifySecondaries { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceModifySpD { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, int?>? OnSourceModifySpe { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Move, Pokemon, Pokemon>? OnSourceModifyType { get; } // MoveEventMethods['onModifyType']
    Action<BattleContext, Pokemon, Pokemon, Pokemon, Move>? OnSourceModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    Func<BattleContext, int, Pokemon, int?>? OnSourceModifyWeight { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnSourceMoveAborted { get; } // CommonHandlers['VoidMove']
    Func<BattleContext, Pokemon, string, bool?>? OnSourceNegateImmunity { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, string?>? OnSourceOverrideAction { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnSourcePrepareHit { get; } // CommonHandlers['ResultSourceMove']
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
    Func<BattleContext, Pokemon, Pokemon, Move, object>? OnSourceTryHit { get; } // MoveEventMethods['onTryHit']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnSourceTryHitField { get; } // MoveEventMethods['onTryHitField']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnSourceTryHitSide { get; } // CommonHandlers['ResultMove']
    Func<BattleContext, Pokemon, Pokemon, Move, IntBoolUnion?>? OnSourceInvulnerability { get; } // CommonHandlers['ExtResultMove']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnSourceTryMove { get; } // MoveEventMethods['onTryMove']
    Func<BattleContext, Pokemon, Pokemon, Move, object?>? OnSourceTryPrimaryHit { get; }
    Func<BattleContext, List<string>, Pokemon, List<string>?>? OnSourceType { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnSourceModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']

    // Any event handlers (triggered for any Pokémon action in BattleContext)
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnAnyDamagingHit { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAnyAfterHit { get; } // MoveEventMethods['onAfterHit']
    Action<BattleContext, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; }
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnAnyAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    Action<BattleContext, Pokemon>? OnAnyAfterSwitchInSelf { get; }
    Action<BattleContext, Item, Pokemon>? OnAnyAfterUseItem { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; }
    Action<BattleContext, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; }
    Action<BattleContext, Pokemon>? OnAnyAfterMega { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAnyAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf']
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAnyAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAnyAfterMove { get; } // MoveEventMethods['onAfterMove']
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAnyAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon>? OnAnyAfterTerastallization { get; }
    Action<BattleContext, Pokemon, Pokemon>? OnAnyAttract { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, object?>? OnAnyAccuracy { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyBasePower { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Pokemon, IEffect>? OnAnyBeforeFaint { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAnyBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon>? OnAnyBeforeSwitchIn { get; }
    Action<BattleContext, Pokemon>? OnAnyBeforeSwitchOut { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAnyChargeMove { get; } // CommonHandlers['VoidSourceMove']
    Func<BattleContext, Pokemon, object?, Move, bool?>? OnAnyCriticalHit { get; }
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, object?>? OnAnyDamage { get; }
    Func<BattleContext, Pokemon, Pokemon, int?>? OnAnyDeductPp { get; }
    Action<BattleContext, Pokemon>? OnAnyDisableMove { get; }
    Action<BattleContext, Pokemon, Pokemon?, Move?>? OnAnyDragOut { get; }
    Action<BattleContext, Item, Pokemon>? OnAnyEatItem { get; }
    Func<BattleContext, int, Pokemon?, string, Move, int?>? OnAnyEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    Action<BattleContext, Pokemon, Pokemon, IEffect>? OnAnyFaint { get; } // CommonHandlers['VoidEffect']
    Func<BattleContext, Pokemon, bool?>? OnAnyFlinch { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnAnyHit { get; } //  MoveEventMethods['onHit']
    Action<BattleContext, string, Pokemon>? OnAnyImmunity { get; }
    Func<BattleContext, Pokemon, string?>? OnAnyLockMove { get; }
    Action<BattleContext, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyModifyDef { get; } // CommonHandlers['ModifierMove']
    Action<BattleContext, Move, Pokemon, Pokemon?>? OnAnyModifyMove { get; } // MoveEventMethods['onModifyMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnAnyModifySecondaries { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyModifySpD { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, int?>? OnAnyModifySpe { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Move, Pokemon, Pokemon>? OnAnyModifyType { get; } // MoveEventMethods['onModifyType']
    Action<BattleContext, Pokemon, Pokemon, Pokemon, Move>? OnAnyModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    Func<BattleContext, int, Pokemon, int?>? OnAnyModifyWeight { get; }
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAnyMoveAborted { get; }
    Func<BattleContext, Pokemon, string, bool?>? OnAnyNegateImmunity { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, string?>? OnAnyOverrideAction { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnAnyPrepareHit { get; } // CommonHandlers['ResultSourceMove']
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
    Func<BattleContext, Pokemon, Pokemon, Move, object>? OnAnyTryHit { get; } // MoveEventMethods['onTryHit']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnAnyTryHitField { get; } // MoveEventMethods['onTryHitField']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnAnyTryHitSide { get; } // CommonHandlers['ResultMove']
    Func<BattleContext, Pokemon, Pokemon, Move, IntBoolUnion?>? OnAnyInvulnerability { get; } // CommonHandlers['ExtResultMove']
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnAnyTryMove { get; } // MoveEventMethods['onTryMove']
    Func<BattleContext, Pokemon, Pokemon, Move, object?>? OnAnyTryPrimaryHit { get; }
    Func<BattleContext, List<string>, Pokemon, List<string>?>? OnAnyType { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnAnyModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']

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