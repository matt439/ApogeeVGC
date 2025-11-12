using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Events.Handlers.SideEventMethods;

/// <summary>
/// Event handler info for OnSideRestart event (side-specific).
/// Triggered when a side condition restarts/reactivates.
/// Signature: Action&lt;Battle, Side, Pokemon, IEffect&gt;
/// </summary>
public sealed record OnSideRestartEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSideRestart event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSideRestartEventInfo(
    Action<Battle, Side, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SideRestart;
        Prefix = EventPrefix.None;
      Handler = handler;
        Priority = priority;
   UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Side), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);
    }
}
