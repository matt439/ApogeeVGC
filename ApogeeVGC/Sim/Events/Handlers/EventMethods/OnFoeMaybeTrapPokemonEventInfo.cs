using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeMaybeTrapPokemon event.
/// Triggered to potentially trap a foe Pokemon.
/// Signature: (Battle battle, Pokemon pokemon, Pokemon? source) => void
/// </summary>
public sealed record OnFoeMaybeTrapPokemonEventInfo : EventHandlerInfo
{
    /// <summary>
  /// Creates a new OnFoeMaybeTrapPokemon event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeMaybeTrapPokemonEventInfo(
        Action<Battle, Pokemon, Pokemon?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.MaybeTrapPokemon;
   Prefix = EventPrefix.Foe;
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
