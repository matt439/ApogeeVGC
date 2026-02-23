using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

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
                    if (i is 3 or 5 or 200 or 201 or 199 or 106 or 107)
                    {
                        field = i;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(value),
                            "Order must be one of the predefined values.");
                    }
                    break;
                case FalseIntFalseUnion:
                    throw new ArgumentException("Order cannot be of type FalseIntFalseUnion.", nameof(value));
                default:
                    throw new ArgumentException("Order must be of type FalseIntFalseUnion or IntIntFalseUnion.",
                        nameof(value));
            }
        }
    }
    public int Priority { get; set; }
    public int FractionalPriority { get; init; }
    public int Speed { get; init; }
    public required Pokemon Pokemon { get; init; }
    public int TargetLoc { get; set; }
    public Pokemon? OriginalTarget { get; init; }
    public MoveId MoveId { get; init; }
    public required Move Move { get; set; }
    public IEffect? SourceEffect { get; init; }
    public MoveType? Terastallize { get; init; }

    /// <summary>
    /// Cached <see cref="ActiveMove"/> derived from <see cref="Move"/>.
    /// Lazily created on first access and shared across record copies produced by <c>with</c>.
    /// Used by <see cref="Battle.GetActionSpeed"/> and <see cref="BattleQueue.InsertChoice"/>
    /// to avoid repeated heap allocations for priority / fractional-priority event resolution.
    /// </summary>
    private ActiveMove? _cachedActiveMove;

    /// <summary>
    /// Returns a cached <see cref="ActiveMove"/> for this action's <see cref="Move"/>.
    /// The instance is created once and reused for transient event resolution (priority, fractional priority).
    /// </summary>
    public ActiveMove GetOrCreateActiveMove()
    {
        return _cachedActiveMove ??= Move.ToActiveMove();
    }

    // To satisfy IPriorityComparison
    public int SubOrder => 0;
    public int EffectOrder => 0;
}