using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyTrapPokemon event (pokemon/ally-specific).
/// Triggered when trapping ally.
/// Signature: Action<Battle, Pokemon>
/// </summary>
public sealed record OnAllyTrapPokemonEventInfo : EventHandlerInfo
{
    public OnAllyTrapPokemonEventInfo(
    Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TrapPokemon;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
  }
}