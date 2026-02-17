using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceImmunity event.
/// Signature: Action<Battle, PokemonType, Pokemon>
/// </summary>
public sealed record OnSourceImmunityEventInfo : EventHandlerInfo
{
 [Obsolete("Use Create factory method instead.")]
 public OnSourceImmunityEventInfo(
      Action<Battle, PokemonType, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.Immunity;
   Prefix = EventPrefix.Source;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(PokemonType), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceImmunityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Immunity;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceImmunityEventInfo Create(
        Action<Battle, PokemonType, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceImmunityEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.SourceType!.Value,
                context.GetSourceOrTargetPokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
