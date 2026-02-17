using System.Reflection;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyTryHeal event.
/// Triggered when attempting to heal a Pokemon.
/// Has multiple signatures:
/// - (Battle, int, Pokemon, Pokemon, IEffect) => IntBoolUnion?
/// - (Battle, Pokemon) => bool?
/// - bool constant
/// </summary>
public sealed record OnAnyTryHealEventInfo : UnionEventHandlerInfo<OnTryHeal>
{
   public OnAnyTryHealEventInfo(
       EventHandlerDelegate contextHandler,
       int? priority = null,
       bool usesSpeed = true)
   {
       Id = EventId.TryHeal;
       Prefix = EventPrefix.Any;
       ContextHandler = contextHandler;
       Priority = priority;
       UsesSpeed = usesSpeed;
   }

   /// <summary>
   /// Creates strongly-typed context-based handler (5-param signature).
   /// </summary>
   public static OnAnyTryHealEventInfo Create(
       Func<Battle, int, Pokemon, Pokemon, IEffect, RelayVar?> handler,
       int? priority = null,
       bool usesSpeed = true)
   {
       return new OnAnyTryHealEventInfo(
           context => handler(
               context.Battle,
               context.GetIntRelayVar(),
               context.GetTargetOrSourcePokemon(),
               context.GetSourceOrTargetPokemon(),
               context.GetSourceEffect<IEffect>()
           ),
           priority,
           usesSpeed
       );
   }

   /// <summary>
   /// Creates strongly-typed context-based handler (2-param signature).
   /// </summary>
   public static OnAnyTryHealEventInfo Create(
       Func<Battle, Pokemon, RelayVar?> handler,
       int? priority = null,
       bool usesSpeed = true)
   {
       return new OnAnyTryHealEventInfo(
           context => handler(
               context.Battle,
               context.GetTargetOrSourcePokemon()
           ),
           priority,
           usesSpeed
       );
   }

   /// <summary>
   /// Custom validation for OnAnyTryHeal which supports multiple delegate signatures.
   /// </summary>
    public new void Validate()
    {
      if (UnionValue == null) return;

        // If it's a constant value, skip delegate validation
        if (IsConstantValue())
        {
        return;
     }

        // If it's a delegate, determine which signature and validate
        var extractedDelegate = ExtractDelegate();
        if (extractedDelegate == null) return;

        MethodInfo method = extractedDelegate.Method;
        var actualParams = method.GetParameters();
    int paramCount = actualParams.Length;

// Determine expected types based on parameter count
        Type[]? expectedParamTypes;
        Type? expectedRetType;

        // Signature 1: (Battle, int, Pokemon, Pokemon, IEffect) => IntBoolUnion?
        if (paramCount == 5)
        {
      expectedParamTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
         expectedRetType = typeof(IntBoolUnion);
    }
        // Signature 2: (Battle, Pokemon) => bool?
    else if (paramCount == 2)
        {
        expectedParamTypes = [typeof(Battle), typeof(Pokemon)];
    expectedRetType = typeof(bool);
   }
        else
        {
      throw new InvalidOperationException(
 $"OnAnyTryHeal: Invalid parameter count {paramCount}. Expected 5 or 2.");
        }

        // Perform validation using local variables
        ValidateParameters(method, actualParams, expectedParamTypes);
        ValidateReturnType(method, expectedRetType);
    }

  private void ValidateParameters(MethodInfo method, ParameterInfo[] actualParams, Type[] expectedParamTypes)
    {
        if (actualParams.Length != expectedParamTypes.Length)
        {
 throw new InvalidOperationException(
                $"Event {Id}: Expected {expectedParamTypes.Length} parameters, " +
$"got {actualParams.Length}. " +
      $"Expected: ({string.Join(", ", expectedParamTypes.Select(t => t.Name))}), " +
    $"Got: ({string.Join(", ", actualParams.Select(p => p.ParameterType.Name))})");
    }

      for (int i = 0; i < actualParams.Length; i++)
     {
Type expectedType = expectedParamTypes[i];
            Type actualType = actualParams[i].ParameterType;

     Type expectedBase = Nullable.GetUnderlyingType(expectedType) ?? expectedType;
      Type actualBase = Nullable.GetUnderlyingType(actualType) ?? actualType;

   if (!expectedBase.IsAssignableFrom(actualBase))
         {
      throw new InvalidOperationException(
    $"Event {Id}: Parameter {i} ({actualParams[i].Name}) type mismatch. " +
             $"Expected: {expectedType.Name}, Got: {actualType.Name}");
          }
        }
    }

    private void ValidateReturnType(MethodInfo method, Type expectedRetType)
    {
     Type actualReturnType = method.ReturnType;
    Type actualBase = Nullable.GetUnderlyingType(actualReturnType) ?? actualReturnType;
        Type expectedBase = Nullable.GetUnderlyingType(expectedRetType) ?? expectedRetType;

        if (!expectedBase.IsAssignableFrom(actualBase))
        {
       throw new InvalidOperationException(
       $"Event {Id}: Return type mismatch. " +
    $"Expected: {expectedRetType.Name} (or nullable variant), " +
          $"Got: {actualReturnType.Name}");
        }
    }
}
