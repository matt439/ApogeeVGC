using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events;

public interface ISideEventMethods : IEventMethods
{
    /// <summary>
    /// battle, target, source, sourceEffect
    /// </summary>
    Action<BattleContext, Side, Pokemon, IEffect>? OnSideStart { get; }

    /// <summary>
    /// battle, target, source, sourceEffect
    /// </summary>
    Action<BattleContext, Side, Pokemon, IEffect>? OnSideRestart { get; }

    /// <summary>
    /// battle, target, source, effect
    /// </summary>
    Action<BattleContext, Side, Pokemon, IEffect>? OnSideResidual { get; }

    /// <summary>
    /// battle, target
    /// </summary>
    Action<BattleContext, Side>? OnSideEnd { get; }
    int? OnSideResidualOrder { get; }
    int? OnSideResidualPriority { get; }
    int? OnSideResidualSubOrder { get; }
}