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

      // Buffer to collect all initialization messages
        var initMessages = new List<string>();
 
        // Room init
   initMessages.Add($">{BattleId}");
    initMessages.Add("|init|battle");
  initMessages.Add($"|title|{player1Username} vs {Player2Username}");
    initMessages.Add($">{BattleId}");
   initMessages.Add($"|j|{player1Username}");

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

        await _streams.Omniscient.WriteAsync(startCommand, cancellationToken);

        // Give battle time to process and generate initialization messages
        await Task.Delay(300, cancellationToken);
  
     // Collect all initialization messages from P1 stream
   var reader = _streams.P1.OutputChannel.Reader;
        while (reader.TryRead(out string? chunk))
     {
        var lines = chunk.Split('\n', StringSplitOptions.RemoveEmptyEntries);
       foreach (var line in lines)
     {
       if (string.IsNullOrWhiteSpace(line)) continue;
initMessages.Add($">{BattleId}");
       initMessages.Add(line);
       }
     }
        
   // Send ALL initialization messages at once
   foreach (var msg in initMessages)
        {
      await sendToClientAsync(msg);
   }

     // NOW start the stream consumer task for ongoing messages
_streamConsumerTask = Task.Run(async () =>
{
   try
      {
         await foreach (string chunk in _streams.P1.ReadAllAsync(cancellationToken))
       {
       var lines = chunk.Split('\n', StringSplitOptions.RemoveEmptyEntries);
     foreach (var line in lines)
         {
        if (string.IsNullOrWhiteSpace(line)) continue;
   
       string formattedMessage = $">{BattleId}\n{line}";
              await sendToClientAsync(formattedMessage);
       }
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
