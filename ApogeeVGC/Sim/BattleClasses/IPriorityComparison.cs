using ApogeeVGC.Sim.Actions;

namespace ApogeeVGC.Sim.BattleClasses;

public interface IPriorityComparison
{
    ActionOrder Order { get; }
    int Priority { get; }
    int Speed { get; }
    int SubOrder { get; }
    int EffectOrder { get; }
}