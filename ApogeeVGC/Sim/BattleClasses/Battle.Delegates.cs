using System.Reflection;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    private RelayVar? InvokeDelegateEffectDelegate(Delegate del, bool hasRelayVar, RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect)
    {
        // Cache parameter info to avoid repeated reflection calls
        var parameters = del.Method.GetParameters();
        int paramCount = parameters.Length;

        // Most common signature: (Battle battle, ...)
        if (paramCount == 0)
        {
            object? result = del.DynamicInvoke(null);
            return ConvertReturnValueToRelayVar(result);
        }

        // Optimize for the most common cases (1-4 parameters)
        // This avoids array allocation for the majority of callbacks
        object? returnValue;
        try
        {
            switch (paramCount)
            {
                case 1:
                    returnValue = del.DynamicInvoke(BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect, 0));
                    return ConvertReturnValueToRelayVar(returnValue);
                case 2:
                    returnValue = del.DynamicInvoke(
                        BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect, 0),
                        BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1)
                    );
                    return ConvertReturnValueToRelayVar(returnValue);
                case 3:
                    returnValue = del.DynamicInvoke(
                        BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect, 0),
                        BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1),
                        BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source, sourceEffect, 2)
                    );
                    return ConvertReturnValueToRelayVar(returnValue);
                case 4:
                    returnValue = del.DynamicInvoke(
                        BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect, 0),
                        BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1),
                        BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source, sourceEffect, 2),
                        BuildSingleArg(parameters[3], hasRelayVar, relayVar, target, source, sourceEffect, 3)
                    );
                    return ConvertReturnValueToRelayVar(returnValue);
            }

            // Fallback for 5+ parameters (rare)
            // Use array allocation for these cases
            object?[] args = new object?[paramCount];
            for (int i = 0; i < paramCount; i++)
            {
                args[i] = BuildSingleArg(parameters[i], hasRelayVar, relayVar, target, source, sourceEffect, i);
            }

            returnValue = del.DynamicInvoke(args);
            return ConvertReturnValueToRelayVar(returnValue);
        }
        catch (TargetParameterCountException ex)
        {
            // Log detailed information about the parameter mismatch
            Console.WriteLine($"[InvokeDelegateEffectDelegate] Parameter count mismatch!");
            Console.WriteLine($"  Delegate: {del.Method.Name}");
            Console.WriteLine($"  Declaring Type: {del.Method.DeclaringType?.Name}");
            Console.WriteLine($"  Expected parameters: {paramCount}");
            Console.WriteLine($"  hasRelayVar: {hasRelayVar}");
            Console.WriteLine($"  target: {target?.GetType().Name ?? "null"}");
            Console.WriteLine($"  source: {source?.GetType().Name ?? "null"}");
            Console.WriteLine($"  sourceEffect: {sourceEffect?.GetType().Name ?? "null"}");

            // Print expected parameter types
            Console.WriteLine("  Parameter types:");
            for (int i = 0; i < parameters.Length; i++)
            {
                Console.WriteLine($"    [{i}] {parameters[i].ParameterType.Name} {parameters[i].Name}");
            }

            throw;
        }
    }

    /// <summary>
    /// Converts various return value types from event handlers to RelayVar.
    /// Event handlers can return many different union types that need to be converted.
    /// </summary>
    private RelayVar? ConvertReturnValueToRelayVar(object? returnValue)
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

            // BoolEmptyVoidUnion -> bool or null
            BoolBoolEmptyVoidUnion boolEmptyVoid => new BoolRelayVar(boolEmptyVoid.Value),
            EmptyBoolEmptyVoidUnion => null,
            VoidUnionBoolEmptyVoidUnion => null,

            // IntBoolVoidUnion -> int, bool, or null
            IntIntBoolVoidUnion intBoolVoid => new IntRelayVar(intBoolVoid.Value),
            BoolIntBoolVoidUnion boolIntVoid => new BoolRelayVar(boolIntVoid.Value),
            VoidIntBoolVoidUnion => null,

            // BoolIntEmptyVoidUnion -> bool, int, or null
            BoolBoolIntEmptyVoidUnion boolIntEmptyVoid => new BoolRelayVar(boolIntEmptyVoid.Value),
            IntBoolIntEmptyVoidUnion intBoolEmptyVoid => new IntRelayVar(intBoolEmptyVoid.Value),
            EmptyBoolIntEmptyVoidUnion => null,
            VoidUnionBoolIntEmptyVoidUnion => null,

            // BoolUndefinedUnion -> bool or undefined
            BoolBoolUndefinedUnion boolUndefined => new BoolRelayVar(boolUndefined.Value),
            UndefinedBoolUndefinedUnion => new UndefinedRelayVar(),

            // BoolIntUndefinedUnion -> bool, int, or undefined
            BoolBoolIntUndefinedUnion boolIntUndefined => new BoolRelayVar(boolIntUndefined.Value),
            IntBoolIntUndefinedUnion intIntUndefined => new IntRelayVar(intIntUndefined.Value),
            UndefinedBoolIntUndefinedUnion => new UndefinedRelayVar(),

            // IntBoolUnion -> int or bool
            IntIntBoolUnion intBool => new IntRelayVar(intBool.Value),
            BoolIntBoolUnion boolBool => new BoolRelayVar(boolBool.Value),

            // IntVoidUnion -> int or null
            IntIntVoidUnion intVoid => new IntRelayVar(intVoid.Value),
            VoidIntVoidUnion => null,

            // DoubleVoidUnion -> double or null
            DoubleDoubleVoidUnion doubleVoid => new DecimalRelayVar((decimal)doubleVoid.Value),
            VoidDoubleVoidUnion => null,

            // MoveIdVoidUnion -> MoveId or null
            MoveIdMoveIdVoidUnion moveIdVoid => new MoveIdRelayVar(moveIdVoid.MoveId),
            VoidMoveIdVoidUnion => null,

            // PokemonVoidUnion -> Pokemon or null
            PokemonPokemonVoidUnion pokemonVoid => new PokemonRelayVar(pokemonVoid.Pokemon),
            VoidPokemonVoidUnion => null,

            // Primitive types
            bool boolValue => new BoolRelayVar(boolValue),
            int intValue => new IntRelayVar(intValue),
            decimal decValue => new DecimalRelayVar(decValue),
            double doubleValue => new DecimalRelayVar((decimal)doubleValue),

            // VoidReturn means no value (return null)
            VoidReturn => null,

            // Undefined means undefined (special RelayVar)
            Undefined => new UndefinedRelayVar(),

            // If we don't recognize the type, log an error and return null
            _ => throw new InvalidOperationException(
                $"Unable to convert return value of type '{returnValue.GetType().Name}' to RelayVar. " +
                $"Event handler returned an unexpected type that needs to be handled in ConvertReturnValueToRelayVar.")
        };
    }

    /// <summary>
    /// Builds a single argument for delegate invocation.
    /// Used by the optimized fast-path for common parameter counts.
    /// </summary>
    private object? BuildSingleArg(ParameterInfo param, bool hasRelayVar, RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect, int position = 0)
    {
        Type paramType = param.ParameterType;
        string paramName = param.Name?.ToLowerInvariant() ?? "";

    // First parameter is typically Battle
        if (position == 0 && paramType.IsAssignableFrom(typeof(Battle)))
        {
return this;
        }

        // Check if this parameter is actually expecting a RelayVar type
        // Only pass relayVar if the parameter type is RelayVar or a derived type
        // Also check for common relayVar types like int, decimal, etc.
        if (hasRelayVar && (paramType.IsAssignableFrom(relayVar.GetType()) || 
       IsRelayVarCompatibleType(paramType, relayVar)))
        {
          return ExtractValueFromRelayVar(relayVar, paramType);
        }

        // Try to extract from target, source, or sourceEffect based on parameter type and name
        // Check parameter name to determine if it's "source" or "target"
        bool isSourceParam = paramName.Contains("source");
        bool isTargetParam = paramName.Contains("target");

        // Try type-based matching
        if (target != null)
 {
EventTargetParameter? targetParam = EventTargetParameter.FromSingleEventTarget(target, paramType);
       if (targetParam != null && (!isSourceParam || !isTargetParam))
     {
 // Only return target if it's not explicitly named "source"
          if (!isSourceParam) return targetParam.ToObject();
       }
        }

  if (source != null)
        {
            EventSourceParameter? sourceParam = EventSourceParameter.FromSingleEventSource(source, paramType);
            if (sourceParam != null && (!isTargetParam || !isSourceParam))
            {
       // Only return source if it's not explicitly named "target"
        if (!isTargetParam) return sourceParam.ToObject();
            }
        }

        if (sourceEffect != null && paramType.IsInstanceOfType(sourceEffect))
        {
    return sourceEffect;
        }

        return null;
    }

    /// <summary>
    /// Checks if a parameter type can accept a RelayVar value (like int, decimal, etc.)
    /// </summary>
  private static bool IsRelayVarCompatibleType(Type paramType, RelayVar relayVar)
    {
        return relayVar switch
        {
     IntRelayVar when paramType == typeof(int) => true,
     DecimalRelayVar when paramType == typeof(decimal) => true,
       BoolRelayVar when paramType == typeof(bool) => true,
            _ => false
     };
    }

    /// <summary>
    /// Extracts the actual value from a RelayVar based on the expected parameter type
    /// </summary>
    private static object? ExtractValueFromRelayVar(RelayVar relayVar, Type paramType)
    {
   return relayVar switch
  {
            IntRelayVar intVar when paramType == typeof(int) => intVar.Value,
  DecimalRelayVar decVar when paramType == typeof(decimal) => decVar.Value,
     BoolRelayVar boolVar when paramType == typeof(bool) => boolVar.Value,
    _ => relayVar // Return the RelayVar itself if no conversion needed
    };
    }
}