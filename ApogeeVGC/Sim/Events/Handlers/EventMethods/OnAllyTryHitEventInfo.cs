using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

public sealed record OnAllyTryHitEventInfo : EventHandlerInfo
{
  public OnAllyTryHitEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?> handler,
        int? priority = null,
      bool usesSpeed = true)
    {
        Id = EventId.TryHit;
        Prefix = EventPrefix.Ally;
    Handler = handler;
        Priority = priority;
 UsesSpeed = usesSpeed;
   ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
   ExpectedReturnType = typeof(BoolIntEmptyVoidUnion);
    }
}
