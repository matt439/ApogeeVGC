using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyMove event.
/// Modifies properties of a move before it executes.
/// Signature: (Battle battle, ActiveMove move, Pokemon pokemon, Pokemon? target) => void
/// </summary>
public sealed record OnModifyMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifyMove event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyMoveEventInfo(
        Action<Battle, ActiveMove, Pokemon, Pokemon?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyMove;
 Handler = handler;
        Priority = priority;
  UsesSpeed = usesSpeed;
      ExpectedParameterTypes =
   [
   typeof(Battle),
   typeof(ActiveMove),
 typeof(Pokemon),
     typeof(Pokemon),
   ];
  ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
 }
}
