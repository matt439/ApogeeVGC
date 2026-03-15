using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
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
        set
        {
            if (value.IsFalse)
                throw new ArgumentException("Order cannot be false.", nameof(value));

            int i = value.Value;
            if (i is 3 or 5 or 200 or 201 or 199 or 106 or 107)
            {
                field = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Order must be one of the predefined values.");
            }
        }
    }
    public double Priority { get; set; }
    public double FractionalPriority { get; set; }
    public double Speed { get; set; }
    public required Pokemon Pokemon { get; set; }
    public int TargetLoc { get; set; }
    public Pokemon? OriginalTarget { get; set; }
    public MoveId MoveId { get; init; }
    public required Move Move { get; set; }
    public IEffect? SourceEffect { get; set; }
    public MoveType? Terastallize { get; init; }
    public SpecieId? Mega { get; init; }

    /// <summary>
    /// Cached <see cref="ActiveMove"/> derived from <see cref="Move"/>.
    /// Lazily created on first access and shared across record copies produced by <c>with</c>.
    /// Used by <see cref="Battle.GetActionSpeed"/> and <see cref="BattleQueue.InsertChoice"/>
    /// to avoid repeated heap allocations for priority / fractional-priority event resolution.
    /// </summary>
    /// <remarks>
    /// Must use <see cref="Move.ToActiveMove"/> (clone) instead of <see cref="Move.AsActiveMove"/>
    /// (shared template) because the returned instance is mutated during battle execution
    /// (e.g., MoveHitData, Priority, PranksterBoosted). Using the shared template caused
    /// concurrent dictionary corruption when parallel battles used the same move.
    /// </remarks>
    private ActiveMove? _cachedActiveMove;

    /// <summary>
    /// Returns a cached <see cref="ActiveMove"/> for this action's <see cref="Move"/>.
    /// The instance is created once and reused for transient event resolution (priority, fractional priority).
    /// </summary>
    public ActiveMove GetOrCreateActiveMove()
    {
        return _cachedActiveMove ??= Move.ToActiveMove();
    }

    /// <summary>
    /// Clears the cached ActiveMove so the next <see cref="GetOrCreateActiveMove"/> call
    /// creates a fresh instance. Must be called after copying via <c>with</c> to avoid
    /// sharing mutable ActiveMove state (e.g. MoveHitData) between battle copies.
    /// </summary>
    internal void ClearCachedActiveMove() => _cachedActiveMove = null;

    // To satisfy IPriorityComparison
    public int SubOrder => 0;
    public int EffectOrder => 0;
}