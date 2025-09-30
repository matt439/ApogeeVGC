using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;


// Move callback delegates (from the original IMoveEventMethods interface)
public delegate IntFalseUnion? BasePowerCallbackHandler(BattleContext battle, Pokemon pokemon, Pokemon target, Move move);
public delegate bool? BeforeMoveCallbackHandler(BattleContext battle, Pokemon pokemon, Pokemon? target, Move move);
public delegate void BeforeTurnCallbackHandler(BattleContext battle, Pokemon pokemon, Pokemon target, Move move);
public delegate IntFalseUnion? DamageCallbackHandler(BattleContext battle, Pokemon pokemon, Pokemon target, Move move);
public delegate void PriorityChargeCallbackHandler(BattleContext battle, Pokemon pokemon);

// Move event delegates
public delegate void OnDisableMoveHandler(BattleContext battle, Pokemon pokemon);
public delegate void OnAfterSubDamageHandler(BattleContext battle, int damage, Pokemon target, Pokemon source, Move move);
public delegate IntBoolUnion? OnDamageHandler(BattleContext battle, int damage, Pokemon target, Pokemon source, IEffect effect);
public delegate int? OnEffectivenessHandler(BattleContext battle, int typeMod, Pokemon? target, string type, Move move);
public delegate bool? OnHitSideHandler(BattleContext battle, Side side, Pokemon source, Move move);
public delegate void OnModifyMoveHandler(BattleContext battle, Move move, Pokemon pokemon, Pokemon? target);
public delegate void OnModifyTypeHandler(BattleContext battle, Move move, Pokemon pokemon, Pokemon target);
public delegate void OnModifyTargetHandler(BattleContext battle, Pokemon relayTarget, Pokemon pokemon, Pokemon target, Move move);
public delegate bool? OnTryHitSideHandler(BattleContext battle, Side side, Pokemon source, Move move);

// Additional move event delegates
public delegate void OnMoveFailHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate void OnUseMoveMessageHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);
public delegate bool? OnTryHitFieldHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate bool? OnTryImmunityHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate bool? OnTryHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);
public delegate IntBoolUnion? OnTryHitHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);
public delegate bool? OnPrepareHitHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate bool? OnTryMoveHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);

// Common move event handlers mapping to common handler types

// OnAfterHit = VoidSourceMoveHandler
// OnAfterMoveSecondarySelf = VoidSourceMoveHandler
// OnAfterMoveSecondary = VoidMoveHandler
// OnAfterMove = VoidSourceMoveHandler
// OnBasePower = ModifierSourceMoveHandler
// OnHit = ResultMoveHandler
// OnHitField = ResultMoveHandler
// OnModifyPriority = ModifierSourceMoveHandler
// OnMoveFail = VoidMoveHandler
// OnPrepareHit = ResultMoveHandler
// OnTry = ResultSourceMoveHandler
// OnTryHit = ExtResultSourceMoveHandler
// OnTryHitField = ResultMoveHandler
// OnTryImmunity = ResultMoveHandler
// OnTryMove = ResultSourceMoveHandler
// OnUseMoveMessage = VoidSourceMoveHandler












//public interface IMoveEventMethods
//{
//    /// <summary>
//    /// battle, pokemon, target, move -> number | false | null
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, IntFalseUnion?>? BasePowerCallback { get; }

//    /// <summary>
//    /// battle, pokemon, target | null, move -> boolean | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon?, Move, bool?>? BeforeMoveCallback { get; } // Return true to stop the move from being used

//    /// <summary>
//    /// battle, pokemon, target, move
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, Move>? BeforeTurnCallback { get; }

//    /// <summary>
//    /// battle, pokemon, target, move -> number | false
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, IntFalseUnion>? DamageCallback { get; }

//    /// <summary>
//    /// battle, pokemon
//    /// </summary>
//    Action<BattleContext, Pokemon>? PriorityChargeCallback { get; }

//    /// <summary>
//    /// battle, pokemon
//    /// </summary>
//    Action<BattleContext, Pokemon>? OnDisableMove { get; }

//    /// <summary>
//    /// battle, source, target, move
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterHit { get; } // CommonHandlers['VoidSourceMove']

//    /// <summary>
//    /// battle, damage, target, source, move
//    /// </summary>
//    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnAfterSubDamage { get; }

//    /// <summary>
//    /// battle, source, target, move
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterMoveSecondarySelf { get; } // CommonHandlers['VoidSourceMove']

//    /// <summary>
//    /// battle, target, source, move
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterMoveSecondary { get; } // CommonHandlers['VoidMove']

//    /// <summary>
//    /// battle, source, target, move
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterMove { get; } // CommonHandlers['VoidSourceMove']

//    /// <summary>
//    /// Priority value for damage calculation ordering
//    /// </summary>
//    int? OnDamagePriority { get; }

//    /// <summary>
//    /// battle, damage, target, source, effect -> number | boolean | null | void
//    /// </summary>
//    Func<BattleContext, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnDamage { get; }

//    /// <summary>
//    /// battle, relayVar, source, target, move -> number | void
//    /// </summary>
//    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnBasePower { get; } // CommonHandlers['ModifierSourceMove']

//    /// <summary>
//    /// battle, typeMod, target | null, type, move -> number | void
//    /// </summary>
//    Func<BattleContext, int, Pokemon?, string, Move, int?>? OnEffectiveness { get; }

//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnHit { get; } // CommonHandlers['ResultMove']

//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnHitField { get; } // CommonHandlers['ResultMove']

//    /// <summary>
//    /// battle, side, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<BattleContext, Side, Pokemon, Move, bool?>? OnHitSide { get; }

//    /// <summary>
//    /// battle, move, pokemon, target | null
//    /// </summary>
//    Action<BattleContext, Move, Pokemon, Pokemon?>? OnModifyMove { get; }

//    /// <summary>
//    /// battle, relayVar, source, target, move -> number | void
//    /// </summary>
//    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifyPriority { get; } // CommonHandlers['ModifierSourceMove']

//    /// <summary>
//    /// battle, target, source, move
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, Move>? OnMoveFail { get; } // CommonHandlers['VoidMove']

//    /// <summary>
//    /// battle, move, pokemon, target
//    /// </summary>
//    Action<BattleContext, Move, Pokemon, Pokemon>? OnModifyType { get; }

//    /// <summary>
//    /// battle, relayTarget, pokemon, target, move
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, Pokemon, Move>? OnModifyTarget { get; }

//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnPrepareHit { get; } // CommonHandlers['ResultMove']

//    /// <summary>
//    /// battle, source, target, move -> boolean | null | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnTry { get; } // CommonHandlers['ResultSourceMove']

//    /// <summary>
//    /// battle, source, target, move -> boolean | null | number | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, IntBoolUnion?>? OnTryHit { get; } // CommonHandlers['ExtResultSourceMove']

//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnTryHitField { get; } // CommonHandlers['ResultMove']

//    /// <summary>
//    /// battle, side, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<BattleContext, Side, Pokemon, Move, bool?>? OnTryHitSide { get; }

//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnTryImmunity { get; } // // CommonHandlers['ResultMove']

//    /// <summary>
//    /// battle, source, target, move -> boolean | null | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnTryMove { get; } // CommonHandlers['ResultSourceMove'];

//    /// <summary>
//    /// battle, source, target, move
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, Move>? OnUseMoveMessage { get; } // CommonHandlers['VoidSourceMove'];
//}