using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAfterMega event.
/// Triggered after a Pokemon Mega Evolves.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnAfterMegaEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAfterMega event handler.
    /// </summary>
 /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAfterMegaEventInfo(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterMega;
        Handler = handler;
     Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
      typeof(Battle),
typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(void);
    }
}
