using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using ApogeeVGC.Data;
using ApogeeVGC.LiveAssist;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils;
using Xunit.Abstractions;

namespace ApogeeVGC.Tests;

/// <summary>
/// Runs multiple equivalence tests by generating random Showdown battles
/// and comparing protocol output with the C# sim.
/// Requires Node.js and a pokemon-showdown checkout at the solution root.
/// </summary>
[Collection(LibraryCollection.Name)]
[Trait("Category", "Integration")]
public class EquivalenceBatchTests
{
    private const int DefaultNumTests = 100;
    private const string DefaultFormat = "gen9randomdoublesbattle";

    private readonly Library _library;
    private readonly ITestOutputHelper _output;

    public EquivalenceBatchTests(LibraryFixture fixture, ITestOutputHelper output)
    {
        _library = fixture.Library;
        _output = output;
    }

    [Fact]
    public void EquivalenceBatch_AllBattlesMatchShowdown()
    {
        string solutionRoot = EquivalenceTestHelper.FindSolutionRoot(AppContext.BaseDirectory);
        string toolDir = Path.Combine(solutionRoot, "Tools", "EquivalenceTest");
        string cacheDir = Path.Combine(toolDir, "batch_cache", DefaultFormat);
        string tempDir = Path.Combine(toolDir, "batch_temp", DefaultFormat);

        Directory.CreateDirectory(cacheDir);
        Directory.CreateDirectory(tempDir);

        // Validate Showdown version against cache
        string showdownDir = Path.Combine(solutionRoot, "pokemon-showdown");
        string currentCommit = EquivalenceTestHelper.GetShowdownCommitId(showdownDir);
        string versionFile = Path.Combine(cacheDir, "showdown_version.txt");
        if (File.Exists(versionFile))
        {
            string cachedCommit = File.ReadAllText(versionFile).Trim();
            if (cachedCommit != currentCommit)
            {
                _output.WriteLine($"WARNING: Showdown version mismatch! Cache={cachedCommit}, Current={currentCommit}");
            }
        }
        else if (currentCommit != "unknown")
        {
            File.WriteAllText(versionFile, currentCommit);
        }

        var failures = new ConcurrentBag<string>();
        int passed = 0;
        int errors = 0;
        int cacheHits = 0;

        Parallel.For(0, DefaultNumTests, new ParallelOptions { MaxDegreeOfParallelism = 32 }, i =>
        {
            int s1 = (i * 7 + 1) % 65536;
            int s2 = (i * 13 + 2) % 65536;
            int s3 = (i * 19 + 3) % 65536;
            int s4 = (i * 31 + 4) % 65536;
            string seedStr = $"{s1},{s2},{s3},{s4}";

            string p1Seed = $"{(i * 41 + 10) % 65536},{(i * 43 + 20) % 65536},{(i * 47 + 30) % 65536},{(i * 53 + 40) % 65536}";
            string p2Seed = $"{(i * 59 + 50) % 65536},{(i * 61 + 60) % 65536},{(i * 67 + 70) % 65536},{(i * 71 + 80) % 65536}";

            string cacheBase = Path.Combine(cacheDir, $"battle_{i:D6}");
            string cachedFixture = cacheBase + ".fixture.json";
            string cachedLog = cacheBase + ".log";

            try
            {
                string fixtureFile;
                string logFile;

                if (File.Exists(cachedFixture) && File.Exists(cachedLog))
                {
                    fixtureFile = cachedFixture;
                    logFile = cachedLog;
                    Interlocked.Increment(ref cacheHits);
                }
                else
                {
                    bool generated = EquivalenceTestHelper.GenerateShowdownFixture(
                        toolDir, DefaultFormat, seedStr, p1Seed, p2Seed, cacheBase, cachedLog);

                    if (!generated)
                    {
                        failures.Add($"SEED {seedStr} — Showdown generation failed");
                        Interlocked.Increment(ref errors);
                        return;
                    }

                    fixtureFile = cachedFixture;
                    logFile = cachedLog;
                }

                (int matches, int mismatches, int totalLines, string? firstMismatch, Exception? ex)
                    = RunComparison(fixtureFile, logFile, seedStr);

                if (ex != null)
                {
                    failures.Add($"SEED {seedStr} — ERROR: {ex.GetType().Name}: {ex.Message}");
                    Interlocked.Increment(ref errors);
                }
                else if (mismatches > 0)
                {
                    string msg = $"SEED {seedStr} — FAIL ({matches}/{totalLines} match)";
                    if (firstMismatch != null) msg += $"\n  {firstMismatch}";
                    failures.Add(msg);
                }
                else
                {
                    Interlocked.Increment(ref passed);
                }
            }
            catch (Exception ex)
            {
                failures.Add($"SEED {seedStr} — ERROR: {ex.GetType().Name}: {ex.Message}");
                Interlocked.Increment(ref errors);
            }
        });

        _output.WriteLine($"Total: {DefaultNumTests}, Passed: {passed}, Failed: {failures.Count - errors}, Errors: {errors}");
        _output.WriteLine($"Cache: {cacheHits}/{DefaultNumTests} hits");

        if (!failures.IsEmpty)
        {
            _output.WriteLine("");
            _output.WriteLine("=== Failed Seeds ===");
            foreach (string failure in failures)
            {
                _output.WriteLine(failure);
            }
        }

        // Clean up temp directory
        try
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
        catch { /* ignore cleanup errors */ }

        Assert.Empty(failures);
    }

    /// <summary>
    /// Runs the equivalence comparison for a single fixture against the C# sim.
    /// </summary>
    private (int Matches, int Mismatches, int TotalLines, string? FirstMismatch, Exception? Exception)
        RunComparison(string fixturePath, string showdownLogPath, string seedStr)
    {
        try
        {
            string fixtureJson = File.ReadAllText(fixturePath);
            using JsonDocument doc = JsonDocument.Parse(fixtureJson);
            JsonElement root = doc.RootElement;

            string fileSeedStr = root.GetProperty("seed").GetString()!;
            string[] seedParts = fileSeedStr.Split(',');
            var gen5Seed = new Gen5RngSeed(
                ushort.Parse(seedParts[0]),
                ushort.Parse(seedParts[1]),
                ushort.Parse(seedParts[2]),
                ushort.Parse(seedParts[3]));

            string formatId = root.GetProperty("formatid").GetString()!;
            FormatId fmtId = EquivalenceTestHelper.ResolveFormatId(formatId);

            var resolver = new ShowdownNameResolver(_library);
            PokemonSet[] p1Team = EquivalenceTestHelper.ParseShowdownTeam(root.GetProperty("p1Team"), resolver, _library);
            PokemonSet[] p2Team = EquivalenceTestHelper.ParseShowdownTeam(root.GetProperty("p2Team"), resolver, _library);

            var inputLog = new List<string>();
            foreach (JsonElement entry in root.GetProperty("inputLog").EnumerateArray())
            {
                inputLog.Add(entry.GetString()!);
            }

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

            var battle = new Battle(battleOptions, _library);
            battle.Start();

            // Parse and replay choices
            var choices = new Queue<string>();
            foreach (string logEntry in inputLog)
            {
                if (!logEntry.StartsWith('>')) continue;
                string line = logEntry[1..];
                if (line.StartsWith("start") || line.StartsWith("player")) continue;
                choices.Enqueue(line);
            }

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

            // Compare protocol output
            string[] showdownLines = File.ReadAllLines(showdownLogPath);
            List<string> csharpFiltered = EquivalenceTestHelper.FilterProtocolLines(battle.Log);
            List<string> showdownFiltered = EquivalenceTestHelper.FilterProtocolLines(showdownLines);

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
                        var ctx = new StringBuilder();
                        int ctxStart = Math.Max(0, j - 3);
                        for (int k = ctxStart; k < j; k++)
                        {
                            string prevCs = k < csharpFiltered.Count ? csharpFiltered[k] : "(missing)";
                            ctx.AppendLine($"  [{k}] {EquivalenceTestHelper.Truncate(prevCs, 80)}");
                        }
                        ctx.Append($"  >>>[{j}] C#=\"{EquivalenceTestHelper.Truncate(csLine, 80)}\" SD=\"{EquivalenceTestHelper.Truncate(sdLine, 80)}\"");
                        firstMismatch = ctx.ToString();
                    }
                }
            }

            return (matches, mismatches, maxLines, firstMismatch, null);
        }
        catch (Exception ex)
        {
            return (0, 0, 0, null, ex);
        }
    }
}
