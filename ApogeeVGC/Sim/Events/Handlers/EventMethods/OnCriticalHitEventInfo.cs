using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnCriticalHit event.
/// Determines if a move should critically hit.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => BoolVoidUnion
/// </summary>
public sealed record OnCriticalHitEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnCriticalHit event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnCriticalHitEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
  int? priority = null,
  bool usesSpeed = true)
    {
    Id = EventId.CriticalHit;
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
  }
}
