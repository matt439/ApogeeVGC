using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnEntryHazard event.
/// Triggered when entry hazards affect a Pokemon.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnEntryHazardEventInfo : EventHandlerInfo
{
  /// <summary>
  /// Creates a new OnEntryHazard event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
  /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
  public OnEntryHazardEventInfo(
 Action<Battle, Pokemon> handler,
        int? priority = null,
  bool usesSpeed = true)
  {
      Id = EventId.EntryHazard;
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
