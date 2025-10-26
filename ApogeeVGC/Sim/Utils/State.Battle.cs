using ApogeeVGC.Data;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Extensions;
using System.Reflection;
using System.Text.Json.Nodes;

namespace ApogeeVGC.Sim.Utils;

public static partial class State
{
    public static JsonObject SerializeBattle(IBattle battle)
    {
        // Battle skip list from TypeScript
        var skip = new List<string>
        {
            "Dex", "Gen", "RuleTable", "Id", "Log", "Inherit", "Format", "TeamGenerator",
            "HIT_SUBSTITUTE", "NOT_FAIL", "FAIL", "SILENT_FAIL", "Field", "Sides", "Prng",
            "Hints", "Deserialized", "Queue", "Actions", "Library",
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
                    var hintsList = (from object? hint in hintsEnum
                                     select hint?.ToString() ??
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

    public static IBattle DeserializeBattle(JsonObject serialized, Library library)
    {
        // Extract battle configuration from serialized state
        string formatId = serialized["formatid"]?.GetValue<string>() ?? "gen9ou";

        // Extract PRNG seed
        PrngSeed? prngSeed = null;
        if (serialized.ContainsKey("prng") && serialized["prng"] is JsonValue prngValue)
        {
            // The PRNG seed is stored as a string  representation
            if (prngValue.TryGetValue(out string? seedStr) && !string.IsNullOrEmpty(seedStr))
            {
                // Parse the seed string to an integer
                if (int.TryParse(seedStr, out int seedInt))
                {
                    prngSeed = new PrngSeed(seedInt);
                }
            }
        }

        // Extract other battle options
        bool? rated = null;
        if (serialized.ContainsKey("rated") && serialized["rated"] is JsonValue ratedValue)
        {
            rated = ratedValue.GetValue<bool>();
        }

        bool debugMode = false;
        if (serialized.ContainsKey("debugMode") && serialized["debugMode"] is JsonValue debugValue)
        {
            debugMode = debugValue.GetValue<bool>();
        }

        bool strictChoices = false;
        if (serialized.ContainsKey("strictChoices") && serialized["strictChoices"] is JsonValue strictValue)
        {
            strictChoices = strictValue.GetValue<bool>();
        }

        // Extract side data to build player options
        if (serialized["sides"] is not JsonArray sidesArray || sidesArray.Count != 2)
        {
            throw new InvalidOperationException("Serialized battle must have exactly 2 sides");
        }

        var playerOptions = new PlayerOptions[2];

        for (int i = 0; i < 2; i++)
        {
            if (sidesArray[i] is not JsonObject sideState)
            {
                throw new InvalidOperationException($"Side {i} is not a valid JsonObject");
            }

            // Extract team ordering
            string teamStr = sideState["team"]?.GetValue<string>() ?? throw new InvalidOperationException($"Side {i} missing team data");
            string[] teamPositions = teamStr.Length > 9 ? teamStr.Split(',') : teamStr.Select(c => c.ToString()).ToArray();

            // Extract pokemon array
            if (sideState["pokemon"] is not JsonArray pokemonArray)
            {
                throw new InvalidOperationException($"Side {i} missing pokemon array");
            }

            // Build team in original order
            var team = new List<PokemonSet>();
            foreach (string posStr in teamPositions)
            {
                int position = int.Parse(posStr) - 1;
                if (position < 0 || position >= pokemonArray.Count)
                {
                    throw new InvalidOperationException($"Invalid team position: {posStr}");
                }

                if (pokemonArray[position] is not JsonObject pokemonState)
                {
                    throw new InvalidOperationException($"Pokemon at position {position} is not a valid JsonObject");
                }

                // Extract the PokemonSet
                if (pokemonState["set"] is not JsonObject setObj)
                {
                    throw new InvalidOperationException($"Pokemon at position {position} missing set data");
                }

                // Deserialize the set
                PokemonSet pokemonSet = DeserializePokemonSet(setObj);
                team.Add(pokemonSet);
            }

            // Extract player info
            string? name = sideState["name"]?.GetValue<string>();
            string? avatar = sideState["avatar"]?.GetValue<string>();

            // Get side ID (p1 or p2)
            SideId sideId = i == 0 ? SideId.P1 : SideId.P2;

            playerOptions[i] = new PlayerOptions
            {
                Name = name ?? sideId.GetSideIdName(),
                Avatar = avatar,
                Team = team,
            };
        }

        // Create battle options
        var options = new BattleOptions
        {
            Id = Enum.TryParse(formatId, true, out FormatId parsedFormatId)
                ? parsedFormatId
                : FormatId.Gen9Ou,
            Seed = prngSeed,
            Rated = rated,
            Debug = debugMode,
            Deserialized = true, // Mark as deserialized to prevent auto-start
            StrictChoices = strictChoices,
            P1 = playerOptions[0],
            P2 = playerOptions[1],
        };

        // Create the Battle instance
        var battle = new BattleAsync(options, library);

        // Reorder Pokemon arrays to match serialization state
        // The Battle constructor orders pokemon by original team order,
        // but we need to reorder them to match the serialized state
        for (int i = 0; i < sidesArray.Count; i++)
        {
            if (sidesArray[i] is not JsonObject sideState)
            {
                continue;
            }

            string teamStr = sideState["team"]?.GetValue<string>() ?? string.Empty;
            string[] teamPositions = teamStr.Length > 9 ? teamStr.Split(',') : teamStr.Select(c => c.ToString()).ToArray();

            Side side = battle.Sides[i];
            var ordered = new Pokemon[side.Pokemon.Count];

            for (int j = 0; j < teamPositions.Length; j++)
            {
                int position = int.Parse(teamPositions[j]) - 1;
                ordered[position] = side.Pokemon[j];
            }

            // Update the side's pokemon array
            PropertyInfo? pokemonProp = typeof(Side).GetProperty("Pokemon");
            if (pokemonProp?.CanWrite == true)
            {
                pokemonProp.SetValue(side, ordered.ToList());
            }
            else
            {
                // If Pokemon property is readonly, use reflection to set the backing field
                FieldInfo? pokemonField = typeof(Side).GetField("<Pokemon>k__BackingField",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (pokemonField != null)
                {
                    pokemonField.SetValue(side, ordered.ToList());
                }
            }
        }

        // Deserialize battle state (excluding special fields handled separately)
        var battleSkip = new List<string>
        {
            "Dex", "Gen", "RuleTable", "Id", "Log", "Inherit", "Format", "TeamGenerator",
            "HIT_SUBSTITUTE", "NOT_FAIL", "FAIL", "SILENT_FAIL", "Field", "Sides", "Prng",
            "Hints", "Deserialized", "Queue", "Actions", "Library", "formatid"
        };
        Deserialize(serialized, battle, battleSkip, battle);

        // Deserialize Field
        if (serialized["field"] is JsonObject fieldState)
        {
            DeserializeField(fieldState, battle.Field, battle);
        }

        // Deserialize Sides
        bool activeRequests = false;
        for (int i = 0; i < sidesArray.Count; i++)
        {
            if (sidesArray[i] is JsonObject sideState)
            {
                DeserializeSide(sideState, battle.Sides[i], battle);

                // Check if this side has an undefined activeRequest (needs recomputation)
                if (!sideState.ContainsKey("activeRequest"))
                {
                    activeRequests = true;
                }
            }
        }

        // Recompute active requests if any were undefined
        if (activeRequests)
        {
            // Get fresh requests from current battle state
            // Note: This requires a GetRequests method on Battle that we may need to call differently
            try
            {
                // Try to get requests using reflection if not directly accessible
                MethodInfo? getRequestsMethod = typeof(BattleAsync).GetMethod("GetRequests",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (getRequestsMethod != null)
                {
                    if (getRequestsMethod.Invoke(battle, [battle.RequestState]) is object?[] requests)
                    {
                        for (int i = 0; i < sidesArray.Count; i++)
                        {
                            if (sidesArray[i] is JsonObject sideState)
                            {
                                // If activeRequest was null (tombstone), keep it null
                                if (sideState.ContainsKey("activeRequest") &&
                                    sideState["activeRequest"]?.GetValue<string>() == null)
                                {
                                    PropertyInfo? activeRequestProp = typeof(Side).GetProperty("ActiveRequest");
                                    if (activeRequestProp?.CanWrite == true)
                                    {
                                        activeRequestProp.SetValue(battle.Sides[i], null);
                                    }
                                }
                                // If activeRequest was undefined, use the recomputed one
                                else if (!sideState.ContainsKey("activeRequest"))
                                {
                                    PropertyInfo? activeRequestProp = typeof(Side).GetProperty("ActiveRequest");
                                    if (activeRequestProp?.CanWrite == true && i < requests.Length)
                                    {
                                        activeRequestProp.SetValue(battle.Sides[i], requests[i]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // If we can't get requests, just skip this step
                // The battle should still be functional without active requests
            }
        }

        // Restore PRNG state
        if (serialized.ContainsKey("prng") && serialized["prng"] is JsonValue prngJsonValue)
        {
            if (prngJsonValue.TryGetValue(out string? prngSeedStr) && !string.IsNullOrEmpty(prngSeedStr))
            {
                if (int.TryParse(prngSeedStr, out int seedInt))
                {
                    battle.Prng = new Prng(new PrngSeed(seedInt));
                }
            }
        }

        // Deserialize queue
        if (serialized.ContainsKey("queue"))
        {
            object? queueData = DeserializeWithRefs(serialized["queue"], battle);
            if (queueData is List<object?> queueList)
            {
                // Clear existing queue and populate with deserialized actions
                battle.Queue.Clear();

                // Convert objects to IAction if possible
                foreach (object? item in queueList)
                {
                    if (item is IAction action)
                    {
                        battle.Queue.List.Add(action);
                    }
                }
            }
        }

        // Restore hints
        if (serialized.ContainsKey("hints") && serialized["hints"] is JsonArray hintsArray)
        {
            var hintsSet = new HashSet<string>();
            foreach (JsonNode? hint in hintsArray)
            {
                if (hint != null)
                {
                    hintsSet.Add(hint.GetValue<string>());
                }
            }

            // Set hints using reflection
            PropertyInfo? hintsProp = typeof(BattleAsync).GetProperty("Hints",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (hintsProp?.CanWrite == true)
            {
                hintsProp.SetValue(battle, hintsSet);
            }
        }

        // Restore log
        if (serialized.ContainsKey("log") && serialized["log"] is JsonArray logArray)
        {
            var log = new List<string>();
            foreach (JsonNode? logLine in logArray)
            {
                if (logLine != null)
                {
                    log.Add(logLine.GetValue<string>());
                }
            }

            PropertyInfo? logProp = typeof(BattleAsync).GetProperty("Log");
            if (logProp?.CanWrite == true)
            {
                logProp.SetValue(battle, log);
            }
        }

        return battle;
    }

    public static IBattle DeserializeBattle(JsonObject serialized)
    {
        throw new NotImplementedException(
            "DeserializeBattle from JsonObject requires a Library parameter. " +
            "Use DeserializeBattle(JsonObject serialized, Library library) instead.");
    }

    public static IBattle DeserializeBattle(string serialized, Library library)
    {
        JsonObject? state = JsonNode.Parse(serialized)?.AsObject();
        if (state == null)
        {
            throw new InvalidOperationException("Failed to parse serialized battle state");
        }

        return DeserializeBattle(state, library);
    }

    public static IBattle DeserializeBattle(string serialized)
    {
        throw new NotImplementedException(
            "DeserializeBattle from string requires a Library parameter. " +
            "Use DeserializeBattle(string serialized, Library library) instead.");
    }
}