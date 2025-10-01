using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public interface IAbilityEventMethods
{
    Action<BattleContext, Pokemon>? OnCheckShow { get; }
    Action<BattleContext, PokemonSideFieldUnion>? OnEnd { get; }
    Action<BattleContext, Pokemon>? OnStart { get; }
}