using System.Reflection;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Abstract base class for event handler information.
/// Each concrete event type (OnDamagingHit, OnBasePower, etc.) should inherit from this
/// to provide compile-time type safety and consistent metadata across all effect types.
/// </summary>
public abstract record EventHandlerInfo(Delegate Handler)
{
    /// <summary>
    /// The unique identifier for this event
    /// </summary>
    public EventId Id { get; init; }

    /// <summary>
    /// The actual delegate handler for this event (can be null if not implemented)
    /// </summary>
    public Delegate Handler { get; init; } = Handler;

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

        MethodInfo method = Handler.Method;
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

            // Validate each parameter type (handling nullability)
            for (int i = 0; i < actualParams.Length; i++)
            {
                Type expectedType = ExpectedParameterTypes[i];
                Type actualType = actualParams[i].ParameterType;

                // Strip nullability for comparison
                Type expectedBase = Nullable.GetUnderlyingType(expectedType) ?? expectedType;
                Type actualBase = Nullable.GetUnderlyingType(actualType) ?? actualType;

                // For reference types, check if either is nullable reference type
                if (!expectedBase.IsValueType && !actualBase.IsValueType)
                {
                    // Reference type comparison - check base types
                    if (!expectedBase.IsAssignableFrom(actualBase))
                    {
                        throw new InvalidOperationException(
                            $"Event {Id}: Parameter {i} ({actualParams[i].Name}) type mismatch. " +
                            $"Expected: {expectedType.Name}, Got: {actualType.Name}");
                    }
                }
                else
                {
                    // Value type comparison - strip nullability and compare
                    if (!expectedBase.IsAssignableFrom(actualBase))
                    {
                        throw new InvalidOperationException(
                            $"Event {Id}: Parameter {i} ({actualParams[i].Name}) type mismatch. " +
                            $"Expected: {expectedType.Name}, Got: {actualType.Name}");
                    }
                }
            }
        }

        // Validate return type (handling nullability)
        if (ExpectedReturnType != null)
        {
            Type actualReturnType = method.ReturnType;

            // Strip nullability for comparison
            Type actualBase = Nullable.GetUnderlyingType(actualReturnType) ?? actualReturnType;
            Type expectedBase = Nullable.GetUnderlyingType(ExpectedReturnType) ?? ExpectedReturnType;

            // Check base types match (nullability doesn't matter for runtime compatibility)
            if (!expectedBase.IsAssignableFrom(actualBase))
            {
                throw new InvalidOperationException(
                    $"Event {Id}: Return type mismatch. " +
                    $"Expected: {ExpectedReturnType.Name} (or nullable variant), " +
                    $"Got: {actualReturnType.Name}");
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

        string returnTypeName = ExpectedReturnType == typeof(void) ? "void" : ExpectedReturnType.Name;
        string paramNames = string.Join(", ", ExpectedParameterTypes.Select(t => t.Name));

        return $"{returnTypeName} ({paramNames})";
    }
}