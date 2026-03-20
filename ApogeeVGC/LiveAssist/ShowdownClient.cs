using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// WebSocket client that connects to a Pokemon Showdown server,
/// authenticates, and provides message send/receive primitives.
/// Reference: PROTOCOL.md (connection, auth, message format).
/// </summary>
public sealed class ShowdownClient : IAsyncDisposable
{
    private readonly Uri _serverUri;
    private readonly string _loginUrl;
    private ClientWebSocket? _ws;
    private readonly byte[] _readBuffer = new byte[64 * 1024];

    // Auth state
    private TaskCompletionSource<string>? _challstrTcs;
    private TaskCompletionSource<bool>? _loginTcs;
    private string? _bufferedChallstr; // buffer in case challstr arrives before AuthenticateAsync
    private string? _username;

    // Battle room state
    private string? _currentBattleRoomId;
    private TaskCompletionSource<string>? _battleRoomTcs;
    private TaskCompletionSource<string>? _battleEndTcs;

    // Protocol log for the current battle (all raw lines)
    private readonly List<string> _battleLog = [];

    /// <summary>Fires for each batch of battle protocol lines (roomId, lines[]).</summary>
    public event Action<string, string[]>? OnBattleMessage;

    /// <summary>Fires when a |request| JSON is received (raw JSON string).</summary>
    public event Action<string>? OnRequest;

    /// <summary>Fires when |win|USER is received.</summary>
    public event Action<string>? OnWin;

    /// <summary>Fires when |tie| is received.</summary>
    public event Action? OnTie;

    /// <summary>Fires on |error| messages.</summary>
    public event Action<string>? OnError;

    /// <summary>Full protocol transcript for the current battle.</summary>
    public IReadOnlyList<string> BattleLog => _battleLog;

    /// <summary>Clear all event handlers between battles to prevent stacking.</summary>
    public void ClearEventHandlers()
    {
        OnBattleMessage = null;
        OnRequest = null;
        OnWin = null;
        OnTie = null;
        OnError = null;
    }

    public string? CurrentBattleRoomId => _currentBattleRoomId;

    public ShowdownClient(string serverUrl, string loginUrl)
    {
        _serverUri = new Uri(serverUrl);
        _loginUrl = loginUrl;
    }

    public async Task ConnectAsync(CancellationToken ct)
    {
        _ws = new ClientWebSocket();
        await _ws.ConnectAsync(_serverUri, ct);
        Console.WriteLine($"[ShowdownClient] Connected to {_serverUri}");
    }

    /// <summary>
    /// Authenticate with the Showdown server using username/password.
    /// 1. Wait for |challstr|
    /// 2. POST to login API
    /// 3. Send /trn with assertion
    /// 4. Wait for |updateuser| confirmation
    /// </summary>
    public async Task AuthenticateAsync(string username, string password, CancellationToken ct)
    {
        _username = username;
        _challstrTcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        _loginTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        // If challstr was already buffered (arrived before we started auth), use it immediately
        if (_bufferedChallstr != null)
            _challstrTcs.TrySetResult(_bufferedChallstr);

        // Wait for challstr (set by message loop or from buffer above)
        string challstr = await _challstrTcs.Task.WaitAsync(ct);

        // POST to login API
        using var http = new HttpClient();
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["name"] = username,
            ["pass"] = password,
            ["challstr"] = challstr,
        });

        HttpResponseMessage response = await http.PostAsync(_loginUrl, content, ct);
        string body = await response.Content.ReadAsStringAsync(ct);

        Console.WriteLine($"[ShowdownClient] Login API status: {response.StatusCode}");

        // Response starts with ']' followed by JSON
        if (!body.StartsWith(']'))
            throw new InvalidOperationException($"Unexpected login response: {body[..Math.Min(200, body.Length)]}");

        string json = body[1..];
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (!root.TryGetProperty("assertion", out JsonElement assertionElem))
        {
            // Extract actionerror if present
            string errorDetail = root.TryGetProperty("actionerror", out JsonElement err)
                ? err.GetString() ?? "unknown error"
                : json[..Math.Min(300, json.Length)];
            throw new InvalidOperationException($"Login failed: {errorDetail}");
        }

        string assertion = assertionElem.GetString()
            ?? throw new InvalidOperationException("Login failed: null assertion");

        if (assertion.StartsWith(";;"))
            throw new InvalidOperationException($"Login failed: {assertion[2..]}");

        // Send /trn to complete login
        await SendGlobalAsync($"/trn {username},0,{assertion}", ct);

        // Wait for |updateuser| confirmation
        await _loginTcs.Task.WaitAsync(ct);
        Console.WriteLine($"[ShowdownClient] Logged in as {username}");
    }

    /// <summary>Send a global command (no room prefix).</summary>
    public Task SendGlobalAsync(string command, CancellationToken ct)
    {
        return SendRawAsync($"|{command}", ct);
    }

    /// <summary>Send a command to a specific room.</summary>
    public Task SendRoomAsync(string roomId, string command, CancellationToken ct)
    {
        return SendRawAsync($"{roomId}|{command}", ct);
    }

    private async Task SendRawAsync(string message, CancellationToken ct)
    {
        if (_ws?.State != WebSocketState.Open)
            throw new InvalidOperationException("WebSocket is not connected");

        byte[] bytes = Encoding.UTF8.GetBytes(message);
        await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, ct);
    }

    /// <summary>
    /// Prepare to wait for a new battle room to be created.
    /// Call before /search or /accept.
    /// </summary>
    public Task<string> WaitForBattleRoomAsync(CancellationToken ct)
    {
        _battleRoomTcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        return _battleRoomTcs.Task.WaitAsync(ct);
    }

    /// <summary>
    /// Prepare to wait for the current battle to end.
    /// </summary>
    public Task<string> WaitForBattleEndAsync(CancellationToken ct)
    {
        _battleEndTcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        return _battleEndTcs.Task.WaitAsync(ct);
    }

    /// <summary>Clear the battle log for a new battle.</summary>
    public void ResetBattleLog()
    {
        _battleLog.Clear();
        _currentBattleRoomId = null;
    }

    /// <summary>
    /// Background message loop — reads WebSocket frames and dispatches protocol messages.
    /// Run this on a background task.
    /// </summary>
    public async Task RunMessageLoopAsync(CancellationToken ct)
    {
        if (_ws == null) throw new InvalidOperationException("Not connected");

        try
        {
            while (_ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
            {
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;
                do
                {
                    result = await _ws.ReceiveAsync(_readBuffer, ct);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("[ShowdownClient] Server closed connection");
                        return;
                    }
                    ms.Write(_readBuffer, 0, result.Count);
                } while (!result.EndOfMessage);

                string raw = Encoding.UTF8.GetString(ms.ToArray());
                ProcessServerMessage(raw);
            }
        }
        catch (OperationCanceledException) { }
        catch (WebSocketException ex)
        {
            Console.WriteLine($"[ShowdownClient] WebSocket error: {ex.Message}");
        }
    }

    private void ProcessServerMessage(string raw)
    {
        string[] lines = raw.Split('\n');
        string? roomId = null;

        // Detect >ROOMID prefix
        int startIdx = 0;
        if (lines.Length > 0 && lines[0].StartsWith('>'))
        {
            roomId = lines[0][1..];
            startIdx = 1;
        }

        // Collect lines for this message batch
        var battleLines = new List<string>();

        for (int i = startIdx; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrEmpty(line)) continue;

            // Log battle protocol
            if (roomId != null)
                _battleLog.Add(line);

            // Parse |TYPE|DATA...
            if (!line.StartsWith('|'))
            {
                battleLines.Add(line);
                continue;
            }

            string[] parts = line.Split('|');
            string type = parts.Length > 1 ? parts[1] : "";

            switch (type)
            {
                case "challstr":
                    // |challstr|CHALLSTR (challstr may contain |)
                    string challstr = line[("|challstr|".Length)..];
                    if (_challstrTcs != null)
                        _challstrTcs.TrySetResult(challstr);
                    else
                        _bufferedChallstr = challstr; // buffer for later
                    break;

                case "updateuser":
                    // |updateuser|USER|NAMED|AVATAR|SETTINGS
                    if (parts.Length > 3 && parts[3] == "1")
                        _loginTcs?.TrySetResult(true);
                    break;

                case "init":
                    // |init|battle — new battle room
                    if (parts.Length > 2 && parts[2] == "battle" && roomId != null)
                    {
                        _currentBattleRoomId = roomId;
                        _battleRoomTcs?.TrySetResult(roomId);
                    }
                    break;

                case "request":
                    // |request|JSON
                    if (parts.Length > 2)
                    {
                        string requestJson = line[("|request|".Length)..];
                        if (!string.IsNullOrWhiteSpace(requestJson))
                            OnRequest?.Invoke(requestJson);
                    }
                    break;

                case "win":
                    // |win|USERNAME
                    if (parts.Length > 2)
                    {
                        string winner = parts[2];
                        OnWin?.Invoke(winner);
                        _battleEndTcs?.TrySetResult($"win:{winner}");
                    }
                    break;

                case "tie":
                    OnTie?.Invoke();
                    _battleEndTcs?.TrySetResult("tie");
                    break;

                case "error":
                    // |error|MESSAGE
                    string errorMsg = line[("|error|".Length)..];
                    OnError?.Invoke(errorMsg);
                    break;

                case "popup":
                    // |popup|MESSAGE (team validation errors)
                    string popupMsg = line[("|popup|".Length)..];
                    Console.WriteLine($"[ShowdownClient] Popup: {popupMsg}");
                    break;

                default:
                    battleLines.Add(line);
                    break;
            }
        }

        // Dispatch remaining battle lines
        if (roomId != null && battleLines.Count > 0)
        {
            OnBattleMessage?.Invoke(roomId, battleLines.ToArray());
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_ws != null)
        {
            if (_ws.State == WebSocketState.Open)
            {
                try
                {
                    await _ws.CloseAsync(
                        WebSocketCloseStatus.NormalClosure, "done",
                        CancellationToken.None);
                }
                catch { /* best effort */ }
            }
            _ws.Dispose();
            _ws = null;
        }
    }
}
