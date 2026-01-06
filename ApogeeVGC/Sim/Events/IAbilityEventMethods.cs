using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

public interface IAbilityEventMethods
{
    /// <summary>
    /// battle, pokemon
    /// </summary>
    Action<Battle, Pokemon>? OnCheckShow { get; }

    /// <summary>
    /// battle, target
    /// </summary>
    Action<Battle, PokemonSideFieldUnion>? OnEnd { get; }

    /// <summary>
    /// battle, target
    /// </summary>
    Action<Battle, Pokemon>? OnStart { get; }
}