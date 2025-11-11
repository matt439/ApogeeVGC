using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFractionalPriority event.
/// Modifies fractional priority for move ordering.
/// Signature: (Battle battle, int priority, Pokemon pokemon, ActiveMove move) => double
/// </summary>
public sealed record OnFractionalPriorityEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFractionalPriority event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFractionalPriorityEventInfo(
        Func<Battle, int, Pokemon, ActiveMove, double> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.FractionalPriority;
   Handler = handler;
        Priority = priority;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
[
      typeof(Battle),
          typeof(int),
  typeof(Pokemon),
   typeof(ActiveMove),
        ];
  ExpectedReturnType = typeof(double);
 }
}
