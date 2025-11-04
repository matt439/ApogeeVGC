using System.Reflection;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    private RelayVar? InvokeDelegateEffectDelegate(Delegate del, bool hasRelayVar,
        RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect)
    {
        // Cache parameter info to avoid repeated reflection calls
        var parameters = del.Method.GetParameters();
        int paramCount = parameters.Length;

        // Most common signature: (IBattle battle, ...)
        if (paramCount == 0)
        {
            object? invokeResult = del.DynamicInvoke(null);
            return ConvertToRelayVar(invokeResult);
        }

        // Check if first parameter is IBattle (used for proper position tracking)
        bool firstParamIsBattle = paramCount > 0 &&
                                  parameters[0].ParameterType.IsAssignableFrom(typeof(IBattle));

        // Optimize for the most common cases (1-5 parameters)
        // This avoids array allocation for the majority of callbacks
        object? invokeResult2;
        try
        {
            switch (paramCount)
            {
                case 1:
                    invokeResult2 = del.DynamicInvoke(BuildSingleArg(parameters[0], hasRelayVar,
                        relayVar, target, source, sourceEffect, 0, firstParamIsBattle));
                    return ConvertToRelayVar(invokeResult2);
                case 2:
                    invokeResult2 = del.DynamicInvoke(
                        BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source,
                            sourceEffect, 0, firstParamIsBattle),
                        BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source,
                            sourceEffect, 1, firstParamIsBattle)
                    );
                    return ConvertToRelayVar(invokeResult2);
                case 3:
                    invokeResult2 = del.DynamicInvoke(
                        BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source,
                            sourceEffect, 0, firstParamIsBattle),
                        BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source,
                            sourceEffect, 1, firstParamIsBattle),
                        BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source,
                            sourceEffect, 2, firstParamIsBattle)
                    );
                    return ConvertToRelayVar(invokeResult2);
                case 4:
                    invokeResult2 = del.DynamicInvoke(
                        BuildSingleArg(parameters[0], hasRelayVar, relayVar, target, source,
                            sourceEffect, 0, firstParamIsBattle),
                        BuildSingleArg(parameters[1], hasRelayVar, relayVar, target, source,
                            sourceEffect, 1, firstParamIsBattle),
                        BuildSingleArg(parameters[2], hasRelayVar, relayVar, target, source,
                            sourceEffect, 2, firstParamIsBattle),
                        BuildSingleArg(parameters[3], hasRelayVar, relayVar, target, source,
                            sourceEffect, 3, firstParamIsBattle)
                    );
                    return ConvertToRelayVar(invokeResult2);
            }
        }
        catch (TargetParameterCountException ex)
        {
            if (DisplayUi)
            {
                Debug($"Parameter count mismatch for {del.Method.Name}:");
                Debug($"  Expected {paramCount} params: {string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"))}");
                Debug($"  Has relay var: {hasRelayVar}");
                Debug($"  Target: {target?.GetType().Name ?? "null"}");
                Debug($"  Source: {source?.GetType().Name ?? "null"}");
                Debug($"  SourceEffect: {sourceEffect?.GetType().Name ?? "null"}");
            }
            throw;
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
            // Unwrap the relayVar to match the expected parameter type
            Type expectedType = parameters[argIndex].ParameterType;
            object? unwrapped = UnwrapRelayVar(relayVar, expectedType);
            if (unwrapped != null)
            {
                args[argIndex++] = unwrapped;
            }
            else if (expectedType.IsAssignableFrom(relayVar.GetType()))
            {
                // If unwrapping failed but the type matches, use the RelayVar itself
                args[argIndex++] = relayVar;
            }
            else
            {
                // Skip this parameter if we can't match it
                // This shouldn't happen in normal operation
                argIndex++;
            }
        }

        // Add remaining standard parameters: target, source, sourceEffect
        while (argIndex < paramCount)
        {
            Type paramType = parameters[argIndex].ParameterType;

            // Try to match target parameter
            if (target != null)
            {
                EventTargetParameter? targetParam =
                    EventTargetParameter.FromSingleEventTarget(target, paramType);
                if (targetParam != null)
                {
                    args[argIndex++] = targetParam.ToObject();
                    continue;
                }
            }

            // Try to match source parameter
            if (source != null)
            {
                EventSourceParameter? sourceParam =
                    EventSourceParameter.FromSingleEventSource(source, paramType);
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

        object? finalResult = del.DynamicInvoke(args);
        return ConvertToRelayVar(finalResult);
    }

    /// <summary>
    /// Converts common return types from event handlers to RelayVar.
    /// Handles BoolEmptyVoidUnion and other union types that may be returned.
    /// </summary>
    private RelayVar? ConvertToRelayVar(object? result)
    {
        if (result == null) return null;

        // If it's already a RelayVar, return it
        if (result is RelayVar relay) return relay;

        // Handle BoolIntUndefinedUnion (from TryPrimaryHit and similar events)
        if (result is BoolIntUndefinedUnion boolIntUndef)
        {
            return new BoolIntUndefinedUnionRelayVar(boolIntUndef);
        }

        // Handle BoolEmptyVoidUnion (from ResultMove handlers)
        if (result is BoolEmptyVoidUnion boolEmptyVoid)
        {
            return boolEmptyVoid switch
            {
                BoolBoolEmptyVoidUnion b => new BoolRelayVar(b.Value),
                EmptyBoolEmptyVoidUnion => RelayVar.FromUndefined(), // Empty maps to undefined
                VoidUnionBoolEmptyVoidUnion => RelayVar.FromVoid(), // Void means no return value
                _ => null,
            };
        }

        // Handle BoolIntEmptyVoidUnion
        if (result is BoolIntEmptyVoidUnion boolIntEmptyVoid)
        {
            return boolIntEmptyVoid switch
            {
                BoolBoolIntEmptyVoidUnion b => new BoolRelayVar(b.Value),
                IntBoolIntEmptyVoidUnion i => new IntRelayVar(i.Value),
                EmptyBoolIntEmptyVoidUnion => RelayVar.FromUndefined(),
                VoidUnionBoolIntEmptyVoidUnion => RelayVar.FromVoid(),
                _ => null
            };
        }

        // Handle primitive types
        if (result is bool boolVal) return new BoolRelayVar(boolVal);
        if (result is int intVal) return new IntRelayVar(intVal);

        // If we don't know how to convert, return null
        return null;
    }

    /// <summary>
    /// Builds a single argument for delegate invocation.
    /// Used by the optimized fast-path for common parameter counts.
    /// </summary>
    private object? BuildSingleArg(ParameterInfo param, bool hasRelayVar, RelayVar relayVar,
        SingleEventTarget? target, SingleEventSource? source, IEffect? sourceEffect,
        int position = 0,
        bool firstParamIsBattle = false)
    {
        Type paramType = param.ParameterType;

        // First parameter is typically IBattle
        if (position == 0 && paramType.IsAssignableFrom(typeof(IBattle)))
        {
            return this;
        }

        // Second parameter might be relayVar if explicitly provided
        // BUT: we need to unwrap the RelayVar to match the expected parameter type
        if (hasRelayVar && ((firstParamIsBattle && position == 1) ||
                            (!firstParamIsBattle && position == 0)))
        {
            // Try to unwrap the RelayVar to match the parameter type
            object? unwrapped = UnwrapRelayVar(relayVar, paramType);
            if (unwrapped != null) return unwrapped;
            // If unwrapping failed, return the RelayVar itself (for delegates that expect RelayVar)
            if (paramType.IsAssignableFrom(relayVar.GetType()))
 {
    return relayVar;
 }
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
      EventTargetParameter? targetParam =
              EventTargetParameter.FromSingleEventTarget(target, paramType);
      if (targetParam != null) return targetParam.ToObject();
          }

  break;
      case 1:
          // Try source second
       if (source != null)
          {
        EventSourceParameter? sourceParam =
        EventSourceParameter.FromSingleEventSource(source, paramType);
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

    /// <summary>
    /// Unwraps a RelayVar to extract the underlying value that matches the expected parameter type.
    /// </summary>
    private object? UnwrapRelayVar(RelayVar relayVar, Type expectedType)
    {
        return relayVar switch
        {
 BoolRelayVar b when expectedType == typeof(bool) => b.Value,
        IntRelayVar i when expectedType == typeof(int) => i.Value,
       EffectRelayVar e when expectedType.IsAssignableFrom(e.Effect.GetType()) => e.Effect,
 PokemonTypeRelayVar t when expectedType == typeof(PokemonType) => t.Type,
       PokemonRelayVar p when expectedType.IsAssignableFrom(typeof(Pokemon)) => p.Pokemon,
            MoveIdRelayVar m when expectedType == typeof(MoveId) => m.MoveId,
  StringRelayVar s when expectedType == typeof(string) => s.Value,
            DecimalRelayVar d when expectedType == typeof(decimal) => d.Value,
     TypesRelayVar t when expectedType.IsAssignableFrom(typeof(List<PokemonType>)) => t.Types,
BoostsTableRelayVar b when expectedType.IsAssignableFrom(typeof(BoostsTable)) => b.Table,
  SparseBoostsTableRelayVar s when expectedType.IsAssignableFrom(typeof(SparseBoostsTable)) => s.Table,
     _ => null,
        };
    }
}