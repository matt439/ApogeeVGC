using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

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

    public ActionOrder Order
    {
        get;
        init
        {
            if (value is ActionOrder.S3 or ActionOrder.S5 or ActionOrder.S200 or ActionOrder.S201 or
                ActionOrder.S199 or ActionOrder.S106)
            {
                field = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(Order), "Invalid ActionOrder for MoveAction.");
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