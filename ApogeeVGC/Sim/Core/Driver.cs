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

    private const int PlayerRandom1Seed = 12345;
    private const int PlayerRandom2Seed = 1818;
    private PrngSeed DefaultSeed { get; } = new(9876);
    private const int RandomEvaluationNumTest = 1;
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
            Seed = DefaultSeed,
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
            Seed = DefaultSeed,
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
            Seed = DefaultSeed,
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

        RunRandomVsRandomEvaluationTestAsync().Wait();
    }

    private async Task RunRandomVsRandomEvaluationTestAsync()
    {
        const bool debug = false;

        var simResults = new ConcurrentBag<SimulatorResult>();
        var stopwatch = Stopwatch.StartNew();

        int seedCounter = 0;
        var turnOnBattleEnd = new ConcurrentBag<int>();

        using var semaphore = new SemaphoreSlim(NumThreads, NumThreads);

        // Create tasks for each simulation with concurrency control
        var tasks = Enumerable.Range(0, RandomEvaluationNumTest)
            .Select(async _ =>
            {
                await semaphore.WaitAsync(); // Wait for available slot
                try
                {
                    int currentSeed = Interlocked.Increment(ref seedCounter);
                    int player1Seed = PlayerRandom1Seed + currentSeed;
                    int player2Seed = PlayerRandom2Seed + currentSeed;

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
                        Id = FormatId.CustomSingles,
                        Player1Options = player1Options,
                        Player2Options = player2Options,
                        Debug = debug,
                        Sync = true, // Use synchronous mode for evaluation
                        Seed = new PrngSeed(currentSeed),
                    };

                    var simulator = new SyncSimulator();
                    SimulatorResult result = simulator.Run(Library, battleOptions, printDebug: debug);
                    simResults.Add(result);
                    turnOnBattleEnd.Add(simulator.Battle?.Turn ?? 0);
                }
                finally
                {
                    semaphore.Release(); // Release the slot
                }
            })
            .ToArray();

        // Wait for all simulations to complete
        await Task.WhenAll(tasks);

        stopwatch.Stop();

        // Convert to list for counting (now from ConcurrentBag)
        var resultsList = simResults.ToList();
        var turnsList = turnOnBattleEnd.ToList();
        int player1Wins = resultsList.Count(result => result == SimulatorResult.Player1Win);
        int player2Wins = resultsList.Count(result => result == SimulatorResult.Player2Win);
        int ties = resultsList.Count(result => result == SimulatorResult.Tie);

        // Calculate timing metrics
        double totalSeconds = stopwatch.Elapsed.TotalSeconds;
        double timePerSimulation = totalSeconds / RandomEvaluationNumTest;
        double simulationsPerSecond = RandomEvaluationNumTest / totalSeconds;

        // Calculate turn statistics
        double meanTurns = turnsList.Mean();
        double stdDevTurns = turnsList.StandardDeviation();
        double medianTurns = turnsList.Median();
        int minTurns = turnsList.Minimum();
        int maxTurns = turnsList.Maximum();

        // Print results
        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine($"Random vs Random Evaluation Results ({RandomEvaluationNumTest} battles):");
        sb.AppendLine("Format: Singles");
        sb.AppendLine();
        sb.AppendLine("Battle Results:");
        sb.AppendLine($"Player 1 Wins: {player1Wins}");
        sb.AppendLine($"Player 2 Wins: {player2Wins}");
        sb.AppendLine($"Ties: {ties}");
        sb.AppendLine($"Win Rate for Player 1: {(double)player1Wins / RandomEvaluationNumTest:P2}");
        sb.AppendLine($"Win Rate for Player 2: {(double)player2Wins / RandomEvaluationNumTest:P2}");
        sb.AppendLine($"Tie Rate: {(double)ties / RandomEvaluationNumTest:P2}");
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