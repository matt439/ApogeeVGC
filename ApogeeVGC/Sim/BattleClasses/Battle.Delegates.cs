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

        // Optimize for the most common cases (1-5 parameters)
        // This avoids array allocation for the majority of callbacks
        object? returnValue;
        switch (paramCount)
        {
            case 1:
                returnValue = del.DynamicInvoke(BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect));
                return ConvertReturnValueToRelayVar(returnValue);
            case 2:
                returnValue = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1)
                );
                return ConvertReturnValueToRelayVar(returnValue);
            case 3:
                returnValue = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1),
                    BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source, sourceEffect, 2)
                );
                return ConvertReturnValueToRelayVar(returnValue);
            case 4:
                returnValue = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1),
                    BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source, sourceEffect, 2),
                    BuildSingleArg(parameters[3], hasRelayVar, relayVar, target, source, sourceEffect, 3)
                );
                return ConvertReturnValueToRelayVar(returnValue);
        }

        // Fallback for 5+ parameters (rare)
        // Use array allocation for these cases
        object?[] args = new object?[paramCount];
        int argIndex = 0;

        // First parameter is typically Battle (this)
        if (parameters[0].ParameterType.IsAssignableFrom(typeof(Battle)))
        {
            args[argIndex++] = this;
        }

        // Add relayVar if it was explicitly provided and if the delegate expects it
        if (hasRelayVar)
        {
            args[argIndex++] = relayVar;
        }

        // Add remaining standard parameters: target, source, sourceEffect
        while (argIndex < paramCount)
        {
            Type paramType = parameters[argIndex].ParameterType;

            // Try to match target parameter
            if (target != null)
            {
                EventTargetParameter? targetParam = EventTargetParameter.FromSingleEventTarget(target, paramType);
                if (targetParam != null)
                {
                    args[argIndex++] = targetParam.ToObject();
                    continue;
                }
            }

            // Try to match source parameter
            if (source != null)
            {
                EventSourceParameter? sourceParam = EventSourceParameter.FromSingleEventSource(source, paramType);
                if (sourceParam != null)
                {
                    args[argIndex++] = sourceParam.ToObject();
                    continue;
                }
            }

            // Try to match sourceEffect parameter
            if (sourceEffect != null && paramType.IsInstanceOfType(sourceEffect))
            {
                args[argIndex++] = sourceEffect;
                continue;
            }

            // If we couldn't match, add null
            args[argIndex++] = null;
        }

        returnValue = del.DynamicInvoke(args);
        return ConvertReturnValueToRelayVar(returnValue);
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

        // First parameter is typically Battle
        if (position == 0 && paramType.IsAssignableFrom(typeof(Battle)))
        {
            return this;
        }

        // Second parameter might be relayVar if explicitly provided
        if (position == 1 && hasRelayVar)
        {
            return relayVar;
        }

        // Adjust position if Battle was first
        int adjustedPos = position;
        if (position > 0 && paramType.IsAssignableFrom(typeof(Battle)))
        {
            adjustedPos--;
        }
        if (hasRelayVar && adjustedPos > 0)
        {
            adjustedPos--;
        }

        // Try to match standard parameters in order: target, source, sourceEffect
        switch (adjustedPos)
        {
            case 0:
                // Try target first
                if (target != null)
                {
                    EventTargetParameter? targetParam = EventTargetParameter.FromSingleEventTarget(target, paramType);
                    if (targetParam != null) return targetParam.ToObject();
                }
                break;
            case 1:
                // Try source second
                if (source != null)
                {
                    EventSourceParameter? sourceParam = EventSourceParameter.FromSingleEventSource(source, paramType);
                    if (sourceParam != null) return sourceParam.ToObject();
                }
                break;
            case 2:
                // Try sourceEffect third
                if (sourceEffect != null && paramType.IsInstanceOfType(sourceEffect))
                {
                    return sourceEffect;
                }
                break;
        }

        return null;
    }
}