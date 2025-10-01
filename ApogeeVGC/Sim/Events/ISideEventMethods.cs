using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events;

public interface ISideEventMethods : IEventMethods
{
    Action<BattleContext, Side, Pokemon, IEffect>? OnSideStart { get; }
    Action<BattleContext, Side, Pokemon, IEffect>? OnSideRestart { get; }
    Action<BattleContext, Side, Pokemon, IEffect>? OnSideResidual { get; }
    Action<BattleContext, Side>? OnSideEnd { get; }
    int? OnSideResidualOrder { get; }
    int? OnSideResidualPriority { get; }
    int? OnSideResidualSubOrder { get; }
}