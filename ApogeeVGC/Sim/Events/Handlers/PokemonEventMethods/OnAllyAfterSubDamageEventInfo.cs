using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyAfterSubDamage event (pokemon/ally-specific).
/// Triggered after substitute damage to ally.
/// Signature: Action<Battle, int, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnAllyAfterSubDamageEventInfo : EventHandlerInfo
{
    public OnAllyAfterSubDamageEventInfo(
    Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterSubDamage;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
  }
}