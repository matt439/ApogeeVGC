using ApogeeVGC.Data;
using ApogeeVGC.Gui;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace ApogeeVGC.Sim.Core;

public class Driver
{
    private Library Library { get; } = new();

    // Seeds for standard test runs - can be changed freely
    //private const int PlayerRandom1Seed = 12352;
    private const int PlayerRandom2Seed = 1826;
    private const int BattleSeed = 9885;

    // Don't change these - used for evaluation test to reproduce errors
    private const int PlayerRandom1EvalSeed = 12345;
    private const int PlayerRandom2EvalSeed = 1818;
    private const int BattleEvalSeed = 9876;

    // Seeds for VGC Regulation I random team generation evaluation
    private const int Team1EvalSeed = 54321;
    private const int Team2EvalSeed = 67890;

    private const int RandomEvaluationNumTest = 1000;
    private const int NumThreads = 24;
    private const int BattleTimeoutMilliseconds = 3000; // 3 seconds timeout per battle

    private const int IncrementalDebugMaxIterations = 500;

    // Format configuration - change this to use different VGC regulations
    // Available: Gen9VgcRegulationA through Gen9VgcRegulationI, CustomSingles, CustomDoubles
    //private const FormatId DefaultVgcFormat = FormatId.Gen9VgcRegulationG;
    //private const FormatId DefaultSinglesFormat = FormatId.CustomSingles;
    //private const FormatId DefaultDoublesFormat = FormatId.CustomDoubles;


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
            //case DriverMode.RandomVsRandomSingles:
            //    RunRandomVsRandomSinglesTest();
            //    break;
            //case DriverMode.RandomVsRandomDoubles:
            //    RunRandomVsRandomDoublesTest();
            //    break;
            case DriverMode.RandomVsRandomSinglesEvaluation:
                RunRandomVsRandomSinglesEvaluationTest();
                break;
            case DriverMode.RandomVsRandomDoublesEvaluation:
                RunRandomVsRandomDoublesEvaluationTest();
                break;
            case DriverMode.RndVsRndVgcRegIEvaluation:
                RunRndVsRndVgcRegIEvaluation();
                break;
            case DriverMode.IncrementalDebug:
                RunIncrementalDebugTest();
                break;
            case DriverMode.SingleBattleDebug:
                RunSingleBattleDebug();
                break;
            default:
                throw new InvalidOperationException($"Driver mode {mode} is not implemented.");
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
        const bool debug = true;

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Console,
            Name = "Matt",
            Team = TeamGenerator.GenerateTestTeam(Library),
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
            PrintDebug = debug,
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomDoubles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = debug,
            Sync = false,
            Seed = new PrngSeed(BattleSeed),
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Simulator created");

        // Run the battle asynchronously on the main thread
        SimulatorResult result =
            simulator.RunAsync(Library, battleOptions, printDebug: debug).Result;

        Console.WriteLine($"[Driver] Battle completed with result: {result}");
    }

    private void RunGuiVsRandomSinglesTest()
    {
        Console.WriteLine("[Driver] Starting GUI vs Random Singles test");
        const bool debug = true;

        // Create the MonoGame window
        using var game = new BattleGame();

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Gui,
            Name = "Matt",
            Team = TeamGenerator.GenerateTestTeam(Library),
            GuiWindow = game,
            GuiChoiceCoordinator = game.GetChoiceCoordinator(),
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
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

        // Start the battle (will defer until LoadContent completes)
        game.StartBattle(Library, battleOptions, simulator);
        Console.WriteLine("[Driver] Battle start queued");

        // Run MonoGame on main thread (blocking call)
        game.Run();

        Console.WriteLine("[Driver] Battle completed");
    }

    private void RunGuiVsRandomDoublesTest()
    {
        Console.WriteLine("[Driver] Starting GUI vs Random Doubles test");
        const bool debug = true;

        // Create the MonoGame window
        using var game = new BattleGame();

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Gui,
            Name = "Matt",
            Team = TeamGenerator.GenerateTestTeam(Library),
            GuiWindow = game,
            GuiChoiceCoordinator = game.GetChoiceCoordinator(),
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
            PrintDebug = debug,
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomDoubles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = debug,
            Sync = false,
            Seed = new PrngSeed(BattleSeed),
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Simulator created");

        // Start the battle (will defer until LoadContent completes)
        game.StartBattle(Library, battleOptions, simulator);
        Console.WriteLine("[Driver] Battle start queued");

        // Run MonoGame on main thread (blocking call)
        game.Run();

        Console.WriteLine("[Driver] Battle completed");
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
        using var cts = new CancellationTokenSource();
        var simulationTask = Task.Run(() =>
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

    private void RunRandomVsRandomSinglesEvaluationTest()
    {
        Console.WriteLine("[Driver] Starting Random vs Random Singles Evaluation Test");
        Console.WriteLine($"[Driver] Running {RandomEvaluationNumTest} battles with {NumThreads} threads");

        const bool debug = false;

        var simResults = new ConcurrentBag<SimulatorResult>();
        var stopwatch = Stopwatch.StartNew();

        var seedCounter = 0;
        var completedBattles = 0;
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

            int localPlayer1Seed = PlayerRandom1EvalSeed + offset1;
            int localPlayer2Seed = PlayerRandom2EvalSeed + offset2;
            int localBattleSeed = BattleEvalSeed + offset3;

            try
            {
                (SimulatorResult result, int turn) = RunBattleWithTimeout(
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    FormatId.CustomSingles,
                    debug);

                simResults.Add(result);
                turnOnBattleEnd.Add(turn);

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
                LogExceptionWithSeeds(
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    ex,
                    "RandomVsRandomSingles");
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
        var minTurns = 0;
        var maxTurns = 0;

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
        sb.AppendLine($"Git Commit: {GetGitCommitId()}");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {RandomEvaluationNumTest}");
        sb.AppendLine();

        if (failedBattles > 0)
        {
            sb.AppendLine("??  EXCEPTION SUMMARY:");
            sb.AppendLine("-----------------------------------------------------------");
            for (var i = 0; i < exceptionsList.Count; i++)
            {
                (int p1Seed, int p2Seed, int bSeed, Exception ex) = exceptionsList[i];
                sb.AppendLine($"Exception #{i + 1}:");
                sb.AppendLine($"  Player 1 Seed: {p1Seed}");
                sb.AppendLine($"  Player 2 Seed: {p2Seed}");
                sb.AppendLine($"  Battle Seed:   {bSeed}");
                sb.AppendLine($"  Exception:     {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine();
            }

            sb.AppendLine("-----------------------------------------------------------");
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

        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
    }

    private void RunRandomVsRandomDoublesEvaluationTest()
    {
        Console.WriteLine("[Driver] Starting Random vs Random Doubles Evaluation Test");
        Console.WriteLine($"[Driver] Running {RandomEvaluationNumTest} battles with {NumThreads} threads");

        const bool debug = false;

        var simResults = new ConcurrentBag<SimulatorResult>();
        var stopwatch = Stopwatch.StartNew();

        var seedCounter = 0;
        var completedBattles = 0;
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

            int localPlayer1Seed = PlayerRandom1EvalSeed + offset1;
            int localPlayer2Seed = PlayerRandom2EvalSeed + offset2;
            int localBattleSeed = BattleEvalSeed + offset3;

            try
            {
                (SimulatorResult result, int turn) = RunBattleWithTimeout(
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    FormatId.Gen9VgcRegulationA,
                    debug);

                simResults.Add(result);
                turnOnBattleEnd.Add(turn);

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
                LogExceptionWithSeeds(
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    ex,
                    "RandomVsRandomDoubles");
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
        var minTurns = 0;
        var maxTurns = 0;

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
        sb.AppendLine("Format: Doubles");
        sb.AppendLine($"Git Commit: {GetGitCommitId()}");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {RandomEvaluationNumTest}");
        sb.AppendLine();

        if (failedBattles > 0)
        {
            sb.AppendLine("??  EXCEPTION SUMMARY:");
            sb.AppendLine("-----------------------------------------------------------");
            for (var i = 0; i < exceptionsList.Count; i++)
            {
                (int p1Seed, int p2Seed, int bSeed, Exception ex) = exceptionsList[i];
                sb.AppendLine($"Exception #{i + 1}:");
                sb.AppendLine($"  Player 1 Seed: {p1Seed}");
                sb.AppendLine($"  Player 2 Seed: {p2Seed}");
                sb.AppendLine($"  Battle Seed:   {bSeed}");
                sb.AppendLine($"  Exception:     {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine();
            }

            sb.AppendLine("-----------------------------------------------------------");
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

        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
    }

    private void RunRndVsRndVgcRegIEvaluation()
    {
        Console.WriteLine("[Driver] Starting Random Team vs Random Team VGC Reg I Evaluation");
        Console.WriteLine($"[Driver] Running {RandomEvaluationNumTest} battles with {NumThreads} threads");

        const bool debug = false;

        var simResults = new ConcurrentBag<SimulatorResult>();
        var stopwatch = Stopwatch.StartNew();

        var seedCounter = 0;
        var completedBattles = 0;
        var turnOnBattleEnd = new ConcurrentBag<int>();
        var exceptions =
            new ConcurrentBag<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception
                Exception)>();

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
            int offset4 = Interlocked.Increment(ref seedCounter);
            int offset5 = Interlocked.Increment(ref seedCounter);

            int localTeam1Seed = Team1EvalSeed + offset1;
            int localTeam2Seed = Team2EvalSeed + offset2;
            int localPlayer1Seed = PlayerRandom1EvalSeed + offset3;
            int localPlayer2Seed = PlayerRandom2EvalSeed + offset4;
            int localBattleSeed = BattleEvalSeed + offset5;

            try
            {
                (SimulatorResult result, int turn) = RunBattleWithRandomTeamsAndTimeout(
                    localTeam1Seed,
                    localTeam2Seed,
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    debug);

                simResults.Add(result);
                turnOnBattleEnd.Add(turn);

                int completed = Interlocked.Increment(ref completedBattles);
                if (completed % 100 == 0)
                {
                    Console.WriteLine($"[Driver] Completed {completed}/{RandomEvaluationNumTest} battles");
                }
            }
            catch (Exception ex)
            {
                // Store exception with all seed information for debugging
                exceptions.Add(
                    (localTeam1Seed, localTeam2Seed, localPlayer1Seed, localPlayer2Seed, localBattleSeed, ex));

                // Log immediately to console
                LogExceptionWithAllSeeds(
                    localTeam1Seed,
                    localTeam2Seed,
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    ex,
                    "RndVsRndVgcRegIEvaluation");
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
        var minTurns = 0;
        var maxTurns = 0;

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
        sb.AppendLine($"Random Team vs Random Team VGC Reg I Evaluation Results ({RandomEvaluationNumTest} battles):");
        sb.AppendLine("Format: VGC 2025 Regulation I (Random Teams)");
        sb.AppendLine($"Git Commit: {GetGitCommitId()}");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {RandomEvaluationNumTest}");
        sb.AppendLine();

        if (failedBattles > 0)
        {
            sb.AppendLine("??  EXCEPTION SUMMARY:");
            sb.AppendLine("-----------------------------------------------------------");
            for (var i = 0; i < exceptionsList.Count; i++)
            {
                (int t1Seed, int t2Seed, int p1Seed, int p2Seed, int bSeed, Exception ex) = exceptionsList[i];
                sb.AppendLine($"Exception #{i + 1}:");
                sb.AppendLine($"  Team 1 Seed:   {t1Seed}");
                sb.AppendLine($"  Team 2 Seed:   {t2Seed}");
                sb.AppendLine($"  Player 1 Seed: {p1Seed}");
                sb.AppendLine($"  Player 2 Seed: {p2Seed}");
                sb.AppendLine($"  Battle Seed:   {bSeed}");
                sb.AppendLine($"  Exception:     {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine();
            }

            sb.AppendLine("-----------------------------------------------------------");
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

        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
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
        using var cts = new CancellationTokenSource();
        var simulationTask = Task.Run(() =>
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

    /// <summary>
    /// Runs the incremental debug test to identify problematic elements.
    /// Starts with a known-good baseline and incrementally adds elements.
    /// </summary>
    private void RunIncrementalDebugTest()
    {
        Console.WriteLine("[Driver] Starting Incremental Debug Test");
        Console.WriteLine("[Driver] This mode incrementally tests elements to identify bugs.");
        Console.WriteLine();

        var runner = new IncrementalTestRunner(
            Library,
            formatId: FormatId.CustomDoubles,
            battlesPerVerification: IncrementalDebugMaxIterations,
            battleTimeoutMs: BattleTimeoutMilliseconds,
            printDebug: false,
            stateDirectory: ".");

        // Run unlimited iterations (0 = test all elements)
        IncrementalTestRunner.TestingSummary summary;
        try
        {
            summary = runner.Run(maxIterations: 0);
        }
        catch (Exception ex)
        {
            // Each battle consumes 3 consecutive seeds, so the last battle used these:
            int lastBattleSeed = runner.SeedCounter;
            int lastP2Seed = lastBattleSeed - 1;
            int lastP1Seed = lastBattleSeed - 2;

            Console.WriteLine();
            Console.WriteLine("[Driver] UNHANDLED EXCEPTION during incremental debug test!");
            Console.WriteLine($"[Driver] Exception: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"[Driver] Last seeds: P1={lastP1Seed}, P2={lastP2Seed}, Battle={lastBattleSeed}");
            Console.WriteLine("[Driver] Use these seeds in RunSingleBattleDebug to reproduce the failure.");
            throw;
        }

        Console.WriteLine();
        Console.WriteLine("Incremental debug test complete.");
        Console.WriteLine($"Total iterations: {summary.IterationsCompleted}");
        Console.WriteLine($"Failed elements: {summary.FailedMoves.Count} moves, " +
                          $"{summary.FailedAbilities.Count} abilities, " +
                          $"{summary.FailedItems.Count} items, " +
                          $"{summary.FailedSpecies.Count} species");

        Console.WriteLine();
        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
    }

    /// <summary>
    /// Runs a single battle with specific seeds for debugging.
    /// Edit the seed constants and testing element in this method to reproduce
    /// specific battle scenarios from IncrementalDebugTest failures.
    /// </summary>
    private void RunSingleBattleDebug()
    {
        // !! EDIT THESE VALUES TO DEBUG SPECIFIC BATTLES !!
        // Copy the seeds and testing element from the IncrementalDebugTest failure output.
        const int debugPlayer1Seed = 25834;
        const int debugPlayer2Seed = debugPlayer1Seed + 1;
        const int debugBattleSeed = debugPlayer2Seed + 1;
        const bool debug = true;

        // Set the element that was being tested when the failure occurred.
        // Only ONE of these should be set; the rest must remain None/default.
        // Copy the value from the "Testing Moves/Abilities/Items:" output line.
        const MoveId debugTestingMove = MoveId.LunarDance;
        const AbilityId debugTestingAbility = AbilityId.None;
        const ItemId debugTestingItem = ItemId.None;
        const SpecieId debugTestingSpecies = default; // SpecieId has no None; default (Bulbasaur) means "not set"

        Console.WriteLine("[Driver] Starting Single Battle Debug");
        Console.WriteLine($"[Driver] Seeds - Player1: {debugPlayer1Seed}, Player2: {debugPlayer2Seed}, Battle: {debugBattleSeed}");
        Console.WriteLine();

        // Load the saved pools state (after failure, the testing element is in the Failed set).
        // We restore it to Testing so Available = Allowed + Testing matches the failing battle.
        var pools = new DebugElementPools();
        if (!pools.LoadState())
        {
            Console.WriteLine("WARNING: No debug-element-pools-state.json found!");
            Console.WriteLine("This file is required to recreate battles from IncrementalDebugTest.");
            Console.WriteLine("Make sure the state file exists in the working directory.");
            Console.WriteLine();
            Console.WriteLine("Press Enter key to exit...");
            Console.ReadLine();
            return;
        }

        // Restore the testing element from Failed back to Testing
        if (debugTestingMove != MoveId.None)
        {
            pools.RestoreFailedToTesting(debugTestingMove);
            Console.WriteLine($"Restored testing move: {debugTestingMove}");
        }
        if (debugTestingAbility != AbilityId.None)
        {
            pools.RestoreFailedToTesting(debugTestingAbility);
            Console.WriteLine($"Restored testing ability: {debugTestingAbility}");
        }
        if (debugTestingItem != ItemId.None)
        {
            pools.RestoreFailedToTesting(debugTestingItem);
            Console.WriteLine($"Restored testing item: {debugTestingItem}");
        }
        if (debugTestingSpecies != default)
        {
            pools.RestoreFailedToTesting(debugTestingSpecies);
            Console.WriteLine($"Restored testing species: {debugTestingSpecies}");
        }

        Console.WriteLine("Loaded debug element pools state:");
        Console.WriteLine(pools.GetSummary());
        Console.WriteLine();

        try
        {
            // Create teams using the same DebugTeamGenerator as IncrementalTestRunner
            var team1Gen = new DebugTeamGenerator(Library, pools, debugPlayer1Seed);
            var team2Gen = new DebugTeamGenerator(Library, pools, debugPlayer2Seed);

            var team1 = team1Gen.GenerateTeam();
            var team2 = team2Gen.GenerateTeam();

            PlayerOptions player1Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Debug1",
                Team = team1,
                Seed = new PrngSeed(debugPlayer1Seed),
                PrintDebug = debug,
            };

            PlayerOptions player2Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Debug2",
                Team = team2,
                Seed = new PrngSeed(debugPlayer2Seed),
                PrintDebug = debug,
            };

            BattleOptions battleOptions = new()
            {
                Id = FormatId.CustomDoubles, // Same format as IncrementalTestRunner
                Player1Options = player1Options,
                Player2Options = player2Options,
                Debug = debug,
                Sync = true,
                Seed = new PrngSeed(debugBattleSeed),
                MaxTurns = 1000,
            };

            var simulator = new SyncSimulator();
            SimulatorResult result = simulator.Run(Library, battleOptions, printDebug: debug);

            Console.WriteLine();
            Console.WriteLine("=== Battle Result ===");
            Console.WriteLine($"Winner: {result}");
            Console.WriteLine($"Turns: {simulator.Battle?.Turn ?? 0}");
            Console.WriteLine($"Battle completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("=== Battle Failed ===");
            Console.WriteLine($"Exception Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Stack Trace:");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine();
        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
    }

    //private void RunRandomVsRandomSinglesTest()
    //{
    //    Console.WriteLine("[Driver] Starting Random vs Random Singles test (SYNCHRONOUS)");
    //    Console.WriteLine(
    //        $"[Driver] Using seeds - Player1: {PlayerRandom1Seed}, Player2: {PlayerRandom2Seed}, Battle: {BattleSeed}");

    //    const bool debug = true;

    //    PlayerOptions player1Options = new()
    //    {
    //        Type = Player.PlayerType.Random,
    //        Name = "Random 1",
    //        Team = TeamGenerator.GenerateTestTeam(Library),
    //        Seed = new PrngSeed(PlayerRandom1Seed),
    //        PrintDebug = debug,
    //    };

    //    PlayerOptions player2Options = new()
    //    {
    //        Type = Player.PlayerType.Random,
    //        Name = "Random 2",
    //        Team = TeamGenerator.GenerateTestTeam(Library),
    //        Seed = new PrngSeed(PlayerRandom2Seed),
    //        PrintDebug = debug,
    //    };

    //    BattleOptions battleOptions = new()
    //    {
    //        Id = FormatId.CustomSingles,
    //        Player1Options = player1Options,
    //        Player2Options = player2Options,
    //        Debug = debug,
    //        Sync = true, // Enable synchronous mode
    //        Seed = new PrngSeed(BattleSeed),
    //    };

    //    var simulator = new SyncSimulator();
    //    Console.WriteLine("[Driver] SyncSimulator created");

    //    // Run the battle completely synchronously - no async/await needed!
    //    var result = simulator.Run(Library, battleOptions, printDebug: debug);

    //    Console.WriteLine($"[Driver] Battle completed with result: {result}");

    //    // Show final statistics
    //    Console.WriteLine("\n=== Battle Complete ===");
    //    Console.WriteLine($"Winner: {result}");
    //}

    //private void RunRandomVsRandomDoublesTest()
    //{
    //    Console.WriteLine("[Driver] Starting Random vs Random Doubles test (SYNCHRONOUS)");
    //    Console.WriteLine(
    //        $"[Driver] Using seeds - Player1: {PlayerRandom1Seed}, Player2: {PlayerRandom2Seed}, Battle: {BattleSeed}");

    //    const bool debug = true;

    //    PlayerOptions player1Options = new()
    //    {
    //        Type = Player.PlayerType.Random,
    //        Name = "Random 1",
    //        Team = TeamGenerator.GenerateTestTeam(Library),
    //        Seed = new PrngSeed(PlayerRandom1Seed),
    //        PrintDebug = debug,
    //    };

    //    PlayerOptions player2Options = new()
    //    {
    //        Type = Player.PlayerType.Random,
    //        Name = "Random 2",
    //        Team = TeamGenerator.GenerateTestTeam(Library),
    //        Seed = new PrngSeed(PlayerRandom2Seed),
    //        PrintDebug = debug,
    //    };

    //    BattleOptions battleOptions = new()
    //    {
    //        Id = FormatId.CustomDoubles,
    //        Player1Options = player1Options,
    //        Player2Options = player2Options,
    //        Debug = debug,
    //        Sync = true, // Enable synchronous mode
    //        Seed = new PrngSeed(BattleSeed),
    //    };

    //    var simulator = new SyncSimulator();
    //    Console.WriteLine("[Driver] SyncSimulator created");

    //    // Run the battle completely synchronously - no async/await needed!
    //    var result = simulator.Run(Library, battleOptions, printDebug: debug);

    //    Console.WriteLine($"[Driver] Battle completed with result: {result}");

    //    // Show final statistics
    //    Console.WriteLine("\n=== Battle Complete ===");
    //    Console.WriteLine($"Winner: {result}");
    //}
}