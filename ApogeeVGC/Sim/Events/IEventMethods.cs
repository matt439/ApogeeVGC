using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public interface IEventMethods
{
    /// <summary>
    /// battle, damage, target, source, move
    /// </summary>
    Action<IBattle, int, Pokemon, Pokemon, Move>? OnDamagingHit { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnEmergencyExit { get; }

    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterEachBoost { get; }

    VoidSourceMoveHandler? OnAfterHit { get; } // MoveEventMethods['onAfterHit']
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAfterMega { get; }

    /// <summary>
    /// battle, status, target, source, effect
    /// </summary>
    Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnAfterSetStatus { get; }

    OnAfterSubDamageHandler? OnAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAfterSwitchInSelf { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAfterTerastallization { get; }

    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<IBattle, Item, Pokemon>? OnAfterUseItem { get; }

    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<IBattle, Item, Pokemon>? OnAfterTakeItem { get; }

    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAfterBoost { get; }

    /// <summary>
    /// battle, length, target, source, effect
    /// </summary>
    Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnAfterFaint { get; }

    VoidSourceMoveHandler? OnAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf'] 
    VoidMoveHandler? OnAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    VoidSourceMoveHandler? OnAfterMove { get; } // MoveEventMethods['onAfterMove']
    VoidSourceMoveHandler? OnAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    /// <summary>
    /// battle, target, source
    /// </summary>
    Action<IBattle, Pokemon, Pokemon>? OnAttract { get; }

    /// <summary>
    /// battle, accuracy, target, source, move -> number | boolean | null
    /// </summary>
    Func<IBattle, int, Pokemon, Pokemon, Move, IntBoolUnion?>? OnAccuracy { get; }

    ModifierSourceMoveHandler? OnBasePower { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, pokemon, effect
    /// </summary>
    Action<IBattle, Pokemon, IEffect>? OnBeforeFaint { get; }

    VoidSourceMoveHandler? OnBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnBeforeSwitchIn { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnBeforeSwitchOut { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnBeforeTurn { get; }

    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnChangeBoost { get; }

    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnTryBoost { get; }

    VoidSourceMoveHandler? OnChargeMove { get; } // CommonHandlers['VoidSourceMove']
    OnCriticalHit? OnCriticalHit { get; }

    /// <summary>
    /// battle, damage, target, source, effect -> number | boolean | null
    /// </summary>
    Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnDamage { get; }

    /// <summary>
    /// battle, target, source -> number
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, int?>? OnDeductPp { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnDisableMove { get; }

    /// <summary>
    /// battle, pokemon, source?, move?
    /// </summary>
    Action<IBattle, Pokemon, Pokemon?, Move?>? OnDragOut { get; }

    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<IBattle, Item, Pokemon>? OnEatItem { get; }

    OnEffectivenessHandler? OnEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnEntryHazard { get; }

    VoidEffectHandler? OnFaint { get; } // CommonHandlers['VoidEffect']
    OnFlinch? OnFlinch { get; }
    OnFractionalPriority? OnFractionalPriority { get; }
    ResultMoveHandler? OnHit { get; } // MoveEventMethods['onHit']
    /// <summary>
    /// battle, type, pokemon
    /// </summary>
    Action<IBattle, PokemonType, Pokemon>? OnImmunity { get; }

    /// <summary>
    /// battle, pokemon -> move
    /// </summary>
    Func<IBattle, Pokemon, Move?>? OnLockMove { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnMaybeTrapPokemon { get; }

    ModifierMoveHandler? OnModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    ModifierSourceMoveHandler? OnModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, boosts, pokemon -> SparseBoostsTable
    /// </summary>
    Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnModifyBoost { get; }

    ModifierSourceMoveHandler? OnModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnModifyDef { get; } // CommonHandlers['ModifierMove']
    OnModifyMoveHandler? OnModifyMove { get; } // MoveEventMethods['onModifyMove']
    ModifierSourceMoveHandler? OnModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, secondaries, target, source, move
    /// </summary>
    Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnModifySecondaries { get; }

    OnModifyTypeHandler? OnModifyType { get; } // MoveEventMethods['onModifyType']
    OnModifyTargetHandler? OnModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    ModifierSourceMoveHandler? OnModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnModifySpD { get; } // CommonHandlers['ModifierMove']
    /// <summary>
    /// battle, spe, pokemon -> number
    /// </summary>
    Func<IBattle, int, Pokemon, double?>? OnModifySpe { get; }

    ModifierSourceMoveHandler? OnModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, weighthg, pokemon -> number
    /// </summary>
    Func<IBattle, int, Pokemon, int?>? OnModifyWeight { get; }

    VoidMoveHandler? OnMoveAborted { get; } // CommonHandlers['VoidMove']
    OnNegateImmunity? OnNegateImmunity { get; }

    /// <summary>
    /// battle, pokemon, target, move -> string
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Move, Delegate?>? OnOverrideAction { get; }

    ResultSourceMoveHandler? OnPrepareHit { get; } // CommonHandlers['ResultSourceMove']
    /// <summary>
    /// battle, target, source, pseudoWeather
    /// </summary>
    Action<IBattle, Pokemon, Pokemon, Condition>? OnPseudoWeatherChange { get; }

    /// <summary>
    /// battle, target, source, source2, move -> Pokemon
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnRedirectTarget { get; }

    /// <summary>
    /// battle, target, source, effect
    /// </summary>
    Action<IBattle, Pokemon, Pokemon, IEffect>? OnResidual { get; }

    /// <summary>
    /// battle, ability, target, source, effect
    /// </summary>
    Action<IBattle, Ability, Pokemon, Pokemon, IEffect>? OnSetAbility { get; }

    /// <summary>
    /// battle, status, target, source, effect -> boolean | null
    /// </summary>
    Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSetStatus { get; }

    /// <summary>
    /// battle, target, source, weather -> boolean
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Condition, bool?>? OnSetWeather { get; }

    /// <summary>
    /// battle, target, source, sideCondition
    /// </summary>
    Action<IBattle, Side, Pokemon, Condition>? OnSideConditionStart { get; }

    /// <summary>
    /// battle, pokemon -> boolean
    /// </summary>
    Func<IBattle, Pokemon, bool?>? OnStallMove { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnSwitchIn { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnSwitchOut { get; }

    /// <summary>
    /// battle, target, source
    /// </summary>
    Action<IBattle, Pokemon, Pokemon>? OnSwap { get; }

    OnTakeItem? OnTakeItem { get; }

    /// <summary>
    /// battle, target, source, sourceEffect
    /// </summary>
    Action<IBattle, Pokemon, Pokemon, IEffect>? OnWeatherChange { get; }

    /// <summary>
    /// battle, target, source, sourceEffect
    /// </summary>
    Action<IBattle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnTrapPokemon { get; }

    /// <summary>
    /// battle, status, target, source, sourceEffect -> boolean | null
    /// </summary>
    Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnTryAddVolatile { get; }

    /// <summary>
    /// battle, item, pokemon -> boolean
    /// </summary>
    Func<IBattle, Item, Pokemon, bool?>? OnTryEatItem { get; }

    /// <summary>
    /// battle, relayVar, target, source, effect -> number | boolean | null
    /// </summary>
    Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnTryHeal { get; }

    ExtResultSourceMoveHandler? OnTryHit { get; } // MoveEventMethods['onTryHit']
    ResultMoveHandler? OnTryHitField { get; } // MoveEventMethods['onTryHitField']
    ResultMoveHandler? OnTryHitSide { get; } // CommonHandlers['ResultMove']
    ExtResultMoveHandler? OnInvulnerability { get; } // CommonHandlers['ExtResultMove']
    ResultSourceMoveHandler? OnTryMove { get; } // MoveEventMethods['onTryMove']

    /// <summary>
    /// battle, target, source, move -> boolean | null | number
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?>? OnTryPrimaryHit { get; }

    /// <summary>
    /// battle, types, pokemon -> string[]
    /// </summary>
    Func<IBattle, PokemonType[], Pokemon, List<PokemonType>?>? OnType { get; }

    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<IBattle, Item, Pokemon>? OnUseItem { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnUpdate { get; }

    /// <summary>
    /// battle, target, source, effect
    /// </summary>
    Action<IBattle, Pokemon, object?, Condition>? OnWeather { get; }

    ModifierSourceMoveHandler? OnWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, damage, target, source, move
    /// </summary>
    Action<IBattle, int, Pokemon, Pokemon, Move>? OnFoeDamagingHit { get; }

    /// <summary>
    /// battle, boost, target, source
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon>? OnFoeAfterEachBoost { get; }

    VoidSourceMoveHandler? OnFoeAfterHit { get; } // MoveEventMethods['onAfterHit']
    /// <summary>
    /// battle, status, target, source, effect
    /// </summary>
    Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnFoeAfterSetStatus { get; }

    OnAfterSubDamageHandler? OnFoeAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnFoeAfterSwitchInSelf { get; }

    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<IBattle, Item, Pokemon>? OnFoeAfterUseItem { get; }

    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeAfterBoost { get; }

    /// <summary>
    /// battle, length, target, source, effect
    /// </summary>
    Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnFoeAfterFaint { get; }

    VoidSourceMoveHandler? OnFoeAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf']
    VoidMoveHandler? OnFoeAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    VoidSourceMoveHandler? OnFoeAfterMove { get; } // MoveEventMethods['onAfterMove']
    VoidSourceMoveHandler? OnFoeAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    /// <summary>
    /// battle, target, source
    /// </summary>
    Action<IBattle, Pokemon, Pokemon>? OnFoeAttract { get; }

    /// <summary>
    /// battle, accuracy, target, source, move -> number | boolean | null
    /// </summary>
    Func<IBattle, int, Pokemon, Pokemon, Move, IntBoolUnion?>? OnFoeAccuracy { get; }

    ModifierSourceMoveHandler? OnFoeBasePower { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, pokemon, effect
    /// </summary>
    Action<IBattle, Pokemon, IEffect>? OnFoeBeforeFaint { get; }

    VoidSourceMoveHandler? OnFoeBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnFoeBeforeSwitchIn { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnFoeBeforeSwitchOut { get; }

    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnFoeTryBoost { get; }

    VoidSourceMoveHandler? OnFoeChargeMove { get; } // CommonHandlers['VoidSourceMove']
    OnCriticalHit? OnFoeCriticalHit { get; }

    /// <summary>
    /// battle, damage, target, source, effect -> number | boolean | null
    /// </summary>
    Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnFoeDamage { get; }

    /// <summary>
    /// battle, target, source -> number
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, int?>? OnFoeDeductPp { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnFoeDisableMove { get; }

    /// <summary>
    /// battle, pokemon, source?, move?
    /// </summary>
    Action<IBattle, Pokemon, Pokemon?, Move?>? OnFoeDragOut { get; }

    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<IBattle, Item, Pokemon>? OnFoeEatItem { get; }

    OnEffectivenessHandler? OnFoeEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    VoidEffectHandler? OnFoeFaint { get; } // CommonHandlers['VoidEffect']
    OnFlinch? OnFoeFlinch { get; }
    ResultMoveHandler? OnFoeHit { get; } // MoveEventMethods['onHit']
    /// <summary>
    /// battle, type, pokemon
    /// </summary>
    Action<IBattle, PokemonType, Pokemon>? OnFoeImmunity { get; }

    /// <summary>
    /// battle, pokemon -> move
    /// </summary>
    Func<IBattle, Pokemon, Move?>? OnFoeLockMove { get; }

    /// <summary>
    /// battle, pokemon, source?
    /// </summary>
    Action<IBattle, Pokemon, Pokemon?>? OnFoeMaybeTrapPokemon { get; }

    ModifierMoveHandler? OnFoeModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    ModifierSourceMoveHandler? OnFoeModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, boosts, pokemon -> SparseBoostsTable
    /// </summary>
    Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnFoeModifyBoost { get; }

    ModifierSourceMoveHandler? OnFoeModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnFoeModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnFoeModifyDef { get; } // CommonHandlers['ModifierMove']
    OnModifyMoveHandler? OnFoeModifyMove { get; } // MoveEventMethods['onModifyMove']
    ModifierSourceMoveHandler? OnFoeModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, secondaries, target, source, move
    /// </summary>
    Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnFoeModifySecondaries { get; }

    ModifierSourceMoveHandler? OnFoeModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnFoeModifySpD { get; } // CommonHandlers['ModifierMove']
    /// <summary>
    /// battle, spe, pokemon -> number
    /// </summary>
    Func<IBattle, int, Pokemon, int?>? OnFoeModifySpe { get; }

    ModifierSourceMoveHandler? OnFoeModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    OnModifyTypeHandler? OnFoeModifyType { get; } // MoveEventMethods['onModifyType']
    OnModifyTargetHandler? OnFoeModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    /// <summary>
    /// battle, weighthg, pokemon -> number
    /// </summary>
    Func<IBattle, int, Pokemon, int?>? OnFoeModifyWeight { get; }

    VoidMoveHandler? OnFoeMoveAborted { get; } // CommonHandlers['VoidMove']
    OnNegateImmunity? OnFoeNegateImmunity { get; }

    /// <summary>
    /// battle, pokemon, target, move -> string
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Move, Delegate?>? OnFoeOverrideAction { get; }

    ResultSourceMoveHandler? OnFoePrepareHit { get; } // CommonHandlers['ResultSourceMove']
    /// <summary>
    /// battle, target, source, source2, move -> Pokemon
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnFoeRedirectTarget { get; }

    /// <summary>
    /// battle, target & side, source, effect
    /// </summary>
    Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnFoeResidual { get; }

    /// <summary>
    /// battle, ability, target, source, effect -> boolean
    /// </summary>
    Func<IBattle, Ability, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetAbility { get; }

    /// <summary>
    /// battle, status, target, source, effect -> boolean | null
    /// </summary>
    Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeSetStatus { get; }

    /// <summary>
    /// battle, target, source, weather -> boolean
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Condition, bool?>? OnFoeSetWeather { get; }

    /// <summary>
    /// battle, pokemon -> boolean
    /// </summary>
    Func<IBattle, Pokemon, bool?>? OnFoeStallMove { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnFoeSwitchOut { get; }

    OnTakeItem? OnFoeTakeItem { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnFoeTerrain { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnFoeTrapPokemon { get; }

    /// <summary>
    /// battle, status, target, source, sourceEffect -> boolean | null
    /// </summary>
    Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnFoeTryAddVolatile { get; }

    /// <summary>
    /// battle, item, pokemon -> boolean
    /// </summary>
    Func<IBattle, Item, Pokemon, bool?>? OnFoeTryEatItem { get; }

    OnTryHeal? OnFoeTryHeal { get; } // MoveEventMethods['onTryHeal']
    ExtResultSourceMoveHandler? OnFoeTryHit { get; } // MoveEventMethods['onTryHit']
    ResultMoveHandler? OnFoeTryHitField { get; } // MoveEventMethods['onTryHitField']
    ResultMoveHandler? OnFoeTryHitSide { get; } // CommonHandlers['ResultMove']
    ExtResultMoveHandler? OnFoeInvulnerability { get; } // CommonHandlers['ExtResultMove']
    ResultSourceMoveHandler? OnFoeTryMove { get; } // MoveEventMethods['onTryMove']
    /// <summary>
    /// battle, target, source, move -> boolean | null | number
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?>? OnFoeTryPrimaryHit { get; }

    /// <summary>
    /// battle, types, pokemon -> string[]
    /// </summary>
    Func<IBattle, PokemonType[], Pokemon, PokemonType[]?>? OnFoeType { get; }

    ModifierSourceMoveHandler? OnFoeWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnFoeModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnFoeModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, damage, target, source, move
    /// </summary>
    Action<IBattle, int, Pokemon, Pokemon, Move>? OnSourceDamagingHit { get; }

    /// <summary>
    /// battle, boost, target, source
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon>? OnSourceAfterEachBoost { get; }

    VoidSourceMoveHandler? OnSourceAfterHit { get; } // MoveEventMethods['onAfterHit']
    /// <summary>
    /// battle, status, target, source, effect
    /// </summary>
    Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnSourceAfterSetStatus { get; }

    OnAfterSubDamageHandler? OnSourceAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnSourceAfterSwitchInSelf { get; }

    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<IBattle, Item, Pokemon>? OnSourceAfterUseItem { get; }

    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceAfterBoost { get; }

    /// <summary>
    /// battle, length, target, source, effect
    /// </summary>
    Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; }

    VoidSourceMoveHandler? OnSourceAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf']
    VoidMoveHandler? OnSourceAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    VoidSourceMoveHandler? OnSourceAfterMove { get; } // MoveEventMethods['onAfterMove']
    VoidSourceMoveHandler? OnSourceAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    /// <summary>
    /// battle, target, source
    /// </summary>
    Action<IBattle, Pokemon, Pokemon>? OnSourceAttract { get; }

    /// <summary>
    /// battle, accuracy, target, source, move -> number | boolean | null
    /// </summary>
    Func<IBattle, int, Pokemon, Pokemon, Move, IntBoolUnion?>? OnSourceAccuracy { get; }

    ModifierSourceMoveHandler? OnSourceBasePower { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, pokemon, effect
    /// </summary>
    Action<IBattle, Pokemon, IEffect>? OnSourceBeforeFaint { get; }

    VoidSourceMoveHandler? OnSourceBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnSourceBeforeSwitchIn { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnSourceBeforeSwitchOut { get; }

    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnSourceTryBoost { get; }

    VoidSourceMoveHandler? OnSourceChargeMove { get; } // CommonHandlers['VoidSourceMove']
    OnCriticalHit? OnSourceCriticalHit { get; }

    /// <summary>
    /// battle, damage, target, source, effect -> number | boolean | null
    /// </summary>
    Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnSourceDamage { get; }

    /// <summary>
    /// battle, target, source -> number
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, int?>? OnSourceDeductPp { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnSourceDisableMove { get; }

    /// <summary>
    /// battle, pokemon, source?, move?
    /// </summary>
    Action<IBattle, Pokemon, Pokemon?, Move?>? OnSourceDragOut { get; }

    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<IBattle, Item, Pokemon>? OnSourceEatItem { get; }

    OnEffectivenessHandler? OnSourceEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    VoidEffectHandler? OnSourceFaint { get; } // CommonHandlers['VoidEffect']
    OnFlinch? OnSourceFlinch { get; }
    ResultMoveHandler? OnSourceHit { get; } // MoveEventMethods['onHit']
    /// <summary>
    /// battle, type, pokemon
    /// </summary>
    Action<IBattle, PokemonType, Pokemon>? OnSourceImmunity { get; }

    /// <summary>
    /// battle, pokemon -> move
    /// </summary>
    Func<IBattle, Pokemon, Move?>? OnSourceLockMove { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnSourceMaybeTrapPokemon { get; }

    ModifierMoveHandler? OnSourceModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    ModifierSourceMoveHandler? OnSourceModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, boosts, pokemon -> SparseBoostsTable
    /// </summary>
    Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnSourceModifyBoost { get; }

    ModifierSourceMoveHandler? OnSourceModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnSourceModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnSourceModifyDef { get; } // CommonHandlers['ModifierMove']
    OnModifyMoveHandler? OnSourceModifyMove { get; } // MoveEventMethods['onModifyMove']
    ModifierSourceMoveHandler? OnSourceModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, secondaries, target, source, move
    /// </summary>
    Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnSourceModifySecondaries { get; }

    ModifierSourceMoveHandler? OnSourceModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnSourceModifySpD { get; } // CommonHandlers['ModifierMove']
    /// <summary>
    /// battle, spe, pokemon -> number
    /// </summary>
    Func<IBattle, int, Pokemon, int?>? OnSourceModifySpe { get; }

    ModifierSourceMoveHandler? OnSourceModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    OnModifyTypeHandler? OnSourceModifyType { get; } // MoveEventMethods['onModifyType']
    OnModifyTargetHandler? OnSourceModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    /// <summary>
    /// battle, weighthg, pokemon -> number
    /// </summary>
    Func<IBattle, int, Pokemon, int?>? OnSourceModifyWeight { get; }

    VoidMoveHandler? OnSourceMoveAborted { get; } // CommonHandlers['VoidMove']
    OnNegateImmunity? OnSourceNegateImmunity { get; }

    /// <summary>
    /// battle, pokemon, target, move -> string
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Move, Delegate?>? OnSourceOverrideAction { get; }

    ResultSourceMoveHandler? OnSourcePrepareHit { get; } // CommonHandlers['ResultSourceMove']
    /// <summary>
    /// battle, target, source, source2, move -> Pokemon
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnSourceRedirectTarget { get; }

    /// <summary>
    /// battle, target & side, source, effect
    /// </summary>
    Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnSourceResidual { get; }

    /// <summary>
    /// battle, ability, target, source, effect -> boolean
    /// </summary>
    Func<IBattle, Ability, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetAbility { get; }

    /// <summary>
    /// battle, status, target, source, effect -> boolean | null
    /// </summary>
    Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceSetStatus { get; }

    /// <summary>
    /// battle, target, source, weather -> boolean
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Condition, bool?>? OnSourceSetWeather { get; }

    /// <summary>
    /// battle, pokemon -> boolean
    /// </summary>
    Func<IBattle, Pokemon, bool?>? OnSourceStallMove { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnSourceSwitchOut { get; }

    OnTakeItem? OnSourceTakeItem { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnSourceTerrain { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnSourceTrapPokemon { get; }

    /// <summary>
    /// battle, status, target, source, sourceEffect -> boolean | null
    /// </summary>
    Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnSourceTryAddVolatile { get; }

    /// <summary>
    /// battle, item, pokemon -> boolean
    /// </summary>
    Func<IBattle, Item, Pokemon, bool?>? OnSourceTryEatItem { get; }

    OnTryHeal? OnSourceTryHeal { get; } // MoveEventMethods['onTryHeal']
    ExtResultSourceMoveHandler? OnSourceTryHit { get; } // MoveEventMethods['onTryHit']
    ResultMoveHandler? OnSourceTryHitField { get; } // MoveEventMethods['onTryHitField']
    ResultMoveHandler? OnSourceTryHitSide { get; } // CommonHandlers['ResultMove']
    ExtResultMoveHandler? OnSourceInvulnerability { get; } // CommonHandlers['ExtResultMove']
    ResultSourceMoveHandler? OnSourceTryMove { get; } // MoveEventMethods['onTryMove']
    /// <summary>
    /// battle, target, source, move -> boolean | null | number
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?>? OnSourceTryPrimaryHit { get; }

    /// <summary>
    /// battle, types, pokemon -> string[]
    /// </summary>
    Func<IBattle, PokemonType[], Pokemon, PokemonType[]?>? OnSourceType { get; }

    ModifierSourceMoveHandler? OnSourceWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnSourceModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnSourceModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, damage, target, source, move
    /// </summary>
    Action<IBattle, int, Pokemon, Pokemon, Move>? OnAnyDamagingHit { get; }

    /// <summary>
    /// battle, boost, target, source
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon>? OnAnyAfterEachBoost { get; }

    VoidSourceMoveHandler? OnAnyAfterHit { get; } // MoveEventMethods['onAfterHit']
    /// <summary>
    /// battle, status, target, source, effect
    /// </summary>
    Action<IBattle, Condition, Pokemon, Pokemon, IEffect>? OnAnyAfterSetStatus { get; }

    OnAfterSubDamageHandler? OnAnyAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAnyAfterSwitchInSelf { get; }

    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<IBattle, Item, Pokemon>? OnAnyAfterUseItem { get; }

    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyAfterBoost { get; }

    /// <summary>
    /// battle, length, target, source, effect
    /// </summary>
    Action<IBattle, int, Pokemon, Pokemon, IEffect>? OnAnyAfterFaint { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAnyAfterMega { get; }

    VoidSourceMoveHandler? OnAnyAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf']
    VoidMoveHandler? OnAnyAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    VoidSourceMoveHandler? OnAnyAfterMove { get; } // MoveEventMethods['onAfterMove']
    VoidSourceMoveHandler? OnAnyAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAnyAfterTerastallization { get; }

    /// <summary>
    /// battle, target, source
    /// </summary>
    Action<IBattle, Pokemon, Pokemon>? OnAnyAttract { get; }
    /// <summary>
    /// battle, accuracy, target, source, move -> number | boolean | null
    /// </summary>
    Func<IBattle, int, Pokemon, Pokemon, Move, IntBoolUnion?>? OnAnyAccuracy { get; }

    ModifierSourceMoveHandler? OnAnyBasePower { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, pokemon, effect
    /// </summary>
    Action<IBattle, Pokemon, IEffect>? OnAnyBeforeFaint { get; }

    VoidSourceMoveHandler? OnAnyBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAnyBeforeSwitchIn { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAnyBeforeSwitchOut { get; }

    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<IBattle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAnyTryBoost { get; }

    VoidSourceMoveHandler? OnAnyChargeMove { get; } // CommonHandlers['VoidSourceMove']
    OnCriticalHit? OnAnyCriticalHit { get; }
    /// <summary>
    /// battle, damage, target, source, effect -> number | boolean | null
    /// </summary>
    Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnAnyDamage { get; }

    /// <summary>
    /// battle, target, source -> number
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, int?>? OnAnyDeductPp { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAnyDisableMove { get; }

    /// <summary>
    /// battle, pokemon, source?, move?
    /// </summary>
    Action<IBattle, Pokemon, Pokemon?, Move?>? OnAnyDragOut { get; }

    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<IBattle, Item, Pokemon>? OnAnyEatItem { get; }

    OnEffectivenessHandler? OnAnyEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    VoidEffectHandler? OnAnyFaint { get; } // CommonHandlers['VoidEffect']
    OnFlinch? OnAnyFlinch { get; }
    ResultMoveHandler? OnAnyHit { get; } //  MoveEventMethods['onHit']
    /// <summary>
    /// battle, type, pokemon
    /// </summary>
    Action<IBattle, PokemonType, Pokemon>? OnAnyImmunity { get; }

    /// <summary>
    /// battle, pokemon -> move
    /// </summary>
    Func<IBattle, Pokemon, Move?>? OnAnyLockMove { get; }

    /// <summary>
    /// battle, pokemon, source?
    /// </summary>
    Action<IBattle, Pokemon, Pokemon?>? OnAnyMaybeTrapPokemon { get; }

    ModifierMoveHandler? OnAnyModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    ModifierSourceMoveHandler? OnAnyModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, boosts, pokemon -> SparseBoostsTable
    /// </summary>
    Func<IBattle, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAnyModifyBoost { get; }

    ModifierSourceMoveHandler? OnAnyModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnAnyModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnAnyModifyDef { get; } // CommonHandlers['ModifierMove']
    OnModifyMoveHandler? OnAnyModifyMove { get; } // MoveEventMethods['onModifyMove']
    ModifierSourceMoveHandler? OnAnyModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    /// <summary>
    /// battle, secondaries, target, source, move
    /// </summary>
    Action<IBattle, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnAnyModifySecondaries { get; }

    ModifierSourceMoveHandler? OnAnyModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnAnyModifySpD { get; } // CommonHandlers['ModifierMove']
    /// <summary>
    /// battle, spe, pokemon -> number
    /// </summary>
    Func<IBattle, int, Pokemon, int?>? OnAnyModifySpe { get; }

    ModifierSourceMoveHandler? OnAnyModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    OnModifyTypeHandler? OnAnyModifyType { get; } // MoveEventMethods['onModifyType']
    OnModifyTargetHandler? OnAnyModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    /// <summary>
    /// battle, weighthg, pokemon -> number
    /// </summary>
    Func<IBattle, int, Pokemon, int?>? OnAnyModifyWeight { get; }

    /// <summary>
    /// battle, target, source, move
    /// </summary>
    Action<IBattle, Pokemon, Pokemon, Move>? OnAnyMoveAborted { get; }

    OnNegateImmunity? OnAnyNegateImmunity { get; }
    /// <summary>
    /// battle, pokemon, target, move -> string
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Move, Delegate?>? OnAnyOverrideAction { get; }

    ResultSourceMoveHandler? OnAnyPrepareHit { get; } // CommonHandlers['ResultSourceMove']
    /// <summary>
    /// battle, target, source, pseudoWeather
    /// </summary>
    Action<IBattle, Pokemon, Pokemon, Condition>? OnAnyPseudoWeatherChange { get; }

    /// <summary>
    /// battle, target, source, source2, move -> Pokemon
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnAnyRedirectTarget { get; }

    /// <summary>
    /// battle, target & side, source, effect
    /// </summary>
    Action<IBattle, PokemonSideUnion, Pokemon, IEffect>? OnAnyResidual { get; }

    /// <summary>
    /// battle, ability, target, source, effect -> boolean
    /// </summary>
    Func<IBattle, Ability, Pokemon, Pokemon, IEffect, bool?>? OnAnySetAbility { get; }

    /// <summary>
    /// battle, status, target, source, effect -> boolean | null
    /// </summary>
    Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnySetStatus { get; }

    /// <summary>
    /// battle, target, source, weather -> boolean
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Condition, bool?>? OnAnySetWeather { get; }

    /// <summary>
    /// battle, pokemon -> boolean
    /// </summary>
    Func<IBattle, Pokemon, bool?>? OnAnyStallMove { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAnySwitchIn { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAnySwitchOut { get; }

    OnTakeItem? OnAnyTakeItem { get; }
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAnyTerrain { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnAnyTrapPokemon { get; }

    /// <summary>
    /// battle, status, target, source, sourceEffect -> boolean | null
    /// </summary>
    Func<IBattle, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAnyTryAddVolatile { get; }

    /// <summary>
    /// battle, item, pokemon -> boolean
    /// </summary>
    Func<IBattle, Item, Pokemon, bool?>? OnAnyTryEatItem { get; }

    OnTryHeal? OnAnyTryHeal { get; } // MoveEventMethods['onTryHeal']
    ExtResultSourceMoveHandler? OnAnyTryHit { get; } // MoveEventMethods['onTryHit']
    ResultMoveHandler? OnAnyTryHitField { get; } // MoveEventMethods['onTryHitField']
    ResultMoveHandler? OnAnyTryHitSide { get; } // CommonHandlers['ResultMove']
    ExtResultMoveHandler? OnAnyInvulnerability { get; } // CommonHandlers['ExtResultMove']
    ResultSourceMoveHandler? OnAnyTryMove { get; } // MoveEventMethods['onTryMove']
    /// <summary>
    /// battle, target, source, move -> boolean | null | number
    /// </summary>
    Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?>? OnAnyTryPrimaryHit { get; }

    /// <summary>
    /// battle, types, pokemon -> string[]
    /// </summary>
    Func<IBattle, PokemonType[], Pokemon, PokemonType[]?>? OnAnyType { get; }

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