using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Events.Handlers.SideEventMethods;

/// <summary>
/// Event handler info for OnSideEnd event (side-specific).
/// Triggered when a side condition ends.
/// Signature: Action&lt;Battle, Side&gt;
/// </summary>
public sealed record OnSideEndEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSideEnd event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSideEndEventInfo(
        Action<Battle, Side> handler,
        int? priority = null,
    bool usesSpeed = true)
    {
        Id = EventId.SideEnd;
        Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Side)];
  ExpectedReturnType = typeof(void);
    }
}
