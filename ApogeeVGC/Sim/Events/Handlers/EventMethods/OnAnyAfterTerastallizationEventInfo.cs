using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

public sealed record OnAnyAfterTerastallizationEventInfo : EventHandlerInfo
{
    public OnAnyAfterTerastallizationEventInfo(
   Action<Battle, Pokemon> handler,
   int? priority = null,
bool usesSpeed = true)
    {
 Id = EventId.AfterTerastallization;
        Prefix = EventPrefix.Any;
        Handler = handler;
  Priority = priority;
   UsesSpeed = usesSpeed;
   ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
    }
}
