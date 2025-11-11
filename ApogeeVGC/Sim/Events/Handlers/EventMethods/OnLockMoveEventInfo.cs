using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnLockMove event.
/// Returns which move a Pokemon is locked into.
/// Signature: (Battle battle, Pokemon pokemon) => ActiveMove?
/// </summary>
public sealed record OnLockMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnLockMove event handler.
  /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnLockMoveEventInfo(
        Func<Battle, Pokemon, ActiveMove?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
      Id = EventId.LockMove;
     Handler = handler;
 Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
    typeof(Pokemon),
        ];
      ExpectedReturnType = typeof(ActiveMove);
    }
}
