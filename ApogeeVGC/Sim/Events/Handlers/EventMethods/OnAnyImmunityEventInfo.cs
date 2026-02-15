using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyImmunity event.
/// Signature: Action<Battle, PokemonType, Pokemon>
/// </summary>
public sealed record OnAnyImmunityEventInfo : EventHandlerInfo
{
 public OnAnyImmunityEventInfo(
      Action<Battle, PokemonType, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.Immunity;
   Prefix = EventPrefix.Any;
        Handler = handler;
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
    public OnAnyImmunityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Immunity;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyImmunityEventInfo Create(
        Action<Battle, PokemonType, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyImmunityEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.SourceType!.Value,
                context.GetSourcePokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}