using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

public class EventListener : EventListenerWithoutPriority, IPriorityComparison
{
    public IntFalseUnion Order { get; set; } = int.MaxValue;
    public double Priority { get; set; }
    public int Speed { get; set; }
    public int SubOrder { get; set; }
    public int EffectOrder { get; set; }
}