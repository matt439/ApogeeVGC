using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public interface ICommonHandlers
{
    /// <summary>
    /// battle, relayVar, target, source, effect -> number | void
    /// </summary>
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, int?> ModifierEffect { get; }

    /// <summary>
    /// battle, relayVar, target, source, move -> number | void
    /// </summary>
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?> ModifierMove { get; }
    
    /// <summary>
    /// battle, target, source, move -> boolean | null | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, bool?> ResultMove { get; }
    
    /// <summary>
    /// battle, target, source, move -> boolean | null | number | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, IntBoolUnion?> ExtResultMove { get; }
    
    /// <summary>
    /// battle, target, source, effect
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, IEffect> VoidEffect { get; }
    
    /// <summary>
    /// battle, target, source, move
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, Move> VoidMove { get; }
    
    /// <summary>
    /// battle, relayVar, source, target, effect
    /// </summary>
    Func<BattleContext, int, Pokemon, Pokemon, IEffect, int?> ModifierSourceEffect { get; }
    
    /// <summary>
    /// battle, relayVar, source, target, move
    /// </summary>
    Func<BattleContext, int, Pokemon, Pokemon, Move, int?> ModifierSourceMove { get; }
    
    /// <summary>
    /// battle, source, target, move -> boolean | null | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, bool?> ResultSourceMove { get; }
    
    /// <summary>
    /// battle, source, target, move -> boolean | null | number | "" | void
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, Move, IntBoolUnion?> ExtResultSourceMove { get; }
    
    /// <summary>
    /// battle, source, target, effect
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, IEffect> VoidSourceEffect { get; }
    
    /// <summary>
    /// battle, source, target, move
    /// </summary>
    Action<BattleContext, Pokemon, Pokemon, Move> VoidSourceMove { get; }
}