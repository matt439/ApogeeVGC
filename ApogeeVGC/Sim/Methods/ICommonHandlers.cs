using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Unions;

namespace ApogeeVGC.Sim.Methods;

public interface ICommonHandlers
{
    /// <summary>battle, relayVar, target, source, effect</summary>
    /// <returns>The modified value, or null if no modification should occur</returns>
    public Func<Battle, int, Pokemon, Pokemon, IEffect, int?> ModifierEffect { get; }

    /// <summary>battle, relayVar, target, source, move</summary>
    /// <returns>The modified value, or null if no modification should occur</returns>
    public Func<Battle, int, Pokemon, Pokemon, Move, int?> ModifierMove { get; }

    /// <summary>battle, target, source, move</summary>
    /// <returns>True/false for the result, or null if no override should occur</returns>
    public Func<Battle, Pokemon, Pokemon, Move, bool?> ResultMove { get; }

    /// <summary>battle, target, source, move</summary>
    /// <returns>Boolean, integer, or null result depending on the specific interaction</returns>
    public Func<Battle, Pokemon, Pokemon, Move, IntBoolUnion?> ExtResultMove { get; }

    /// <summary>battle, target, source, effect</summary>
    public Action<Battle, Pokemon, Pokemon, IEffect> VoidEffect { get; }

    /// <summary>battle, target, source, move</summary>
    public Action<Battle, Pokemon, Pokemon, Move> VoidMove { get; }

    /// <summary>battle, relayVar, source, target, effect</summary>
    /// <returns>The modified value, or null if no modification should occur</returns>
    public Func<Battle, int, Pokemon, Pokemon, IEffect, int?> ModifierSourceEffect { get; }

    /// <summary>battle, relayVar, source, target, move</summary>
    /// <returns>The modified value, or null if no modification should occur</returns>
    public Func<Battle, int, Pokemon, Pokemon, Move, int?> ModifierSourceMove { get; }

    /// <summary>battle, source, target, move</summary>
    /// <returns>True/false for the result, or null if no override should occur</returns>
    public Func<Battle, Pokemon, Pokemon, Move, bool?> ResultSourceMove { get; }

    /// <summary>battle, source, target, move</summary>
    /// <returns>Boolean, integer, or null result depending on the specific interaction</returns>
    public Func<Battle, Pokemon, Pokemon, Move, IntBoolUnion?> ExtResultSourceMove { get; }

    /// <summary>battle, source, target, effect</summary>
    public Action<Battle, Pokemon, Pokemon, IEffect> VoidSourceEffect { get; }

    /// <summary>battle, source, target, move</summary>
    public Action<Battle, Pokemon, Pokemon, Move> VoidSourceMove { get; }
}