using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceTrapPokemon event.
/// Signature: Action<Battle, Pokemon>
/// </summary>
public sealed record OnSourceTrapPokemonEventInfo : EventHandlerInfo
{
    public OnSourceTrapPokemonEventInfo(
        Action<Battle, Pokemon> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.TrapPokemon;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
    }
}