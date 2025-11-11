using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAfterMoveSelf event.
/// Triggered after a move is executed by the user on itself.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => void
/// </summary>
public sealed record OnAfterMoveSelfEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAfterMoveSelf event handler.
    /// </summary>
 /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAfterMoveSelfEventInfo(
    Action<Battle, Pokemon, Pokemon, ActiveMove> handler,
      int? priority = null,
   bool usesSpeed = true)
    {
        Id = EventId.AfterMoveSelf;
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
