using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Actions;

public record MoveAction : IAction
{
    public ActionId Choice
    {
        get;
        init
        {
            if (value is ActionId.Move or ActionId.BeforeTurnMove or ActionId.PriorityChargeMove)
            {
                field = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(Choice), "Invalid ActionId for MoveAction.");
            }
        }
    }

    public required IntFalseUnion Order
    {
        get;
        init
        {
            switch (value)
            {
                case IntIntFalseUnion iifu:
                    int i = iifu.Value;
                    if (i is 3 or 5 or 200 or 201 or 199 or 106)
                    {
                        field = i;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(value),
                            "Order must be one of the predefined values.");
                    }
                    break;
                case FalseIntFalseUnion f:
                    throw new ArgumentException("Order cannot be of type FalseIntFalseUnion.", nameof(value));
                default:
                    throw new ArgumentException("Order must be of type FalseIntFalseUnion or IntIntFalseUnion.",
                        nameof(value));
            }
        }
    }
    public int Priority { get; init; }
    public int FractionalPriority { get; init; }
    public int Speed { get; init; }
    public required Pokemon Pokemon { get; init; }
    public int TargetLoc { get; init; }
    public required Pokemon OriginalTarget { get; init; }
    public MoveId MoveId { get; init; }
    public required Move Move { get; init; }
    public IEffect? SourceEffect { get; init; }


    // To satisfy IPriorityComparison
    public int SubOrder => 0;
    public int EffectOrder => 0;
}