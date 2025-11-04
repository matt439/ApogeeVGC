using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Generators;
using System.Text.Json;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.Utils;
using System.Text.Json.Nodes;

namespace ApogeeVGC.Sim.Core;

public class Driver
{
    private Library Library { get; } = new();

    public async Task StartTest(string format = "doubles")
    {
        PlayerStreams streams = BattleStreamExtensions.GetPlayerStreams(new BattleStream(Library));

        // Battle specification - select format based on parameter
        string formatid = format.ToLower() switch
        {
            "singles" => "gen9customgame",
            "doubles" => "gen9doublescustomgame",
            _ => throw new ArgumentException($"Invalid format: {format}. Use 'singles' or 'doubles'.")
        };

        var spec = new
        {
            formatid = formatid,
            seed = new[] { 0x71A, 0x462, 0, 0 } // 1818, 1122
        };

        PlayerOptions p1Spec = new()
        {
            Name = "Bot 1",
            Team = TeamGenerator.GenerateTestTeam(Library),
        };

        PlayerOptions p2Spec = new()
        {
            Name = "Bot 2",
            Team = TeamGenerator.GenerateTestTeam(Library),
        };

        // Fixed seeds for reproducible results
        var p1Seed = new PrngSeed(1818);
        var p2Seed = new PrngSeed(1122);

        var p1 = new RandomPlayerAi(streams.P1, seed: p1Seed);
        var p2 = new RandomPlayerAi(streams.P2, seed: p2Seed);

        // Start the AI players (don't await - they run in background)
        Task p1Task = p1.StartAsync();
        Task p2Task = p2.StartAsync();

        // Start consuming the omniscient stream to see battle output
        Task streamConsumerTask = Task.Run(async () =>
        {
            try
            {
                await foreach (string chunk in streams.Omniscient.ReadAllAsync())
                {
                    Console.WriteLine(chunk);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stream consumer error: {ex.Message}");
            }
        });

        // Initialize the battle by writing commands to the omniscient stream
        // Convert PlayerOptions to Showdown format dictionaries
        Dictionary<string, object?> p1Dict = State.SerializePlayerOptionsForShowdown(p1Spec);
        Dictionary<string, object?> p2Dict = State.SerializePlayerOptionsForShowdown(p2Spec);

        // Convert dictionaries to JsonObject for serialization
        JsonObject p1JsonObj = DictionaryToJsonObject(p1Dict);
        JsonObject p2JsonObj = DictionaryToJsonObject(p2Dict);

        string p1Json = p1JsonObj.ToJsonString();
        string p2Json = p2JsonObj.ToJsonString();

        string startCommand = $">start {JsonSerializer.Serialize(spec)}\n" +
         $">player p1 {p1Json}\n" +
           $">player p2 {p2Json}";

        await streams.Omniscient.WriteAsync(startCommand);


// Wait for all tasks to complete (or timeout)
        try
        {
            Task allTasks = Task.WhenAll(p1Task, p2Task, streamConsumerTask);
            Task timeoutTask = Task.Delay(TimeSpan.FromMinutes(5));

            Task completedTask = await Task.WhenAny(allTasks, timeoutTask);

            if (completedTask == timeoutTask)
            {
                Console.WriteLine("Battle timed out after 5 minutes.");
            }
            else
            {
                // Wait for the allTasks to ensure we capture any exceptions
                await allTasks;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Battle error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
            }
        }
        finally
        {
            // Ensure all tasks are completed before disposing streams
            // Give tasks a short grace period to finish after stream closure
            try
            {
                await Task.WhenAll(p1Task, p2Task, streamConsumerTask)
                    .WaitAsync(TimeSpan.FromSeconds(5));
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Warning: Some tasks did not complete within grace period.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task completion error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }
            }

            // Cleanup
            streams.Dispose();
            Console.WriteLine("Battle completed or timed out.");
        }
    }

    /// <summary>
    /// Converts a Dictionary to JsonObject recursively.
    /// </summary>
    private static JsonObject DictionaryToJsonObject(Dictionary<string, object?> dict)
    {
        var jsonObj = new JsonObject();

        foreach (var kvp in dict)
        {
            jsonObj[kvp.Key] = ConvertToJsonNode(kvp.Value);
        }

        return jsonObj;
    }

    /// <summary>
    /// Converts an object to a JsonNode.
    /// </summary>
    private static JsonNode? ConvertToJsonNode(object? value)
    {
        return value switch
        {
            null => null,
            string s => JsonValue.Create(s),
            int i => JsonValue.Create(i),
            long l => JsonValue.Create(l),
            double d => JsonValue.Create(d),
            bool b => JsonValue.Create(b),
            Dictionary<string, object?> dict => DictionaryToJsonObject(dict),
            Dictionary<string, int> intDict => new JsonObject(intDict.Select(kvp =>
                new KeyValuePair<string, JsonNode?>(kvp.Key, JsonValue.Create(kvp.Value)))),
            // Handle arrays before lists (more specific patterns first)
            Dictionary<string, object?>[] dictArray => new JsonArray(dictArray
                .Select(DictionaryToJsonObject).ToArray<JsonNode?>()),
            string[] stringArray => new JsonArray(stringArray
                .Select(s => (JsonNode?)JsonValue.Create(s)).ToArray()),
            object[] objArray => new JsonArray(objArray.Select(ConvertToJsonNode).ToArray()),
            // Handle lists
            List<Dictionary<string, object?>> dictList => new JsonArray(dictList
                .Select(DictionaryToJsonObject).ToArray<JsonNode?>()),
            List<object?> list => new JsonArray(list.Select(ConvertToJsonNode).ToArray()),
            List<string> stringList => new JsonArray(stringList
                .Select(s => (JsonNode?)JsonValue.Create(s)).ToArray()),
            _ => JsonValue.Create(value.ToString() ?? "")
        };
    }
}