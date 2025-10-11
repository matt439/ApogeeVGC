using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Actions;

public interface IAction : IActionChoice
{
    ActionId Choice { get; }
    //int Priority { get; }
    //int Speed { get; }
    Pokemon? Pokemon { get; }
}