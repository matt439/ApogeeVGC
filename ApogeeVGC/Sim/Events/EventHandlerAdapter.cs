using System.Reflection;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Provides adapters to convert legacy parameter-based event handlers
/// to the new context-based handlers.
/// </summary>
internal static class EventHandlerAdapter
{
    /// <summary>
    /// Converts a legacy delegate handler to a context-based handler.
    /// Uses reflection to map EventContext properties to delegate parameters.
    /// </summary>
    public static EventHandlerDelegate AdaptLegacyHandler(
        Delegate legacyHandler,
        EventHandlerInfo handlerInfo)
    {
        var parameters = legacyHandler.Method.GetParameters();

        return context =>
        {
            // Build arguments for the legacy handler
            object?[] args = new object?[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = ResolveParameter(parameters[i], i, context, handlerInfo);
            }

            // Invoke the legacy handler
            try
            {
                object? result = legacyHandler.DynamicInvoke(args);
                return ConvertReturnValue(result, handlerInfo);
            }
            catch (TargetInvocationException ex)
            {
                throw new InvalidOperationException(
                    $"Event {handlerInfo.Id}: Exception in legacy handler {legacyHandler.Method.DeclaringType?.Name}.{legacyHandler.Method.Name}",
                    ex.InnerException ?? ex);
            }
        };
    }

    /// <summary>
    /// Resolves a single parameter from the event context.
    /// </summary>
    private static object? ResolveParameter(
        ParameterInfo param,
        int position,
        EventContext context,
        EventHandlerInfo handlerInfo)
    {
        Type paramType = param.ParameterType;
        string paramName = param.Name?.ToLowerInvariant() ?? "";

        // Validate against EventHandlerInfo if available
        if (handlerInfo.ExpectedParameterTypes != null &&
            position < handlerInfo.ExpectedParameterTypes.Length)
        {
            Type expectedType = handlerInfo.ExpectedParameterTypes[position];
            ValidateParameterType(paramType, expectedType, position, handlerInfo);
        }

        // Position 0 is typically Battle
        if (position == 0 &&
            (paramType == typeof(Battle) || paramType.IsAssignableFrom(typeof(Battle))))
        {
            return context.Battle;
        }

        // Check for RelayVar types
        if (context.HasRelayVar)
        {
            if (paramType.IsInstanceOfType(context.RelayVar!))
            {
                return context.RelayVar;
            }

            // Check for primitive unwrapping
            if (TryUnwrapRelayVar(context.RelayVar!, paramType, out object? unwrapped))
            {
                return unwrapped;
            }
        }

        // Match by parameter name
        if (paramName.Contains("target") && paramName.Contains("pokemon") || paramName == "target")
        {
            return context.TargetPokemon;
        }

        if (paramName.Contains("source") && paramName.Contains("pokemon") || paramName == "source")
        {
            return context.SourcePokemon;
        }

        if (paramName.Contains("move"))
        {
            return context.Move;
        }

        if (paramName.Contains("effect") || paramName.Contains("sourceeffect"))
        {
            return context.SourceEffect;
        }

        if (paramName.Contains("side"))
        {
            return context.TargetSide;
        }

        // Match by type
        if (paramType == typeof(Pokemon) || typeof(Pokemon).IsAssignableFrom(paramType))
        {
            // Prefer target over source if ambiguous
            return context.TargetPokemon ?? context.SourcePokemon;
        }

        // Check for PokemonSideFieldUnion types
        if (paramType == typeof(PokemonSideFieldUnion) || typeof(PokemonSideFieldUnion).IsAssignableFrom(paramType))
        {
            // Wrap the target Pokemon in a union type
            if (context.TargetPokemon != null)
            {
                return new PokemonSideFieldPokemon(context.TargetPokemon);
            }
            // Fallback to Side if available
            if (context.TargetSide != null)
            {
                return new PokemonSideFieldSide(context.TargetSide);
            }
            // Fallback to Field if available
            if (context.TargetField != null)
            {
                return new PokemonSideFieldField(context.TargetField);
            }
        }

        if (typeof(IEffect).IsAssignableFrom(paramType))
        {
            return context.SourceEffect;
        }

        // Check nullability
        if (handlerInfo.ParameterNullability != null &&
            position < handlerInfo.ParameterNullability.Length &&
            !handlerInfo.ParameterNullability[position])
        {
            throw new InvalidOperationException(
                $"Event {handlerInfo.Id}: Parameter {position} ({paramType.Name} {paramName}) is non-nullable " +
                $"but no matching value found in context");
        }

        return null;
    }

    /// <summary>
    /// Validates that a parameter type matches the expected type from EventHandlerInfo.
    /// </summary>
    private static void ValidateParameterType(
        Type actualType,
        Type expectedType,
        int position,
        EventHandlerInfo handlerInfo)
    {
        // Strip nullability for comparison
        Type expectedBase = Nullable.GetUnderlyingType(expectedType) ?? expectedType;
        Type actualBase = Nullable.GetUnderlyingType(actualType) ?? actualType;

        // For reference types, check assignability
        if (!expectedBase.IsValueType && !actualBase.IsValueType)
        {
            if (!expectedBase.IsAssignableFrom(actualBase) &&
                !actualBase.IsAssignableFrom(expectedBase))
            {
                throw new InvalidOperationException(
                    $"Event {handlerInfo.Id}: Parameter {position} type mismatch. " +
                    $"Expected {expectedType.Name}, got {actualType.Name}");
            }
        }
        // For value types, check equality
        else if (!expectedBase.IsAssignableFrom(actualBase) &&
                 !actualBase.IsAssignableFrom(expectedBase))
        {
            throw new InvalidOperationException(
                $"Event {handlerInfo.Id}: Parameter {position} type mismatch. " +
                $"Expected {expectedType.Name}, got {actualType.Name}");
        }
    }

    /// <summary>
    /// Tries to unwrap a RelayVar to a primitive type.
    /// </summary>
    private static bool TryUnwrapRelayVar(RelayVar relayVar, Type targetType, out object? value)
    {
        value = null;

        switch (relayVar)
        {
            case IntRelayVar intVar when targetType == typeof(int):
                value = intVar.Value;
                return true;
            case DecimalRelayVar decVar when targetType == typeof(decimal):
                value = decVar.Value;
                return true;
            case BoolRelayVar boolVar when targetType == typeof(bool):
                value = boolVar.Value;
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Converts a legacy handler's return value to a RelayVar.
    /// </summary>
    private static RelayVar? ConvertReturnValue(object? returnValue, EventHandlerInfo handlerInfo)
    {
        if (returnValue == null)
        {
            return null;
        }

        // If it's already a RelayVar, return it directly
        if (returnValue is RelayVar relayVar)
        {
            return relayVar;
        }

        // Convert common union types to RelayVar
        return returnValue switch
        {
            // BoolVoidUnion -> bool or null
            BoolBoolVoidUnion boolVoid => new BoolRelayVar(boolVoid.Value),
            VoidBoolVoidUnion => null,

            // DoubleVoidUnion -> decimal or null
            DoubleDoubleVoidUnion doubleVoid => new DecimalRelayVar((decimal)doubleVoid.Value),
            VoidDoubleVoidUnion => null,

            // IntVoidUnion -> int or null
            IntIntVoidUnion intVoid => new IntRelayVar(intVoid.Value),
            VoidIntVoidUnion => null,

            // IntBoolVoidUnion -> int, bool, or null
            IntIntBoolVoidUnion intBoolVoid => new IntRelayVar(intBoolVoid.Value),
            BoolIntBoolVoidUnion boolIntVoid => new BoolRelayVar(boolIntVoid.Value),
            VoidIntBoolVoidUnion => null,

            // Primitive types
            bool boolValue => new BoolRelayVar(boolValue),
            int intValue => new IntRelayVar(intValue),
            decimal decValue => new DecimalRelayVar(decValue),
            double doubleValue => new DecimalRelayVar((decimal)doubleValue),

            // VoidReturn means no value
            VoidReturn => null,

            // Undefined
            Undefined => new UndefinedRelayVar(),

            _ => throw new InvalidOperationException(
                $"Event {handlerInfo.Id}: Unable to convert return value of type '{returnValue.GetType().Name}' to RelayVar")
        };
    }
}