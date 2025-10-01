using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public interface IAbilityEventMethods
{
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<IBattle, Pokemon>? OnCheckShow { get; }

    /// <summary>
    /// battle, target
    /// </summary>
    Action<IBattle, PokemonSideFieldUnion>? OnEnd { get; }

    /// <summary>
    /// battle, target
    /// </summary>
    Action<IBattle, Pokemon>? OnStart { get; }
}