using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyModifyAccuracy event.
/// Modifies accuracy for any move in battle.
/// Signature: (Battle battle, int relayVar, Pokemon target, Pokemon source, ActiveMove move) => DoubleVoidUnion
/// </summary>
public sealed record OnAnyModifyAccuracyEventInfo : EventHandlerInfo
{
    /// <summary>
/// Creates a new OnAnyModifyAccuracy event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAnyModifyAccuracyEventInfo(
Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
   Id = EventId.ModifyAccuracy;
  Prefix = EventPrefix.Any;
 Handler = handler;
   Priority = priority;
        UsesSpeed = usesSpeed;
  ExpectedParameterTypes =
        [
   typeof(Battle),
typeof(int),
   typeof(Pokemon),
            typeof(Pokemon),
  typeof(ActiveMove),
        ];
     ExpectedReturnType = typeof(DoubleVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
