using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Actions;

public record ResidualAction : IActionChoice
{
    public static ActionId Choice => ActionId.Residual;
    public IntFalseUnion Order => int.MaxValue;
    public int Priority => 0;
    public int Speed => 0;
    public int SubOrder => 0;
    public int EffectOrder => 0;
}