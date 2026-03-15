using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Actions;

public record StartGameAction : IAction
{
    public ActionId Choice => ActionId.Start;
    public IntFalseUnion Order => 2; // Showdown: orders.start = 2
    public double Priority => 0;
    public double Speed => 0;
    public int SubOrder => 0;
    public int EffectOrder => 0;
    public Pokemon? Pokemon => null;
}