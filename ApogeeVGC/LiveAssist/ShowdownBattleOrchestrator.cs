using System.Diagnostics;
using System.Text.Json;
using ApogeeVGC.Data;
using ApogeeVGC.Mcts;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Configuration loaded from showdown_config.json.
/// </summary>
public sealed class ShowdownBotConfig
{
    public string ServerUrl { get; set; } = "wss://sim3.psim.us/showdown/websocket";
    public string LoginUrl { get; set; } = "https://play.pokemonshowdown.com/api/login";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Format { get; set; } = "gen9vgc2026regi";
    public string TeamFile { get; set; } = "team.txt";
    public string Player { get; set; } = "dlgreedy";
    public int NumBattles { get; set; } = 100;
    public string LogDirectory { get; set; } = "logs/showdown";
    public int DelayBetweenBattlesMs { get; set; } = 5000;
    public int BattleTimeoutSeconds { get; set; } = 600;
}

/// <summary>
/// Per-battle result for structured logging.
/// </summary>
public sealed class BattleResult
{
    public int BattleId { get; init; }
    public string Result { get; init; } = ""; // "win", "loss", "tie", "error", "timeout"
    public string? Opponent { get; init; }
    public int Turns { get; init; }
    public string Timestamp { get; init; } = "";
    public string? LogFile { get; init; }
}

/// <summary>
/// Orchestrates multiple Showdown ladder battles, logging results and transcripts.
/// </summary>
public sealed class ShowdownBattleOrchestrator
{
    private readonly ShowdownBotConfig _config;
    private readonly Library _library;
    private readonly Vocab _vocab;
    private readonly TeamPreviewInference _previewModel;
    private readonly IShowdownPlayer _player;
    private readonly string _packedTeam;

    private int _wins;
    private int _losses;
    private int _ties;
    private int _errors;
    private readonly List<BattleResult> _results = [];

    public ShowdownBattleOrchestrator(
        ShowdownBotConfig config,
        Library library,
        Vocab vocab,
        TeamPreviewInference previewModel,
        IShowdownPlayer player,
        string packedTeam)
    {
        _config = config;
        _library = library;
        _vocab = vocab;
        _previewModel = previewModel;
        _player = player;
        _packedTeam = packedTeam;
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        // Ensure log directory exists
        Directory.CreateDirectory(_config.LogDirectory);

        Console.WriteLine($"[Orchestrator] Starting {_config.NumBattles} ladder battles");
        Console.WriteLine($"[Orchestrator] Player: {_player.Name}");
        Console.WriteLine($"[Orchestrator] Format: {_config.Format}");
        Console.WriteLine($"[Orchestrator] User: {_config.Username}");
        Console.WriteLine($"[Orchestrator] Logs: {_config.LogDirectory}");
        Console.WriteLine();

        var totalTimer = Stopwatch.StartNew();

        await using var client = new ShowdownClient(_config.ServerUrl, _config.LoginUrl);

        // Connect and authenticate
        await client.ConnectAsync(ct);

        // Start message loop in background
        using var loopCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        Task messageLoop = Task.Run(() => client.RunMessageLoopAsync(loopCts.Token), loopCts.Token);

        // Small delay to receive initial server messages
        await Task.Delay(1000, ct);

        await client.AuthenticateAsync(_config.Username, _config.Password, ct);

        for (int i = 0; i < _config.NumBattles && !ct.IsCancellationRequested; i++)
        {
            BattleResult result = await RunSingleBattleAsync(client, i + 1, ct);
            _results.Add(result);

            int total = _wins + _losses + _ties + _errors;
            float winRate = total > 0 ? _wins * 100f / total : 0;
            Console.WriteLine(
                $"[Battle {i + 1}/{_config.NumBattles}] {result.Result} " +
                $"({result.Turns} turns) — Win rate: {winRate:F1}% " +
                $"({_wins}W/{_losses}L/{_ties}T/{_errors}E)");
            Console.WriteLine();

            // Delay between battles
            if (i < _config.NumBattles - 1)
                await Task.Delay(_config.DelayBetweenBattlesMs, ct);
        }

        totalTimer.Stop();

        // Cancel the message loop
        await loopCts.CancelAsync();
        try { await messageLoop; } catch (OperationCanceledException) { }

        PrintSummary(totalTimer.Elapsed);
    }

    private async Task<BattleResult> RunSingleBattleAsync(
        ShowdownClient client, int battleId, CancellationToken ct)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        client.ResetBattleLog();

        var agent = new ShowdownBattleAgent(_library, _vocab, _previewModel, _player);
        string? opponent = null;
        int turns = 0;
        string resultStr = "error";

        // Wire up events
        client.OnBattleMessage += (roomId, lines) =>
        {
            agent.ProcessBattleLines(lines);

            // Track opponent from |player| lines
            foreach (string line in lines)
            {
                if (line.StartsWith("|player|") && !line.Contains(_config.Username))
                {
                    string[] parts = line.Split('|');
                    if (parts.Length > 3)
                        opponent = parts[3];
                }
                if (line.StartsWith("|turn|"))
                {
                    string[] parts = line.Split('|');
                    if (parts.Length > 2 && int.TryParse(parts[2], out int t))
                        turns = t;
                }
            }
        };

        string? pendingChoice = null;
        client.OnRequest += requestJson =>
        {
            try
            {
                pendingChoice = agent.HandleRequest(requestJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Orchestrator] Request handling error: {ex.Message}");
                pendingChoice = "default";
            }
        };

        string? battleEndResult = null;
        client.OnWin += winner =>
        {
            battleEndResult = string.Equals(winner, _config.Username, StringComparison.OrdinalIgnoreCase)
                ? "win" : "loss";
        };
        client.OnTie += () => { battleEndResult = "tie"; };

        client.OnError += errorMsg =>
        {
            Console.WriteLine($"[Orchestrator] Error: {errorMsg}");
            if (errorMsg.StartsWith("[Invalid choice]") || errorMsg.StartsWith("[Unavailable choice]"))
            {
                // Will get a new request for Unavailable; for Invalid, send default
                if (errorMsg.StartsWith("[Invalid choice]") && client.CurrentBattleRoomId != null)
                {
                    _ = client.SendRoomAsync(client.CurrentBattleRoomId, "/choose default", ct);
                }
            }
        };

        try
        {
            // Set team and search for a game
            await client.SendGlobalAsync($"/utm {_packedTeam}", ct);
            await Task.Delay(500, ct);

            Task<string> battleRoomTask = client.WaitForBattleRoomAsync(ct);
            await client.SendGlobalAsync($"/search {_config.Format}", ct);
            Console.WriteLine($"[Battle {battleId}] Searching for opponent...");

            // Wait for battle room (with timeout)
            using var searchCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            searchCts.CancelAfter(TimeSpan.FromSeconds(_config.BattleTimeoutSeconds));
            string roomId = await battleRoomTask.WaitAsync(searchCts.Token);
            Console.WriteLine($"[Battle {battleId}] Battle started in {roomId}");

            // Start the battle timer to prevent opponent stalling
            await client.SendRoomAsync(roomId, "/timer on", ct);

            // Wait for battle to end
            Task<string> battleEndTask = client.WaitForBattleEndAsync(ct);

            // Poll loop: process pending choices and check for battle end
            using var battleCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            battleCts.CancelAfter(TimeSpan.FromSeconds(_config.BattleTimeoutSeconds));

            while (battleEndResult == null && !battleCts.Token.IsCancellationRequested)
            {
                // Send pending choice if any
                if (pendingChoice != null)
                {
                    string choice = pendingChoice;
                    pendingChoice = null;
                    await client.SendRoomAsync(roomId, $"/choose {choice}", ct);
                }

                await Task.Delay(100, battleCts.Token);
            }

            resultStr = battleEndResult ?? "timeout";

            if (resultStr == "timeout")
            {
                Console.WriteLine($"[Battle {battleId}] Timeout — forfeiting");
                await client.SendRoomAsync(roomId, "/forfeit", ct);
                await Task.Delay(1000, ct);
            }

            // Leave the battle room
            await client.SendGlobalAsync($"/leave {roomId}", ct);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            resultStr = "timeout";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Battle {battleId}] Error: {ex.Message}");
            resultStr = "error";
        }
        finally
        {
            // Unsubscribe events (clear delegates)
            // Note: in practice the client events will be overwritten next battle
        }

        // Update counters
        switch (resultStr)
        {
            case "win": _wins++; break;
            case "loss": _losses++; break;
            case "tie": _ties++; break;
            default: _errors++; break;
        }

        // Save battle log
        string? logFile = SaveBattleLog(client.BattleLog, battleId, timestamp);

        // Save structured result
        var result = new BattleResult
        {
            BattleId = battleId,
            Result = resultStr,
            Opponent = opponent,
            Turns = turns,
            Timestamp = timestamp,
            LogFile = logFile,
        };
        AppendResultJsonl(result);

        return result;
    }

    private string? SaveBattleLog(IReadOnlyList<string> log, int battleId, string timestamp)
    {
        if (log.Count == 0) return null;

        string filename = $"battle_{battleId:D4}_{timestamp}.log";
        string path = Path.Combine(_config.LogDirectory, filename);
        File.WriteAllLines(path, log);
        return filename;
    }

    private void AppendResultJsonl(BattleResult result)
    {
        string path = Path.Combine(_config.LogDirectory, "results.jsonl");
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        });
        File.AppendAllText(path, json + Environment.NewLine);
    }

    private void PrintSummary(TimeSpan elapsed)
    {
        int total = _wins + _losses + _ties + _errors;
        float winRate = total > 0 ? _wins * 100f / total : 0;

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine("         Showdown Battle Summary       ");
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine($"  Total battles:  {total}");
        Console.WriteLine($"  Wins:           {_wins}");
        Console.WriteLine($"  Losses:         {_losses}");
        Console.WriteLine($"  Ties:           {_ties}");
        Console.WriteLine($"  Errors:         {_errors}");
        Console.WriteLine($"  Win rate:       {winRate:F1}%");
        Console.WriteLine($"  Total time:     {elapsed:hh\\:mm\\:ss}");
        if (total > 0)
            Console.WriteLine($"  Avg per battle: {elapsed.TotalSeconds / total:F1}s");
        Console.WriteLine($"  Results log:    {_config.LogDirectory}/results.jsonl");
        Console.WriteLine("═══════════════════════════════════════");
    }
}
