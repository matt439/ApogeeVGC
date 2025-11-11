using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

public sealed record OnAllySwitchOutEventInfo : EventHandlerInfo
{
    public OnAllySwitchOutEventInfo(
  Action<Battle, Pokemon> handler,
 int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.SwitchOut;
        Prefix = EventPrefix.Ally;
        Handler = handler;
   Priority = priority;
   UsesSpeed = usesSpeed;
  ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
   ExpectedReturnType = typeof(void);
    }
}
