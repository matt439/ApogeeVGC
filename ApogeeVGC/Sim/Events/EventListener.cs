using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

public record EventListener : EventListenerWithoutPriority, IPriorityComparison
{
    public IntFalseUnion Order
    {
        get;
        init
        {
            switch (value)
            {
                case FalseIntFalseUnion f:
                    field = f;
                    break;
                case IntIntFalseUnion iifu:
                    int i = iifu.Value;
                    if (i is 1 or 2 or 3 or 4 or 5 or 6 or 7 or 8 or 9 or 10 or 26 or 27 or 28 or 103 or 106 or 199 or 200 or 201 or int.MaxValue)
                    {
                        field = i;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(value),
                            "Order must be one of the predefined values.");
                    }
                    break;
                default:
                    throw new ArgumentException("Order must be of type FalseIntFalseUnion or IntIntFalseUnion.",
                        nameof(value));
            }
        }
    } = new IntIntFalseUnion(int.MaxValue);
    public int Priority { get; init; }
    public int Speed { get; init; }
    public int SubOrder { get; init; }
    public int EffectOrder { get; init; }
}