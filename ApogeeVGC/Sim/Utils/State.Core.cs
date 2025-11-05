using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;
using System.Reflection;
using System.Text.Json.Nodes;

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

            // Special handling for Pokemon Showdown protocol types
            case Nature nature:
                return nature.ShowdownId;

            case StatsTable stats:
                return SerializeStatsTable(stats);

            case PokemonSet pokemonSet:
                return SerializePokemonSet(pokemonSet);

            case PlayerOptions playerOptions:
                return SerializePlayerOptions(playerOptions);

            // Special handling for ActiveMove - serialize with only changed fields
            case ActiveMove activeMove:
                return SerializeActiveMove(activeMove, battle);

            // Special handling for request data classes - serialize only relevant fields
            case PokemonMoveData moveData:
                return new Dictionary<string, object?>
                {
                    ["move"] = moveData.Move.Name,
                    ["id"] = moveData.Id.ToString().ToLower(),
                    ["pp"] = moveData.Pp,
                    ["maxpp"] = moveData.MaxPp,
                    ["target"] = moveData.Target?.ToShowdownId(),
                    ["disabled"] = SerializeWithRefs(moveData.Disabled, battle)
                };

            case PokemonSwitchRequestData switchData:
                return new Dictionary<string, object?>
                {
                    ["ident"] = switchData.Ident,
                    ["details"] = switchData.Details,
                    ["condition"] = switchData.Condition.ToString(),
                    ["active"] = switchData.Active,
                    ["stats"] = switchData.Stats, // Already a Dictionary<string, int> without HP
                    ["moves"] = switchData.MoveNames.ToList(),
                    ["baseAbility"] = switchData.BaseAbilityName.ToLower(),
                    ["item"] = switchData.ItemName.ToLower(),
                    ["pokeball"] = switchData.Pokeball.ToString().ToLower(),
                    ["ability"] = switchData.AbilityName.ToLower(),
                    ["commanding"] = switchData.Commanding,
                    ["reviving"] = switchData.Reviving,
                    ["teraType"] = switchData.TeraType.ToString().ToLower(),
                    ["terastallized"] = switchData.Terastallized
                };

            case PokemonMoveRequestData moveReqData:
                return SerializeChoiceRequest(moveReqData, battle);

            case SideRequestData sideReqData:
                return SerializeChoiceRequest(sideReqData, battle);

            // Handle choice request types - serialize using reflection to get all properties
            case MoveRequest moveRequest:
                return SerializeChoiceRequest(moveRequest, battle);

            case SwitchRequest switchRequest:
                return SerializeChoiceRequest(switchRequest, battle);

            case TeamPreviewRequest teamRequest:
                return SerializeChoiceRequest(teamRequest, battle);

            case WaitRequest waitRequest:
                return SerializeChoiceRequest(waitRequest, battle);

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
                    _ => throw new InvalidOperationException(
                        $"Unhandled referable type: {obj.GetType()}")
                };
                return ToRef(referable);

            // Handle collections
            case System.Collections.IEnumerable enumerable when obj.GetType().IsArray ||
                                                                obj.GetType().IsGenericType &&
                                                                obj.GetType()
                                                                    .GetGenericTypeDefinition() ==
                                                                typeof(List<>):
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
                    string key = entry.Key.ToString() ??
                                 throw new InvalidOperationException(
                                     "Dictionary key cannot be null");
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
                // Special case for SideId - needs to be lowercase
                if (enumValue is SideId sideId)
                {
                    return sideId.ToString().ToLower();
                }
                return enumValue.ToString();

            // Handle union types - extract the actual value
            case MoveTypeFalseUnion union:
                return union switch
                {
                    MoveTypeMoveTypeFalseUnion m => m.MoveType.ToString().ToLower(),
                    FalseMoveTypeFalseUnion => false,
                    _ => null
                };

            case MoveIdBoolUnion moveIdBoolUnion:
                return moveIdBoolUnion switch
                {
                    MoveIdMoveIdBoolUnion m => m.MoveId.ToString().ToLower(),
                    BoolMoveIdBoolUnion b => b.Value,
                    _ => null
                };

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

    /// <summary>
    /// Serializes a StatsTable to Pokemon Showdown format (lowercase stat names).
    /// </summary>
    private static Dictionary<string, int> SerializeStatsTable(StatsTable stats)
    {
        return new Dictionary<string, int>
        {
            ["hp"] = stats.Hp,
            ["atk"] = stats.Atk,
            ["def"] = stats.Def,
            ["spa"] = stats.SpA,
            ["spd"] = stats.SpD,
            ["spe"] = stats.Spe
        };
    }

    /// <summary>
    /// Serializes a choice request object by recursively serializing its properties.
    /// </summary>
    private static Dictionary<string, object?> SerializeChoiceRequest(object request,
        IBattle battle)
    {
        var result = new Dictionary<string, object?>();
        var properties =
            request.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo prop in properties)
        {
            if (!prop.CanRead) continue;

            object? value = prop.GetValue(request);
            object? serialized = SerializeWithRefs(value, battle);

            if (serialized != null)
            {
                result[ToCamelCase(prop.Name)] = serialized;
            }
        }

        return result;
    }

    /// <summary>
    /// Serializes a PokemonSet to Pokemon Showdown JSON format.
    /// </summary>
    private static Dictionary<string, object?> SerializePokemonSet(PokemonSet set)
    {
        var result = new Dictionary<string, object?>
        {
            ["name"] = set.Name,
            ["species"] = set.Species.ToShowdownId(),
            ["item"] = set.Item.ToShowdownId(),
            ["ability"] = set.Ability.ToShowdownId(),
            ["moves"] =
                set.Moves.Select(m => m.ToShowdownId())
                    .ToArray(), // Convert to array for proper JSON serialization
            ["nature"] = set.Nature.ShowdownId,
            ["evs"] = SerializeStatsTable(set.Evs),
            ["ivs"] = SerializeStatsTable(set.Ivs),
            ["level"] = set.Level,
            ["gender"] = set.Gender.GenderIdString(),
        };

        // Only include optional fields if they differ from defaults
        if (set.Shiny)
        {
            result["shiny"] = true;
        }

        if (set.Happiness != 0)
        {
            result["happiness"] = set.Happiness;
        }

        if (set.Pokeball != PokeballId.Pokeball)
        {
            result["pokeball"] = set.Pokeball.ToShowdownId();
        }

        // Always include teraType for Gen 9
        result["teraType"] = set.TeraType.ToShowdownId();

        return result;
    }

    /// <summary>
    /// Serializes PlayerOptions to Pokemon Showdown format.
    /// </summary>
    private static Dictionary<string, object?> SerializePlayerOptions(PlayerOptions options)
    {
        var result = new Dictionary<string, object?>();

        if (options.Name != null)
        {
            result["name"] = options.Name;
        }

        if (options.Avatar != null)
        {
            result["avatar"] = options.Avatar;
        }

        if (options.Rating != null)
        {
            result["rating"] = options.Rating;
        }

        if (options.Team != null)
        {
            // Serialize team as array of dictionaries with primitive types only
            result["team"] = options.Team.Select(SerializePokemonSet).ToArray();
        }

        if (options.Seed != null)
        {
            result["seed"] = options.Seed;
        }

        return result;
    }

    /// <summary>
    /// Public method to serialize PlayerOptions for Pokemon Showdown protocol without requiring a battle instance.
    /// </summary>
    public static Dictionary<string, object?> SerializePlayerOptionsForShowdown(
        PlayerOptions options)
    {
        return SerializePlayerOptions(options);
    }

    /// <summary>
    /// Deserializes PlayerOptions from Pokemon Showdown JSON format back to C# objects.
    /// </summary>
    public static PlayerOptions DeserializePlayerOptionsFromShowdown(JsonObject json,
        Library library)
    {
        string? name = json.ContainsKey("name") ? json["name"]?.GetValue<string>() : null;
        string? avatar = json.ContainsKey("avatar") ? json["avatar"]?.GetValue<string>() : null;
        int? rating = json.ContainsKey("rating") ? json["rating"]?.GetValue<int>() : null;

        List<PokemonSet>? team = null;
        if (json.ContainsKey("team") && json["team"] is JsonArray teamArray)
        {
            team = teamArray
                .Select(item => DeserializePokemonSetFromShowdown((JsonObject)item!, library))
                .ToList();
        }

        PrngSeed? seed = null;
        if (json.ContainsKey("seed"))
        {
            // Handle seed deserialization if needed
            // seed = ...;
        }

        return new PlayerOptions
        {
            Name = name,
            Avatar = avatar,
            Rating = rating,
            Team = team,
            Seed = seed
        };
    }

    /// <summary>
    /// Deserializes a single PokemonSet from Pokemon Showdown JSON format.
    /// </summary>
    private static PokemonSet DeserializePokemonSetFromShowdown(JsonObject json, Library library)
    {
        try
        {
            string name = json["name"]?.GetValue<string>() ??
                          throw new InvalidOperationException("Pokemon name is required");
            string speciesStr = json["species"]?.GetValue<string>() ??
                                throw new InvalidOperationException("Pokemon species is required");
            string itemStr = json["item"]?.GetValue<string>() ??
                             throw new InvalidOperationException("Pokemon item is required");
            string abilityStr = json["ability"]?.GetValue<string>() ??
                                throw new InvalidOperationException("Pokemon ability is required");
            string natureStr = json["nature"]?.GetValue<string>() ??
                               throw new InvalidOperationException("Pokemon nature is required");
            string genderStr = json["gender"]?.GetValue<string>() ?? "";
            int level = json["level"]?.GetValue<int>() ?? 50;

            // Parse moves
            List<MoveId> moves = new();
            if (json["moves"] is JsonArray movesArray)
            {
                foreach (var moveNode in movesArray)
                {
                    string moveStr = moveNode?.GetValue<string>() ??
                                     throw new InvalidOperationException("Move cannot be null");
                    moves.Add(ParseShowdownId<MoveId>(moveStr));
                }
            }
            else
            {
                throw new InvalidOperationException("Moves must be an array");
            }

            // Parse EVs
            StatsTable evs = new();
            if (json["evs"] is JsonObject evsObj)
            {
                evs.Hp = evsObj["hp"]?.GetValue<int>() ?? 0;
                evs.Atk = evsObj["atk"]?.GetValue<int>() ?? 0;
                evs.Def = evsObj["def"]?.GetValue<int>() ?? 0;
                evs.SpA = evsObj["spa"]?.GetValue<int>() ?? 0;
                evs.SpD = evsObj["spd"]?.GetValue<int>() ?? 0;
                evs.Spe = evsObj["spe"]?.GetValue<int>() ?? 0;
            }

            // Parse IVs
            StatsTable ivs = new();
            if (json["ivs"] is JsonObject ivsObj)
            {
                ivs.Hp = ivsObj["hp"]?.GetValue<int>() ?? 31;
                ivs.Atk = ivsObj["atk"]?.GetValue<int>() ?? 31;
                ivs.Def = ivsObj["def"]?.GetValue<int>() ?? 31;
                ivs.SpA = ivsObj["spa"]?.GetValue<int>() ?? 31;
                ivs.SpD = ivsObj["spd"]?.GetValue<int>() ?? 31;
                ivs.Spe = ivsObj["spe"]?.GetValue<int>() ?? 31;
            }

            // Parse optional fields
            bool shiny = json.ContainsKey("shiny") && json["shiny"]!.GetValue<bool>();
            int happiness = json.ContainsKey("happiness") ? json["happiness"]!.GetValue<int>() : 0;
            PokeballId pokeball = json.ContainsKey("pokeball")
                ? ParseShowdownId<PokeballId>(json["pokeball"]!.GetValue<string>())
                : PokeballId.Pokeball;
            MoveType teraType = json.ContainsKey("teraType")
                ? ParseShowdownId<MoveType>(json["teraType"]!.GetValue<string>())
                : MoveType.Normal;

            // Parse enums from showdown IDs
            SpecieId species = ParseShowdownId<SpecieId>(speciesStr);
            ItemId item = ParseShowdownId<ItemId>(itemStr);
            AbilityId ability = ParseShowdownId<AbilityId>(abilityStr);
            NatureId natureId = ParseShowdownId<NatureId>(natureStr);
            GenderId gender = ParseGenderId(genderStr);

            return new PokemonSet
            {
                Name = name,
                Species = species,
                Item = item,
                Ability = ability,
                Moves = moves,
                Nature = library.Natures[natureId],
                Gender = gender,
                Evs = evs,
                Ivs = ivs,
                Level = level,
                Shiny = shiny,
                Happiness = happiness,
                Pokeball = pokeball,
                TeraType = teraType
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing PokemonSet: {ex.Message}");
            Console.WriteLine($"JSON: {json.ToJsonString()}");
            throw;
        }
    }

    /// <summary>
    /// Parses a Showdown ID string (lowercase, no spaces) back to an enum value.
    /// </summary>
    private static T ParseShowdownId<T>(string showdownId) where T : struct, Enum
    {
// Try to parse directly with case-insensitive matching
        // This should handle most cases like "calyrexice" -> "CalyrexIce"
        if (Enum.TryParse(showdownId, ignoreCase: true, out T result))
        {
            return result;
        }

        throw new ArgumentException(
            $"Could not parse showdown ID '{showdownId}' as {typeof(T).Name}");
    }

    /// <summary>
    /// Parses gender string from Showdown format.
    /// </summary>
    private static GenderId ParseGenderId(string genderStr)
    {
        return genderStr.ToUpperInvariant() switch
        {
            "M" => GenderId.M,
            "F" => GenderId.F,
            "" => GenderId.Empty,
            _ => GenderId.N
        };
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
                    UndefinedReferableUndefinedUnion =>
                        s, // Not a reference, return original string
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
                                return ToRef(
                                    battle.Library.Items.GetValueOrDefault(
                                        Enum.Parse<ItemId>(idStr)));
                            }
                            else if (effectType == "Ability")
                            {
                                // Return reference to the ability from Library
                                return ToRef(
                                    battle.Library.Abilities.GetValueOrDefault(
                                        Enum.Parse<AbilityId>(idStr)));
                            }
                            else if (effectType == "Condition" || effectType == "Weather" ||
                                     effectType == "Status" || effectType == "Terrain")
                            {
                                // Return reference to the condition from Library
                                return ToRef(
                                    battle.Library.Conditions.GetValueOrDefault(
                                        Enum.Parse<ConditionId>(idStr)));
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
                    string key = entry.Key.ToString() ??
                                 throw new InvalidOperationException(
                                     "Dictionary key cannot be null");
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
                object? convertedValue =
                    ConvertToPropertyType(deserializedValue, prop.PropertyType);

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

                if (targetType.IsGenericType &&
                    targetType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type elementType = targetType.GetGenericArguments()[0];
                    var listInstance =
                        (System.Collections.IList)Activator.CreateInstance(targetType)!;
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
                if (targetType.IsGenericType &&
                    targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type keyType = targetType.GetGenericArguments()[0];
                    Type valueType = targetType.GetGenericArguments()[1];
                    var dictInstance =
                        (System.Collections.IDictionary)Activator.CreateInstance(targetType)!;

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

    /// <summary>
    /// Serializes a choice request to a JSON string, properly handling delegates and complex objects.
    /// This should be used instead of System.Text.Json.JsonSerializer for battle-related objects.
    /// </summary>
    public static string SerializeRequest(object request, IBattle battle)
    {
        // Use our custom serialization that handles delegates
        object? serialized = SerializeWithRefs(request, battle);

        // Convert to JSON string
        return System.Text.Json.JsonSerializer.Serialize(serialized,
            new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition =
                    System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
    }
}
