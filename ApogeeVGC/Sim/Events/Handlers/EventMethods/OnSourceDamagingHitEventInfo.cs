using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceDamagingHit event.
/// Triggered after dealing a damaging hit as the source.
/// Signature: (Battle battle, int damage, Pokemon target, Pokemon source, ActiveMove move) => void
/// </summary>
public sealed record OnSourceDamagingHitEventInfo : EventHandlerInfo
{
    /// <summary>
  /// Creates a new OnSourceDamagingHit event handler.
    /// </summary>
  /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSourceDamagingHitEventInfo(
 Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DamagingHit;
 Prefix = EventPrefix.Source;
  Handler = handler;
        Priority = priority;
 UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
  [
            typeof(Battle),
       typeof(int),
      typeof(Pokemon),
        typeof(Pokemon),
  typeof(ActiveMove),
  ];
  ExpectedReturnType = typeof(void);
 }
}
