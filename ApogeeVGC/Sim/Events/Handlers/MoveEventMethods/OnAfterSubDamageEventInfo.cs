using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnAfterSubDamage event (move-specific).
/// Triggered after substitute damage.
/// Signature: Action<Battle, int, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnAfterSubDamageEventInfo : EventHandlerInfo
{
public OnAfterSubDamageEventInfo(
        Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
    Id = EventId.AfterSubDamage;
        Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
      UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
    }
}