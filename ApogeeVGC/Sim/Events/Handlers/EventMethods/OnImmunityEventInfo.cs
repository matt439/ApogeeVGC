using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnImmunity event.
/// Triggered to check type immunity.
/// Signature: (Battle battle, PokemonType type, Pokemon pokemon) => void
/// </summary>
public sealed record OnImmunityEventInfo : EventHandlerInfo
{
  /// <summary>
    /// Creates a new OnImmunity event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
  public OnImmunityEventInfo(
        Action<Battle, PokemonType, Pokemon> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
        Id = EventId.Immunity;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
     [
    typeof(Battle),
            typeof(PokemonType),
            typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(void);
    }
}
