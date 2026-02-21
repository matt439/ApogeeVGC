using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

public class EventListener : EventListenerWithoutPriority, IPriorityComparison
{
    public IntFalseUnion Order { get; set; } = new IntIntFalseUnion(int.MaxValue);
    public int Priority { get; set; }
    public int Speed { get; set; }
    public int SubOrder { get; set; }
    public int EffectOrder { get; set; }
}