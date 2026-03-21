using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Client that connects to a remote MctsWorkerServer.
/// Forwards Showdown protocol to the worker and receives decisions back.
/// Used by the home machine to offload MCTS computation to EC2.
/// </summary>
public sealed class MctsWorkerClient : IDisposable
{
    private readonly TcpClient _tcp;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    public bool IsConnected => _tcp.Connected;

    private MctsWorkerClient(TcpClient tcp, StreamReader reader, StreamWriter writer)
    {
        _tcp = tcp;
        _reader = reader;
        _writer = writer;
    }

    /// <summary>
    /// Connect to the MCTS worker server.
    /// </summary>
    public static async Task<MctsWorkerClient> ConnectAsync(string host, int port, CancellationToken ct = default)
    {
        var tcp = new TcpClient();
        await tcp.ConnectAsync(host, port, ct);
        NetworkStream stream = tcp.GetStream();
        var reader = new StreamReader(stream, Encoding.UTF8);
        var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        Console.WriteLine($"[WorkerClient] Connected to {host}:{port}");
        return new MctsWorkerClient(tcp, reader, writer);
    }

    /// <summary>
    /// Send init message to start a new battle session on the worker.
    /// </summary>
    public async Task<string> SendInitAsync(string format, CancellationToken ct = default)
    {
        string msg = JsonSerializer.Serialize(new { type = "init", format });
        return await SendAndReceiveAsync(msg, ct);
    }

    /// <summary>
    /// Forward battle protocol lines to the worker.
    /// </summary>
    public async Task<string> SendBattleLinesAsync(string[] lines, CancellationToken ct = default)
    {
        string msg = JsonSerializer.Serialize(new { type = "battle", lines });
        return await SendAndReceiveAsync(msg, ct);
    }

    /// <summary>
    /// Forward a request JSON and get the decision back.
    /// Returns the /choose command string, or null if no decision needed.
    /// </summary>
    public async Task<string?> SendRequestAsync(string requestJson, CancellationToken ct = default)
    {
        string msg = JsonSerializer.Serialize(new { type = "request", json = requestJson });
        string response = await SendAndReceiveAsync(msg, ct);

        using JsonDocument doc = JsonDocument.Parse(response);
        string responseType = doc.RootElement.GetProperty("type").GetString() ?? "";

        return responseType switch
        {
            "choice" or "team" => doc.RootElement.GetProperty("value").GetString(),
            "error" => throw new InvalidOperationException(
                $"Worker error: {doc.RootElement.GetProperty("message").GetString()}"),
            _ => null, // "none"
        };
    }

    /// <summary>
    /// Reset the worker for the next battle.
    /// </summary>
    public async Task SendResetAsync(CancellationToken ct = default)
    {
        string msg = JsonSerializer.Serialize(new { type = "reset" });
        await SendAndReceiveAsync(msg, ct);
    }

    private async Task<string> SendAndReceiveAsync(string message, CancellationToken ct)
    {
        await _writer.WriteLineAsync(message.AsMemory(), ct);
        string? response = await _reader.ReadLineAsync(ct);
        return response ?? throw new InvalidOperationException("Worker disconnected");
    }

    public void Dispose()
    {
        _reader.Dispose();
        _writer.Dispose();
        _tcp.Dispose();
    }
}
