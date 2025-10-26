using System.Reflection;
using System.Text.Json.Nodes;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Utils;

public static class State
{
    private const string Positions = "abcdefghijklmnopqrstuvwx";

    //private static readonly IReadOnlyList<string> Battle = new List<string>
    //{
    //    "dex",
    //    "gen",
    //    "ruleTable",
    //    "id",
    //    "log",
    //    "inherit",
    //    "format",
    //    "teamGenerator",
    //    "HIT_SUBSTITUTE",
    //    "NOT_FAIL",
    //    "FAIL",
    //    "SILENT_FAIL",
    //    "field",
    //    "sides",
    //    "prng",
    //    "hints",
    //    "deserialized",
    //    "queue",
    //    "actions",
    //};

    //private static readonly IReadOnlyList<string> Side = new List<string>
    //{
    //    "battle",
    //    "team",
    //    "pokemon",
    //    "choice",
    //    "activeRequest",
    //};

    //private static readonly IReadOnlyList<string> Pokemon = new List<string>
    //{
    //    "side",
    //    "battle",
    //    "set",
    //    "name",
    //    "fullname",
    //    "id",
    //    "happiness",
    //    "level",
    //    "pokeball",
    //    "baseMoveSlots",
    //};

    //private static readonly IReadOnlyList<string> Choice = new List<string>
    //{
    //    "switchIns",
    //};

    private static readonly IReadOnlyList<string> ActiveMove = new List<string>
    {
        "move",
    };

    public static JsonObject SerializeBattle(IBattle battle)
    {
        // Battle skip list from TypeScript
        var skip = new List<string> 
        { 
            "Dex", "Gen", "RuleTable", "Id", "Log", "Inherit", "Format", "TeamGenerator",
            "HIT_SUBSTITUTE", "NOT_FAIL", "FAIL", "SILENT_FAIL", "Field", "Sides", "Prng", 
            "Hints", "Deserialized", "Queue", "Actions", "Library"
        };
        
        JsonObject state = Serialize(battle, skip, battle);
        
        // Serialize Field
        state["field"] = SerializeField(battle.Field);
        
        // Serialize Sides array
        state["sides"] = new JsonArray(
            battle.Sides.Select(SerializeSide).ToArray()
        );
        
        // Serialize PRNG seed
        state["prng"] = battle.Prng.GetSeed().Seed;
        
        // Serialize hints as array (if battle has hints)
        // Note: hints might not be accessible on IBattle interface
        // We'll handle this via reflection if needed
        try
        {
            PropertyInfo? hintsProperty = battle.GetType().GetProperty("Hints");
            if (hintsProperty != null)
            {
                object? hints = hintsProperty.GetValue(battle);
                if (hints is System.Collections.IEnumerable hintsEnum)
                {
                    var hintsList = (from object? hint in hintsEnum select hint?.ToString() ??
                        string.Empty).ToList();
                    state["hints"] = new JsonArray(hintsList.Select(h => JsonValue.Create(h)).ToArray());
                }
            }
        }
        catch
        {
            // If hints aren't available, just skip them
            state["hints"] = new JsonArray();
        }
        
        // Serialize log - we treat log specially to avoid mutations
        // Note: log might not be accessible on IBattle interface
        try
        {
            PropertyInfo? logProperty = battle.GetType().GetProperty("Log");
            if (logProperty != null)
            {
                object? log = logProperty.GetValue(battle);
                if (log is List<string> logList)
                {
                    state["log"] = new JsonArray(logList.Select(l => JsonValue.Create(l)).ToArray());
                }
            }
        }
        catch
        {
            // If log isn't available, use empty array
            state["log"] = new JsonArray();
        }
        
        // Serialize queue list
        state["queue"] = SerializeWithRefs(battle.Queue.List, battle) as JsonNode;
        
        // Get format ID if available
        try
        {
            PropertyInfo? formatProperty = battle.GetType().GetProperty("Format");
            if (formatProperty != null)
            {
                object? format = formatProperty.GetValue(battle);
                if (format != null)
                {
                    PropertyInfo? idProperty = format.GetType().GetProperty("Id");
                    if (idProperty != null)
                    {
                        object? formatId = idProperty.GetValue(format);
                        state["formatid"] = formatId?.ToString() ?? "gen9customgame";
                    }
                }
            }
            else
            {
                state["formatid"] = "gen9customgame";
            }
        }
        catch
        {
            state["formatid"] = "gen9customgame";
        }
        
        return state;
    }

    public static IBattle DeserializeBattle(JsonObject serialized)
    {
        throw new NotImplementedException(
            "DeserializeBattle from JsonObject requires battle reconstruction logic. " +
            "This would need to:\n" +
            "1. Extract team data from sides\n" +
            "2. Create a new Battle instance with the teams\n" +
            "3. Reorder Pokemon arrays to match serialized state\n" +
            "4. Deserialize all battle state\n" +
            "5. Restore queue and other runtime state\n\n" +
            "This is complex and requires knowledge of Battle constructor and initialization. " +
            "For now, use the string overload with JSON, or implement battle-specific reconstruction logic.");
    }

    public static IBattle DeserializeBattle(string serialized)
    {
        JsonObject? state = JsonNode.Parse(serialized)?.AsObject();
        if (state == null)
        {
            throw new InvalidOperationException("Failed to parse serialized battle state");
        }
        
        return DeserializeBattle(state);
    }

    public static JsonObject Normalize(JsonObject state)
    {
        // Normalize the log to remove timestamp variations
        if (state.ContainsKey("log"))
        {
            if (state["log"] is JsonArray logArray)
            {
                var normalizedLog = NormalizeLog(
                    logArray.Select(l => l?.GetValue<string>() ?? string.Empty).ToList()
                );
                state["log"] = new JsonArray(normalizedLog.Select(l => JsonValue.Create(l)).ToArray());
            }
            else if (state["log"]?.GetValue<string>() is { } logString)
            {
                var normalizedLog = NormalizeLog(logString);
                state["log"] = new JsonArray(normalizedLog.Select(l => JsonValue.Create(l)).ToArray());
            }
        }
        
        return state;
    }

    public static List<string> NormalizeLog(List<string>? log = null)
    {
        if (log == null || log.Count == 0)
        {
            return new List<string>();
        }
        
        // Normalize each line by removing timestamp variations
        // Lines starting with "|t:|" are normalized to just "|t:|"
        return log.Select(line =>
            line.StartsWith("|t:|") ? "|t:|" : line
        ).ToList();
    }

    public static List<string> NormalizeLog(string log)
    {
        if (string.IsNullOrEmpty(log))
        {
            return new List<string>();
        }
        
        // Split the log string into lines
        string[] lines = log.Split('\n');
        
        // Normalize each line
        var normalized = lines.Select(line =>
            line.StartsWith("|t:|") ? "|t:|" : line
        ).ToList();
        
        return normalized;
    }

    public static JsonObject SerializeField(Field field)
    {
        // Field has special skip list - we skip 'id' and 'battle'
        var skip = new List<string> { "Id", "Battle" };
        JsonObject state = Serialize(field, skip, field.Battle);
        
        // Weather and Terrain are just IDs (enums), they serialize fine
        // WeatherState and TerrainState are EffectState objects that need to be serialized
        // PseudoWeather is a Dictionary<ConditionId, EffectState> that needs special handling
        
        // The generic Serialize should handle most of this, but we need to ensure
        // PseudoWeather dictionary is properly converted
        if (field.PseudoWeather.Count > 0)
        {
            var pseudoWeatherJson = new JsonObject();
            foreach (var kvp in field.PseudoWeather)
            {
                pseudoWeatherJson[kvp.Key.ToString()] = SerializeWithRefs(kvp.Value, field.Battle) as JsonNode;
            }
            state["pseudoWeather"] = pseudoWeatherJson;
        }
        
        return state;
    }

    public static void DeserializeField(JsonObject state, out Field field)
    {
        throw new NotImplementedException("DeserializeField requires a battle context - use the IBattle parameter version");
    }
    
    public static void DeserializeField(JsonObject state, Field field, IBattle battle)
    {
        // Field deserialization is simpler - we just update the existing field
        var skip = new List<string> { "Id", "Battle", "PseudoWeather" };
        Deserialize(state, field, skip, battle);
        
        // Handle PseudoWeather dictionary specially
        if (state.ContainsKey("pseudoWeather") && state["pseudoWeather"] is JsonObject pseudoWeatherJson)
        {
            field.PseudoWeather.Clear();
            foreach (var kvp in pseudoWeatherJson)
            {
                var conditionId = Enum.Parse<ConditionId>(kvp.Key);
                if (DeserializeWithRefs(kvp.Value, battle) is EffectState effectState)
                {
                    field.PseudoWeather[conditionId] = effectState;
                }
            }
        }
    }

    public static JsonObject SerializeSide(Side side)
    {
        // Side skip list from TypeScript: battle, team, pokemon, choice, activeRequest
        var skip = new List<string> { "Battle", "Team", "Pokemon", "Choice", "ActiveRequest" };
        JsonObject state = Serialize(side, skip, side.Battle);
        
        // Manually serialize pokemon array
        state["pokemon"] = new JsonArray(
            side.Pokemon.Select(SerializePokemon).ToArray()
        );
        
        // Encode team as position string (e.g., "1,2,3" or "123")
        // This represents the original team order mapping to current pokemon array positions
        var team = side.Pokemon.Select(t =>
            side.Team.IndexOf(t.Set)).Select(teamIndex => teamIndex + 1).ToList();
        state["team"] = team.Count > 9 
            ? string.Join(",", team) 
            : string.Join("", team);
        
        // Serialize choice (special handling for SwitchIns set)
        JsonObject choiceState = Serialize(side.Choice, ["SwitchIns"], side.Battle);
        choiceState["switchIns"] = new JsonArray(
            side.Choice.SwitchIns.Select(i => JsonValue.Create(i)).ToArray()
        );
        state["choice"] = choiceState;
        
        // ActiveRequest: If null, encode as tombstone to prevent recomputation
        if (side.ActiveRequest == null)
        {
            state["activeRequest"] = JsonValue.Create((string?)null);
        }
        // Otherwise, skip it (will be recomputed during deserialization)
        
        return state;
    }

    public static void DeserializeSide(JsonObject state, out Side side)
    {
        throw new NotImplementedException("DeserializeSide requires a battle context - use the IBattle parameter version");
    }
    
    public static void DeserializeSide(JsonObject state, Side side, IBattle battle)
    {
        // Side skip list (same as serialization)
        var skip = new List<string> { "Battle", "Team", "Pokemon", "Choice", "ActiveRequest" };
        Deserialize(state, side, skip, battle);
        
        // Deserialize pokemon array
        if (state.ContainsKey("pokemon") && state["pokemon"] is JsonArray pokemonArray)
        {
            for (int i = 0; i < pokemonArray.Count && i < side.Pokemon.Count; i++)
            {
                if (pokemonArray[i] is JsonObject pokemonState)
                {
                    DeserializePokemon(pokemonState, side.Pokemon[i], battle);
                }
            }
        }
        
        // Deserialize choice
        if (state.ContainsKey("choice") && state["choice"] is JsonObject choiceState)
        {
            var choiceSkip = new List<string> { "SwitchIns" };
            Deserialize(choiceState, side.Choice, choiceSkip, battle);
            
            // Deserialize SwitchIns set
            if (choiceState.ContainsKey("switchIns") && choiceState["switchIns"] is JsonArray switchInsArray)
            {
                side.Choice.SwitchIns.Clear();
                foreach (JsonNode? item in switchInsArray)
                {
                    if (item != null)
                    {
                        side.Choice.SwitchIns.Add(item.GetValue<int>());
                    }
                }
            }
        }
        
        // Note: team string is not deserialized back - it's only used during
        // battle reconstruction to reorder pokemon correctly
    }

    public static JsonObject SerializePokemon(Pokemon pokemon)
    {
        // Pokemon skip list from TypeScript: side, battle, set, name, fullname, id, 
        // happiness, level, pokeball, baseMoveSlots
        var skip = new List<string> 
        { 
            "Side", "Battle", "Set", "Name", "FullName", "Fullname", "Id",
            "Happiness", "Level", "Pokeball", "BaseMoveSlots",
        };
        JsonObject state = Serialize(pokemon, skip, pokemon.Battle);
        
        // Manually add the set
        state["set"] = SerializeWithRefs(pokemon.Set, pokemon.Battle) as JsonNode;
        
        // Only serialize baseMoveSlots if they differ from moveSlots
        if (pokemon.BaseMoveSlots.Count != pokemon.MoveSlots.Count ||
            !pokemon.BaseMoveSlots.Select((ms, i) => (ms, i))
                .All(pair => ReferenceEquals(pair.ms, pokemon.MoveSlots[pair.i])))
        {
            state["baseMoveSlots"] = SerializeWithRefs(pokemon.BaseMoveSlots, pokemon.Battle) as JsonNode;
        }
        
        return state;
    }

    public static void DeserializePokemon(JsonObject state, out Pokemon pokemon)
    {
        throw new NotImplementedException("DeserializePokemon requires a battle context - use the IBattle parameter version");
    }
    
    public static void DeserializePokemon(JsonObject state, Pokemon pokemon, IBattle battle)
    {
        // Pokemon skip list (same as serialization plus baseMoveSlots which needs special handling)
        var skip = new List<string> 
        { 
            "Side", "Battle", "Set", "Name", "FullName", "Fullname", "Id",
            "Happiness", "Level", "Pokeball", "BaseMoveSlots"
        };
        Deserialize(state, pokemon, skip, battle);
        
        // Set is readonly, so we skip it during deserialization
        // baseMoveSlots and moveSlots need to point to the same objects (identity, not equality)
        if (state.ContainsKey("baseMoveSlots"))
        {
            if (DeserializeWithRefs(state["baseMoveSlots"], battle) is List<object?> baseMoveSlots)
            {
                var typedBaseMoveSlots = new List<MoveSlot>();
                for (int i = 0; i < baseMoveSlots.Count; i++)
                {
                    if (baseMoveSlots[i] is MoveSlot moveSlot)
                    {
                        // If this matches a moveSlot, use the moveSlot reference instead
                        if (i < pokemon.MoveSlots.Count)
                        {
                            MoveSlot currentMoveSlot = pokemon.MoveSlots[i];
                            // Check if IDs match and it's not virtual
                            if (currentMoveSlot.Id == moveSlot.Id && (currentMoveSlot.Virtual != true))
                            {
                                typedBaseMoveSlots.Add(currentMoveSlot);
                                continue;
                            }
                        }
                        typedBaseMoveSlots.Add(moveSlot);
                    }
                }
                
                // Update BaseMoveSlots using reflection since it might be readonly
                PropertyInfo? baseMoveSlotsProp = typeof(Pokemon).GetProperty("BaseMoveSlots");
                if (baseMoveSlotsProp?.CanWrite == true)
                {
                    baseMoveSlotsProp.SetValue(pokemon, typedBaseMoveSlots);
                }
            }
        }
        else
        {
            // baseMoveSlots = moveSlots.slice()
            PropertyInfo? baseMoveSlotsProp = typeof(Pokemon).GetProperty("BaseMoveSlots");
            if (baseMoveSlotsProp?.CanWrite == true)
            {
                baseMoveSlotsProp.SetValue(pokemon, pokemon.MoveSlots.ToList());
            }
        }
        
        // Handle showCure special case - if undefined in state, set to undefined
        if (!state.ContainsKey("showCure"))
        {
            PropertyInfo? showCureProp = typeof(Pokemon).GetProperty("ShowCure");
            if (showCureProp?.CanWrite == true)
            {
                showCureProp.SetValue(pokemon, null);
            }
        }
    }

    public static bool IsActiveMove(JsonObject obj)
    {
        // Simply looking for a 'hit' field to determine if an object is an ActiveMove or not seems
        // pretty fragile, but it's no different than what the simulator is doing. We go further and
        // also check if the object has an 'id', as that's what we will interpret as the Move.
        return obj.ContainsKey("hit") && 
               (obj.ContainsKey("id") || obj.ContainsKey("move"));
    }

    public static JsonObject SerializeActiveMove(ActiveMove activeMove, IBattle battle)
    {
        // ActiveMove is somewhat problematic as it sometimes extends a Move and adds on
        // some mutable fields. We'd like to avoid displaying all the readonly fields of Move
        // (which in theory should not be changed by the ActiveMove...), so we collapse them
        // into a 'move: [Move:...]' reference. If isActiveMove returns a false positive *and*
        // an object contains an 'id' field matching a Move *and* it contains fields with the
        // same name as said Move then we'll miss them during serialization and won't
        // deserialize properly. This is unlikely to be the case, and would probably indicate
        // a bug in the simulator if it ever happened.
        
        Move baseMove = battle.Library.Moves[activeMove.Id];
        var skip = new HashSet<string>(ActiveMove);
        
        // Skip fields that haven't changed from the base Move
        // We use reflection to compare properties
        Type activeMoveType = typeof(ActiveMove);
        Type baseMoveType = typeof(Move);
        var baseProperties = baseMoveType.GetProperties(BindingFlags.Public | 
                                                        BindingFlags.Instance);
        
        foreach (PropertyInfo prop in baseProperties)
        {
            if (!prop.CanRead) continue;
            
            object? baseValue = prop.GetValue(baseMove);
            object? activeValue = prop.GetValue(activeMove);
            
            // This should really be a deepEquals check to see if anything on ActiveMove was
            // modified from the base Move, but that ends up being expensive and mostly unnecessary
            // as ActiveMove currently only mutates its simple fields (eg. `type`, `target`) anyway.
            bool valuesEqual = false;
            
            if (baseValue == null && activeValue == null)
            {
                valuesEqual = true;
            }
            else if (baseValue != null && activeValue != null)
            {
                // For complex types (reference types that aren't strings), just check reference equality
                // For value types and strings, use Equals
                if (baseValue.GetType().IsClass && baseValue.GetType() != typeof(string))
                {
                    valuesEqual = ReferenceEquals(baseValue, activeValue);
                }
                else
                {
                    valuesEqual = baseValue.Equals(activeValue);
                }
            }
            
            if (valuesEqual)
            {
                skip.Add(prop.Name);
            }
        }
        
        JsonObject state = Serialize(activeMove, skip.ToList(), battle);
        state["move"] = ToRef((Referable)baseMove);
        
        return state;
    }

    public static ActiveMove DeserializeActiveMove(JsonObject state, IBattle battle)
    {
        // First, get the base move reference
        if (!state.ContainsKey("move"))
        {
            throw new InvalidOperationException("ActiveMove serialization missing 'move' reference");
        }
        
        string moveRef = state["move"]?.GetValue<string>() ?? 
            throw new InvalidOperationException("ActiveMove 'move' reference is null");
        
        ReferableUndefinedUnion referableUnion = FromRef(moveRef, battle);
        Move baseMove = referableUnion switch
        {
            ReferableReferableUndefinedUnion { Referable: MoveReferable m } => m.Move,
            _ => throw new InvalidOperationException($"Invalid move reference: {moveRef}"),
        };
        
        // Use the Move's ToActiveMove method to create a(n) ActiveMove with all properties copied
        var activeMove = baseMove.ToActiveMove();
        
        // Now deserialize the changed properties onto the active move
        Deserialize(state, activeMove, ActiveMove.ToList(), battle);
        
        return activeMove;
    }

    public static object? SerializeWithRefs(object? obj, IBattle battle)
    {
        switch (obj)
        {
            case null:
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
            case IBattle _:
            case Field _:
            case Side _:
            case Pokemon _:
            case Condition _:
            case Ability _:
            case Item _:
            case Move _:
            case Species _:
                // Convert to Referable union type and then to reference string
                Referable referable = obj switch
                {
                    IBattle b => Referable.FromIBattle(b),
                    Field f => f,
                    Side s => s,
                    Pokemon p => p,
                    Condition c => c,
                    Ability a => a,
                    Item i => i,
                    Move m => (Referable)m,
                    Species s => s,
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
                    list.Add(SerializeWithRefs(item, battle));
                }
                return list;
                
            // Handle dictionaries
            case System.Collections.IDictionary dictionary:
                var dict = new Dictionary<string, object?>();
                foreach (System.Collections.DictionaryEntry entry in dictionary)
                {
                    string key = entry.Key.ToString() ?? throw new InvalidOperationException("Dictionary key cannot be null");
                    dict[key] = SerializeWithRefs(entry.Value, battle);
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
                        SpeciesReferable sp => sp.Species,
                        _ => s // Not a reference, return original string
                    },
                    UndefinedReferableUndefinedUnion => s, // Not a reference, return original string
                    _ => s
                };
                
            // Handle arrays/lists stored as JsonArray or List
            case JsonArray jsonArray:
                var list = new List<object?>();
                foreach (JsonNode? item in jsonArray)
                {
                    list.Add(DeserializeWithRefs(item?.AsValue().GetValue<object>(), battle));
                }
                return list;
                
            case System.Collections.IList listObj:
                var resultList = new List<object?>();
                foreach (object? item in listObj)
                {
                    resultList.Add(DeserializeWithRefs(item, battle));
                }
                return resultList;
                
            // Handle objects stored as JsonObject or Dictionary
            case JsonObject jsonObject:
                // Check if this is an ActiveMove
                if (IsActiveMove(jsonObject))
                {
                    return DeserializeActiveMove(jsonObject, battle);
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

    public static bool IsReferable(object obj)
    {
        return obj switch
        {
            IBattle => true,
            Field => true,
            Side => true,
            Pokemon => true,
            Condition => true,
            Ability => true,
            Item => true,
            Moves.ActiveMove => true,  // Check ActiveMove before Move since it inherits from Move
            Move => true,
            Species => true,
            _ => false,
        };
    }

    public static string ToRef(Referable obj)
    {
        return obj switch
        {
            // Pokemon's 'id' is not only more verbose than a position, it also isn't guaranteed
            // to be uniquely identifying in custom games without Nickname/Species Clause.
            PokemonReferable p => 
                $"[Pokemon:{p.Pokemon.Side.Id.GetSideIdName()}{Positions[p.Pokemon.Position]}]",
            
            SideReferable s => $"[Side:{s.Side.Id.GetSideIdName()}]",
            
            // Battle and Field don't need IDs as there's only one instance of each
            BattleReferable _ => "[Battle]",
            FieldReferable _ => "[Field]",
            
            // For immutable data types (Dex types), use their ID
            ConditionReferable c => $"[Condition:{c.Condition.Id}]",
            AbilityReferable a => $"[Ability:{a.Ability.Id}]",
            ItemReferable i => $"[Item:{i.Item.Id}]",
            MoveReferable m => $"[Move:{m.Move.Id}]",
            SpeciesReferable s => $"[Species:{s.Species.Id}]",
            
            _ => throw new ArgumentException($"Unknown referable type: {obj.GetType()}", nameof(obj))
        };
    }

    public static ReferableUndefinedUnion FromRef(string reference, IBattle battle)
    {
        // References are sort of fragile - we're mostly just counting on there
        // being a low chance that some string field in a simulator object will not
        // 'look' like one. However, it also needs to match one of the Referable
        // class types to be decoded, so we're probably OK. We could make the reference
        // markers more esoteric with additional sigils etc to avoid collisions, but
        // we're making a conscious decision to favor readability over robustness.
        if (!reference.StartsWith('[') || !reference.EndsWith(']'))
        {
            return ReferableUndefinedUnion.FromUndefined();
        }

        // Remove the brackets
        string content = reference.Substring(1, reference.Length - 2);

        // There's only one instance of these thus they don't need an id to differentiate.
        if (content == "Battle")
        {
            return Referable.FromIBattle(battle);
        }
        if (content == "Field")
        {
            return (Referable)battle.Field;
        }

        // Split on the first colon to get type and id
        int colonIndex = content.IndexOf(':');
        if (colonIndex == -1)
        {
            // No colon means it's invalid (except for Battle/Field which we handled above)
            return ReferableUndefinedUnion.FromUndefined();
        }

        string type = content[..colonIndex];
        string id = content[(colonIndex + 1)..];

        return type switch
        {
            "Side" => ParseSideRef(id, battle),
            "Pokemon" => ParsePokemonRef(id, battle),
            "Ability" => (Referable)battle.Library.Abilities[Enum.Parse<AbilityId>(id)],
            "Item" => (Referable)battle.Library.Items[Enum.Parse<ItemId>(id)],
            "Move" => ParseMoveRef(id, battle),
            "Condition" => (Referable)battle.Library.Conditions[Enum.Parse<ConditionId>(id)],
            "Species" => (Referable)battle.Library.Species[Enum.Parse<SpecieId>(id)],
            _ => ReferableUndefinedUnion.FromUndefined() // Unknown type, might just be a regular string
        };
    }

    private static Referable ParseSideRef(string id, IBattle battle)
    {
        // Side reference format: "p1" or "p2"
        // The side ID has a format like "p1" where the number indicates the side index
        
        // Parse "p1" -> index 0, "p2" -> index 1
        if (id.Length < 2 || id[0] != 'p')
        {
            throw new ArgumentException($"Invalid side reference format: {id}", nameof(id));
        }

        int sideIndex = int.Parse(id[1..]) - 1;
        
        if (sideIndex < 0 || sideIndex >= battle.Sides.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(id), 
                $"Side index {sideIndex} is out of range (0-{battle.Sides.Count - 1})");
        }

        return battle.Sides[sideIndex];
    }

    private static Referable ParsePokemonRef(string id, IBattle battle)
    {
        // Pokemon reference format: "p1a" where "p1" is side and "a" is position
        // Format: {sideId}{positionLetter}
        
        if (id.Length < 3)
        {
            throw new ArgumentException($"Invalid pokemon reference format: {id}", nameof(id));
        }

        // Extract side ID (e.g., "p1")
        string sideId = id[..2];
        
        // Extract position letter (e.g., "a")
        char positionLetter = id[2];
        
        // Parse the side
        int sideIndex = int.Parse(sideId[1..]) - 1;
        
        if (sideIndex < 0 || sideIndex >= battle.Sides.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(id), 
                $"Side index {sideIndex} is out of range (0-{battle.Sides.Count - 1})");
        }

        Side side = battle.Sides[sideIndex];
        
        // Find position index from letter
        int position = Positions.IndexOf(positionLetter);
        
        if (position == -1)
        {
            throw new ArgumentException($"Invalid position letter: {positionLetter}", nameof(id));
        }

        if (position < 0 || position >= side.Pokemon.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(id), 
                $"Position {position} is out of range (0-{side.Pokemon.Count - 1})");
        }

        return side.Pokemon[position];
    }

    private static Referable ParseMoveRef(string id, IBattle battle)
    {
        // Move references store only the base Move, not ActiveMove
        // ActiveMove is handled specially in IsActiveMove/SerializeActiveMove/DeserializeActiveMove
        Move move = battle.Library.Moves[Enum.Parse<MoveId>(id)];
        return (Referable)move;
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
                                    _ => JsonValue.Create(kvp.Value.ToString())
                                }
                            ))
                        ),
                        _ => JsonValue.Create(serialized.ToString())
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
            return ConvertToPropertyType(value, underlyingType);
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