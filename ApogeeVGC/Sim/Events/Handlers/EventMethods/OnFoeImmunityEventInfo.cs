using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeImmunity event.
/// Signature: Action<Battle, PokemonType, Pokemon>
/// </summary>
public sealed record OnFoeImmunityEventInfo : EventHandlerInfo
{
 [Obsolete("Use Create factory method instead.")]
 public OnFoeImmunityEventInfo(
      Action<Battle, PokemonType, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.Immunity;
   Prefix = EventPrefix.Foe;
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
    public OnFoeImmunityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Immunity;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeImmunityEventInfo Create(
        Action<Battle, PokemonType, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeImmunityEventInfo(
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
