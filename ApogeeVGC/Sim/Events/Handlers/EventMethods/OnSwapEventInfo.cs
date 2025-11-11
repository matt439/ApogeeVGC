using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSwap event.
/// Triggered when two Pokemon swap positions.
/// Signature: (Battle battle, Pokemon target, Pokemon source) => void
/// </summary>
public sealed record OnSwapEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSwap event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSwapEventInfo(
        Action<Battle, Pokemon, Pokemon> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
 Id = EventId.Swap;
Handler = handler;
Priority = priority;
UsesSpeed = usesSpeed;
     ExpectedParameterTypes =
 [
     typeof(Battle),
            typeof(Pokemon),
  typeof(Pokemon),
];
 ExpectedReturnType = typeof(void);
    }
}
