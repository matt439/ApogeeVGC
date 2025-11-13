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
}
