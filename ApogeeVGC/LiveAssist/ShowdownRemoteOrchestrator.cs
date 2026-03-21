using System.Diagnostics;
using System.Text.Json;
using ApogeeVGC.Mcts;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Orchestrates Showdown ladder battles using a remote MCTS worker.
/// Runs on the home machine — connects to Showdown (your IP) and
/// forwards game state to EC2 worker for MCTS decisions.
/// </summary>
public sealed class ShowdownRemoteOrchestrator
{
    private readonly ShowdownBotConfig _config;
    private readonly MctsWorkerClient _worker;
    private readonly string _packedTeam;

    private int _wins, _losses, _ties, _errors;
    private readonly List<BattleResult> _results = [];

    public ShowdownRemoteOrchestrator(
        ShowdownBotConfig config,
        MctsWorkerClient worker,
        string packedTeam)
    {
        _config = config;
        _worker = worker;
        _packedTeam = packedTeam;
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        Directory.CreateDirectory(_config.LogDirectory);

        Console.WriteLine($"[Remote] Starting {_config.NumBattles} ladder battles");
        Console.WriteLine($"[Remote] Format: {_config.Format}");
        Console.WriteLine($"[Remote] User: {_config.Username}");
        Console.WriteLine();

        var totalTimer = Stopwatch.StartNew();

        await using var client = new ShowdownClient(_config.ServerUrl, _config.LoginUrl);
        await client.ConnectAsync(ct);

        using var loopCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        Task messageLoop = Task.Run(() => client.RunMessageLoopAsync(loopCts.Token), loopCts.Token);

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

            if (i < _config.NumBattles - 1)
                await Task.Delay(_config.DelayBetweenBattlesMs, ct);
        }

        totalTimer.Stop();
        await loopCts.CancelAsync();
        try { await messageLoop; } catch (OperationCanceledException) { }

        PrintSummary(totalTimer.Elapsed);
    }

    private async Task<BattleResult> RunSingleBattleAsync(
        ShowdownClient client, int battleId, CancellationToken ct)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        client.ResetBattleLog();
        client.ClearEventHandlers();

        // Initialize worker for this battle
        await _worker.SendInitAsync(_config.Format, ct);

        string? opponent = null;
        int turns = 0;
        string resultStr = "error";
        string? battleEndResult = null;

        // Forward battle lines to worker
        client.OnBattleMessage += (roomId, lines) =>
        {
            // Forward to worker (fire and forget — worker processes asynchronously)
            _ = _worker.SendBattleLinesAsync(lines, ct);

            foreach (string line in lines)
            {
                if (line.StartsWith("|player|") && !line.Contains(_config.Username))
                {
                    string[] parts = line.Split('|');
                    if (parts.Length > 3) opponent = parts[3];
                }
                if (line.StartsWith("|turn|"))
                {
                    string[] parts = line.Split('|');
                    if (parts.Length > 2 && int.TryParse(parts[2], out int t)) turns = t;
                }
            }
        };

        // Forward requests to worker and get decisions
        string? pendingChoice = null;
        client.OnRequest += requestJson =>
        {
            try
            {
                // Synchronously wait for worker decision
                string? choice = _worker.SendRequestAsync(requestJson, ct).GetAwaiter().GetResult();
                pendingChoice = choice;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Remote] Worker error: {ex.Message}");
                pendingChoice = "default";
            }
        };

        client.OnWin += winner =>
        {
            battleEndResult = string.Equals(winner, _config.Username, StringComparison.OrdinalIgnoreCase)
                ? "win" : "loss";
        };
        client.OnTie += () => { battleEndResult = "tie"; };

        client.OnTimer += timerMsg =>
        {
            Console.WriteLine($"  [Timer] {timerMsg}");
        };

        client.OnError += errorMsg =>
        {
            Console.WriteLine($"[Remote] Error: {errorMsg}");
            if (errorMsg.StartsWith("[Invalid choice]") && client.CurrentBattleRoomId != null)
                _ = client.SendRoomAsync(client.CurrentBattleRoomId, "/choose default", ct);
        };

        try
        {
            await client.SendGlobalAsync($"/utm {_packedTeam}", ct);
            await Task.Delay(500, ct);

            Task<string> battleRoomTask = client.WaitForBattleRoomAsync(ct);
            await client.SendGlobalAsync($"/search {_config.Format}", ct);
            Console.WriteLine($"[Battle {battleId}] Searching for opponent...");

            using var searchCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            searchCts.CancelAfter(TimeSpan.FromSeconds(_config.BattleTimeoutSeconds));
            string roomId = await battleRoomTask.WaitAsync(searchCts.Token);
            Console.WriteLine($"[Battle {battleId}] Battle started in {roomId}");

            await client.SendRoomAsync(roomId, "/timer on", ct);

            using var battleCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            battleCts.CancelAfter(TimeSpan.FromSeconds(_config.BattleTimeoutSeconds));

            while (battleEndResult == null && !battleCts.Token.IsCancellationRequested)
            {
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

        // Reset worker for next battle
        try { await _worker.SendResetAsync(ct); } catch { }

        switch (resultStr)
        {
            case "win": _wins++; break;
            case "loss": _losses++; break;
            case "tie": _ties++; break;
            default: _errors++; break;
        }

        string? logFile = SaveBattleLog(client.BattleLog, battleId, timestamp);
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
        Console.WriteLine("     Remote Showdown Battle Summary    ");
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
        Console.WriteLine("═══════════════════════════════════════");
    }
}
