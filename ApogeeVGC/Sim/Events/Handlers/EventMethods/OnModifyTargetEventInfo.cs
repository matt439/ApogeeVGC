using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyTarget event.
/// Modifies the target of a move.
/// Signature: (Battle battle, Pokemon relayTarget, Pokemon pokemon, Pokemon target, ActiveMove move) => void
/// </summary>
public sealed record OnModifyTargetEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifyTarget event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyTargetEventInfo(
        Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove> handler,
  int? priority = null,
bool usesSpeed = true)
    {
      Id = EventId.ModifyTarget;
  Handler = handler;
        Priority = priority;
      UsesSpeed = usesSpeed;
ExpectedParameterTypes =
 [
        typeof(Battle),
            typeof(Pokemon),
      typeof(Pokemon),
    typeof(Pokemon),
     typeof(ActiveMove),
        ];
     ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
 }
}
