using System.Diagnostics;
using System.Text.Json;
using ApogeeVGC.LiveAssist;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    // Equivalence batch evaluation settings
    private const int EquivalenceBatchNumTests = 10000;
    private const string EquivalenceBatchFormat = "gen9randomdoublesbattle";

    /// <summary>
    /// Result of a single equivalence test run.
    /// </summary>
    private record EquivalenceResult(
        string SeedStr,
        int TotalLines,
        int Matches,
        int Mismatches,
        int CSharpLines,
        int ShowdownLines,
        string? FirstMismatchContext,
        Exception? Exception,
        int CSharpPrngCalls = 0,
        int ShowdownPrngCalls = 0,
        string? CSharpPrngFinal = null,
        string? ShowdownPrngFinal = null);

    /// <summary>
    /// Runs multiple equivalence tests by generating random Showdown battles
    /// and comparing protocol output with the C# sim.
    ///
    /// Usage pattern: run this batch to find failures, then copy the failing
    /// seed into RunEquivalenceTest (EquivalenceTest mode) for debugging.
    /// </summary>
    private void RunEquivalenceBatchTest()
    {
        string solutionRoot = FindSolutionRoot(AppContext.BaseDirectory);
        string toolDir = Path.Combine(solutionRoot, "Tools", "EquivalenceTest");
        string tempDir = Path.Combine(toolDir, "batch_temp");

        Console.WriteLine($"[EquivBatch] Starting batch equivalence test");
        Console.WriteLine($"[EquivBatch] Format: {EquivalenceBatchFormat}");
        Console.WriteLine($"[EquivBatch] Tests: {EquivalenceBatchNumTests}");
        Console.WriteLine($"[EquivBatch] Temp dir: {tempDir}");
        Console.WriteLine();

        // Create temp directory for fixtures
        Directory.CreateDirectory(tempDir);

        var results = new EquivalenceResult[EquivalenceBatchNumTests];
        var stopwatch = Stopwatch.StartNew();
        int passed = 0;
        int failed = 0;
        int errors = 0;
        int completed = 0;

        Parallel.For(0, EquivalenceBatchNumTests, new ParallelOptions { MaxDegreeOfParallelism = 32 }, i =>
        {
            // Generate a unique seed for each battle
            // Use 4 values as Gen5RNG seed, derived from the test index
            int s1 = (i * 7 + 1) % 65536;
            int s2 = (i * 13 + 2) % 65536;
            int s3 = (i * 19 + 3) % 65536;
            int s4 = (i * 31 + 4) % 65536;
            string seedStr = $"{s1},{s2},{s3},{s4}";

            // Player AI seeds (also deterministic per test)
            string p1Seed = $"{(i * 41 + 10) % 65536},{(i * 43 + 20) % 65536},{(i * 47 + 30) % 65536},{(i * 53 + 40) % 65536}";
            string p2Seed = $"{(i * 59 + 50) % 65536},{(i * 61 + 60) % 65536},{(i * 67 + 70) % 65536},{(i * 71 + 80) % 65536}";

            string outputBase = Path.Combine(tempDir, $"battle_{i:D4}");
            string fixtureFile = outputBase + ".fixture.json";
            string logFile = outputBase + ".log";

            try
            {
                // Step 1: Generate Showdown battle via Node.js
                bool generated = GenerateShowdownFixture(
                    toolDir, EquivalenceBatchFormat, seedStr, p1Seed, p2Seed, outputBase, logFile);

                if (!generated)
                {
                    results[i] = new EquivalenceResult(seedStr, 0, 0, 0, 0, 0, "Showdown generation failed", null);
                    int done = Interlocked.Increment(ref completed);
                    Console.WriteLine($"  [{done}/{EquivalenceBatchNumTests}] SEED {seedStr} — ERROR (Showdown gen failed)");
                    return;
                }

                // Step 2: Run equivalence comparison
                EquivalenceResult result = RunEquivalenceComparison(fixtureFile, logFile, seedStr);
                results[i] = result;

                int doneNow = Interlocked.Increment(ref completed);
                if (result.Mismatches == 0 && result.Exception == null)
                {
                    Console.WriteLine($"  [{doneNow}/{EquivalenceBatchNumTests}] SEED {seedStr} — PASS ({result.TotalLines} lines)");
                }
                else if (result.Exception != null)
                {
                    Console.WriteLine($"  [{doneNow}/{EquivalenceBatchNumTests}] SEED {seedStr} — ERROR: {result.Exception.Message}");
                }
                else
                {
                    Console.WriteLine($"  [{doneNow}/{EquivalenceBatchNumTests}] SEED {seedStr} — FAIL ({result.Matches}/{result.TotalLines} match)");
                    if (result.FirstMismatchContext != null)
                        Console.WriteLine($"    {result.FirstMismatchContext}");
                }
            }
            catch (Exception ex)
            {
                results[i] = new EquivalenceResult(seedStr, 0, 0, 0, 0, 0, null, ex);
                int done = Interlocked.Increment(ref completed);
                Console.WriteLine($"  [{done}/{EquivalenceBatchNumTests}] SEED {seedStr} — ERROR: {ex.Message}");
            }
        });

        // Count results
        foreach (EquivalenceResult r in results)
        {
            if (r.Exception != null) errors++;
            else if (r.Mismatches == 0) passed++;
            else failed++;
        }

        stopwatch.Stop();

        // Summary
        Console.WriteLine();
        Console.WriteLine("=== Equivalence Batch Results ===");
        Console.WriteLine($"  Total:  {EquivalenceBatchNumTests}");
        Console.WriteLine($"  Passed: {passed}");
        Console.WriteLine($"  Failed: {failed}");
        Console.WriteLine($"  Errors: {errors}");
        Console.WriteLine($"  Time:   {stopwatch.Elapsed.TotalSeconds:F1}s ({stopwatch.Elapsed.TotalSeconds / EquivalenceBatchNumTests:F2}s/test)");

        // List all failures with seeds for easy reproduction
        if (failed + errors > 0)
        {
            Console.WriteLine();
            Console.WriteLine("=== Failed Seeds (copy to EquivalenceTest mode to debug) ===");
            foreach (EquivalenceResult r in results)
            {
                if (r.Mismatches > 0 || r.Exception != null)
                {
                    string status = r.Exception != null ? "ERROR" : "FAIL";
                    string prngMatch = (r.CSharpPrngFinal != null && r.ShowdownPrngFinal != null)
                        ? (r.CSharpPrngFinal == r.ShowdownPrngFinal ? "PRNG=OK" : "PRNG=DIFF")
                        : "PRNG=?";
                    string callInfo = r.ShowdownPrngCalls > 0
                        ? $"calls=C#{r.CSharpPrngCalls}/SD{r.ShowdownPrngCalls}"
                        : $"calls=C#{r.CSharpPrngCalls}";
                    Console.WriteLine($"  [{status}] --seed {r.SeedStr}  ({r.Matches}/{r.TotalLines} match) {callInfo} {prngMatch}");
                    if (r.FirstMismatchContext != null)
                        Console.WriteLine($"         {r.FirstMismatchContext}");
                    if (r.Exception != null)
                        Console.WriteLine($"         {r.Exception.GetType().Name}: {r.Exception.Message}");
                    if (prngMatch == "PRNG=DIFF")
                        Console.WriteLine($"         C#={r.CSharpPrngFinal} SD={r.ShowdownPrngFinal}");
                }
            }
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine($"  ALL {EquivalenceBatchNumTests} TESTS PASSED!");
        }

        // Clean up temp files on full success
        if (failed + errors == 0)
        {
            try
            {
                Directory.Delete(tempDir, true);
                Console.WriteLine($"  Cleaned up temp directory: {tempDir}");
            }
            catch { /* ignore cleanup errors */ }
        }
        else
        {
            Console.WriteLine($"\n  Fixture files preserved in: {tempDir}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Generate a Showdown battle fixture by running Node.js.
    /// Returns true if successful, false if the process failed.
    /// </summary>
    private static bool GenerateShowdownFixture(
        string toolDir, string format, string seed, string p1Seed, string p2Seed,
        string outputBase, string logFile)
    {
        string scriptPath = Path.Combine(toolDir, "run_showdown_battle.js");

        var psi = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = $"\"{scriptPath}\" --random --format {format} --seed {seed} --p1seed {p1Seed} --p2seed {p2Seed} --out \"{outputBase}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = toolDir,
        };

        using var process = Process.Start(psi);
        if (process == null) return false;

        // Read stdout/stderr asynchronously to avoid deadlocks
        Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
        Task<string> stderrTask = process.StandardError.ReadToEndAsync();

        if (!process.WaitForExit(30_000))
        {
            // Process hung — kill it
            try { process.Kill(true); } catch { /* ignore */ }
            return false;
        }

        string stdout = stdoutTask.GetAwaiter().GetResult();
        string stderr = stderrTask.GetAwaiter().GetResult();

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"    Node.js error: {stderr}");
            return false;
        }

        // Write protocol log to file
        File.WriteAllText(logFile, stdout);
        return File.Exists(outputBase + ".fixture.json");
    }

    /// <summary>
    /// Run the equivalence comparison for a single fixture, returning structured results.
    /// This is the non-interactive version of RunEquivalenceTest.
    /// </summary>
    private EquivalenceResult RunEquivalenceComparison(string fixturePath, string showdownLogPath, string seedStr)
    {
        try
        {
            string fixtureJson = File.ReadAllText(fixturePath);
            using JsonDocument doc = JsonDocument.Parse(fixtureJson);
            JsonElement root = doc.RootElement;

            // Parse seed
            string fileSeedStr = root.GetProperty("seed").GetString()!;
            string[] seedParts = fileSeedStr.Split(',');
            var gen5Seed = new Gen5RngSeed(
                ushort.Parse(seedParts[0]),
                ushort.Parse(seedParts[1]),
                ushort.Parse(seedParts[2]),
                ushort.Parse(seedParts[3]));

            // Parse format
            string formatId = root.GetProperty("formatid").GetString()!;
            FormatId fmtId = ResolveFormatId(formatId);

            // Parse teams
            var resolver = new ShowdownNameResolver(Library);
            PokemonSet[] p1Team = ParseShowdownTeam(root.GetProperty("p1Team"), resolver);
            PokemonSet[] p2Team = ParseShowdownTeam(root.GetProperty("p2Team"), resolver);

            // Parse input log
            var inputLog = new List<string>();
            foreach (JsonElement entry in root.GetProperty("inputLog").EnumerateArray())
            {
                inputLog.Add(entry.GetString()!);
            }

            // Create and run battle
            var battleOptions = new BattleOptions
            {
                Id = fmtId,
                Sync = true,
                DisplayUi = true,
                Seed = PrngSeed.FromGen5(gen5Seed),
                Player1Options = new PlayerOptions
                {
                    Name = "Player1",
                    Team = p1Team.ToList(),
                },
                Player2Options = new PlayerOptions
                {
                    Name = "Player2",
                    Team = p2Team.ToList(),
                },
                MaxTurns = 1000,
            };

            var battle = new Battle(battleOptions, Library);
            // Enable PRNG tracing for a specific seed
            if (Environment.GetEnvironmentVariable("TRACE_SEED") == seedStr)
            {
                battle.Prng.TraceEnabled = true;
            }
            battle.Start();

            // Parse choices
            var choices = new Queue<string>();
            foreach (string logEntry in inputLog)
            {
                if (!logEntry.StartsWith(">")) continue;
                string line = logEntry[1..];
                if (line.StartsWith("start") || line.StartsWith("player")) continue;
                choices.Enqueue(line);
            }

            // Replay choices
            while (!battle.Ended && battle.RequestState != RequestState.None)
            {
                bool madeProgress = true;
                while (choices.Count > 0 && !battle.AllChoicesDone() && madeProgress)
                {
                    madeProgress = false;
                    int scanned = 0;
                    int total = choices.Count;
                    while (scanned < total && !battle.AllChoicesDone())
                    {
                        string line = choices.Dequeue();
                        scanned++;

                        Side? side = null;
                        string choice = "";
                        if (line.StartsWith("p1 "))
                        {
                            side = battle.P1;
                            choice = line[3..];
                        }
                        else if (line.StartsWith("p2 "))
                        {
                            side = battle.P2;
                            choice = line[3..];
                        }

                        if (side is not null && !side.IsChoiceDone())
                        {
                            side.Choose(choice);
                            madeProgress = true;
                        }
                        else if (side is not null && side.IsChoiceDone())
                        {
                            choices.Enqueue(line);
                        }
                    }
                }

                if (battle.AllChoicesDone())
                    battle.CommitChoices();
                else
                    break;
            }

            // Get PRNG state
            int prngCalls = battle.Prng.CallCount;
            string? csharpPrngFinal = null;
            if (battle.Prng.Gen5 is { } gen5)
            {
                var finalSeed = gen5.GetSeed();
                csharpPrngFinal = $"{finalSeed.S0},{finalSeed.S1},{finalSeed.S2},{finalSeed.S3}";
            }

            // Get Showdown PRNG final seed and call count from fixture
            string? showdownPrngFinal = null;
            int showdownPrngCalls = 0;
            if (root.TryGetProperty("prngFinalSeed", out JsonElement prngProp) && prngProp.ValueKind == JsonValueKind.String)
            {
                showdownPrngFinal = prngProp.GetString();
            }
            if (root.TryGetProperty("prngCallCount", out JsonElement callCountProp) && callCountProp.ValueKind == JsonValueKind.Number)
            {
                showdownPrngCalls = callCountProp.GetInt32();
            }

            // Compare protocol output
            string[] showdownLines = File.ReadAllLines(showdownLogPath);
            var csharpFiltered = FilterProtocolLines(battle.Log);
            var showdownFiltered = FilterProtocolLines(showdownLines);

            int matches = 0;
            int mismatches = 0;
            int maxLines = Math.Max(csharpFiltered.Count, showdownFiltered.Count);
            string? firstMismatch = null;

            for (int j = 0; j < maxLines; j++)
            {
                string csLine = j < csharpFiltered.Count ? csharpFiltered[j] : "(missing)";
                string sdLine = j < showdownFiltered.Count ? showdownFiltered[j] : "(missing)";

                if (csLine == sdLine)
                {
                    matches++;
                }
                else
                {
                    mismatches++;
                    if (firstMismatch == null)
                    {
                        // Build context with 3 lines before the mismatch
                        var ctx = new System.Text.StringBuilder();
                        int ctxStart = Math.Max(0, j - 3);
                        for (int k = ctxStart; k < j; k++)
                        {
                            string prevCs = k < csharpFiltered.Count ? csharpFiltered[k] : "(missing)";
                            ctx.AppendLine($"  [{k}] {Truncate(prevCs, 80)}");
                        }
                        ctx.Append($"  >>>[{j}] C#=\"{Truncate(csLine, 80)}\" SD=\"{Truncate(sdLine, 80)}\"");
                        firstMismatch = ctx.ToString();
                    }
                }
            }

            return new EquivalenceResult(
                seedStr, maxLines, matches, mismatches,
                csharpFiltered.Count, showdownFiltered.Count,
                firstMismatch, null, prngCalls, showdownPrngCalls, csharpPrngFinal, showdownPrngFinal);
        }
        catch (Exception ex)
        {
            return new EquivalenceResult(seedStr, 0, 0, 0, 0, 0, null, ex);
        }
    }

    private static string Truncate(string s, int maxLen) =>
        s.Length <= maxLen ? s : s[..maxLen] + "...";
}
