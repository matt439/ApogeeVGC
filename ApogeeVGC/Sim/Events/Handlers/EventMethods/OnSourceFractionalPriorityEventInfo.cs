using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceFractionalPriority event.
/// Modifies move priority with fractional adjustment.
/// Signature: (Battle battle, int priority, Pokemon pokemon, ActiveMove move) => double
/// </summary>
public sealed record OnSourceFractionalPriorityEventInfo : UnionEventHandlerInfo<OnFractionalPriority>
{
    /// <summary>
    /// Creates a new OnSourceFractionalPriority event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or double constant)</param>
  /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnSourceFractionalPriorityEventInfo(
      OnFractionalPriority unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.FractionalPriority;
        Prefix = EventPrefix.Source;
        UnionValue = unionValue;
        #pragma warning disable CS0618
        Handler = ExtractDelegate();
        #pragma warning restore CS0618
        Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(double);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceFractionalPriorityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.FractionalPriority;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceFractionalPriorityEventInfo Create(
        Func<Battle, int, Pokemon, ActiveMove, double> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceFractionalPriorityEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetIntRelayVar(),
                context.GetTargetOrSourcePokemon(),
                context.GetMove()
                );
                return new DecimalRelayVar((decimal)result);
            },
            priority,
            usesSpeed
        );
    }
}
