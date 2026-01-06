using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

public record EventListener : EventListenerWithoutPriority, IPriorityComparison
{
    public IntFalseUnion Order { get; init; } = new IntIntFalseUnion(int.MaxValue);
    public int Priority { get; init; }
    public int Speed { get; init; }
    public int SubOrder { get; init; }
    public int EffectOrder { get; init; }
}