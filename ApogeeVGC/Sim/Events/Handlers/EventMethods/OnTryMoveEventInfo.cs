using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryMove event.
/// Triggered when attempting to use a move.
/// Signature: (Battle battle, Pokemon source, Pokemon target, ActiveMove move) => BoolEmptyVoidUnion?
/// </summary>
public sealed record OnTryMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnTryMove event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
 /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
  public OnTryMoveEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
      Id = EventId.TryMove;
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
        ExpectedReturnType = typeof(BoolEmptyVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
