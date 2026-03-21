using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Client that connects to a remote MctsWorkerServer.
/// Forwards Showdown protocol to the worker and receives decisions back.
/// Thread-safe: uses a semaphore to serialize access to the TCP stream.
/// </summary>
public sealed class MctsWorkerClient : IDisposable
{
    private readonly TcpClient _tcp;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public bool IsConnected => _tcp.Connected;

    private MctsWorkerClient(TcpClient tcp, StreamReader reader, StreamWriter writer)
    {
        _tcp = tcp;
        _reader = reader;
        _writer = writer;
    }

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

    public async Task<string> SendInitAsync(string format, CancellationToken ct = default)
    {
        string msg = JsonSerializer.Serialize(new { type = "init", format });
        return await SendAndReceiveAsync(msg, ct);
    }

    public async Task<string> SendBattleLinesAsync(string[] lines, CancellationToken ct = default)
    {
        string msg = JsonSerializer.Serialize(new { type = "battle", lines });
        return await SendAndReceiveAsync(msg, ct);
    }

    public async Task<string?> SendRequestAsync(string requestJson, CancellationToken ct = default)
    {
        string msg = JsonSerializer.Serialize(new { type = "request", json = requestJson });
        string response = await SendAndReceiveAsync(msg, ct);

        using JsonDocument doc = JsonDocument.Parse(response);
        string responseType = doc.RootElement.GetProperty("type").GetString() ?? "";

        // Print worker logs if present
        if (doc.RootElement.TryGetProperty("logs", out JsonElement logsElem))
        {
            string? logs = logsElem.GetString();
            if (!string.IsNullOrEmpty(logs))
                Console.WriteLine(logs);
        }

        return responseType switch
        {
            "choice" or "team" => doc.RootElement.GetProperty("value").GetString(),
            "error" => throw new InvalidOperationException(
                $"Worker error: {doc.RootElement.GetProperty("message").GetString()}"),
            _ => null,
        };
    }

    public async Task SendResetAsync(CancellationToken ct = default)
    {
        string msg = JsonSerializer.Serialize(new { type = "reset" });
        await SendAndReceiveAsync(msg, ct);
    }

    private async Task<string> SendAndReceiveAsync(string message, CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            await _writer.WriteLineAsync(message.AsMemory(), ct);
            string? response = await _reader.ReadLineAsync(ct);
            return response ?? throw new InvalidOperationException("Worker disconnected");
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        _lock.Dispose();
        _reader.Dispose();
        _writer.Dispose();
        _tcp.Dispose();
    }
}
