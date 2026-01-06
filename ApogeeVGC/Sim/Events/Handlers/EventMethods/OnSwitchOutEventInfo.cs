using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSwitchOut event.
/// Triggered when a Pokemon switches out.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnSwitchOutEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSwitchOut event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
 /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSwitchOutEventInfo(
 Action<Battle, Pokemon> handler,
  int? priority = null,
  bool usesSpeed = true)
    {
        Id = EventId.SwitchOut;
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
}
