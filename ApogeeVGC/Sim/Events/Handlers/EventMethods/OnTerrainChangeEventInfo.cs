using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTerrainChange event.
/// Triggered when terrain changes.
/// Signature: (Battle battle, Pokemon target, Pokemon source, IEffect sourceEffect) => void
/// </summary>
public sealed record OnTerrainChangeEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnTerrainChange event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
 public OnTerrainChangeEventInfo(
   Action<Battle, Pokemon, Pokemon, IEffect> handler,
 int? priority = null,
   bool usesSpeed = true)
    {
Id = EventId.TerrainChange;
   Handler = handler;
        Priority = priority;
     UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
   [
      typeof(Battle),
     typeof(Pokemon),
        typeof(Pokemon),
  typeof(IEffect),
];
   ExpectedReturnType = typeof(void);
    }
}
