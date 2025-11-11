using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

public sealed record OnAnyTryMoveEventInfo : EventHandlerInfo
{
    public OnAnyTryMoveEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
bool usesSpeed = true)
    {
   Id = EventId.TryMove;
        Prefix = EventPrefix.Any;
        Handler = handler;
        Priority = priority;
  UsesSpeed = usesSpeed;
 ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
      ExpectedReturnType = typeof(BoolEmptyVoidUnion);
    }
}
