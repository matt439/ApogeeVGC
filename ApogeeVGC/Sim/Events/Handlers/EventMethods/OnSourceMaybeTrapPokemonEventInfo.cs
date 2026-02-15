using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceMaybeTrapPokemon event.
/// Signature: Action<Battle, Pokemon>
/// </summary>
public sealed record OnSourceMaybeTrapPokemonEventInfo : EventHandlerInfo
{
    public OnSourceMaybeTrapPokemonEventInfo(
        Action<Battle, Pokemon> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.MaybeTrapPokemon;
        Prefix = EventPrefix.Source;
 Handler = handler;
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
    public OnSourceMaybeTrapPokemonEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.MaybeTrapPokemon;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceMaybeTrapPokemonEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceMaybeTrapPokemonEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetPokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}