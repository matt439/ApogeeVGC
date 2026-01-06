using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyType event.
/// Modifies the type of a move before it executes.
/// Signature: (Battle battle, ActiveMove move, Pokemon pokemon, Pokemon target) => void
/// </summary>
public sealed record OnModifyTypeEventInfo : EventHandlerInfo
{
    /// <summary>
  /// Creates a new OnModifyType event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyTypeEventInfo(
  Action<Battle, ActiveMove, Pokemon, Pokemon> handler,
  int? priority = null,
      bool usesSpeed = true)
    {
   Id = EventId.ModifyType;
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
