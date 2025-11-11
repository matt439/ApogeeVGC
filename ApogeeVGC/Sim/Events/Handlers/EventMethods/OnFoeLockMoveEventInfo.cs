using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeLockMove event.
/// Signature: Func<Battle, Pokemon, ActiveMove?>
/// </summary>
public sealed record OnFoeLockMoveEventInfo : EventHandlerInfo
{
 public OnFoeLockMoveEventInfo(
      Func<Battle, Pokemon, ActiveMove?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.LockMove;
   Prefix = EventPrefix.Foe;
        Handler = handler;
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(ActiveMove);
    }
}