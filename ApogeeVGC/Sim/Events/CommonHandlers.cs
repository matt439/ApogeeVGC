using ApogeeVGC.Sim.BattleClasses;
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
//    Func<IBattle, int, Pokemon, Pokemon, IEffect, int?> ModifierEffect { get; }

//    /// <summary>
//    /// battle, relayVar, target, source, move -> number | void
//    /// </summary>
//    Func<IBattle, int, Pokemon, Pokemon, Move, int?> ModifierMove { get; }
    
//    /// <summary>
//    /// battle, target, source, move -> boolean | null | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, bool?> ResultMove { get; }
    
//    /// <summary>
//    /// battle, target, source, move -> boolean | null | number | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?> ExtResultMove { get; }
    
//    /// <summary>
//    /// battle, target, source, effect
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, IEffect> VoidEffect { get; }
    
//    /// <summary>
//    /// battle, target, source, move
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, Move> VoidMove { get; }
    
//    /// <summary>
//    /// battle, relayVar, source, target, effect
//    /// </summary>
//    Func<IBattle, int, Pokemon, Pokemon, IEffect, int?> ModifierSourceEffect { get; }
    
//    /// <summary>
//    /// battle, relayVar, source, target, move
//    /// </summary>
//    Func<IBattle, int, Pokemon, Pokemon, Move, int?> ModifierSourceMove { get; }
    
//    /// <summary>
//    /// battle, source, target, move -> boolean | null | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, bool?> ResultSourceMove { get; }
    
//    /// <summary>
//    /// battle, source, target, move -> boolean | null | number | "" | void
//    /// </summary>
//    Func<IBattle, Pokemon, Pokemon, Move, IntBoolUnion?> ExtResultSourceMove { get; }
    
//    /// <summary>
//    /// battle, source, target, effect
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, IEffect> VoidSourceEffect { get; }
    
//    /// <summary>
//    /// battle, source, target, move
//    /// </summary>
//    Action<IBattle, Pokemon, Pokemon, Move> VoidSourceMove { get; }
//}



public delegate int? ModifierEffectHandler(IBattle battle, int relayVar, Pokemon target,
    Pokemon source, IEffect effect);

public delegate int? ModifierMoveHandler(IBattle battle, int relayVar, Pokemon target,
    Pokemon source, Move move);

public delegate bool? ResultMoveHandler(IBattle battle, Pokemon target, Pokemon source, Move move);
public delegate IntBoolUnion? ExtResultMoveHandler(IBattle battle, Pokemon target, Pokemon source, Move move);
public delegate void VoidEffectHandler(IBattle battle, Pokemon target, Pokemon source, IEffect effect);
public delegate void VoidMoveHandler(IBattle battle, Pokemon target, Pokemon source, Move move);

// Source-based variants (source comes first in parameters)
public delegate int? ModifierSourceEffectHandler(IBattle battle, int relayVar, Pokemon source,
    Pokemon target, IEffect effect);

public delegate int? ModifierSourceMoveHandler(IBattle battle, int relayVar, Pokemon source,
    Pokemon target, Move move);

public delegate bool? ResultSourceMoveHandler(IBattle battle, Pokemon source, Pokemon target, Move move);
public delegate IntBoolUnion? ExtResultSourceMoveHandler(IBattle battle, Pokemon source,
    Pokemon target, Move move);

public delegate void VoidSourceEffectHandler(IBattle battle, Pokemon source, Pokemon target, IEffect effect);
public delegate void VoidSourceMoveHandler(IBattle battle, Pokemon source, Pokemon target, Move move);