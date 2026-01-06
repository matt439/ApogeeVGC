using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.SideEventMethods;

/// <summary>
/// Event handler info for OnSideEnd event (side-specific).
/// Triggered when a side condition ends.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Side) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnSideEndEventInfo : EventHandlerInfo
{
 /// <summary>
    /// Creates a new OnSideEnd event handler using the legacy strongly-typed pattern.
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
        
        // Nullability: All parameters non-nullable by default (adjust as needed)
     ParameterNullability = new[] { false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
 }
    
 /// <summary>
  /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetSide
 /// </summary>
  public OnSideEndEventInfo(
   EventHandlerDelegate contextHandler,
        int? priority = null,
     bool usesSpeed = true)
    {
    Id = EventId.SideEnd;
  Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
   Priority = priority;
   UsesSpeed = usesSpeed;
    }
    
  /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnSideEndEventInfo Create(
  Action<Battle, Side> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
  return new OnSideEndEventInfo(
        context =>
   {
      handler(
  context.Battle,
   context.GetTargetSide()
       );
        return null;
     },
  priority,
         usesSpeed
     );
    }
}
