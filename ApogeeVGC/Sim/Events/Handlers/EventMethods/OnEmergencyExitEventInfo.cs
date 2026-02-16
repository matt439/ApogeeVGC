using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnEmergencyExit event.
/// Triggered when a Pokemon's HP falls below a certain threshold.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnEmergencyExitEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnEmergencyExit event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnEmergencyExitEventInfo(
        Action<Battle, Pokemon> handler,
        bool usesSpeed = true)
  {
   Id = EventId.EmergencyExit;
        Handler = handler;
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
    public OnEmergencyExitEventInfo(
        EventHandlerDelegate contextHandler,
        bool usesSpeed = true)
    {
        Id = EventId.EmergencyExit;
        ContextHandler = contextHandler;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnEmergencyExitEventInfo Create(
        Action<Battle, Pokemon> handler,
        bool usesSpeed = true)
    {
        return new OnEmergencyExitEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon()
                );
                return null;
            },
            usesSpeed
        );
    }
}
