using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public interface IAbilityEventMethods
{
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<BattleContext, Pokemon>? OnCheckShow { get; }

    /// <summary>
    /// battle, target
    /// </summary>
    Action<BattleContext, PokemonSideFieldUnion>? OnEnd { get; }

    /// <summary>
    /// battle, target
    /// </summary>
    Action<BattleContext, Pokemon>? OnStart { get; }
}