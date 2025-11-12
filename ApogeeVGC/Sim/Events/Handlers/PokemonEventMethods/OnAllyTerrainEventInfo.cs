using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyTerrain event (pokemon/ally-specific).
/// Triggered for terrain affecting ally.
/// Signature: Action<Battle, Pokemon>
/// </summary>
public sealed record OnAllyTerrainEventInfo : EventHandlerInfo
{
    public OnAllyTerrainEventInfo(
    Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Terrain;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
  }
}