using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifySpA event.
/// Modifies the Special Attack stat.
/// Signature: (Battle battle, int relayVar, Pokemon source, Pokemon target, ActiveMove move) => DoubleVoidUnion
/// </summary>
public sealed record OnModifySpAEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifySpA event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifySpAEventInfo(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
     int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.ModifySpA;
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
