using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

public interface IPokemonEventMethods : IEventMethods
{
    /// <summary>
    /// battle, damage, target, source, move
    /// </summary>
    Action<Battle, int, Pokemon, Pokemon, ActiveMove>? OnAllyDamagingHit { get; }
    
    /// <summary>
    /// battle, boost, target, source
    /// </summary>
    Action<Battle, SparseBoostsTable, Pokemon, Pokemon>? OnAllyAfterEachBoost { get; }
    
    VoidSourceMoveHandler? OnAllyAfterHit { get; } // MoveEventMethods['onAfterHit']
    
    /// <summary>
    /// battle, status, target, source, effect
    /// </summary>
    Action<Battle, Condition, Pokemon, Pokemon, IEffect>? OnAllyAfterSetStatus { get; }
    
    OnAfterSubDamageHandler? OnAllyAfterSubDamage { get; } // MoveEventMethods['onAfterSubDamage']
    
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<Battle, Pokemon>? OnAllyAfterSwitchInSelf { get; }
    
    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<Battle, Item, Pokemon>? OnAllyAfterUseItem { get; }
    
    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyAfterBoost { get; }
    
    /// <summary>
    /// battle, length, target, source, effect
    /// </summary>
    Action<Battle, int, Pokemon, Pokemon, IEffect>? OnAllyAfterFaint { get; }
    
    VoidSourceMoveHandler? OnAllyAfterMoveSecondarySelf { get; } // MoveEventMethods['onAfterMoveSecondarySelf']
    VoidMoveHandler? OnAllyAfterMoveSecondary { get; } // MoveEventMethods['onAfterMoveSecondary']
    VoidSourceMoveHandler? OnAllyAfterMove { get; } // MoveEventMethods['onAfterMove']
    VoidSourceMoveHandler? OnAllyAfterMoveSelf { get; } // CommonHandlers['VoidSourceMove']
    
    /// <summary>
    /// battle, target, source
    /// </summary>
    Action<Battle, Pokemon, Pokemon>? OnAllyAttract { get; }
    
    /// <summary>
    /// battle, accuracy, target, source, move -> number | boolean | null
    /// </summary>
    Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAllyAccuracy { get; }
    
    ModifierSourceMoveHandler? OnAllyBasePower { get; } // CommonHandlers['ModifierSourceMove']
    
    /// <summary>
    /// battle, pokemon, effect
    /// </summary>
    Action<Battle, Pokemon, IEffect>? OnAllyBeforeFaint { get; }
    
    VoidSourceMoveHandler? OnAllyBeforeMove { get; } // CommonHandlers['VoidSourceMove']
    
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<Battle, Pokemon>? OnAllyBeforeSwitchIn { get; }
    
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<Battle, Pokemon>? OnAllyBeforeSwitchOut { get; }
    
    /// <summary>
    /// battle, boost, target, source, effect
    /// </summary>
    Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>? OnAllyTryBoost { get; }
    
    VoidSourceMoveHandler? OnAllyChargeMove { get; } // CommonHandlers['VoidSourceMove']
    OnCriticalHit? OnAllyCriticalHit { get; }
    
    /// <summary>
    /// battle, damage, target, source, effect -> number | boolean | null
    /// </summary>
    Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>? OnAllyDamage { get; }
    
    /// <summary>
    /// battle, target, source -> number
    /// </summary>
    Func<Battle, Pokemon, Pokemon, IntVoidUnion>? OnAllyDeductPp { get; }
    
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<Battle, Pokemon>? OnAllyDisableMove { get; }
    
    /// <summary>
    /// battle, pokemon, source?, move?
    /// </summary>
    Action<Battle, Pokemon, Pokemon?, ActiveMove?>? OnAllyDragOut { get; }
    
    /// <summary>
    /// battle, item, pokemon
    /// </summary>
    Action<Battle, Item, Pokemon>? OnAllyEatItem { get; }
    
    OnEffectivenessHandler? OnAllyEffectiveness { get; } // MoveEventMethods['onEffectiveness']
    VoidEffectHandler? OnAllyFaint { get; } // CommonHandlers['VoidEffect']
    OnFlinch? OnAllyFlinch { get; }
    ResultMoveHandler? OnAllyHit { get; } // MoveEventMethods['onHit']
    
    /// <summary>
    /// battle, type, pokemon
    /// </summary>
    Action<Battle, PokemonType, Pokemon>? OnAllyImmunity { get; }

    /// <summary>
    /// battle, pokemon -> move
    /// </summary>
    OnLockMove? OnAllyLockMove { get; }
    
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<Battle, Pokemon>? OnAllyMaybeTrapPokemon { get; }
    
    ModifierMoveHandler? OnAllyModifyAccuracy { get; } // CommonHandlers['ModifierMove']
    ModifierSourceMoveHandler? OnAllyModifyAtk { get; } // CommonHandlers['ModifierSourceMove']
    
    /// <summary>
    /// battle, boosts, pokemon -> SparseBoostsTable
    /// </summary>
    Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>? OnAllyModifyBoost { get; }
    
    ModifierSourceMoveHandler? OnAllyModifyCritRatio { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnAllyModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnAllyModifyDef { get; } // CommonHandlers['ModifierMove']
    OnModifyMoveHandler? OnAllyModifyMove { get; } // MoveEventMethods['onModifyMove']
    ModifierSourceMoveHandler? OnAllyModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    
    /// <summary>
    /// battle, secondaries, target, source, move
    /// </summary>
    Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>? OnAllyModifySecondaries { get; }
    
    ModifierSourceMoveHandler? OnAllyModifySpA { get; } // CommonHandlers['ModifierSourceMove']
    ModifierMoveHandler? OnAllyModifySpD { get; } // CommonHandlers['ModifierMove']
    
    /// <summary>
    /// battle, spe, pokemon -> number
    /// </summary>
    Func<Battle, int, Pokemon, IntVoidUnion>? OnAllyModifySpe { get; }
    
    ModifierSourceMoveHandler? OnAllyModifyStab { get; } // CommonHandlers['ModifierSourceMove']
    OnModifyTypeHandler? OnAllyModifyType { get; } // MoveEventMethods['onModifyType']
    OnModifyTargetHandler? OnAllyModifyTarget { get; } // MoveEventMethods['onModifyTarget']
    
    /// <summary>
    /// battle, weighthg, pokemon -> number
    /// </summary>
    Func<Battle, int, Pokemon, IntVoidUnion>? OnAllyModifyWeight { get; }
    
    VoidMoveHandler? OnAllyMoveAborted { get; } // CommonHandlers['VoidMove']
    OnNegateImmunity? OnAllyNegateImmunity { get; }
    
    /// <summary>
    /// battle, pokemon, target, move -> string
    /// </summary>
    Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>? OnAllyOverrideAction { get; }
    
    ResultSourceMoveHandler? OnAllyPrepareHit { get; } // CommonHandlers['ResultSourceMove']
    
    /// <summary>
    /// battle, target, source, source2, move -> Pokemon
    /// </summary>
    Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>? OnAllyRedirectTarget { get; }
    
    /// <summary>
    /// battle, target & side, source, effect
    /// </summary>
    Action<Battle, PokemonSideUnion, Pokemon, IEffect>? OnAllyResidual { get; }
    
    /// <summary>
    /// battle, ability, target, source, effect -> boolean
    /// </summary>
    Func<Battle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>? OnAllySetAbility { get; }
    
    /// <summary>
    /// battle, status, target, source, effect -> boolean | null
    /// </summary>
    Func<Battle, Condition, Pokemon, Pokemon, IEffect, PokemonVoidUnion?>? OnAllySetStatus { get; }
    
    /// <summary>
    /// battle, target, source, weather -> boolean
    /// </summary>
    Func<Battle, Pokemon, Pokemon, Condition, PokemonVoidUnion>? OnAllySetWeather { get; }
    
    /// <summary>
    /// battle, pokemon -> boolean
    /// </summary>
    Func<Battle, Pokemon, PokemonVoidUnion>? OnAllyStallMove { get; }
    
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<Battle, Pokemon>? OnAllySwitchOut { get; }
    
    OnTakeItem? OnAllyTakeItem { get; }
    
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<Battle, Pokemon>? OnAllyTerrain { get; }
    
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<Battle, Pokemon>? OnAllyTrapPokemon { get; }
    
    /// <summary>
    /// battle, status, target, source, sourceEffect -> boolean | null
    /// </summary>
    Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>? OnAllyTryAddVolatile { get; }
    
    /// <summary>
    /// battle, item, pokemon -> boolean
    /// </summary>
    Func<Battle, Item, Pokemon, BoolVoidUnion>? OnAllyTryEatItem { get; }
    
    OnTryHeal? OnAllyTryHeal { get; }
    ExtResultSourceMoveHandler? OnAllyTryHit { get; } // MoveEventMethods['onTryHit']
    ExtResultSourceMoveHandler? OnAllyTryHitField { get; } // MoveEventMethods['onTryHit']
    ResultMoveHandler? OnAllyTryHitSide { get; } // CommonHandlers['ResultMove']
    ExtResultMoveHandler? OnAllyInvulnerability { get; } // CommonHandlers['ExtResultMove']
    ResultSourceMoveHandler? OnAllyTryMove { get; } // MoveEventMethods['onTryMove']
    
    /// <summary>
    /// battle, target, source, move -> boolean | null | number
    /// </summary>
    Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>? OnAllyTryPrimaryHit { get; }
    
    /// <summary>
    /// battle, types, pokemon -> string[]
    /// </summary>
    Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>? OnAllyType { get; }
    
    ModifierSourceMoveHandler? OnAllyWeatherModifyDamage { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnAllyModifyDamagePhase1 { get; } // CommonHandlers['ModifierSourceMove']
    ModifierSourceMoveHandler? OnAllyModifyDamagePhase2 { get; } // CommonHandlers['ModifierSourceMove']
}