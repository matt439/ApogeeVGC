using ApogeeVGC.Data;
using ApogeeVGC.Gui;
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
            case DriverMode.DebugSingleBattle:
                RunDebugSingleBattle();
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
        var result =
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
        var result =
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
            var result = simulator.Run(Library, battleOptions, printDebug: debug);
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
            var offset1 = Interlocked.Increment(ref seedCounter);
            var offset2 = Interlocked.Increment(ref seedCounter);
            var offset3 = Interlocked.Increment(ref seedCounter);

            var localPlayer1Seed = PlayerRandom1EvalSeed + offset1;
            var localPlayer2Seed = PlayerRandom2EvalSeed + offset2;
            var localBattleSeed = BattleEvalSeed + offset3;

            try
            {
                var (result, turn) = RunBattleWithTimeout(
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    FormatId.CustomSingles,
                    debug);

                simResults.Add(result);
                turnOnBattleEnd.Add(turn);

                var completed = Interlocked.Increment(ref completedBattles);
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

        var successfulBattles = resultsList.Count;
        var failedBattles = exceptionsList.Count;
        var player1Wins = resultsList.Count(result => result == SimulatorResult.Player1Win);
        var player2Wins = resultsList.Count(result => result == SimulatorResult.Player2Win);
        var ties = resultsList.Count(result => result == SimulatorResult.Tie);

        // Calculate timing metrics
        var totalSeconds = stopwatch.Elapsed.TotalSeconds;
        var timePerSimulation = totalSeconds / RandomEvaluationNumTest;
        var simulationsPerSecond = RandomEvaluationNumTest / totalSeconds;

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
                var (p1Seed, p2Seed, bSeed, ex) = exceptionsList[i];
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
            var offset1 = Interlocked.Increment(ref seedCounter);
            var offset2 = Interlocked.Increment(ref seedCounter);
            var offset3 = Interlocked.Increment(ref seedCounter);

            var localPlayer1Seed = PlayerRandom1EvalSeed + offset1;
            var localPlayer2Seed = PlayerRandom2EvalSeed + offset2;
            var localBattleSeed = BattleEvalSeed + offset3;

            try
            {
                var (result, turn) = RunBattleWithTimeout(
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    FormatId.Gen9VgcRegulationA,
                    debug);

                simResults.Add(result);
                turnOnBattleEnd.Add(turn);

                var completed = Interlocked.Increment(ref completedBattles);
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

        var successfulBattles = resultsList.Count;
        var failedBattles = exceptionsList.Count;
        var player1Wins = resultsList.Count(result => result == SimulatorResult.Player1Win);
        var player2Wins = resultsList.Count(result => result == SimulatorResult.Player2Win);
        var ties = resultsList.Count(result => result == SimulatorResult.Tie);

        // Calculate timing metrics
        var totalSeconds = stopwatch.Elapsed.TotalSeconds;
        var timePerSimulation = totalSeconds / RandomEvaluationNumTest;
        var simulationsPerSecond = RandomEvaluationNumTest / totalSeconds;

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
                var (p1Seed, p2Seed, bSeed, ex) = exceptionsList[i];
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
            var offset1 = Interlocked.Increment(ref seedCounter);
            var offset2 = Interlocked.Increment(ref seedCounter);
            var offset3 = Interlocked.Increment(ref seedCounter);
            var offset4 = Interlocked.Increment(ref seedCounter);
            var offset5 = Interlocked.Increment(ref seedCounter);

            var localTeam1Seed = Team1EvalSeed + offset1;
            var localTeam2Seed = Team2EvalSeed + offset2;
            var localPlayer1Seed = PlayerRandom1EvalSeed + offset3;
            var localPlayer2Seed = PlayerRandom2EvalSeed + offset4;
            var localBattleSeed = BattleEvalSeed + offset5;

            try
            {
                var (result, turn) = RunBattleWithRandomTeamsAndTimeout(
                    localTeam1Seed,
                    localTeam2Seed,
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    debug);

                simResults.Add(result);
                turnOnBattleEnd.Add(turn);

                var completed = Interlocked.Increment(ref completedBattles);
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

        var successfulBattles = resultsList.Count;
        var failedBattles = exceptionsList.Count;
        var player1Wins = resultsList.Count(result => result == SimulatorResult.Player1Win);
        var player2Wins = resultsList.Count(result => result == SimulatorResult.Player2Win);
        var ties = resultsList.Count(result => result == SimulatorResult.Tie);

        // Calculate timing metrics
        var totalSeconds = stopwatch.Elapsed.TotalSeconds;
        var timePerSimulation = totalSeconds / RandomEvaluationNumTest;
        var simulationsPerSecond = RandomEvaluationNumTest / totalSeconds;

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
                var (t1Seed, t2Seed, p1Seed, p2Seed, bSeed, ex) = exceptionsList[i];
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
    /// Debug method to run a single battle with specific seeds for reproduction.
    /// Modify the seed values directly in this method when debugging specific issues.
    /// </summary>
    private void RunDebugSingleBattle()
    {
        Console.WriteLine("[Driver] Running Debug Single Battle");
        Console.WriteLine("[Driver] This mode is for debugging specific battle issues");
        Console.WriteLine();

        // MODIFY THESE VALUES TO REPRODUCE SPECIFIC BATTLES
        const int debugTeam1Seed = 56202;
        const int debugTeam2Seed = 69772;
        const int debugPlayer1Seed = 14228;
        const int debugPlayer2Seed = 3702;
        const int debugBattleSeed = 11761;
        const bool debug = true;  // Set to true for verbose logging

        Console.WriteLine("Seeds:");
        Console.WriteLine($"  Team 1 Seed:   {debugTeam1Seed}");
        Console.WriteLine($"  Team 2 Seed:   {debugTeam2Seed}");
        Console.WriteLine($"  Player 1 Seed: {debugPlayer1Seed}");
        Console.WriteLine($"  Player 2 Seed: {debugPlayer2Seed}");
        Console.WriteLine($"  Battle Seed:   {debugBattleSeed}");
        Console.WriteLine();

        try
        {
            // Generate teams using same logic as evaluation
            var team1Generator = new RandomTeamGenerator(Library, FormatId.Gen9VgcRegulationI, debugTeam1Seed);
            var team2Generator = new RandomTeamGenerator(Library, FormatId.Gen9VgcRegulationI, debugTeam2Seed);

            var team1 = team1Generator.GenerateTeam();
            var team2 = team2Generator.GenerateTeam();

            Console.WriteLine("Team 1:");
            foreach (var set in team1)
            {
                Console.WriteLine($"  - {set.Name} (L{set.Level} {set.Species})");
            }
            Console.WriteLine();

            Console.WriteLine("Team 2:");
            foreach (var set in team2)
            {
                Console.WriteLine($"  - {set.Name} (L{set.Level} {set.Species})");
            }
            Console.WriteLine();

            PlayerOptions player1Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Player 1",
                Team = team1,
                Seed = new PrngSeed(debugPlayer1Seed),
                PrintDebug = debug,
            };

            PlayerOptions player2Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Player 2",
                Team = team2,
                Seed = new PrngSeed(debugPlayer2Seed),
                PrintDebug = debug,
            };

            BattleOptions battleOptions = new()
            {
                Id = FormatId.Gen9VgcRegulationI,
                Player1Options = player1Options,
                Player2Options = player2Options,
                Debug = debug,
                Sync = true,
                Seed = new PrngSeed(debugBattleSeed),
                MaxTurns = 1000,
            };

            Console.WriteLine("Starting battle...");
            Console.WriteLine();

            var simulator = new SyncSimulator();
            var result = simulator.Run(Library, battleOptions, printDebug: debug);
            var turn = simulator.Battle?.Turn ?? 0;

            Console.WriteLine();
            Console.WriteLine("=== Battle Result ===");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"Turns: {turn}");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("=== EXCEPTION OCCURRED ===");
            Console.WriteLine($"Exception Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Stack Trace:");
            Console.WriteLine(ex.StackTrace);

            if (ex.InnerException != null)
            {
                Console.WriteLine();
                Console.WriteLine("Inner Exception:");
                Console.WriteLine($"  Type: {ex.InnerException.GetType().Name}");
                Console.WriteLine($"  Message: {ex.InnerException.Message}");
                Console.WriteLine($"  Stack Trace: {ex.InnerException.StackTrace}");
            }
        }

        Console.WriteLine();
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
            var result = simulator.Run(Library, battleOptions, printDebug: debug);
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