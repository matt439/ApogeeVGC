using System.Reflection;
using System.Text.Json.Nodes;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Utils;

public static partial class State
{
    public static object? SerializeWithRefs(object? obj, IBattle battle)
    {
  switch (obj)
        {
            case null:
      return null;
    
            // Delegates (functions) should be elided (return undefined, which we represent as null)
        case Delegate:
    return null;
                
   // Primitive types - serialize as-is
    case bool b:
         return b;
 case int i:
       return i;
    case long l:
                return l;
   case double d:
                return d;
            case decimal dec:
  return dec;
            case string s:
    return s;
       
       // Special handling for ActiveMove - serialize with only changed fields
  case ActiveMove activeMove:
         return SerializeActiveMove(activeMove, battle);
      
            // Check if this is a referable type (circular reference handling)
            // Item, Ability, Condition, Move, and Species are referable (immutable from library)
            case IBattle:
            case Field:
      case Side:
     case Pokemon:
            case Condition:
            case Ability:
            case Item:
    case Move:
    case Species:
      // Convert to Referable union type and then to reference string
  Referable referable = obj switch
  {
      IBattle b => Referable.FromIBattle(b),
  Field f => f,
      Side side => side,
      Pokemon p => p,
   Condition c => c,
   Ability a => a,
           Item i => i,
  Move m => (Referable)m,
 Species sp => sp,
          _ => throw new InvalidOperationException($"Unhandled referable type: {obj.GetType()}")
      };
    return ToRef(referable);
    
 // Handle collections
         case System.Collections.IEnumerable enumerable when obj.GetType().IsArray || 
            obj.GetType().IsGenericType && 
  obj.GetType().GetGenericTypeDefinition() == typeof(List<>):
    var list = new List<object?>();
       foreach (object? item in enumerable)
         {
          object? serialized = SerializeWithRefs(item, battle);
   // Only add if not null/undefined (elide null delegate results)
           if (serialized != null)
        {
      list.Add(serialized);
 }
      }
              return list;
        
            // Handle dictionaries
      case System.Collections.IDictionary dictionary:
        var dict = new Dictionary<string, object?>();
    foreach (System.Collections.DictionaryEntry entry in dictionary)
    {
       string key = entry.Key.ToString() ?? throw new InvalidOperationException("Dictionary key cannot be null");
        object? serialized = SerializeWithRefs(entry.Value, battle);
        // Only add if not null/undefined (elide null delegate results)
           if (serialized != null)
        {
   dict[key] = serialized;
    }
 }
          return dict;
        
    // Handle enums - serialize as string
        case Enum enumValue:
          return enumValue.ToString();
    
         default:
           // For plain objects (POCOs), serialize all public properties
          if (obj.GetType().IsClass && obj.GetType() != typeof(string))
  {
                 // If we're getting this error, some 'special' field has been added to
  // an object and we need to update the logic in this file to handle it.
            // The most common case is that someone added a Set/Map which probably
   // needs to be serialized as an Array/Object respectively.
            
     // Only serialize simple DTOs/POCOs, not complex types we haven't explicitly handled
     if (obj.GetType().Namespace?.StartsWith("ApogeeVGC") == true)
    {
                throw new NotSupportedException(
   $"Unsupported type {obj.GetType().Name}: {obj}. " +
         "This type needs explicit handling in SerializeWithRefs.");
   }
     
    // For external types, try to serialize properties
var result = new Dictionary<string, object?>();
           var properties = obj.GetType().GetProperties(BindingFlags.Public | 
    BindingFlags.Instance);
      foreach (PropertyInfo prop in properties)
            {
    if (prop.CanRead)
     {
          object? value = prop.GetValue(obj);
   object? serialized = SerializeWithRefs(value, battle);
         // Only add if not null/undefined
 if (serialized != null)
       {
 result[ToCamelCase(prop.Name)] = serialized;
              }
             }
        }
     return result;
    }
       
      throw new NotSupportedException($"Cannot serialize type {obj.GetType()}: {obj}");
        }
    }

    public static object? DeserializeWithRefs(object? obj, IBattle battle)
    {
        switch (obj)
        {
            case null:
                return null;
                
            // Primitive types - return as-is
            case bool b:
                return b;
            case int i:
                return i;
            case long l:
                return l;
            case double d:
                return d;
            case decimal dec:
                return dec;
                
            // Check if this is a reference string
            case string s:
                ReferableUndefinedUnion refResult = FromRef(s, battle);
                return refResult switch
                {
                    ReferableReferableUndefinedUnion r => r.Referable switch
                    {
                        BattleReferable b => b.Battle,
                        FieldReferable f => f.Field,
                        SideReferable side => side.Side,
                        PokemonReferable p => p.Pokemon,
                        ConditionReferable c => c.Condition,
                        AbilityReferable a => a.Ability,
                        ItemReferable i => i.Item,
                        MoveReferable m => m.Move,
                        SpeciesReferable ss => ss.Species,
                        _ => s // Not a reference, return original string
                    },
                    UndefinedReferableUndefinedUnion => s, // Not a reference, return original string
                    _ => s,
                };
                
            // Handle arrays/lists stored as JsonArray or List
            case JsonArray jsonArray:
                var list = jsonArray.Select(item =>
                    DeserializeWithRefs(item?.AsValue().GetValue<object>(), battle)).ToList();
                return list;
                
            case System.Collections.IList listObj:
                var resultList = (from object? item in listObj
                    select DeserializeWithRefs(item, battle)).ToList();
                return resultList;
                
            // Handle objects stored as JsonObject or Dictionary
            case JsonObject jsonObject:
                // Check if this is an ActiveMove
                if (IsActiveMove(jsonObject))
                {
                    return DeserializeActiveMove(jsonObject, battle);
                }
                
                // Check if this is an Item, Ability, or Condition serialized object
                // These will be reconstructed from the battle's Library based on their ID
                if (jsonObject.ContainsKey("id"))
                {
                    string? idStr = jsonObject["id"]?.GetValue<string>();
                    if (!string.IsNullOrEmpty(idStr))
                    {
                        // Check effectType to determine which type this is
                        if (jsonObject.ContainsKey("effectType"))
                        {
                            string effectType = jsonObject["effectType"]?.GetValue<string>() ?? "";
                            
                            if (effectType == "Item")
                            {
                                // Return reference to the item from Library
                                return ToRef(battle.Library.Items.GetValueOrDefault(Enum.Parse<ItemId>(idStr)));
                            }
                            else if (effectType == "Ability")
                            {
                                // Return reference to the ability from Library
                                return ToRef(battle.Library.Abilities.GetValueOrDefault(Enum.Parse<AbilityId>(idStr)));
                            }
                            else if (effectType == "Condition" || effectType == "Weather" || 
                                     effectType == "Status" || effectType == "Terrain")
                            {
                                // Return reference to the condition from Library
                                return ToRef(battle.Library.Conditions.GetValueOrDefault(Enum.Parse<ConditionId>(idStr)));
                            }
                        }
                    }
                }
                
                var resultDict = new Dictionary<string, object?>();
                foreach (var kvp in jsonObject)
                {
                    resultDict[kvp.Key] = DeserializeWithRefs(kvp.Value, battle);
                }
                return resultDict;
                
            case System.Collections.IDictionary dictionary:
                var dict = new Dictionary<string, object?>();
                foreach (System.Collections.DictionaryEntry entry in dictionary)
                {
                    string key = entry.Key.ToString() ?? throw new InvalidOperationException("Dictionary key cannot be null");
                    dict[key] = DeserializeWithRefs(entry.Value, battle);
                }
                return dict;
                
            default:
                throw new NotSupportedException($"Cannot deserialize type {obj.GetType()}: {obj}");
        }
    }

    /// <summary>
    /// Converts PascalCase property names to camelCase for JSON serialization.
    /// </summary>
    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
            return str;
            
        return char.ToLowerInvariant(str[0]) + str[1..];
    }

    public static JsonObject Serialize(object obj, List<string> skip, IBattle battle)
    {
        var state = new JsonObject();
        Type type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | 
                                           BindingFlags.Instance);
        
        foreach (PropertyInfo prop in properties)
        {
    // Skip properties in the skip list (case-insensitive comparison for flexibility)
            if (skip.Any(s => s.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)))
   {
    continue;
         }
            
   // Skip properties that can't be read
     if (!prop.CanRead)
   {
         continue;
    }
      
   try
      {
       object? value = prop.GetValue(obj);
        object? serialized = SerializeWithRefs(value, battle);
        
          // JSON.stringify will get rid of keys with undefined values anyway, but
    // we also do it here so that comparisons work correctly.
              // In C#, we represent "undefined" as null for delegates that were never set.
      if (serialized != null)
          {
    // Convert property name to camelCase for JSON
     string jsonKey = ToCamelCase(prop.Name);
 
        // Handle different serialized types
         state[jsonKey] = serialized switch
        {
            JsonObject jo => jo,
      JsonArray ja => ja,
          bool b => JsonValue.Create(b),
        int i => JsonValue.Create(i),
   long l => JsonValue.Create(l),
       double d => JsonValue.Create(d),
 decimal dec => JsonValue.Create(dec),
        string s => JsonValue.Create(s),
 List<object?> list => new JsonArray(list.Select(item => item switch
      {
        JsonNode node => node,
       null => null,
    bool b => JsonValue.Create(b),
           int i => JsonValue.Create(i),
          long l => JsonValue.Create(l),
      double d => JsonValue.Create(d),
   decimal dec => JsonValue.Create(dec),
       string s => JsonValue.Create(s),
    _ => JsonValue.Create(item.ToString())
  }).ToArray()),
    Dictionary<string, object?> dict => new JsonObject(
    dict.Select(kvp => new KeyValuePair<string, JsonNode?>(
 kvp.Key,
     kvp.Value switch
      {
          JsonNode node => node,
  null => null,
       bool b => JsonValue.Create(b),
        int i => JsonValue.Create(i),
      long l => JsonValue.Create(l),
   double d => JsonValue.Create(d),
    decimal dec => JsonValue.Create(dec),
 string s => JsonValue.Create(s),
       _ => JsonValue.Create(kvp.Value.ToString()),
       }
))
  ),
        _ => JsonValue.Create(serialized.ToString()),
        };
            }
  }
  catch (Exception ex)
     {
           // Log or handle serialization errors for specific properties
     throw new InvalidOperationException(
            $"Failed to serialize property '{prop.Name}' of type '{type.Name}': {ex.Message}", 
              ex);
    }
     }
        
        return state;
    }

    public static void Deserialize(JsonObject state, object obj, List<string> skip, IBattle battle)
    {
        Type type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | 
                                           BindingFlags.Instance)
                             .ToDictionary(p => ToCamelCase(p.Name), p => p, 
                                         StringComparer.OrdinalIgnoreCase);
        
        foreach ((string key, JsonNode? jsonValue) in state)
        {
            // Skip properties in the skip list (already camelCased)
            if (skip.Any(s => ToCamelCase(s).Equals(key, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }
            
            // Find the property (case-insensitive)
            if (!properties.TryGetValue(key, out PropertyInfo? prop))
            {
                // Property doesn't exist on the object, skip it
                continue;
            }
            
            // Skip properties that can't be written
            if (!prop.CanWrite)
            {
                continue;
            }
            
            try
            {
                if (jsonValue == null)
                {
                    prop.SetValue(obj, null);
                    continue;
                }
                
                // Deserialize the value with reference resolution
                object? deserializedValue = DeserializeWithRefs(jsonValue, battle);
                
                // Try to convert the deserialized value to the property type
                object? convertedValue = ConvertToPropertyType(deserializedValue, prop.PropertyType);
                
                prop.SetValue(obj, convertedValue);
            }
            catch (Exception ex)
            {
                // Log or handle deserialization errors for specific properties
                throw new InvalidOperationException(
                    $"Failed to deserialize property '{prop.Name}' of type '{type.Name}': {ex.Message}", 
                    ex);
            }
        }
    }

    /// <summary>
    /// Attempts to convert a deserialized value to the target property type.
    /// </summary>
    private static object? ConvertToPropertyType(object? value, Type targetType)
    {
        while (true)
        {
            if (value == null)
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }

            // If already the correct type, return as-is
            if (targetType.IsInstanceOfType(value))
            {
                return value;
            }

            // Handle nullable types
            Type? underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                targetType = underlyingType;
                continue;
            }

            // Handle enums
            if (targetType.IsEnum)
            {
                if (value is string strValue)
                {
                    return Enum.Parse(targetType, strValue, ignoreCase: true);
                }

                return Enum.ToObject(targetType, value);
            }

            // Handle numeric conversions
            if (targetType.IsPrimitive || targetType == typeof(decimal))
            {
                return Convert.ChangeType(value, targetType);
            }

            // Handle collections
            if (value is List<object?> list)
            {
                if (targetType.IsArray)
                {
                    Type elementType = targetType.GetElementType()!;
                    var array = Array.CreateInstance(elementType, list.Count);
                    for (int i = 0; i < list.Count; i++)
                    {
                        array.SetValue(ConvertToPropertyType(list[i], elementType), i);
                    }

                    return array;
                }

                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type elementType = targetType.GetGenericArguments()[0];
                    var listInstance = (System.Collections.IList)Activator.CreateInstance(targetType)!;
                    foreach (object? item in list)
                    {
                        listInstance.Add(ConvertToPropertyType(item, elementType));
                    }

                    return listInstance;
                }
            }

            // Handle dictionaries
            if (value is Dictionary<string, object?> dict)
            {
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type keyType = targetType.GetGenericArguments()[0];
                    Type valueType = targetType.GetGenericArguments()[1];
                    var dictInstance = (System.Collections.IDictionary)Activator.CreateInstance(targetType)!;

                    foreach (var kvp in dict)
                    {
                        object? key = ConvertToPropertyType(kvp.Key, keyType);
                        object? val = ConvertToPropertyType(kvp.Value, valueType);
                        dictInstance.Add(key!, val);
                    }

                    return dictInstance;
                }
            }

            // If we can't convert, return the value as-is and hope for the best
            return value;
        }
    }
}
