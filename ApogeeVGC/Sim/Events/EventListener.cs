using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.Sim.Events;

public record EventListener : EventListenerWithoutPriority, IPriorityComparison
{
    public ActionOrder Order { get; init; } = ActionOrder.Max;
    public int Priority { get; init; }
    public int Speed { get; init; }
    public int SubOrder { get; init; }
    public int EffectOrder { get; init; }
}