using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyMoveAborted event (pokemon/ally-specific).
/// Triggered when ally's move is aborted.
/// Signature: Action<Battle, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnAllyMoveAbortedEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnAllyMoveAbortedEventInfo(
    Action<Battle, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.MoveAborted;
  Prefix = EventPrefix.Ally;
  #pragma warning disable CS0618
  Handler = handler;
  #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
  }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyMoveAbortedEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.MoveAborted;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyMoveAbortedEventInfo Create(
        Action<Battle, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyMoveAbortedEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetMove()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
