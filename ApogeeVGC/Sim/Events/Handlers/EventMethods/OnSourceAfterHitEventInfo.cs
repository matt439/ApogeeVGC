using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceAfterHit event.
/// Triggered after this Pokemon hits with a move as the source.
/// Signature: (Battle battle, Pokemon source, Pokemon target, ActiveMove move) => BoolVoidUnion
/// </summary>
public sealed record OnSourceAfterHitEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSourceAfterHit event handler.
    /// </summary>
 /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSourceAfterHitEventInfo(
  Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
   int? priority = null,
    bool usesSpeed = true)
    {
        Id = EventId.AfterHit;
        Prefix = EventPrefix.Source;
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
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
