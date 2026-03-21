using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using ApogeeVGC.Data;
using ApogeeVGC.Mcts;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// TCP server that runs on EC2. Receives Showdown protocol from the home client,
/// maintains a ShowdownBattleAgent with shadow battle and ensemble MCTS,
/// and returns /choose decisions.
///
/// Protocol (newline-delimited JSON messages):
///   Client → Server:
///     {"type":"init","format":"gen9vgc2026regi","mySide":"p2"}
///     {"type":"battle","lines":["line1","line2",...]}
///     {"type":"request","json":"..."}
///     {"type":"reset"}  (between battles)
///
///   Server → Client:
///     {"type":"choice","value":"move 1 +1, switch 3"}
///     {"type":"team","value":"team 2546"}
///     {"type":"none"}  (no decision needed)
///     {"type":"error","message":"..."}
/// </summary>
public sealed class MctsWorkerServer
{
    private readonly Library _library;
    private readonly Vocab _vocab;
    private readonly TeamPreviewInference _previewModel;
    private readonly int _port;

    private ShowdownBattleAgent? _agent;
    private IShowdownPlayer? _player;

    public MctsWorkerServer(Library library, Vocab vocab,
        TeamPreviewInference previewModel, int port = 9100)
    {
        _library = library;
        _vocab = vocab;
        _previewModel = previewModel;
        _port = port;
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        var listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();
        Console.WriteLine($"[Worker] Listening on port {_port}");

        while (!ct.IsCancellationRequested)
        {
            Console.WriteLine("[Worker] Waiting for client connection...");
            TcpClient client = await listener.AcceptTcpClientAsync(ct);
            Console.WriteLine($"[Worker] Client connected: {client.Client.RemoteEndPoint}");

            try
            {
                await HandleClientAsync(client, ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Worker] Client error: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("[Worker] Client disconnected");
            }
        }

        listener.Stop();
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken ct)
    {
        using NetworkStream stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        while (!ct.IsCancellationRequested)
        {
            string? line = await reader.ReadLineAsync(ct);
            if (line == null) break; // Client disconnected

            try
            {
                string response = ProcessMessage(line);
                await writer.WriteLineAsync(response);
            }
            catch (Exception ex)
            {
                string error = JsonSerializer.Serialize(new
                {
                    type = "error",
                    message = ex.Message,
                });
                await writer.WriteLineAsync(error);
            }
        }
    }

    private string ProcessMessage(string messageJson)
    {
        using JsonDocument doc = JsonDocument.Parse(messageJson);
        string type = doc.RootElement.GetProperty("type").GetString() ?? "";

        switch (type)
        {
            case "init":
                return HandleInit(doc.RootElement);

            case "battle":
                return HandleBattle(doc.RootElement);

            case "request":
                return HandleRequest(doc.RootElement);

            case "reset":
                return HandleReset();

            default:
                return JsonSerializer.Serialize(new { type = "error", message = $"Unknown message type: {type}" });
        }
    }

    private string HandleInit(JsonElement root)
    {
        string format = root.GetProperty("format").GetString() ?? "gen9vgc2026regi";
        Console.WriteLine($"[Worker] Init: format={format}");

        // Create ensemble player and agent
        var formatId = Sim.Core.EquivalenceTestHelper.ResolveFormatId(format);
        _player = new ShowdownPlayerEnsemble(_library, formatId);
        _agent = new ShowdownBattleAgent(_library, _vocab, _previewModel, _player);

        return JsonSerializer.Serialize(new { type = "none" });
    }

    private string HandleBattle(JsonElement root)
    {
        if (_agent == null)
            return JsonSerializer.Serialize(new { type = "error", message = "Not initialized" });

        string[] lines = root.GetProperty("lines").EnumerateArray()
            .Select(e => e.GetString() ?? "")
            .ToArray();

        _agent.ProcessBattleLines(lines);
        return JsonSerializer.Serialize(new { type = "none" });
    }

    private string HandleRequest(JsonElement root)
    {
        if (_agent == null)
            return JsonSerializer.Serialize(new { type = "error", message = "Not initialized" });

        string requestJson = root.GetProperty("json").GetString() ?? "{}";

        // Capture console output during request handling
        var logCapture = new System.IO.StringWriter();
        TextWriter originalOut = Console.Out;
        Console.SetOut(logCapture);

        string? choice;
        try
        {
            choice = _agent.HandleRequest(requestJson);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        string logs = logCapture.ToString().TrimEnd();

        // Also print to worker console
        if (!string.IsNullOrEmpty(logs))
            Console.WriteLine(logs);

        if (choice == null)
            return JsonSerializer.Serialize(new { type = "none", logs });

        bool isTeam = choice.StartsWith("team ");
        return JsonSerializer.Serialize(new
        {
            type = isTeam ? "team" : "choice",
            value = choice,
            logs,
        });
    }

    private string HandleReset()
    {
        Console.WriteLine("[Worker] Reset — ready for next battle");
        _agent = null;
        _player = null;
        return JsonSerializer.Serialize(new { type = "none" });
    }
}
