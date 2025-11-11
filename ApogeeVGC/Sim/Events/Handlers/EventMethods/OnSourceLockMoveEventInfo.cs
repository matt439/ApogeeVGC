using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceLockMove event.
/// Signature: Func<Battle, Pokemon, ActiveMove?>
/// </summary>
public sealed record OnSourceLockMoveEventInfo : EventHandlerInfo
{
 public OnSourceLockMoveEventInfo(
      Func<Battle, Pokemon, ActiveMove?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.LockMove;
   Prefix = EventPrefix.Source;
        Handler = handler;
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(ActiveMove);
    }
}