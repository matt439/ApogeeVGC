using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public interface IPokemonEventMethods : IEventMethods
{
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnAllyDamagingHit { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; }
    VoidSourceMoveHandler? OnAllyAfterHit { get; } // MoveEventMethods['onAfterHit']
    Action<BattleContext, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; }
    OnAfterSubDamageHandler? OnAllyAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    Action<BattleContext, Pokemon>? OnAllyAfterSwitchInSelf { get; }
    Action<BattleContext, Item, Pokemon>? OnAllyAfterUseItem { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; }
    Action<BattleContext, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; }
    VoidSourceMoveHandler? OnAllyAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf']
    VoidMoveHandler? OnAllyAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    VoidSourceMoveHandler? OnAllyAfterMove { get; } // MoveEventMethods['onAfterMove']
    VoidSourceMoveHandler? OnAllyAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon, Pokemon>? OnAllyAttract { get; }
    Func<BattleContext, int, Pokemon, Pokemon, Move, IntBoolUnion?>? OnAllyAccuracy { get; }
    ModifierSourceMoveHandler? OnAllyBasePower { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, Pokemon, IEffect>? OnAllyBeforeFaint { get; }
    VoidSourceMoveHandler? OnAllyBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    Action<BattleContext, Pokemon>? OnAllyBeforeSwitchIn { get; }
    Action<BattleContext, Pokemon>? OnAllyBeforeSwitchOut { get; }
    Action<BattleContext, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; }
    VoidSourceMoveHandler? OnAllyChargeMove { get; } // CommonHandlers['VoidSourceMove']
    OnCriticalHit? OnAllyCriticalHit { get; }
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnAllyDamage { get; }
    Func<BattleContext, Pokemon, Pokemon, int?>? OnAllyDeductPp { get; }
    Action<BattleContext, Pokemon>? OnAllyDisableMove { get; }
    Action<BattleContext, Pokemon, Pokemon?, Move?>? OnAllyDragOut { get; }
    Action<BattleContext, Item, Pokemon>? OnAllyEatItem { get; }
    OnEffectivenessHandler? OnAllyEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    VoidEffectHandler? OnAllyFaint { get; } // CommonHandlers['VoidEffect']
    OnFlinch? OnAllyFlinch { get; }
    ResultMoveHandler? OnAllyHit { get; } // MoveEventMethods['onHit']
    Action<BattleContext, PokemonType, Pokemon>? OnAllyImmunity { get; }
    Func<BattleContext, Pokemon, Move?>? OnAllyLockMove { get; }
    Action<BattleContext, Pokemon>? OnAllyMaybeTrapPokemon { get; }
    ModifierMoveHandler? OnAllyModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    ModifierSourceMoveHandler? OnAllyModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    Func<BattleContext, SparseBoostsTable, Pokemon, SparseBoostsTable?>? OnAllyModifyBoost { get; }
    ModifierSourceMoveHandler? OnAllyModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnAllyModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnAllyModifyDef { get; } // CommonHandlers['ModifierMove']
    OnModifyMoveHandler? OnAllyModifyMove { get; } // MoveEventMethods['onModifyMove']
    ModifierSourceMoveHandler? OnAllyModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    Action<BattleContext, List<SecondaryEffect>, Pokemon, Pokemon, Move>? OnAllyModifySecondaries { get; }
    ModifierSourceMoveHandler? OnAllyModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnAllyModifySpD { get; } // CommonHandlers['ModifierMove']
    Func<BattleContext, int, Pokemon, int?>? OnAllyModifySpe { get; }
    ModifierSourceMoveHandler? OnAllyModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    OnModifyTypeHandler? OnAllyModifyType { get; } // MoveEventMethods['onModifyType']
    OnModifyTargetHandler? OnAllyModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    Func<BattleContext, int, Pokemon, int?>? OnAllyModifyWeight { get; }
    VoidMoveHandler? OnAllyMoveAborted { get; } // CommonHandlers['VoidMove']
    OnNegateImmunity? OnAllyNegateImmunity { get; }
    Func<BattleContext, Pokemon, Pokemon, Move, Delegate?>? OnAllyOverrideAction { get; }
    ResultSourceMoveHandler? OnAllyPrepareHit { get; } // CommonHandlers['ResultSourceMove']
    Func<BattleContext, Pokemon, Pokemon, IEffect, Move, Pokemon?>? OnAllyRedirectTarget { get; }
    Action<BattleContext, PokemonSideUnion, Pokemon, IEffect>? OnAllyResidual { get; }
    Func<BattleContext, Ability, Pokemon, Pokemon, IEffect, bool?>? OnAllySetAbility { get; }
    Func<BattleContext, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllySetStatus { get; }
    Func<BattleContext, Pokemon, Pokemon, Condition, bool?>? OnAllySetWeather { get; }
    Func<BattleContext, Pokemon, bool?>? OnAllyStallMove { get; }
    Action<BattleContext, Pokemon>? OnAllySwitchOut { get; }
    OnTakeItem? OnAllyTakeItem { get; }
    Action<BattleContext, Pokemon>? OnAllyTerrain { get; }
    Action<BattleContext, Pokemon>? OnAllyTrapPokemon { get; }
    Func<BattleContext, Condition, Pokemon, Pokemon, IEffect, bool?>? OnAllyTryAddVolatile { get; }
    Func<BattleContext, Item, Pokemon, bool?>? OnAllyTryEatItem { get; }
    OnTryHeal? OnAllyTryHeal { get; }
    ExtResultSourceMoveHandler? OnAllyTryHit { get; } // MoveEventMethods['onTryHit']
    ExtResultSourceMoveHandler? OnAllyTryHitField { get; } // MoveEventMethods['onTryHit']
    ResultMoveHandler? OnAllyTryHitSide { get; } // CommonHandlers['ResultMove']
    ExtResultMoveHandler? OnAllyInvulnerability { get; } // CommonHandlers['ExtResultMove']
    ResultSourceMoveHandler? OnAllyTryMove { get; } // MoveEventMethods['onTryMove']
    Func<BattleContext, Pokemon, Pokemon, Move, IntBoolUnion?>? OnAllyTryPrimaryHit { get; }
    Func<BattleContext, PokemonType[], Pokemon, PokemonType[]?>? OnAllyType { get; }
    ModifierSourceMoveHandler? OnAllyWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnAllyModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnAllyModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']
}