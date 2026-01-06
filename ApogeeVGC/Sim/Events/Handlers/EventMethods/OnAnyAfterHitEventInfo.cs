using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyAfterHit event.
/// Triggered after any move hits in battle.
/// Signature: (Battle battle, Pokemon source, Pokemon target, ActiveMove move) => BoolVoidUnion
/// </summary>
public sealed record OnAnyAfterHitEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAnyAfterHit event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAnyAfterHitEventInfo(
   Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
 int? priority = null,
        bool usesSpeed = true)
  {
      Id = EventId.AfterHit;
     Prefix = EventPrefix.Any;
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
