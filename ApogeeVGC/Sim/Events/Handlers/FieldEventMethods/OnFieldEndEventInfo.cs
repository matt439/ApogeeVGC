using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;

/// <summary>
/// Event handler info for OnFieldEnd event (field-specific).
/// Triggered when a field condition ends.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Field) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnFieldEndEventInfo : EventHandlerInfo
{
    public OnFieldEndEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
     bool usesSpeed = true)
    {
        Id = EventId.FieldEnd;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnFieldEndEventInfo Create(
      Action<Battle, Field> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFieldEndEventInfo(
          context =>
 {
     handler(
        context.Battle,
 context.Battle.Field
              );
      return null;
     },
       priority,
            usesSpeed
      );
  }
}
