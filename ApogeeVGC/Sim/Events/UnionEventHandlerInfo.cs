using System.Reflection;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Specialized EventHandlerInfo for events that can accept either a delegate or a constant value (union types).
/// Examples: OnCriticalHit (func | bool), OnTakeItem (func | bool), OnTryHeal (func1 | func2 | bool)
/// </summary>
/// <typeparam name="TUnion">The union type (OnCriticalHit, OnFlinch, OnTakeItem, etc.)</typeparam>
public abstract record UnionEventHandlerInfo<TUnion> : EventHandlerInfo
    where TUnion : IUnionEventHandler
{
    /// <summary>
    /// The union value (either a delegate or constant value wrapped in the union type)
    /// </summary>
    public TUnion? UnionValue { get; init; }

    /// <summary>
    /// Extracts the actual delegate from the union value.
    /// Returns null if the union contains a constant value instead of a delegate.
    /// </summary>
    public Delegate? ExtractDelegate()
    {
        return UnionValue?.GetDelegate();
    }

    /// <summary>
    /// Checks if the union contains a constant value (e.g., true/false) rather than a delegate.
    /// </summary>
    public bool IsConstantValue()
    {
        return UnionValue?.IsConstant() ?? false;
    }

    /// <summary>
    /// Gets the constant value if this union represents a constant (e.g., bool).
    /// Returns null if the union contains a delegate.
    /// </summary>
    public object? GetConstantValue()
    {
        return UnionValue?.GetConstantValue();
    }

    /// <summary>
    /// Overrides validation to handle union types properly.
    /// </summary>
    public new void Validate()
    {
        if (UnionValue == null) return;

        // If it's a constant value, skip delegate validation
        if (IsConstantValue())
        {
            // Optional: validate the constant value type
            return;
        }

        // If it's a delegate, validate it directly
        Delegate? extractedDelegate = ExtractDelegate();
        if (extractedDelegate == null) return;

        MethodInfo method = extractedDelegate.Method;
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
    /// Invokes the union handler with the given arguments.
    /// Handles both delegate and constant value cases.
    /// </summary>
    public object? Invoke(params object[] args)
    {
        // If constant value, return it directly
        if (IsConstantValue())
        {
            return GetConstantValue();
        }

        // If delegate, invoke it
        Delegate? del = ExtractDelegate();
        return del?.DynamicInvoke(args);
    }
}