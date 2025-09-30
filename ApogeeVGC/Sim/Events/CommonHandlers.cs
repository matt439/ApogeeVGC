using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

//public interface ICommonHandlers
//{
//    /// <summary>
//    /// battle, relayVar, target, source, effect -> number | void
//    /// </summary>
//    Func<BattleContext, int, Pokemon, Pokemon, IEffect, int?> ModifierEffect { get; }

//    /// <summary>
//    /// battle, relayVar, target, source, move -> number | void
//    /// </summary>
//    Func<BattleContext, int, Pokemon, Pokemon, Move, int?> ModifierMove { get; }
    
//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, bool?> ResultMove { get; }
    
//    /// <summary>
//    /// battle, target, source, move -> boolean | null | number | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, IntBoolUnion?> ExtResultMove { get; }
    
//    /// <summary>
//    /// battle, target, source, effect
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, IEffect> VoidEffect { get; }
    
//    /// <summary>
//    /// battle, target, source, move
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, Move> VoidMove { get; }
    
//    /// <summary>
//    /// battle, relayVar, source, target, effect
//    /// </summary>
//    Func<BattleContext, int, Pokemon, Pokemon, IEffect, int?> ModifierSourceEffect { get; }
    
//    /// <summary>
//    /// battle, relayVar, source, target, move
//    /// </summary>
//    Func<BattleContext, int, Pokemon, Pokemon, Move, int?> ModifierSourceMove { get; }
    
//    /// <summary>
//    /// battle, source, target, move -> boolean | null | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, bool?> ResultSourceMove { get; }
    
//    /// <summary>
//    /// battle, source, target, move -> boolean | null | number | "" | void
//    /// </summary>
//    Func<BattleContext, Pokemon, Pokemon, Move, IntBoolUnion?> ExtResultSourceMove { get; }
    
//    /// <summary>
//    /// battle, source, target, effect
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, IEffect> VoidSourceEffect { get; }
    
//    /// <summary>
//    /// battle, source, target, move
//    /// </summary>
//    Action<BattleContext, Pokemon, Pokemon, Move> VoidSourceMove { get; }
//}



public delegate int? ModifierEffectHandler(BattleContext battle, int relayVar, Pokemon target,
    Pokemon source, IEffect effect);

public delegate int? ModifierMoveHandler(BattleContext battle, int relayVar, Pokemon target,
    Pokemon source, Move move);

public delegate bool? ResultMoveHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate IntBoolUnion? ExtResultMoveHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate void VoidEffectHandler(BattleContext battle, Pokemon target, Pokemon source, IEffect effect);
public delegate void VoidMoveHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);

// Source-based variants (source comes first in parameters)
public delegate int? ModifierSourceEffectHandler(BattleContext battle, int relayVar, Pokemon source,
    Pokemon target, IEffect effect);

public delegate int? ModifierSourceMoveHandler(BattleContext battle, int relayVar, Pokemon source,
    Pokemon target, Move move);

public delegate bool? ResultSourceMoveHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);
public delegate IntBoolUnion? ExtResultSourceMoveHandler(BattleContext battle, Pokemon source,
    Pokemon target, Move move);

public delegate void VoidSourceEffectHandler(BattleContext battle, Pokemon source, Pokemon target, IEffect effect);
public delegate void VoidSourceMoveHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);