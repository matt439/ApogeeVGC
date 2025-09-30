using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public interface IMoveEventMethods
{
    /// <summary>
    /// battle, pokemon, target, move -> number | false | null
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, IntFalseUnion?>? BasePowerCallback { get; }
    
    /// <summary>
    /// battle, pokemon, target | null, move -> boolean | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon?, Move, bool?>? BeforeMoveCallback { get; } // Return true to stop the move from being used
    
    /// <summary>
    /// battle, pokemon, target, move
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, Move>? BeforeTurnCallback { get; }
    
    /// <summary>
    /// battle, pokemon, target, move -> number | false
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, IntFalseUnion>? DamageCallback { get; }
    
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<BattleContext, Pokemon>? PriorityChargeCallback { get; }
    
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<BattleContext, Pokemon>? OnDisableMove { get; }
    
    /// <summary>
    /// battle, source, target, move
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterHit { get; } // CommonHandlers['VoidSourceMove']
    
    /// <summary>
    /// battle, damage, target, source, move
    /// </summary>
    Action<BattleContext, int, Pokemon, Pokemon, Move>? OnAfterSubDamage { get; }
    
    /// <summary>
    /// battle, source, target, move
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterMoveSecondarySelf { get; } // CommonHandlers['VoidSourceMove']
    
    /// <summary>
    /// battle, target, source, move
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterMoveSecondary { get; } // CommonHandlers['VoidMove']
    
    /// <summary>
    /// battle, source, target, move
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, Move>? OnAfterMove { get; } // CommonHandlers['VoidSourceMove']
    
    /// <summary>
    /// Priority value for damage calculation ordering
    /// </summary>
    int? OnDamagePriority { get; }
    
    /// <summary>
    /// battle, damage, target, source, effect -> number | boolean | null | void
    /// </summary>
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>? OnDamage { get; }
    
    /// <summary>
    /// battle, relayVar, source, target, move -> number | void
    /// </summary>
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnBasePower { get; } // CommonHandlers['ModifierSourceMove']
    
    /// <summary>
    /// battle, typeMod, target | null, type, move -> number | void
    /// </summary>
    Func<BattleContext, int, Pokemon?, string, Move, int?>? OnEffectiveness { get; }
    
    /// <summary>
    /// battle, target, source, move -> boolean | null | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnHit { get; } // CommonHandlers['ResultMove']
    
    /// <summary>
    /// battle, target, source, move -> boolean | null | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnHitField { get; } // CommonHandlers['ResultMove']
    
    /// <summary>
    /// battle, side, source, move -> boolean | null | "" | void
    /// </summary>
    Func<BattleContext, Side, Pokemon, Move, bool?>? OnHitSide { get; }
    
    /// <summary>
    /// battle, move, pokemon, target | null
    /// </summary>
    Action<BattleContext, Move, Pokemon, Pokemon?>? OnModifyMove { get; }
    
    /// <summary>
    /// battle, relayVar, source, target, move -> number | void
    /// </summary>
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?>? OnModifyPriority { get; } // CommonHandlers['ModifierSourceMove']
    
    /// <summary>
    /// battle, target, source, move
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, Move>? OnMoveFail { get; } // CommonHandlers['VoidMove']
    
    /// <summary>
    /// battle, move, pokemon, target
    /// </summary>
    Action<BattleContext, Move, Pokemon, Pokemon>? OnModifyType { get; }
    
    /// <summary>
    /// battle, relayTarget, pokemon, target, move
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, Pokemon, Move>? OnModifyTarget { get; }
    
    /// <summary>
    /// battle, target, source, move -> boolean | null | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnPrepareHit { get; } // CommonHandlers['ResultMove']
    
    /// <summary>
    /// battle, source, target, move -> boolean | null | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnTry { get; } // CommonHandlers['ResultSourceMove']
    
    /// <summary>
    /// battle, source, target, move -> boolean | null | number | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, IntBoolUnion?>? OnTryHit { get; } // CommonHandlers['ExtResultSourceMove']
    
    /// <summary>
    /// battle, target, source, move -> boolean | null | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnTryHitField { get; } // CommonHandlers['ResultMove']
    
    /// <summary>
    /// battle, side, source, move -> boolean | null | "" | void
    /// </summary>
    Func<BattleContext, Side, Pokemon, Move, bool?>? OnTryHitSide { get; }
    
    /// <summary>
    /// battle, target, source, move -> boolean | null | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnTryImmunity { get; } // // CommonHandlers['ResultMove']
    
    /// <summary>
    /// battle, source, target, move -> boolean | null | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, bool?>? OnTryMove { get; } // CommonHandlers['ResultSourceMove'];
    
    /// <summary>
    /// battle, source, target, move
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, Move>? OnUseMoveMessage { get; } // CommonHandlers['VoidSourceMove'];
}