using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using System.Diagnostics;

namespace ApogeeVGC.Sim.Core;

/// <summary>
/// Holds seed values for the current battle simulation, used for debugging.
/// Inspect <see cref="Driver.CurrentBattleSeeds"/> in the debugger to see seeds when an exception is thrown.
/// </summary>
public record BattleSeedContext(
    int Team1Seed,
    int Team2Seed,
    int Player1Seed,
    int Player2Seed,
    int BattleSeed);

public partial class Driver
{
    /// <summary>
    /// Thread-static field holding the seed context for the currently executing battle.
    /// Inspect this in the Watch/Locals window when the debugger breaks on an exception.
    /// </summary>
    [ThreadStatic]
    public static BattleSeedContext? CurrentBattleSeeds;

    /// <summary>
    /// Enriches the exception's <see cref="Exception.Data"/> with seed values so they
    /// appear in the VS Exception Details pane. Used as an exception filter (returns false
    /// so the exception is never caught by this filter).
    /// </summary>
    private static bool EnrichExceptionWithSeeds(Exception ex, BattleSeedContext seeds)
    {
        ex.Data["Team1Seed"] = seeds.Team1Seed;
        ex.Data["Team2Seed"] = seeds.Team2Seed;
        ex.Data["Player1Seed"] = seeds.Player1Seed;
        ex.Data["Player2Seed"] = seeds.Player2Seed;
        ex.Data["BattleRandSeed"] = seeds.BattleSeed;
        return false;
    }

    /// <summary>
    /// Runs a battle with pre-built teams directly on the calling thread (no Task.Run wrapper).
    /// Designed for use inside Parallel.For where the caller already manages parallelism.
    /// Timeout protection is provided by MaxTurns; no separate Task + Wait overhead.
    /// </summary>
    private (SimulatorResult Result, int Turn) RunBattleWithPrebuiltTeamsDirect(
        List<PokemonSet> team1,
        List<PokemonSet> team2,
        int team1Seed,
        int team2Seed,
        int player1Seed,
        int player2Seed,
        int battleSeed,
        FormatId formatId,
        bool debug)
    {
        // Only allocate BattleSeedContext and set thread-static when debugging;
        // in the hot evaluation path (debug=false) this avoids a heap allocation per battle.
        BattleSeedContext? seeds = null;
        if (debug)
        {
            seeds = new BattleSeedContext(team1Seed, team2Seed, player1Seed, player2Seed, battleSeed);
            CurrentBattleSeeds = seeds;
        }

        try
        {
            PlayerOptions player1Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Random 1",
                Team = team1,
                Seed = new PrngSeed(player1Seed),
                PrintDebug = debug,
            };

            PlayerOptions player2Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Random 2",
                Team = team2,
                Seed = new PrngSeed(player2Seed),
                PrintDebug = debug,
            };

            BattleOptions battleOptions = new()
            {
                Id = formatId,
                Player1Options = player1Options,
                Player2Options = player2Options,
                Debug = debug,
                Sync = true,
                Seed = new PrngSeed(battleSeed),
                MaxTurns = 5000,
            };

            var simulator = new SyncSimulator();
            SimulatorResult result = simulator.Run(Library, battleOptions, printDebug: debug);
            return (Result: result, Turn: simulator.Battle?.Turn ?? 0);
        }
        catch (Exception ex) when (seeds is not null && EnrichExceptionWithSeeds(ex, seeds))
        {
            throw;
        }
        finally
        {
            if (debug) CurrentBattleSeeds = null;
        }
    }

    /// <summary>
    /// Runs a battle with MCTS (Player 1) vs Random (Player 2) directly on the calling thread.
    /// Same pattern as RunBattleWithPrebuiltTeamsDirect but Player 1 is MCTS.
    /// </summary>
    private (SimulatorResult Result, int Turn) RunMctsBattleDirect(
        List<PokemonSet> team1,
        List<PokemonSet> team2,
        int team1Seed,
        int team2Seed,
        int player1Seed,
        int player2Seed,
        int battleSeed,
        FormatId formatId,
        bool debug)
    {
        BattleSeedContext? seeds = null;
        if (debug)
        {
            seeds = new BattleSeedContext(team1Seed, team2Seed, player1Seed, player2Seed, battleSeed);
            CurrentBattleSeeds = seeds;
        }

        try
        {
            PlayerOptions player1Options = new()
            {
                Type = Player.PlayerType.Mcts,
                Name = "MCTS",
                Team = team1,
                Seed = new PrngSeed(player1Seed),
                PrintDebug = debug,
            };

            PlayerOptions player2Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Random",
                Team = team2,
                Seed = new PrngSeed(player2Seed),
                PrintDebug = debug,
            };

            BattleOptions battleOptions = new()
            {
                Id = formatId,
                Player1Options = player1Options,
                Player2Options = player2Options,
                Debug = debug,
                Sync = true,
                Seed = new PrngSeed(battleSeed),
                MaxTurns = 5000,
            };

            var simulator = new SyncSimulator();
            SimulatorResult result = simulator.Run(Library, battleOptions, printDebug: debug);
            return (Result: result, Turn: simulator.Battle?.Turn ?? 0);
        }
        catch (Exception ex) when (seeds is not null && EnrichExceptionWithSeeds(ex, seeds))
        {
            throw;
        }
        finally
        {
            if (debug) CurrentBattleSeeds = null;
        }
    }

    /// <summary>
    /// Helper method to log exception details with all seed reproduction instructions (including team seeds).
    /// </summary>
    private void LogExceptionWithAllSeeds(
        int team1Seed,
        int team2Seed,
        int player1Seed,
        int player2Seed,
        int battleSeed,
        Exception ex,
        string debugMode)
    {
        Console.WriteLine();
        Console.WriteLine("-----------------------------------------------------------");

        if (ex is BattleTimeoutException timeoutEx)
        {
            Console.WriteLine("ERROR: Battle exceeded timeout (likely infinite loop!)");
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine($"Timeout: {timeoutEx.TimeoutMilliseconds}ms");
        }
        else if (ex is BattleTurnLimitException turnLimitEx)
        {
            Console.WriteLine("ERROR: Battle exceeded turn limit (likely infinite loop!)");
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine($"Turn: {turnLimitEx.Turn} (Max: {turnLimitEx.MaxTurns})");
        }
        else
        {
            Console.WriteLine("ERROR: Exception occurred during battle simulation!");
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine($"Exception Type: {ex.GetType().Name}");
            Console.WriteLine($"Exception Message: {ex.Message}");
        }

        Console.WriteLine($"Team 1 Seed:   {team1Seed}");
        Console.WriteLine($"Team 2 Seed:   {team2Seed}");
        Console.WriteLine($"Player 1 Seed: {player1Seed}");
        Console.WriteLine($"Player 2 Seed: {player2Seed}");
        Console.WriteLine($"Battle Seed:   {battleSeed}");
        Console.WriteLine();
        Console.WriteLine("To reproduce this error, use these seeds:");
        Console.WriteLine($"  Team 1 Seed:   {team1Seed}");
        Console.WriteLine($"  Team 2 Seed:   {team2Seed}");
        Console.WriteLine($"  Player 1 Seed: {player1Seed}");
        Console.WriteLine($"  Player 2 Seed: {player2Seed}");
        Console.WriteLine($"  Battle Seed:   {battleSeed}");
        Console.WriteLine();
        Console.WriteLine($"Then run with DriverMode.{debugMode} to debug.");

        if (ex is not BattleTimeoutException and not BattleTurnLimitException)
        {
            Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
            LogInnerExceptions(ex);
        }

        Console.WriteLine("-----------------------------------------------------------");
        Console.WriteLine();
    }

    /// <summary>
    /// Logs the full inner exception chain for debugging.
    /// </summary>
    private static void LogInnerExceptions(Exception ex)
    {
        Exception? inner = ex.InnerException;
        int depth = 1;
        while (inner != null)
        {
            Console.WriteLine();
            Console.WriteLine($"--- Inner Exception (depth {depth}) ---");
            Console.WriteLine($"Type: {inner.GetType().Name}");
            Console.WriteLine($"Message: {inner.Message}");
            Console.WriteLine($"Stack Trace:\n{inner.StackTrace}");
            inner = inner.InnerException;
            depth++;
        }
    }

    /// <summary>
    /// Gets the current Git commit ID from the repository.
    /// </summary>
    /// <returns>The short commit ID (first 7 characters), or "Unknown" if unable to retrieve.</returns>
    private static string GetGitCommitId()
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "rev-parse --short HEAD",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process? process = Process.Start(processStartInfo);
            if (process == null)
            {
                return "Unknown";
            }

            string commitId = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            return string.IsNullOrWhiteSpace(commitId) ? "Unknown" : commitId;
        }
        catch
        {
            return "Unknown";
        }
    }
}
