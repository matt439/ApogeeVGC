using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllySwitchOut event (pokemon/ally-specific).
/// Triggered when ally switches out.
/// Signature: Action<Battle, Pokemon>
/// </summary>
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