using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyTerrain event.
/// Signature: Action<Battle, Pokemon>
/// </summary>
public sealed record OnAnyTerrainEventInfo : EventHandlerInfo
{
    public OnAnyTerrainEventInfo(
        Action<Battle, Pokemon> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.Terrain;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
    }
}