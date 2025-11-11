using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyBeforeSwitchOut event.
/// Signature: Action<Battle, Pokemon>
/// </summary>
public sealed record OnAnyBeforeSwitchOutEventInfo : EventHandlerInfo
{
    public OnAnyBeforeSwitchOutEventInfo(
        Action<Battle, Pokemon> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.BeforeSwitchOut;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
    }
}