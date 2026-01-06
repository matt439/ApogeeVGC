using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnBeforeTurn event.
/// Triggered before a turn begins.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnBeforeTurnEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnBeforeTurn event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
/// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnBeforeTurnEventInfo(
    Action<Battle, Pokemon> handler,
   int? priority = null,
 bool usesSpeed = true)
    {
        Id = EventId.BeforeTurn;
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
