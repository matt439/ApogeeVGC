using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace ApogeeVGC.Sim.Core;

public class Driver
{
    private Library Library { get; } = new();

    private const int PlayerRandom1Seed = 12373; //12345;
    private const int PlayerRandom2Seed = 1847; //1818;
    private const int BattleSeed = 9906; //9876;
    private const int RandomEvaluationNumTest = 10;
    private const int NumThreads = 1;

    public void Start(DriverMode mode)
    {
        switch (mode)
        {
            case DriverMode.GuiVsRandomSingles:
                RunGuiVsRandomSinglesTest();
                break;
            case DriverMode.GuiVsRandomDoubles:
                RunGuiVsRandomDoublesTest();
                break;
            case DriverMode.ConsoleVsRandomSingles:
                RunConsoleVsRandomSinglesTest();
                break;
            case DriverMode.ConsoleVsRandomDoubles:
                RunConsoleVsRandomDoublesTest();
                break;
            case DriverMode.RandomVsRandomSingles:
                RunRandomVsRandomSinglesTest();
                break;
            case DriverMode.AsyncRandomVsRandomSingles:
                RunAsyncRandomVsRandomSinglesTest();
                break;
            case DriverMode.RandomVsRandomSinglesEvaluation:
                RunRandomVsRandomSinglesEvaluationTest();
                break;
            default:
                throw new NotImplementedException($"Driver mode {mode} is not implemented.");
        }
    }

    private void RunConsoleVsRandomSinglesTest()
    {
        const bool debug = true;

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Console, // Use Player namespace
            Name = "Matt",
            Team = TeamGenerator.GenerateTestTeam(Library),
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random, // Use Player namespace
            Name = "Random",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
            PrintDebug = debug,
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomSingles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = debug,
            Sync = false,
            Seed = new PrngSeed(BattleSeed),
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Simulator created");

        // Run the battle synchronously on the main thread
        SimulatorResult result =
            simulator.RunAsync(Library, battleOptions, printDebug: debug).Result;

        Console.WriteLine($"[Driver] Battle completed with result: {result}");
    }

    private void RunConsoleVsRandomDoublesTest()
    {
        throw new NotImplementedException();
    }

    private void RunGuiVsRandomSinglesTest()
    {
        throw new NotImplementedException();
    }

    private void RunGuiVsRandomDoublesTest()
    {
        throw new NotImplementedException();
    }

    private void RunRandomVsRandomSinglesTest()
    {
        Console.WriteLine("[Driver] Starting Random vs Random Singles test (SYNCHRONOUS)");
        Console.WriteLine($"[Driver] Using seeds - Player1: {PlayerRandom1Seed}, Player2: {PlayerRandom2Seed}, Battle: {BattleSeed}");

        const bool debug = true;

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random 1",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom1Seed),
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random 2",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
            PrintDebug = debug,
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomSingles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = debug,
            Sync = true, // Enable synchronous mode
            Seed = new PrngSeed(BattleSeed),
        };

        var simulator = new SyncSimulator();
        Console.WriteLine("[Driver] SyncSimulator created");

        // Run the battle completely synchronously - no async/await needed!
        SimulatorResult result = simulator.Run(Library, battleOptions, printDebug: debug);

        Console.WriteLine($"[Driver] Battle completed with result: {result}");

        // Show final statistics
        Console.WriteLine("\n=== Battle Complete ===");
        Console.WriteLine($"Winner: {result}");
    }

    private void RunAsyncRandomVsRandomSinglesTest()
    {
        Console.WriteLine("[Driver] Starting Random vs Random Singles test (ASYNCHRONOUS)");

        const bool debug = false;

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random 1",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom1Seed),
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random 2",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
            PrintDebug = debug,
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomSingles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = debug,
            Sync = false, // Ensure this is false for async
            Seed = new PrngSeed(BattleSeed),
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Async Simulator created");

        // Run the battle asynchronously
        SimulatorResult result =
            simulator.RunAsync(Library, battleOptions, printDebug: debug).Result;

        Console.WriteLine($"[Driver] Battle completed with result: {result}");

        // Show final statistics
        Console.WriteLine("\n=== Battle Complete ===");
        Console.WriteLine($"Winner: {result}");
    }

    private void RunRandomVsRandomSinglesEvaluationTest()
    {
        Console.WriteLine("[Driver] Starting Random vs Random Singles Evaluation Test");
        Console.WriteLine($"[Driver] Running {RandomEvaluationNumTest} battles with {NumThreads} threads");

        const bool debug = false;

        var simResults = new ConcurrentBag<SimulatorResult>();
        var stopwatch = Stopwatch.StartNew();

        int seedCounter = 0;
        int completedBattles = 0;
        var turnOnBattleEnd = new ConcurrentBag<int>();
        var exceptions = new ConcurrentBag<(int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)>();

        // Run simulations in parallel with specified number of threads
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = NumThreads,
        };

        Parallel.For(0, RandomEvaluationNumTest, parallelOptions, _ =>
        {
            // Calculate unique seeds for this battle using atomic operations
            int offset1 = Interlocked.Increment(ref seedCounter);
            int offset2 = Interlocked.Increment(ref seedCounter);
            int offset3 = Interlocked.Increment(ref seedCounter);
            
            int localPlayer1Seed = PlayerRandom1Seed + offset1;
            int localPlayer2Seed = PlayerRandom2Seed + offset2;
            int localBattleSeed = BattleSeed + offset3;

            try
            {
                PlayerOptions player1Options = new()
                {
                    Type = Player.PlayerType.Random,
                    Name = "Random 1",
                    Team = TeamGenerator.GenerateTestTeam(Library),
                    Seed = new PrngSeed(localPlayer1Seed),
                    PrintDebug = debug,
                };

                PlayerOptions player2Options = new()
                {
                    Type = Player.PlayerType.Random,
                    Name = "Random 2",
                    Team = TeamGenerator.GenerateTestTeam(Library),
                    Seed = new PrngSeed(localPlayer2Seed),
                    PrintDebug = debug,
                };

                BattleOptions battleOptions = new()
                {
                    Id = FormatId.CustomSingles,
                    Player1Options = player1Options,
                    Player2Options = player2Options,
                    Debug = debug,
                    Sync = true, // Use synchronous mode for evaluation
                    Seed = new PrngSeed(localBattleSeed),
                };

                var simulator = new SyncSimulator();
                SimulatorResult result = simulator.Run(Library, battleOptions, printDebug: debug);
                simResults.Add(result);
                turnOnBattleEnd.Add(simulator.Battle?.Turn ?? 0);
                
                int completed = Interlocked.Increment(ref completedBattles);
                if (completed % 100 == 0)
                {
                    Console.WriteLine($"[Driver] Completed {completed}/{RandomEvaluationNumTest} battles");
                }
            }
            catch (Exception ex)
            {
                // Store exception with seed information for debugging
                exceptions.Add((localPlayer1Seed, localPlayer2Seed, localBattleSeed, ex));
                
                // Log immediately to console
                Console.WriteLine();
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine("ERROR: Exception occurred during battle simulation!");
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine($"Player 1 Seed: {localPlayer1Seed}");
                Console.WriteLine($"Player 2 Seed: {localPlayer2Seed}");
                Console.WriteLine($"Battle Seed:   {localBattleSeed}");
                Console.WriteLine();
                Console.WriteLine("To reproduce this error, use these constants in Driver.cs:");
                Console.WriteLine($"  private const int PlayerRandom1Seed = {localPlayer1Seed};");
                Console.WriteLine($"  private const int PlayerRandom2Seed = {localPlayer2Seed};");
                Console.WriteLine($"  private const int BattleSeed = {localBattleSeed};");
                Console.WriteLine();
                Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine();
            }
        });

        stopwatch.Stop();

        // Convert to list for counting (now from ConcurrentBag)
        var resultsList = simResults.ToList();
        var turnsList = turnOnBattleEnd.ToList();
        var exceptionsList = exceptions.ToList();
        
        int successfulBattles = resultsList.Count;
        int failedBattles = exceptionsList.Count;
        int player1Wins = resultsList.Count(result => result == SimulatorResult.Player1Win);
        int player2Wins = resultsList.Count(result => result == SimulatorResult.Player2Win);
        int ties = resultsList.Count(result => result == SimulatorResult.Tie);

        // Calculate timing metrics
        double totalSeconds = stopwatch.Elapsed.TotalSeconds;
        double timePerSimulation = totalSeconds / RandomEvaluationNumTest;
        double simulationsPerSecond = RandomEvaluationNumTest / totalSeconds;

        // Calculate turn statistics (only if there are successful battles)
        double meanTurns = 0;
        double stdDevTurns = 0;
        double medianTurns = 0;
        int minTurns = 0;
        int maxTurns = 0;
        
        if (turnsList.Count > 0)
        {
            meanTurns = turnsList.Mean();
            stdDevTurns = turnsList.StandardDeviation();
            medianTurns = turnsList.Median();
            minTurns = turnsList.Minimum();
            maxTurns = turnsList.Maximum();
        }

        // Print results
        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine($"Random vs Random Evaluation Results ({RandomEvaluationNumTest} battles):");
        sb.AppendLine("Format: Singles");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {RandomEvaluationNumTest}");
        sb.AppendLine();
        
        if (failedBattles > 0)
        {
            sb.AppendLine("⚠️  EXCEPTION SUMMARY:");
            sb.AppendLine("═══════════════════════════════════════════════════════════");
            for (int i = 0; i < exceptionsList.Count; i++)
            {
                var (p1Seed, p2Seed, bSeed, ex) = exceptionsList[i];
                sb.AppendLine($"Exception #{i + 1}:");
                sb.AppendLine($"  Player 1 Seed: {p1Seed}");
                sb.AppendLine($"  Player 2 Seed: {p2Seed}");
                sb.AppendLine($"  Battle Seed:   {bSeed}");
                sb.AppendLine($"  Exception:     {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine();
            }
            sb.AppendLine("═══════════════════════════════════════════════════════════");
            sb.AppendLine();
        }
        
        sb.AppendLine("Battle Results (Successful Battles Only):");
        sb.AppendLine($"Player 1 Wins: {player1Wins}");
        sb.AppendLine($"Player 2 Wins: {player2Wins}");
        sb.AppendLine($"Ties: {ties}");
        if (successfulBattles > 0)
        {
            sb.AppendLine($"Win Rate for Player 1: {(double)player1Wins / successfulBattles:P2}");
            sb.AppendLine($"Win Rate for Player 2: {(double)player2Wins / successfulBattles:P2}");
            sb.AppendLine($"Tie Rate: {(double)ties / successfulBattles:P2}");
        }
        sb.AppendLine();
        sb.AppendLine("Performance Metrics:");
        sb.AppendLine($"Number of threads: {NumThreads}");
        sb.AppendLine($@"Total Execution Time: {stopwatch.Elapsed:hh\:mm\:ss\.fff}");
        sb.AppendLine($"Total Execution Time (seconds): {totalSeconds:F3}");
        sb.AppendLine($"Time per Simulation: {timePerSimulation * 1000:F3} ms");
        sb.AppendLine($"Simulations per Second: {simulationsPerSecond:F0}");
        sb.AppendLine();
        sb.AppendLine("Turn Statistics:");
        sb.AppendLine($"Mean Turns: {meanTurns:F2}");
        sb.AppendLine($"Standard Deviation of Turns: {stdDevTurns:F2}");
        sb.AppendLine($"Median Turns: {medianTurns:F2}");
        sb.AppendLine($"Minimum Turns: {minTurns}");
        sb.AppendLine($"Maximum Turns: {maxTurns}");
        Console.WriteLine(sb.ToString());

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}