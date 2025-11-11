using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAllyLockMove event.
/// Signature: Func<Battle, Pokemon, ActiveMove?>
/// </summary>
public sealed record OnAllyLockMoveEventInfo : EventHandlerInfo
{
 public OnAllyLockMoveEventInfo(
      Func<Battle, Pokemon, ActiveMove?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.LockMove;
   Prefix = EventPrefix.Ally;
        Handler = handler;
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(ActiveMove);
    }
}