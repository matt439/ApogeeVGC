using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Unions;

namespace ApogeeVGC.Sim.Methods;

public interface ITestMethods
{
    /// <summary>battle, numFainted, target, source, sourceEffect</summary>
    public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; }

    /// <summary>battle, target, source, effect</summary>
    /// <returns>Whether it succeeded</returns>
    public Func<Battle, Pokemon, Pokemon, IEffect, bool>? OnStart { get; }

    /// <summary>battle, relayVar, target, source, move</summary>
    /// <returns>The modified priority</returns>
    public Func<Battle, int, Pokemon, Pokemon, Move, int>? OnModifyPriority { get; }

    /// <summary>battle, target, source, sourceEffect</summary>
    public Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; }

    /// <summary>battle, target, source, effect</summary>
    public Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get; }
    public int? OnResidualOrder { get; }
    public int? OnResidualSubOrder { get; }

    /// <summary>battle, source, target, move</summary>
    public Action<Battle, Pokemon, Pokemon, Move>? OnBeforeMove { get; }
    public int? OnBeforeMovePriority { get; }

    /// <summary>battle, target, source, effect</summary>
    public Action<Battle, Pokemon>? OnDisableMove { get; }

    /// <summary>battle, pokemon</summary>
    /// <returns>The success</returns>
    public Func<Battle, Pokemon, bool>? OnStallMove { get; }

    /// <summary>battle, target, source, sourceEffect</summary>
    /// <returns>The success</returns>
    public Func<Battle, Pokemon, Pokemon, IEffect, bool>? OnRestart { get; }

    /// <summary>battle, source, target, move</summary>
    /// <returns>A bool or int</returns>
    public Func<Battle, Pokemon, Pokemon, Move, BoolIntBoolUnion>? OnTryHit { get; }
    public int? OnTryHitPriority { get; }

    /// <summary>battle, relayVar source, target, move</summary>
    /// <returns>A multiplier for the damage</returns>
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnAnyModifyDamage { get; }

    /// <summary>battle, damage, target, source, move</summary>
    public Action<Battle, int, Pokemon, Pokemon, Move>? OnDamagingHit { get; }
    public int? OnDamagingHitOrder { get; }

    /// <summary>battle, move, pokemon, target</summary>
    public Action<Battle, Move, Pokemon, Pokemon?>? OnModifyMove { get; }
    public int? OnModifyMovePriority { get; }

    /// <summary>battle, target, source, move</summary>
    /// <returns>True if immune</returns>
    public Func<Battle, Pokemon, Pokemon, Move, bool>? OnTryImmunity { get; }

    /// <summary>battle, target, source, move</summary>
    public Func<Battle, Pokemon, Pokemon, Move, bool>? OnHit { get; }

    /// <summary>battle, relayVar, source, target, move</summary>
    /// <returns>The base power multiplier</returns>
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnBasePower { get; }
    public int? OnBasePowerPriority { get; }

    /// <summary>battle, source, target, move</summary>
    public Func<Battle, Pokemon, Pokemon, Move, bool>? OnTry { get; }

    /// <summary>battle, pokemon, target, move</summary>
    /// <returns>An integer if successful, false otherwise</returns>
    public Func<Battle, Pokemon, Pokemon, Move, IntFalseUnion>? BasePowerCallback { get; }
    public int? BasePowerCallbackPriority { get; }

    /// <summary>battle, target, source, move</summary>
    /// <returns>Success</returns>
    public Func<Battle, Pokemon, Pokemon, Move, bool>? OnPrepareHit { get; }

    /// <summary>battle, relayVar, target, source, move</summary>
    /// <returns>A multiplier for the attack stat</returns>
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnModifyAtk { get; }
    public int? OnModifyAtkPriority { get; }

    /// <summary>battle, relayVar, target, source, move</summary>
    /// <returns>A multiplier for the defence stat</returns>
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnModifyDef { get; }
    public int? OnModifyDefPriority { get; }

    /// <summary>battle, relayVar, target, source, move</summary>
    /// <returns>A multiplier for the special attack stat</returns>
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnModifySpA { get; }
    public int? OnModifySpAPriority { get; }

    /// <summary>battle, relayVar, target, source, move</summary>
    /// <returns>A multiplier for the special defence stat</returns>
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnModifySpD { get; }
    public int? OnModifySpDPriority { get; }

    /// <summary>battle, speed, pokemon</summary>
    /// <returns>A multiplier for the speed stat</returns>
    public Func<Battle, int, Pokemon, double>? OnModifySpe { get; }
    public int? OnModifySpePriority { get; }
}