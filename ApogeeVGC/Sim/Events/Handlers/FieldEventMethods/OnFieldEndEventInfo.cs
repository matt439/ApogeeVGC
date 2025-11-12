using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;

namespace ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;

/// <summary>
/// Event handler info for OnFieldEnd event (field-specific).
/// Triggered when a field condition ends.
/// Signature: Action&lt;Battle, Field&gt;
/// </summary>
public sealed record OnFieldEndEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFieldEnd event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFieldEndEventInfo(
     Action<Battle, Field> handler,
int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.FieldEnd;
     Prefix = EventPrefix.None;
    Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Field)];
 ExpectedReturnType = typeof(void);
    }
}
