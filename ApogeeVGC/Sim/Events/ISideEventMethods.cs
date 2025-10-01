using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events;

public interface ISideEventMethods : IEventMethods
{
    /// <summary>
    /// battle, target, source, sourceEffect
    /// </summary>
    Action<IBattle, Side, Pokemon, IEffect>? OnSideStart { get; }

    /// <summary>
    /// battle, target, source, sourceEffect
    /// </summary>
    Action<IBattle, Side, Pokemon, IEffect>? OnSideRestart { get; }

    /// <summary>
    /// battle, target, source, effect
    /// </summary>
    Action<IBattle, Side, Pokemon, IEffect>? OnSideResidual { get; }

    /// <summary>
    /// battle, target
    /// </summary>
    Action<IBattle, Side>? OnSideEnd { get; }
    int? OnSideResidualOrder { get; }
    int? OnSideResidualPriority { get; }
    int? OnSideResidualSubOrder { get; }
}