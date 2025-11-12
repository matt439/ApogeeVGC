using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyAttract event (pokemon/ally-specific).
/// Triggered when ally is attracted.
/// Signature: Action<Battle, Pokemon, Pokemon>
/// </summary>
public sealed record OnAllyAttractEventInfo : EventHandlerInfo
{
    public OnAllyAttractEventInfo(
    Action<Battle, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Attract;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
  }
}