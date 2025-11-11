namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Extended event methods interface that returns EventHandlerInfo instead of raw delegates.
/// This interface will gradually replace IEventMethods properties, providing type-safe
/// event information with consistent metadata across all effect types.
/// 
/// For now, this contains proof-of-concept events. As we migrate, more events will be added here
/// and eventually IEventMethods will be updated to return EventHandlerInfo directly.
/// </summary>
public interface IEventMethodsV2
{
    // ========== Proof of Concept Events ==========
    
    /// <summary>
    /// OnDamagingHit event with metadata: (Battle, int damage, Pokemon target, Pokemon source, ActiveMove move)
    /// </summary>
    EventHandlerInfo? GetOnDamagingHitInfo();
    
    /// <summary>
    /// OnEmergencyExit event with metadata: (Battle, Pokemon)
    /// </summary>
    EventHandlerInfo? GetOnEmergencyExitInfo();
    
    /// <summary>
    /// OnResidual event with metadata: (Battle, Pokemon target, Pokemon source, IEffect effect)
    /// </summary>
    EventHandlerInfo? GetOnResidualInfo();
    
    /// <summary>
  /// OnBasePower event with metadata: (Battle, int relayVar, Pokemon source, Pokemon target, ActiveMove move) -> DoubleVoidUnion
    /// </summary>
EventHandlerInfo? GetOnBasePowerInfo();
    
    /// <summary>
    /// OnBeforeMove event with metadata: (Battle, Pokemon target, Pokemon source, ActiveMove move) -> BoolVoidUnion
    /// </summary>
    EventHandlerInfo? GetOnBeforeMoveInfo();
    
  /// <summary>
    /// OnAfterSetStatus event with metadata: (Battle, Condition status, Pokemon target, Pokemon source, IEffect effect)
    /// </summary>
  EventHandlerInfo? GetOnAfterSetStatusInfo();
    
 /// <summary>
    /// OnSetStatus event with metadata: (Battle, Condition status, Pokemon target, Pokemon source, IEffect effect) -> BoolVoidUnion?
  /// </summary>
  EventHandlerInfo? GetOnSetStatusInfo();
    
    /// <summary>
    /// OnDamage event with metadata: (Battle, int damage, Pokemon target, Pokemon source, IEffect effect) -> IntBoolVoidUnion?
    /// </summary>
    EventHandlerInfo? GetOnDamageInfo();
}
