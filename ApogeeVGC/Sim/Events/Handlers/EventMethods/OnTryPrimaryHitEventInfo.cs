using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryPrimaryHit event.
/// Triggered when attempting a primary hit.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => IntBoolVoidUnion?
/// </summary>
public sealed record OnTryPrimaryHitEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnTryPrimaryHit event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
/// <param name="priority">Execution priority (higher executes first)</param>
  /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTryPrimaryHitEventInfo(
    Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler,
        int? priority = null,
      bool usesSpeed = true)
    {
        Id = EventId.TryPrimaryHit;
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
        ExpectedReturnType = typeof(IntBoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
