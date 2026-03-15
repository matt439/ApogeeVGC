using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Actions;

public record ResidualAction : IAction
{
    public ActionId Choice => ActionId.Residual;
    public IntFalseUnion Order => 300; // Showdown: orders.residual = 300
    public double Priority => 0;
    public double Speed => 0;
    public int SubOrder => 0;
    public int EffectOrder => 0;
    public Pokemon? Pokemon => null;
}