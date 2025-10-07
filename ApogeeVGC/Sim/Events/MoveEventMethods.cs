using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;


// Move callback delegates (from the original IMoveEventMethods interface)
public delegate IntFalseUnion? BasePowerCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon target, ActiveMove move);
public delegate bool? BeforeMoveCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon? target, ActiveMove move);
public delegate void BeforeTurnCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon target, ActiveMove move);
public delegate IntFalseUnion? DamageCallbackHandler(IBattle battle, Pokemon pokemon, Pokemon target, ActiveMove move);
public delegate void PriorityChargeCallbackHandler(IBattle battle, Pokemon pokemon);

// Move event delegates
public delegate void OnDisableMoveHandler(IBattle battle, Pokemon pokemon);
public delegate void OnAfterSubDamageHandler(IBattle battle, int damage, Pokemon target, Pokemon source, ActiveMove move);
public delegate IntBoolUnion? OnDamageHandler(IBattle battle, int damage, Pokemon target, Pokemon source, IEffect effect);

public delegate int? OnEffectivenessHandler(IBattle battle, int typeMod, Pokemon? target, PokemonType type,
    ActiveMove move);
public delegate bool? OnHitSideHandler(IBattle battle, Side side, Pokemon source, ActiveMove move);
public delegate void OnModifyMoveHandler(IBattle battle, ActiveMove move, Pokemon pokemon, Pokemon? target);
public delegate void OnModifyTypeHandler(IBattle battle, ActiveMove move, Pokemon pokemon, Pokemon target);
public delegate void OnModifyTargetHandler(IBattle battle, Pokemon relayTarget, Pokemon pokemon, Pokemon target, ActiveMove move);
public delegate bool? OnTryHitSideHandler(IBattle battle, Side side, Pokemon source, ActiveMove move);

//// Additional move event delegates
//public delegate void OnUseMoveMessageHandler(IBattle battle, Pokemon source, Pokemon target, ActiveMove move);
//public delegate bool? OnTryHitFieldHandler(IBattle battle, Pokemon target, Pokemon source, ActiveMove move);
//public delegate bool? OnTryImmunityHandler(IBattle battle, Pokemon target, Pokemon source, ActiveMove move);
//public delegate bool? OnTryHandler(IBattle battle, Pokemon source, Pokemon target, ActiveMove move);
//public delegate IntBoolUnion? OnTryHitHandler(IBattle battle, Pokemon source, Pokemon target, ActiveMove move);
//public delegate bool? OnPrepareHitHandler(IBattle battle, Pokemon target, Pokemon source, ActiveMove move);
//public delegate bool? OnTryMoveHandler(IBattle battle, Pokemon source, Pokemon target, ActiveMove move);

public interface IMoveEventHandlers
{
    BasePowerCallbackHandler? BasePowerCallback { get; }
    BeforeMoveCallbackHandler? BeforeMoveCallback { get; }
    BeforeTurnCallbackHandler? BeforeTurnCallback { get; }
    DamageCallbackHandler? DamageCallback { get; }
    PriorityChargeCallbackHandler? PriorityChargeCallback { get; }

    OnDisableMoveHandler? OnDisableMove { get; }
    VoidSourceMoveHandler? OnAfterHit { get; }
    OnAfterSubDamageHandler? OnAfterSubDamage { get; }
    VoidSourceMoveHandler? OnAfterMoveSecondarySelf { get; }
    VoidMoveHandler? OnAfterMoveSecondary { get; }
    VoidSourceMoveHandler? OnAfterMove { get; }
    OnDamageHandler? OnDamage { get; }
    ModifierSourceMoveHandler? OnBasePower { get; }
    OnEffectivenessHandler? OnEffectiveness { get; }
    ResultMoveHandler? OnHit { get; }
    ResultMoveHandler? OnHitField { get; }
    OnHitSideHandler? OnHitSide { get; }
    OnModifyMoveHandler? OnModifyMove { get; }
    ModifierSourceMoveHandler? OnModifyPriority { get; }
    VoidMoveHandler? OnMoveFail { get; }
    OnModifyTypeHandler? OnModifyType { get; }
    OnModifyTargetHandler? OnModifyTarget { get; }
    ResultMoveHandler? OnPrepareHit { get; }
    ResultSourceMoveHandler? OnTry { get; }
    ExtResultSourceMoveHandler? OnTryHit { get; }
    ResultMoveHandler? OnTryHitField { get; }
    OnTryHitSideHandler? OnTryHitSide { get; }
    ResultMoveHandler? OnTryImmunity { get; }
    ResultSourceMoveHandler? OnTryMove { get; }
    VoidSourceMoveHandler? OnUseMoveMessage { get; }
}

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
//    Func<IBattle, Pokemon, Pokemon, Move, IntFalseUnion?>? BasePowerCallback { get; }

//    /// <summary>
//    /// battle, pokemon, target | null, move -> boolean | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon?, Move, bool?>? BeforeMoveCallback { get; } // Return true to stop the move from being used

//    /// <summary>
//    /// battle, pokemon, target, move
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, Move>? BeforeTurnCallback { get; }

//    /// <summary>
//    /// battle, pokemon, target, move -> number | false
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, IntFalseUnion>? DamageCallback { get; }

//    /// <summary>
//    /// battle, pokemon
//    /// </summary>
//    Action<IBattle, Pokemon>? PriorityChargeCallback { get; }

//    /// <summary>
//    /// battle, pokemon
//    /// </summary>
//    Action<IBattle, Pokemon>? OnDisableMove { get; }

//    /// <summary>
//    /// battle, source, target, move
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, Move>? OnAfterHit { get; } // CommonHandlers['VoidSourceMove']

//    /// <summary>
//    /// battle, damage, target, source, move
//    /// </summary>
//    Action<IBattle, int, Pokemon, Pokemon, Move>? OnAfterSubDamage { get; }

//    /// <summary>
//    /// battle, source, target, move
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, Move>? OnAfterMoveSecondarySelf { get; } // CommonHandlers['VoidSourceMove']

//    /// <summary>
//    /// battle, target, source, move
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, Move>? OnAfterMoveSecondary { get; } // CommonHandlers['VoidMove']

//    /// <summary>
//    /// battle, source, target, move
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, Move>? OnAfterMove { get; } // CommonHandlers['VoidSourceMove']

//    /// <summary>
//    /// Priority value for damage calculation ordering
//    /// </summary>
//    int? OnDamagePriority { get; }

//    /// <summary>
//    /// battle, damage, target, source, effect -> number | boolean | null | void
//    /// </summary>
//    Func<IBattle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnDamage { get; }

//    /// <summary>
//    /// battle, relayVar, source, target, move -> number | void
//    /// </summary>
//    Func<IBattle, int, Pokemon, Pokemon, Move, int?>? OnBasePower { get; } // CommonHandlers['ModifierSourceMove']

//    /// <summary>
//    /// battle, typeMod, target | null, type, move -> number | void
//    /// </summary>
//    Func<IBattle, int, Pokemon?, string, Move, int?>? OnEffectiveness { get; }

//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, bool?>? OnHit { get; } // CommonHandlers['ResultMove']

//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, bool?>? OnHitField { get; } // CommonHandlers['ResultMove']

//    /// <summary>
//    /// battle, side, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<IBattle, Side, Pokemon, Move, bool?>? OnHitSide { get; }

//    /// <summary>
//    /// battle, move, pokemon, target | null
//    /// </summary>
//    Action<IBattle, Move, Pokemon, Pokemon?>? OnModifyMove { get; }

//    /// <summary>
//    /// battle, relayVar, source, target, move -> number | void
//    /// </summary>
//    Func<IBattle, int, Pokemon, Pokemon, Move, int?>? OnModifyPriority { get; } // CommonHandlers['ModifierSourceMove']

//    /// <summary>
//    /// battle, target, source, move
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, Move>? OnMoveFail { get; } // CommonHandlers['VoidMove']

//    /// <summary>
//    /// battle, move, pokemon, target
//    /// </summary>
//    Action<IBattle, Move, Pokemon, Pokemon>? OnModifyType { get; }

//    /// <summary>
//    /// battle, relayTarget, pokemon, target, move
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, Pokemon, Move>? OnModifyTarget { get; }

//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, bool?>? OnPrepareHit { get; } // CommonHandlers['ResultMove']

//    /// <summary>
//    /// battle, source, target, move -> boolean | null | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, bool?>? OnTry { get; } // CommonHandlers['ResultSourceMove']

//    /// <summary>
//    /// battle, source, target, move -> boolean | null | number | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?>? OnTryHit { get; } // CommonHandlers['ExtResultSourceMove']

//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, bool?>? OnTryHitField { get; } // CommonHandlers['ResultMove']

//    /// <summary>
//    /// battle, side, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<IBattle, Side, Pokemon, Move, bool?>? OnTryHitSide { get; }

//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, bool?>? OnTryImmunity { get; } // // CommonHandlers['ResultMove']

//    /// <summary>
//    /// battle, source, target, move -> boolean | null | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, bool?>? OnTryMove { get; } // CommonHandlers['ResultSourceMove'];

//    /// <summary>
//    /// battle, source, target, move
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, Move>? OnUseMoveMessage { get; } // CommonHandlers['VoidSourceMove'];
//}