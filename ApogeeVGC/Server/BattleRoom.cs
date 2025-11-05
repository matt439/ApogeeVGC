using System.Net.WebSockets;
using System.Text.Json.Nodes;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Server;

/// <summary>
/// Represents an individual battle instance with players and battle state.
/// </summary>
public class BattleRoom : IDisposable
{
    public string BattleId { get; }
 public string Format { get; }
    public string Player1Username { get; }
    public string Player2Username { get; } = "AI Opponent";
    
    private readonly Library _library;
    private PlayerStreams? _streams;
    private RandomPlayerAi? _aiPlayer;
    private Task? _aiTask;
    private Task? _streamConsumerTask;
    private bool _started;
    private bool _disposed;

    public BattleRoom(string battleId, string player1Username, string format, Library library)
    {
        BattleId = battleId;
        Player1Username = player1Username;
        Format = format;
    _library = library;
    }

    /// <summary>
    /// Starts the battle with the given team for player 1.
    /// </summary>
    public async Task StartBattleAsync(string player1Username, string? teamData, 
        Func<string, Task> sendToClientAsync, 
        CancellationToken cancellationToken)
    {
        if (_started)
        {
            throw new InvalidOperationException("Battle already started");
        }

      _started = true;

        Console.WriteLine($"[BattleRoom] Starting battle {BattleId}");

        // Create battle streams  
        _streams = BattleStreamExtensions.GetPlayerStreams(new BattleStream(_library));

        // Parse team (or use test team)
 var p1Team = string.IsNullOrEmpty(teamData) 
            ? TeamGenerator.GenerateTestTeam(_library)
            : TeamGenerator.ParseShowdownTeam(teamData, _library);

        var p2Team = TeamGenerator.GenerateTestTeam(_library);

        // Set up player options
    PlayerOptions p1Spec = new()
        {
     Name = player1Username,
   Team = p1Team,
        };

        PlayerOptions p2Spec = new()
        {
      Name = Player2Username,
            Team = p2Team,
        };

        // Create AI opponent for player 2
  var p2Seed = new PrngSeed(1122);
    _aiPlayer = new RandomPlayerAi(_streams.P2, seed: p2Seed, debug: true);
        _aiTask = _aiPlayer.StartAsync(cancellationToken);

        // Start the stream consumer task to capture all messages
        _streamConsumerTask = Task.Run(async () =>
        {
          try
        {
       // Send initial room setup messages that don't come from the battle stream
                await sendToClientAsync($">{BattleId}");
      await sendToClientAsync("|init|battle");
         await sendToClientAsync($"|title|{player1Username} vs {Player2Username}");
      await sendToClientAsync($"|j|{player1Username}");

            // Now consume all messages from P1 stream and forward them immediately
    await foreach (string chunk in _streams.P1.ReadAllAsync(cancellationToken))
            {
  Console.WriteLine($"[BattleRoom] Received P1 chunk ({chunk.Length} bytes)");
    Console.WriteLine($"[BattleRoom] First 200 chars: {chunk.Substring(0, Math.Min(200, chunk.Length))}");
     
     // Skip empty or whitespace-only chunks
              if (string.IsNullOrWhiteSpace(chunk))
          {
          Console.WriteLine($"[BattleRoom] Skipping empty chunk");
        continue;
        }
       
     // Process chunk to handle |split| messages correctly
        var lines = chunk.Split('\n');
      var processedLines = new List<string>();
 bool inSplitBlock = false;
                bool skipNextLine = false;
            
     for (int i = 0; i < lines.Length; i++)
                {
var line = lines[i];
       var trimmedLine = line.Trim();
             
           // Skip leading empty lines, separators, and timestamps at the start only
         if (processedLines.Count == 0 && 
          (string.IsNullOrWhiteSpace(trimmedLine) || 
       trimmedLine == "|" || 
    trimmedLine.StartsWith("|t:|")))
   {
     continue;
         }
        
           // Handle |split| directive
      if (trimmedLine.StartsWith("|split|"))
 {
          inSplitBlock = true;
     skipNextLine = false;
       // Don't include the |split| line itself
              continue;
      }
      
           // If we're in a split block, keep first message and skip the second
                 if (inSplitBlock)
               {
  if (!skipNextLine)
  {
          // Keep the first message (P1-specific version with actual HP)
      processedLines.Add(line);
               skipNextLine = true;
             }
    else
           {
      // Skip the second message (percentage version)
                 inSplitBlock = false;
   skipNextLine = false;
             }
       continue;
     }
          
       // Normal line, just add it
       processedLines.Add(line);
      }
                
       // If no lines left after processing, skip this chunk
     if (processedLines.Count == 0)
                {
           Console.WriteLine($"[BattleRoom] Skipping chunk with no content after processing");
 continue;
       }
         
                // Rebuild chunk from processed lines
     var cleanedChunk = string.Join("\n", processedLines);
       
        Console.WriteLine($"[BattleRoom] Sending processed chunk ({cleanedChunk.Length} bytes, {lines.Length - processedLines.Count} lines removed)");
       // Send room context once before the chunk
  await sendToClientAsync($">{BattleId}");
           // Send the cleaned chunk
  await sendToClientAsync(cleanedChunk);
            }

      Console.WriteLine($"[BattleRoom] Battle {BattleId} ended");
      }
   catch (OperationCanceledException)
            {
                Console.WriteLine($"[BattleRoom] Battle {BattleId} cancelled");
            }
   catch (Exception ex)
{
       Console.WriteLine($"[BattleRoom] Stream consumer error: {ex.Message}");
         Console.WriteLine($"Stack trace: {ex.StackTrace}");
     }
        }, cancellationToken);

        // Give the stream consumer a moment to start and send init messages
   await Task.Delay(50, cancellationToken);

  // Initialize the battle
      string formatid = Format.ToLower() switch
    {
            "gen9customgame" => "gen9customgame",
          "gen9doublescustomgame" => "gen9doublescustomgame",
            _ => "gen9doublescustomgame"
        };

    var spec = new
        {
    formatid = formatid,
            seed = new[] { 0x71A, 0x762, 0, 0 } // 1818, 1122
        };

     // Convert PlayerOptions to Showdown format
  Dictionary<string, object?> p1Dict = State.SerializePlayerOptionsForShowdown(p1Spec);
   Dictionary<string, object?> p2Dict = State.SerializePlayerOptionsForShowdown(p2Spec);

    JsonObject p1JsonObj = DictionaryToJsonObject(p1Dict);
        JsonObject p2JsonObj = DictionaryToJsonObject(p2Dict);

     string p1Json = p1JsonObj.ToJsonString();
  string p2Json = p2JsonObj.ToJsonString();

        string startCommand = $">start {System.Text.Json.JsonSerializer.Serialize(spec)}\n" +
      $">player p1 {p1Json}\n" +
        $">player p2 {p2Json}";

        // Now write the start command - stream consumer will capture all resulting messages
      await _streams.Omniscient.WriteAsync(startCommand, cancellationToken);
    }

    /// <summary>
    /// Sends a player choice to the battle.
    /// </summary>
    public async Task SendPlayerChoiceAsync(string choice, CancellationToken cancellationToken)
    {
        if (_streams == null)
{
            throw new InvalidOperationException("Battle not started");
        }

   Console.WriteLine($"[BattleRoom] P1 choice: {choice}");
     await _streams.P1.WriteAsync(choice, cancellationToken);
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
            Dictionary<string, object?>[] dictArray => new JsonArray(dictArray
       .Select(DictionaryToJsonObject).ToArray<JsonNode?>()),
      string[] stringArray => new JsonArray(stringArray
                .Select(s => (JsonNode?)JsonValue.Create(s)).ToArray()),
  object[] objArray => new JsonArray(objArray.Select(ConvertToJsonNode).ToArray()),
      List<Dictionary<string, object?>> dictList => new JsonArray(dictList
         .Select(DictionaryToJsonObject).ToArray<JsonNode?>()),
 List<object?> list => new JsonArray(list.Select(ConvertToJsonNode).ToArray()),
            List<string> stringList => new JsonArray(stringList
     .Select(s => (JsonNode?)JsonValue.Create(s)).ToArray()),
  _ => JsonValue.Create(value.ToString() ?? "")
  };
    }

    public void Dispose()
  {
        if (_disposed) return;
    _disposed = true;

        try
      {
        _streams?.Dispose();
            
       // Wait for tasks to complete with timeout
if (_aiTask != null && !_aiTask.IsCompleted)
    {
   _aiTask.Wait(TimeSpan.FromSeconds(5));
          }
      
         if (_streamConsumerTask != null && !_streamConsumerTask.IsCompleted)
        {
     _streamConsumerTask.Wait(TimeSpan.FromSeconds(5));
      }
     }
        catch (Exception ex)
        {
      Console.WriteLine($"[BattleRoom] Dispose error: {ex.Message}");
  }
    }
}
