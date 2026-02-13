using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
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
        ex.Data["BattleSeed"] = seeds.BattleSeed;
        return false;
    }
    /// <summary>
    /// Helper method to run a single battle simulation with timeout protection.
    /// </summary>
    private (SimulatorResult Result, int Turn) RunBattleWithTimeout(
        int player1Seed,
        int player2Seed,
        int battleSeed,
        FormatId formatId,
        bool debug)
    {
        var seeds = new BattleSeedContext(0, 0, player1Seed, player2Seed, battleSeed);

        using var cts = new CancellationTokenSource();
        var simulationTask = Task.Run(() =>
        {
            // Set thread-static context so seeds are visible in the debugger Locals/Watch window
            CurrentBattleSeeds = seeds;
            try
            {
                PlayerOptions player1Options = new()
                {
                    Type = Player.PlayerType.Random,
                    Name = "Random 1",
                    Team = TeamGenerator.GenerateTestTeam(Library),
                    Seed = new PrngSeed(player1Seed),
                    PrintDebug = debug,
                };

                PlayerOptions player2Options = new()
                {
                    Type = Player.PlayerType.Random,
                    Name = "Random 2",
                    Team = TeamGenerator.GenerateTestTeam(Library),
                    Seed = new PrngSeed(player2Seed),
                    PrintDebug = debug,
                };

                BattleOptions battleOptions = new()
                {
                    Id = formatId,
                    Player1Options = player1Options,
                    Player2Options = player2Options,
                    Debug = debug,
                    Sync = true, // Use synchronous mode for evaluation
                    Seed = new PrngSeed(battleSeed),
                    MaxTurns = 1000, // Enforce turn limit to detect infinite loops
                };

                var simulator = new SyncSimulator();
                SimulatorResult result = simulator.Run(Library, battleOptions, printDebug: debug);
                return (Result: result, Turn: simulator.Battle?.Turn ?? 0);
            }
            // Exception filter enriches Exception.Data before the stack unwinds,
            // making seeds visible in the VS Exception Details pane on first-chance break
            catch (Exception ex) when (EnrichExceptionWithSeeds(ex, seeds))
            {
                // Never reached: EnrichExceptionWithSeeds always returns false
                throw;
            }
            finally
            {
                CurrentBattleSeeds = null;
            }
        }, cts.Token);

        // Wait for simulation with timeout
        if (!simulationTask.Wait(BattleTimeoutMilliseconds))
        {
            // Timeout occurred - throw custom exception with seeds
            throw new BattleTimeoutException(
                player1Seed,
                player2Seed,
                battleSeed,
                BattleTimeoutMilliseconds);
        }

        return simulationTask.Result;
    }

    /// <summary>
    /// Helper method to run a single battle simulation with randomly generated teams and timeout protection.
    /// </summary>
    private (SimulatorResult Result, int Turn) RunBattleWithRandomTeamsAndTimeout(
        int team1Seed,
        int team2Seed,
        int player1Seed,
        int player2Seed,
        int battleSeed,
        bool debug)
    {
        var seeds = new BattleSeedContext(team1Seed, team2Seed, player1Seed, player2Seed, battleSeed);

        using var cts = new CancellationTokenSource();
        var simulationTask = Task.Run(() =>
        {
            // Set thread-static context so seeds are visible in the debugger Locals/Watch window
            CurrentBattleSeeds = seeds;
            try
            {
                // Generate random teams using the team seeds for VGC Regulation I
                var team1Generator = new RandomTeamGenerator(Library, FormatId.Gen9VgcRegulationI, team1Seed);
                var team2Generator = new RandomTeamGenerator(Library, FormatId.Gen9VgcRegulationI, team2Seed);

                var team1 = team1Generator.GenerateTeam();
                var team2 = team2Generator.GenerateTeam();

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
                    Id = FormatId.Gen9VgcRegulationI,
                    Player1Options = player1Options,
                    Player2Options = player2Options,
                    Debug = debug,
                    Sync = true, // Use synchronous mode for evaluation
                    Seed = new PrngSeed(battleSeed),
                    MaxTurns = 1000, // Enforce turn limit to detect infinite loops
                };

                var simulator = new SyncSimulator();
                SimulatorResult result = simulator.Run(Library, battleOptions, printDebug: debug);
                return (Result: result, Turn: simulator.Battle?.Turn ?? 0);
            }
            // Exception filter enriches Exception.Data before the stack unwinds,
            // making seeds visible in the VS Exception Details pane on first-chance break
            catch (Exception ex) when (EnrichExceptionWithSeeds(ex, seeds))
            {
                // Never reached: EnrichExceptionWithSeeds always returns false
                throw;
            }
            catch (Exception ex)
            {
                // Wrap exception with seed context for the console summary output
                throw new InvalidOperationException(
                    $"Battle failed. Seeds: Team1={team1Seed}, Team2={team2Seed}, " +
                    $"P1={player1Seed}, P2={player2Seed}, Battle={battleSeed}",
                    ex);
            }
            finally
            {
                CurrentBattleSeeds = null;
            }
        }, cts.Token);

        // Wait for simulation with timeout
        if (!simulationTask.Wait(BattleTimeoutMilliseconds))
        {
            // Timeout occurred - throw custom exception with seeds
            throw new BattleTimeoutException(
                team1Seed,
                team2Seed,
                player1Seed,
                player2Seed,
                battleSeed,
                BattleTimeoutMilliseconds);
        }

        return simulationTask.Result;
    }

    /// <summary>
    /// Helper method to log exception details with seed reproduction instructions.
    /// </summary>
    private void LogExceptionWithSeeds(
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

        Console.WriteLine($"Player 1 Seed: {player1Seed}");
        Console.WriteLine($"Player 2 Seed: {player2Seed}");
        Console.WriteLine($"Battle Seed:   {battleSeed}");
        Console.WriteLine();
        Console.WriteLine("To reproduce this error, use these constants in Driver.cs:");
        Console.WriteLine($"  private const int PlayerRandom1Seed = {player1Seed};");
        Console.WriteLine($"  private const int PlayerRandom2Seed = {player2Seed};");
        Console.WriteLine($"  private const int BattleSeed = {battleSeed};");
        Console.WriteLine();
        Console.WriteLine($"Then run with DriverMode.{debugMode} to debug.");

        if (ex is not BattleTimeoutException and not BattleTurnLimitException)
        {
            Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
        }

        Console.WriteLine("-----------------------------------------------------------");
        Console.WriteLine();
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
        }

        Console.WriteLine("-----------------------------------------------------------");
        Console.WriteLine();
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
