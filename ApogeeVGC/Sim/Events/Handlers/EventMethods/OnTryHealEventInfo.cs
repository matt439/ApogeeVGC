using System.Reflection;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryHeal event.
/// Triggered when attempting to heal a Pokemon.
/// Has multiple signatures:
/// - (Battle, int, Pokemon, Pokemon, IEffect) => IntBoolUnion?
/// - (Battle, Pokemon) => bool?
/// - bool constant
/// </summary>
public sealed record OnTryHealEventInfo : UnionEventHandlerInfo<OnTryHeal>
{
    /// <summary>
    /// Creates a new OnTryHeal event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate with multiple possible signatures or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTryHealEventInfo(
  OnTryHeal unionValue,
     int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.TryHeal;
   UnionValue = unionValue;
     Handler = ExtractDelegate();
        Priority = priority;
      UsesSpeed = usesSpeed;
 // Don't set ExpectedParameterTypes/ExpectedReturnType here
  // because OnTryHeal has 2 different delegate signatures
        // Validation will determine which signature is used and validate without setting properties
  ExpectedParameterTypes = null;
  ExpectedReturnType = null;
   
        // Nullability info varies by signature - will be checked in custom Validate()
  // Signature 1: (Battle, int, Pokemon, Pokemon, IEffect) - all non-nullable
        // Signature 2: (Battle, Pokemon) - all non-nullable  
   // Return type: nullable for both signatures
        ParameterNullability = null; // Handled in custom validation
        ReturnTypeNullable = true; // Both signatures return nullable types
    
      // Note: Don't call ValidateConfiguration() here because ExpectedParameterTypes is null
        // Custom validation happens in Validate() method
    }

    /// <summary>
    /// Custom validation for OnTryHeal which supports multiple delegate signatures.
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
          $"OnTryHeal: Invalid parameter count {paramCount}. Expected 5 or 2.");
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
