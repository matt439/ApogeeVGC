using ApogeeVGC.Sim.Effects;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Combines event handler delegate with its metadata and signature information.
/// This ensures that all effect types (Ability, Item, Condition, Move) return
/// consistent event information for the same EventId.
/// </summary>
public record EventHandlerInfo
{
    /// <summary>
    /// The unique identifier for this event
    /// </summary>
    public required EventId Id { get; init; }

    /// <summary>
/// The actual delegate handler for this event (can be null if not implemented)
    /// </summary>
    public Delegate? Handler { get; init; }

    // Metadata from EventIdInfo
    
    /// <summary>
    /// Event prefix (e.g., Any, Foe, Source, Ally)
    /// </summary>
  public EventPrefix? Prefix { get; init; }

    /// <summary>
  /// Event suffix for specialized variants
    /// </summary>
    public EventSuffix? Suffix { get; init; }

    /// <summary>
    /// Base event ID if this is a derived event
    /// </summary>
    public EventId? BaseEventId { get; init; }

    /// <summary>
    /// Field-level variant of this event
    /// </summary>
    public EventId? FieldEventId { get; init; }

    /// <summary>
    /// Side-level variant of this event
    /// </summary>
    public EventId? SideEventId { get; init; }

    /// <summary>
    /// Whether this event uses effect ordering
    /// </summary>
    public bool UsesEffectOrder { get; init; }

    /// <summary>
    /// Whether this event uses speed-based ordering
    /// </summary>
    public bool UsesSpeed { get; init; }

    /// <summary>
    /// Whether this event uses fractional speed ordering
 /// </summary>
    public bool UsesFractionalSpeed { get; init; }

    /// <summary>
    /// Whether this event uses left-to-right ordering
    /// </summary>
    public bool UsesLeftToRightOrder { get; init; }

    // Priority/Ordering
 
    /// <summary>
    /// Priority value for event ordering (higher = earlier execution)
    /// </summary>
    public int? Priority { get; init; }

    /// <summary>
    /// Order value for event execution
  /// </summary>
    public int? Order { get; init; }

    /// <summary>
    /// Sub-order value for fine-grained event ordering
    /// </summary>
    public int? SubOrder { get; init; }

 // Type validation

    /// <summary>
    /// Expected parameter types for this event's delegate
    /// </summary>
    public Type[]? ExpectedParameterTypes { get; init; }

    /// <summary>
    /// Expected return type for this event's delegate
    /// </summary>
    public Type? ExpectedReturnType { get; init; }

    /// <summary>
    /// Validates that the handler matches the expected signature.
    /// Throws InvalidOperationException if validation fails.
    /// </summary>
    public void Validate()
  {
        if (Handler == null) return;

        var method = Handler.Method;
        var actualParams = method.GetParameters();

 // Validate parameter count
        if (ExpectedParameterTypes != null)
        {
            if (actualParams.Length != ExpectedParameterTypes.Length)
 {
         throw new InvalidOperationException(
      $"Event {Id}: Expected {ExpectedParameterTypes.Length} parameters, " +
          $"got {actualParams.Length}. " +
      $"Expected: ({string.Join(", ", ExpectedParameterTypes.Select(t => t.Name))}), " +
  $"Got: ({string.Join(", ", actualParams.Select(p => p.ParameterType.Name))})");
 }

     // Validate each parameter type
         for (int i = 0; i < actualParams.Length; i++)
       {
   var expectedType = ExpectedParameterTypes[i];
          var actualType = actualParams[i].ParameterType;

   if (!expectedType.IsAssignableFrom(actualType))
 {
   throw new InvalidOperationException(
               $"Event {Id}: Parameter {i} ({actualParams[i].Name}) type mismatch. " +
       $"Expected: {expectedType.Name}, Got: {actualType.Name}");
             }
}
        }

        // Validate return type
        if (ExpectedReturnType != null)
        {
            var actualReturnType = method.ReturnType;
    if (!ExpectedReturnType.IsAssignableFrom(actualReturnType))
            {
     throw new InvalidOperationException(
                 $"Event {Id}: Return type mismatch. " +
               $"Expected: {ExpectedReturnType.Name}, Got: {actualReturnType.Name}");
    }
        }
    }

    /// <summary>
    /// Gets a friendly description of the expected signature
    /// </summary>
    public string GetSignatureDescription()
    {
      if (ExpectedParameterTypes == null || ExpectedReturnType == null)
  return "Signature not specified";

        var returnTypeName = ExpectedReturnType == typeof(void) ? "void" : ExpectedReturnType.Name;
        var paramNames = string.Join(", ", ExpectedParameterTypes.Select(t => t.Name));

        return $"{returnTypeName} ({paramNames})";
    }
}
