using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyMaybeTrapPokemon event.
/// Signature: Action<Battle, Pokemon, Pokemon?>
/// </summary>
public sealed record OnAnyMaybeTrapPokemonEventInfo : EventHandlerInfo
{
    public OnAnyMaybeTrapPokemonEventInfo(
        Action<Battle, Pokemon, Pokemon?> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.MaybeTrapPokemon;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
    }
}