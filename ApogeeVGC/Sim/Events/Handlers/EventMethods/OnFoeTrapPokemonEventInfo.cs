using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeTrapPokemon event.
/// Triggered when a foe Pokemon is trapped.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnFoeTrapPokemonEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeTrapPokemon event handler.
  /// </summary>
  /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeTrapPokemonEventInfo(
   Action<Battle, Pokemon> handler,
 int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TrapPokemon;
        Prefix = EventPrefix.Foe;
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
