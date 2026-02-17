using System.Reflection;
using System.Text.Json.Serialization;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Unified delegate type for all context-based event handlers.
/// Handlers receive an EventContext and return a RelayVar (or null).
/// </summary>
public delegate RelayVar? EventHandlerDelegate(EventContext context);

/// <summary>
/// Abstract base class for event handler information.
/// Each concrete event type (OnDamagingHit, OnBasePower, etc.) should inherit from this
/// to provide compile-time type safety and consistent metadata across all effect types.
/// 
/// Supports both legacy delegate handlers and new context-based handlers.
/// </summary>
public abstract record EventHandlerInfo
{
    /// <summary>
  /// The unique identifier for this event
    /// </summary>
    public EventId Id { get; init; }

    /// <summary>
    /// The actual delegate handler for this event (can be null if not implemented).
    /// This is the legacy delegate that takes specific parameters.
    /// Use <see cref="ContextHandler"/> and the static Create factory method instead.
    /// </summary>
    [JsonIgnore]
    [Obsolete("Use ContextHandler with the static Create factory method instead.")]
    public Delegate? Handler { get; init; }
    
    /// <summary>
    /// The context-based handler (can be null if using legacy Handler).
    /// This is the new, simplified handler that takes EventContext.
    /// </summary>
    [JsonIgnore]
    public EventHandlerDelegate? ContextHandler { get; init; }
    
    /// <summary>
    /// True if this handler uses the new context-based approach.
    /// </summary>
    [JsonIgnore]
    public bool UsesContextHandler => ContextHandler != null;

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
    [JsonIgnore]
    public Type[]? ExpectedParameterTypes { get; init; }

    /// <summary>
    /// Expected return type for this event's delegate
    /// </summary>
    [JsonIgnore]
    public Type? ExpectedReturnType { get; init; }

    /// <summary>
    /// Indicates which parameters are nullable (true = nullable, false = non-null).
    /// Array indices correspond to ExpectedParameterTypes indices.
    /// If null, all parameters are assumed non-nullable.
    /// </summary>
    public bool[]? ParameterNullability { get; init; }

    /// <summary>
    /// Indicates whether the return type is nullable.
    /// </summary>
    public bool ReturnTypeNullable { get; init; }

    /// <summary>
    /// Validates configuration consistency (parameter nullability array length, etc.).
    /// Should be called in derived class constructors after setting all properties.
    /// </summary>
    protected void ValidateConfiguration()
    {
        // Validate ParameterNullability array length matches ExpectedParameterTypes
        if (ParameterNullability != null && ExpectedParameterTypes != null)
        {
            if (ParameterNullability.Length != ExpectedParameterTypes.Length)
            {
                throw new InvalidOperationException(
                    $"Event {Id}: ParameterNullability length ({ParameterNullability.Length}) " +
                    $"does not match ExpectedParameterTypes length ({ExpectedParameterTypes.Length})");
            }
        }
    }

    #pragma warning disable CS0618 // Handler is obsolete but still needed for legacy fallback
    public Delegate GetDelegateOrThrow()
    {
        if (Handler is null)
        {
            throw new InvalidOperationException("Hander is null.");
        }
        return Handler;
    }
#pragma warning restore CS0618

    /// <summary>
    /// Validates that the handler matches the expected signature.
    /// Throws InvalidOperationException if validation fails.
    /// </summary>
#pragma warning disable CS0618 // Handler is obsolete but still needed for legacy validation
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
                    // Reference type comparison - delegate parameter contravariance:
                    // The actual delegate parameter type must be assignable from the expected type
                    // (e.g., delegate taking 'object' can accept 'Pokemon' arguments)
                    if (!actualBase.IsAssignableFrom(expectedBase))
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
#pragma warning restore CS0618

    /// <summary>
    /// Gets a friendly description of the expected signature including nullability
    /// </summary>
    public string GetSignatureDescription()
    {
        if (ExpectedParameterTypes == null || ExpectedReturnType == null)
            return "Signature not specified";

        string returnTypeName = ExpectedReturnType == typeof(void) ? "void" : ExpectedReturnType.Name;
        if (ReturnTypeNullable && ExpectedReturnType != typeof(void))
        {
            returnTypeName += "?";
        }

        var paramDescriptions = new List<string>();
        for (int i = 0; i < ExpectedParameterTypes.Length; i++)
        {
            string paramName = ExpectedParameterTypes[i].Name;
            if (ParameterNullability?[i] == true)
            {
                paramName += "?";
            }
            paramDescriptions.Add(paramName);
        }

        return $"{returnTypeName} ({string.Join(", ", paramDescriptions)})";
    }

    /// <summary>
    /// Validates parameter nullability - checks if a parameter value is null when it shouldn't be
    /// </summary>
    public void ValidateParameterNullability(object?[] args)
    {
        if (ExpectedParameterTypes == null || args.Length != ExpectedParameterTypes.Length)
        {
            throw new InvalidOperationException("Parameter count mismatch");
        }

        for (int i = 0; i < args.Length; i++)
        {
            bool parameterIsNullable = ParameterNullability?[i] ?? false;
            bool valueIsNull = args[i] == null;

            if (valueIsNull && !parameterIsNullable)
            {
                throw new InvalidOperationException(
                    $"Event {Id}: Parameter {i} ({ExpectedParameterTypes[i].Name}) is non-nullable but null was provided");
            }
        }
    }
}