using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;

/// <summary>
/// Event handler info for OnStart event (ability-specific).
/// Triggered when an ability effect starts/activates.
/// Signature: Action&lt;Battle, Pokemon&gt;
/// </summary>
public sealed record OnStartEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnStart event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnStartEventInfo(
        Action<Battle, Pokemon> handler,
     int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.Start;
     Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
 ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
    ExpectedReturnType = typeof(void);
    }
}
