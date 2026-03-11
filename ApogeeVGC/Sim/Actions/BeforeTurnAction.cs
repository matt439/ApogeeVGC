using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Actions;

public record BeforeTurnAction : IAction
{
    public ActionId Choice => ActionId.BeforeTurn;
    public IntFalseUnion Order => 4; // Showdown: orders.beforeTurn = 4
    public double Priority => 0;
    public int Speed => 0;
    public int SubOrder => 0;
    public int EffectOrder => 0;
    public Pokemon? Pokemon => null;
}