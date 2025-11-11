using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAllyEmergencyExit event.
/// Signature: Action<Battle, Pokemon>
/// </summary>
public sealed record OnAllyEmergencyExitEventInfo : EventHandlerInfo
{
 public OnAllyEmergencyExitEventInfo(
      Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.EmergencyExit;
   Prefix = EventPrefix.Ally;
        Handler = handler;
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
    }
}