using System.Reflection;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
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
                var innerEx = ex.InnerException ?? ex;
                string errorMsg = $"Event {handlerInfo.Id}: Exception in legacy handler {legacyHandler.Method.DeclaringType?.Name}.{legacyHandler.Method.Name}";
                errorMsg += $"\nException type: {innerEx.GetType().Name}";
                errorMsg += $"\nException message: {innerEx.Message}";
                if (innerEx.StackTrace != null)
                {
                    var stackLines = innerEx.StackTrace.Split('\n').Take(10);
                    errorMsg += $"\nStack trace:\n{string.Join("\n", stackLines)}";
                }
                throw new InvalidOperationException(errorMsg, innerEx);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Event {handlerInfo.Id}: Unexpected exception in legacy handler {legacyHandler.Method.DeclaringType?.Name}.{legacyHandler.Method.Name}";
                errorMsg += $"\nException type: {ex.GetType().Name}";
                errorMsg += $"\nException message: {ex.Message}";
                throw new InvalidOperationException(errorMsg, ex);
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
            context.Battle.Debug($"[ResolveParameter] Event={handlerInfo.Id}, Position={position}, ParamType={paramType.Name}, RelayVar={context.RelayVar?.GetType().Name}");
            
            if (paramType.IsInstanceOfType(context.RelayVar!))
            {
                context.Battle.Debug($"[ResolveParameter] Returning RelayVar directly");
                return context.RelayVar;
            }

            // Check for primitive unwrapping
            if (TryUnwrapRelayVar(context.RelayVar!, paramType, out object? unwrapped))
            {
                context.Battle.Debug($"[ResolveParameter] Unwrapped to: {unwrapped}");
                return unwrapped;
            }
            
            context.Battle.Debug($"[ResolveParameter] TryUnwrapRelayVar failed");
            
            // Check for Effect unwrapping from EffectRelayVar
            // This handles cases like TryAddVolatile where a Condition is passed as RelayVar
            if (context.RelayVar is EffectRelayVar effectRelayVar && 
                paramType.IsAssignableFrom(effectRelayVar.Effect.GetType()))
            {
                return effectRelayVar.Effect;
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

        // Add support for Field parameters
        if (paramName.Contains("field"))
        {
            return context.TargetField;
        }

        // Match by type
        if (paramType == typeof(Pokemon) || typeof(Pokemon).IsAssignableFrom(paramType))
        {
            // Prefer target over source if ambiguous
            return context.TargetPokemon ?? context.SourcePokemon;
        }

        // Add support for Field type
        if (paramType == typeof(ApogeeVGC.Sim.FieldClasses.Field) || typeof(ApogeeVGC.Sim.FieldClasses.Field).IsAssignableFrom(paramType))
        {
            return context.TargetField;
        }

        // Add support for Side type
        if (paramType == typeof(ApogeeVGC.Sim.SideClasses.Side) || typeof(ApogeeVGC.Sim.SideClasses.Side).IsAssignableFrom(paramType))
        {
            return context.TargetSide;
        }

        // Add support for Move/ActiveMove type
        if (paramType == typeof(ActiveMove) || typeof(ActiveMove).IsAssignableFrom(paramType))
        {
            return context.Move;
        }

        // Also check for base Move type
        if (paramType == typeof(Move) || typeof(Move).IsAssignableFrom(paramType))
        {
            return context.Move;
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

        // Check for PokemonTypeConditionIdUnion
        if (paramType == typeof(PokemonTypeConditionIdUnion))
        {
            if (context.HasRelayVar)
            {
                // Handle ConditionIdRelayVar -> PokemonTypeConditionIdUnion
                if (context.RelayVar is ConditionIdRelayVar conditionIdVar && conditionIdVar.Id.HasValue)
                {
                    return new PokemonTypeConditionIdUnion(conditionIdVar.Id.Value);
                }
                
                // Handle PokemonTypeRelayVar -> PokemonTypeConditionIdUnion
                if (context.RelayVar is PokemonTypeRelayVar pokemonTypeVar)
                {
                    return new PokemonTypeConditionIdUnion(pokemonTypeVar.Type);
                }
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

        // Handle int/Int32 (they're the same but check both for safety)
        if (relayVar is IntRelayVar intVar && (targetType == typeof(int) || targetType == typeof(Int32)))
        {
            value = intVar.Value;
            return true;
        }

        // Handle decimal -> int conversion (for stat modifications that return decimal but parameter expects int)
        if (relayVar is DecimalRelayVar decVarToInt && (targetType == typeof(int) || targetType == typeof(Int32)))
        {
            value = (int)decVarToInt.Value;
            return true;
        }

        // Handle decimal
        if (relayVar is DecimalRelayVar decVar && targetType == typeof(decimal))
        {
            value = decVar.Value;
            return true;
        }

        // Handle double (might be used for stat modifiers)
        if (relayVar is DecimalRelayVar decVar2 && targetType == typeof(double))
        {
            value = (double)decVar2.Value;
            return true;
        }

        // Handle bool/Boolean
        if (relayVar is BoolRelayVar boolVar && (targetType == typeof(bool) || targetType == typeof(Boolean)))
        {
            value = boolVar.Value;
            return true;
        }

        // Handle SparseBoostsTable
        if (relayVar is SparseBoostsTableRelayVar sparseBoostsVar && targetType == typeof(SparseBoostsTable))
        {
            value = sparseBoostsVar.Table;
            return true;
        }

        // Handle BoostsTable
        if (relayVar is BoostsTableRelayVar boostsVar && targetType == typeof(BoostsTable))
        {
            value = boostsVar.Table;
            return true;
        }

        // Handle SecondaryEffect[] unwrapping (direct array type)
        if (relayVar is SecondaryEffectArrayRelayVar secondaryEffectsVar &&
            targetType == typeof(SecondaryEffect[]))
        {
            value = secondaryEffectsVar.Effects;
            return true;
        }

        // Handle SecondaryEffect[] -> List<SecondaryEffect> conversion
        if (relayVar is SecondaryEffectArrayRelayVar secondaryEffectsListVar &&
            (targetType == typeof(List<SecondaryEffect>) || targetType.IsAssignableFrom(typeof(List<SecondaryEffect>))))
        {
            value = new List<SecondaryEffect>(secondaryEffectsListVar.Effects);
            return true;
        }

        return false;
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
            // BoolVoidUnion -> bool or VoidReturn
            BoolBoolVoidUnion boolVoid => new BoolRelayVar(boolVoid.Value),
            VoidBoolVoidUnion => new VoidReturnRelayVar(),

            // BoolEmptyVoidUnion -> bool, Empty, or VoidReturn
            BoolBoolEmptyVoidUnion boolEmptyVoid => new BoolRelayVar(boolEmptyVoid.Value),
            EmptyBoolEmptyVoidUnion => new BoolRelayVar(false), // Empty means blocked
            VoidUnionBoolEmptyVoidUnion => new VoidReturnRelayVar(),

            // DoubleVoidUnion -> decimal or VoidReturn
            DoubleDoubleVoidUnion doubleVoid => new DecimalRelayVar((decimal)doubleVoid.Value),
            VoidDoubleVoidUnion => new VoidReturnRelayVar(),

            // IntVoidUnion -> int or VoidReturn
            IntIntVoidUnion intVoid => new IntRelayVar(intVoid.Value),
            VoidIntVoidUnion => new VoidReturnRelayVar(),

            // IntBoolVoidUnion -> int, bool, or VoidReturn
            IntIntBoolVoidUnion intBoolVoid => new IntRelayVar(intBoolVoid.Value),
            BoolIntBoolVoidUnion boolIntVoid => new BoolRelayVar(boolIntVoid.Value),
            VoidIntBoolVoidUnion => new VoidReturnRelayVar(),

            // BoolIntEmptyVoidUnion -> bool, int, Empty, or VoidReturn
            BoolBoolIntEmptyVoidUnion boolIntEmptyVoid => new BoolRelayVar(boolIntEmptyVoid.Value),
            IntBoolIntEmptyVoidUnion intIntEmptyVoid => new IntRelayVar(intIntEmptyVoid.Value),
            EmptyBoolIntEmptyVoidUnion => new BoolRelayVar(false), // Empty means blocked (e.g., by Protect)
            VoidUnionBoolIntEmptyVoidUnion => new VoidReturnRelayVar(),

            // BoolIntUndefinedUnion -> bool, int, or Undefined
            BoolBoolIntUndefinedUnion boolIntUndef => new BoolRelayVar(boolIntUndef.Value),
            IntBoolIntUndefinedUnion intIntUndef => new IntRelayVar(intIntUndef.Value),
            UndefinedBoolIntUndefinedUnion => new UndefinedRelayVar(),

            // IntFalseUndefinedUnion -> int, false, or Undefined
            IntIntFalseUndefined intFalseUndef => new IntRelayVar(intFalseUndef.Value),
            FalseIntFalseUndefined => new BoolRelayVar(false),
            UndefinedIntFalseUndefined => new UndefinedRelayVar(),

            // BoolIntEmptyUndefinedUnion -> bool, int, Empty, or Undefined
            BoolBoolIntEmptyUndefinedUnion boolIntEmptyUndef => new BoolRelayVar(boolIntEmptyUndef.Value),
            IntBoolIntEmptyUndefinedUnion intIntEmptyUndef => new IntRelayVar(intIntEmptyUndef.Value),
            EmptyBoolIntEmptyUndefinedUnion => new BoolRelayVar(false), // Empty means blocked
            UndefinedBoolIntEmptyUndefinedUnion => new UndefinedRelayVar(),

            // IntUndefinedFalseEmptyUnion -> int, Undefined, false, or Empty
            IntIntUndefinedFalseEmptyUnion intUndefFalseEmpty => new IntRelayVar(intUndefFalseEmpty.Value),
            UndefinedIntUndefinedFalseEmptyUnion => new UndefinedRelayVar(),
            FalseIntUndefinedFalseEmptyUnion => new BoolRelayVar(false),
            EmptyIntUndefinedFalseEmptyUnion => new BoolRelayVar(false), // Empty means blocked

            // BoolUndefinedUnion -> bool or Undefined
            BoolBoolUndefinedUnion boolUndef => new BoolRelayVar(boolUndef.Value),
            UndefinedBoolUndefinedUnion => new UndefinedRelayVar(),

            // PokemonVoidUnion -> Pokemon or VoidReturn
            PokemonPokemonVoidUnion pokemonVoid => new PokemonRelayVar(pokemonVoid.Pokemon),
            VoidPokemonVoidUnion => new VoidReturnRelayVar(),

            // MoveIdVoidUnion -> MoveId or VoidReturn
            MoveIdMoveIdVoidUnion moveIdVoid => new MoveIdRelayVar(moveIdVoid.MoveId),
            VoidMoveIdVoidUnion => new VoidReturnRelayVar(),

            // Primitive types
            bool boolValue => new BoolRelayVar(boolValue),
            int intValue => new IntRelayVar(intValue),
            decimal decValue => new DecimalRelayVar(decValue),
            double doubleValue => new DecimalRelayVar((decimal)doubleValue),

            // SecondaryEffect[] -> SecondaryEffectArrayRelayVar
            SecondaryEffect[] secondaryEffects => new SecondaryEffectArrayRelayVar(secondaryEffects),

            // VoidReturn - preserve as VoidReturnRelayVar
            VoidReturn => new VoidReturnRelayVar(),

            // Undefined
            Undefined => new UndefinedRelayVar(),

            // Empty - represents blocked move (e.g., by Protect), should return false
            Empty => new BoolRelayVar(false),

            _ => throw new InvalidOperationException(
                $"Event {handlerInfo.Id}: Unable to convert return value of type '{returnValue.GetType().Name}' to RelayVar")
        };
    }
}
