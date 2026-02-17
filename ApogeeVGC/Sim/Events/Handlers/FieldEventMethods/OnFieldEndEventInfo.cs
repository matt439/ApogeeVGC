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
    /// <summary>
    /// Creates a new OnFieldEnd event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnFieldEndEventInfo(
        Action<Battle, Field> handler,
 int? priority = null,
    bool usesSpeed = true)
    {
        Id = EventId.FieldEnd;
   Prefix = EventPrefix.None;
 #pragma warning disable CS0618
 Handler = handler;
 #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
    ExpectedParameterTypes = [typeof(Battle), typeof(Field)];
        ExpectedReturnType = typeof(void);
  
        // Nullability: All parameters non-nullable by default (adjust as needed)
   ParameterNullability = new[] { false, false };
     ReturnTypeNullable = false;
    
      // Validate configuration
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, Field
    /// </summary>
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
