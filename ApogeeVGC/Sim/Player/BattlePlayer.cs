using System.Text.Json;
using System.Text.Json.Nodes;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Sim.Player;

/// <summary>
/// Abstract base class for battle players that interact with a BattleStream.
/// </summary>
public abstract class BattlePlayer(PlayerReadWriteStream stream, bool debug = false)
{
    protected readonly PlayerReadWriteStream Stream = stream ?? throw new ArgumentNullException(nameof(stream));
    protected readonly List<string> Log = [];
    protected readonly bool Debug = debug;

    /// <summary>
    /// Starts reading from the stream and processing messages.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await foreach (string chunk in Stream.ReadAllAsync(cancellationToken))
        {
            Receive(chunk);
        }
    }

    /// <summary>
    /// Receives and processes a chunk of messages.
    /// </summary>
    public void Receive(string chunk)
    {
        foreach (string line in chunk.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            ReceiveLine(line);
        }
    }

    /// <summary>
    /// Processes a single line from the battle stream.
    /// </summary>
    public void ReceiveLine(string line)
    {
        if (Debug)
        {
            Console.WriteLine(line);
        }

        if (!line.StartsWith('|')) return;

        var parts = line[1..].Split('|', 2);
        if (parts.Length < 1) return;

        string cmd = parts[0];
        string rest = parts.Length > 1 ? parts[1] : string.Empty;

        switch (cmd)
        {
            case "request":
            {
                try
                {
                    // Parse as JsonObject and pass to the handler
                    JsonObject? jsonRequest = JsonSerializer.Deserialize<JsonObject>(rest);
 
                    if (jsonRequest != null)
                    {
                        ReceiveRequest(jsonRequest);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BattlePlayer] Error parsing request: {ex.Message}");
                    Console.WriteLine($"[BattlePlayer] Stack trace: {ex.StackTrace}");
                }
                return;
            }

            case "error":
                ReceiveError(new Exception(rest));
                return;
        }

        Log.Add(line);
    }

    /// <summary>
    /// Called when a choice request is received from the battle.
    /// Derived classes must implement this to provide battle choices.
    /// </summary>
    public abstract void ReceiveRequest(JsonObject request);

    /// <summary>
    /// Called when an error is received from the battle.
    /// Default implementation throws the error.
    /// </summary>
    public virtual void ReceiveError(Exception error)
    {
        throw error;
    }

    /// <summary>
    /// Sends a choice to the battle stream.
    /// </summary>
    public async Task ChooseAsync(string choice, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[CHOICE] {choice}");
        await Stream.WriteAsync(choice, cancellationToken);
    }
}