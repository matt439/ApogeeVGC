using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Actions;

public record BeforeTurnAction : IAction
{
    public ActionId Choice => ActionId.BeforeTurn;
    public IntFalseUnion Order => int.MaxValue;
    public int Priority => 0;
    public int Speed => 0;
    public int SubOrder => 0;
    public int EffectOrder => 0;
    public Pokemon? Pokemon => null;
}