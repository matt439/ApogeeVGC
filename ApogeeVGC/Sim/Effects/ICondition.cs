using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Effects;

public interface ICondition : ISideEventMethods, IFieldEventMethods, IPokemonEventMethods, IEffect
{
    ConditionEffectType ConditionEffectType { get; }

    /// <summary>
    /// battle, target, source, effect -> number
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<BattleContext, Pokemon>? OnCopy { get; }

    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<BattleContext, Pokemon>? OnEnd { get; }

    /// <summary>
    /// battle, target, source, sourceEffect -> boolean | null
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, IEffect, bool?>? OnRestart { get; }

    /// <summary>
    /// battle, target, source, sourceEffect -> boolean | null
    /// </summary>
    Func<BattleContext, Pokemon, Pokemon, IEffect, bool?>? OnStart { get; }
}