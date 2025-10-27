using System.Reflection;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    private RelayVar? InvokeDelegateEffectDelegate(Delegate del, bool hasRelayVar, RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect)
    {
        // Cache parameter info to avoid repeated reflection calls
        var parameters = del.Method.GetParameters();
        int paramCount = parameters.Length;

        // Most common signature: (IBattle battle, ...)
        if (paramCount == 0)
        {
            return (RelayVar?)del.DynamicInvoke(null);
        }

        // Check if first parameter is IBattle (used for proper position tracking)
        bool firstParamIsBattle = paramCount > 0 && parameters[0].ParameterType.IsAssignableFrom(typeof(IBattle));

        // Optimize for the most common cases (1-5 parameters)
        // This avoids array allocation for the majority of callbacks
        object? result;
        switch (paramCount)
        {
            case 1:
                result = del.DynamicInvoke(BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect, 0, firstParamIsBattle));
                return (RelayVar?)result;
            case 2:
                result = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect, 0, firstParamIsBattle),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1, firstParamIsBattle)
                );
                return (RelayVar?)result;
            case 3:
                result = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect, 0, firstParamIsBattle),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1, firstParamIsBattle),
                    BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source, sourceEffect, 2, firstParamIsBattle)
                );
                return (RelayVar?)result;
            case 4:
                result = del.DynamicInvoke(
                    BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source, sourceEffect, 0, firstParamIsBattle),
                    BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source, sourceEffect, 1, firstParamIsBattle),
                    BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source, sourceEffect, 2, firstParamIsBattle),
                    BuildSingleArg(parameters[3], hasRelayVar, relayVar, target, source, sourceEffect, 3, firstParamIsBattle)
                );
                return (RelayVar?)result;
        }

        // Fallback for 5+ parameters (rare)
        // Use array allocation for these cases
        object?[] args = new object?[paramCount];
        int argIndex = 0;

        // First parameter is typically IBattle (this)
        if (firstParamIsBattle)
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

        result = del.DynamicInvoke(args);
        return (RelayVar?)result;
    }

    /// <summary>
    /// Builds a single argument for delegate invocation.
    /// Used by the optimized fast-path for common parameter counts.
    /// </summary>
    private object? BuildSingleArg(ParameterInfo param, bool hasRelayVar, RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect, int position = 0,
        bool firstParamIsBattle = false)
    {
        Type paramType = param.ParameterType;

        // First parameter is typically IBattle
        if (position == 0 && paramType.IsAssignableFrom(typeof(IBattle)))
        {
            return this;
        }

        // Second parameter might be relayVar if explicitly provided
        if (hasRelayVar && ((firstParamIsBattle && position == 1) || (!firstParamIsBattle && position == 0)))
        {
            return relayVar;
        }

        // Calculate adjusted position for matching to target/source/sourceEffect
        // We need to account for parameters consumed by IBattle and relayVar
        int adjustedPos = position;

        // If first param was IBattle and we're past it, adjust down
        if (firstParamIsBattle && position > 0)
        {
            adjustedPos--;
        }

        // If relayVar was provided and we're past where it would be, adjust down
        if (hasRelayVar)
        {
            int relayVarPos = firstParamIsBattle ? 1 : 0;
            if (position > relayVarPos)
            {
                adjustedPos--;
            }
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