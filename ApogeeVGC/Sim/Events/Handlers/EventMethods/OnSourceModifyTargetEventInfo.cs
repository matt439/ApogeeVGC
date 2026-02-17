using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceModifyTarget event.
/// Signature: Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnSourceModifyTargetEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnSourceModifyTargetEventInfo(
        Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.ModifyTarget;
        Prefix = EventPrefix.Source;
 #pragma warning disable CS0618
 Handler = handler;
 #pragma warning restore CS0618
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceModifyTargetEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyTarget;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceModifyTargetEventInfo Create(
        Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceModifyTargetEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetTargetOrSourcePokemon(),
                context.GetMove()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
