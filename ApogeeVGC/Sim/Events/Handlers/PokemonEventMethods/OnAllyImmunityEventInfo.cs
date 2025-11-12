using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyImmunity event (pokemon/ally-specific).
/// Triggered for ally immunity check.
/// Signature: Action<Battle, PokemonType, Pokemon>
/// </summary>
public sealed record OnAllyImmunityEventInfo : EventHandlerInfo
{
    public OnAllyImmunityEventInfo(
    Action<Battle, PokemonType, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Immunity;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(PokemonType), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
  }
}