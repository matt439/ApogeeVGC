using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnMoveAborted event.
/// Triggered when a move is aborted.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => void
/// </summary>
public sealed record OnMoveAbortedEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnMoveAborted event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnMoveAbortedEventInfo(
        Action<Battle, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
      bool usesSpeed = true)
    {
        Id = EventId.MoveAborted;
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
        ExpectedReturnType = typeof(void);
    }
}
