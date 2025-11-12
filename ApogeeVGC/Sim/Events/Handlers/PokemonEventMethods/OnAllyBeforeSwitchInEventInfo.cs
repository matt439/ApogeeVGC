using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyBeforeSwitchIn event (pokemon/ally-specific).
/// Triggered before ally switches in.
/// Signature: Action<Battle, Pokemon>
/// </summary>
public sealed record OnAllyBeforeSwitchInEventInfo : EventHandlerInfo
{
    public OnAllyBeforeSwitchInEventInfo(
    Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeSwitchIn;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
  }
}