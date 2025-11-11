using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers;

/// <summary>
/// Event handler info for OnDamagingHit event.
/// Triggered when a Pokemon is hit by a damaging move.
/// Signature: (Battle battle, int damage, Pokemon target, Pokemon source, ActiveMove move) => void
/// </summary>
public sealed record OnDamagingHitEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnDamagingHit event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
  /// <param name="order">Execution order (lower executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
  public OnDamagingHitEventInfo(
     Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
        int? order = null,
        bool usesSpeed = true)
    {
        Id = EventId.DamagingHit;
   Handler = handler;
        Order = order;
 UsesSpeed = usesSpeed;
        ExpectedParameterTypes = new[] 
        { 
            typeof(Battle), 
            typeof(int), 
     typeof(Pokemon), 
            typeof(Pokemon), 
            typeof(ActiveMove) 
 };
        ExpectedReturnType = typeof(void);
    }
}
