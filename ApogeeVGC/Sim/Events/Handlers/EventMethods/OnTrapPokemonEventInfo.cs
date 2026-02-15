using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTrapPokemon event.
/// Triggered when a Pokemon is trapped.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnTrapPokemonEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnTrapPokemon event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTrapPokemonEventInfo(
   Action<Battle, Pokemon> handler,
    int? priority = null,
    bool usesSpeed = true)
    {
   Id = EventId.TrapPokemon;
Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
  ExpectedParameterTypes =
[
    typeof(Battle),
    typeof(Pokemon),
];
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
    public OnTrapPokemonEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TrapPokemon;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnTrapPokemonEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnTrapPokemonEventInfo(
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
