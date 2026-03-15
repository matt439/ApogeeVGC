using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Actions;

public record RunSwitchAction : IAction
{
    public IntFalseUnion Order { get; init; } = IntFalseUnion.FromFalse();
    public double Priority { get; init; }
    public double Speed { get; init; }
    public int SubOrder { get; init; }
    public int EffectOrder { get; init; }
    public ActionId Choice => ActionId.RunSwitch;
    public Pokemon? Pokemon { get; init; }
}