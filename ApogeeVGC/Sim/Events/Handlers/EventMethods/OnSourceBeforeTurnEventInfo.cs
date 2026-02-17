using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceBeforeTurn event.
/// Signature: Action<Battle, Pokemon>
/// </summary>
public sealed record OnSourceBeforeTurnEventInfo : EventHandlerInfo
{
 [Obsolete("Use Create factory method instead.")]
 public OnSourceBeforeTurnEventInfo(
      Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.BeforeTurn;
   Prefix = EventPrefix.Source;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceBeforeTurnEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeTurn;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceBeforeTurnEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceBeforeTurnEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
