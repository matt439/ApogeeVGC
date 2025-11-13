using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnDragOut event.
/// Triggered when a Pokemon is forcibly switched out.
/// Signature: (Battle battle, Pokemon pokemon, Pokemon? source, ActiveMove? move) => void
/// </summary>
public sealed record OnDragOutEventInfo : EventHandlerInfo
{
  /// <summary>
    /// Creates a new OnDragOut event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnDragOutEventInfo(
 Action<Battle, Pokemon, Pokemon?, ActiveMove?> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
   Id = EventId.DragOut;
   Handler = handler;
        Priority = priority;
  UsesSpeed = usesSpeed;
  ExpectedParameterTypes =
      [
  typeof(Battle),
         typeof(Pokemon),
typeof(Pokemon),
         typeof(ActiveMove),
    ];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
