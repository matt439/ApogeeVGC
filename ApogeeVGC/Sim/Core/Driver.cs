using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Generators;
using System.Collections.Concurrent;
using System.Text;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Core;

public enum DriverMode
{
    RandomVsRandom,
    RandomVsRandomEvaluation,
    RandomVsRandomEvaluationDoubles,
    ConsoleVsRandom,
    ConsoleVsRandomDoubles,
    ConsoleVsConsole,
    ConsoleVsMcts,
    MctsVsRandom,
    MctsVsRandomEvaluation,
    MctsVsRandomEvaluationDoubles,
}

public class Driver
{
    private const double Root2 = 1.4142135623730951; // sqrt of 2
    private Library Library { get; } = new();

    private const int RandomEvaluationNumTest = 1000;

    private const int MctsEvaluationNumTest = 100;
    private const int MctsMaxIterations = 100;
    private const double MctsExplorationParameter = 0;
    //private const ChoiceFilterStrategy MctsEvaluationChoiceFilterStrategy =
    //    ChoiceFilterStrategy.ReducedSwitching;
    private readonly int? _mctsMaxTimer = null; // in milliseconds

    private static readonly int NumThreads = Environment.ProcessorCount;

    private const int PlayerRandom1Seed = 439;
    private const int PlayerRandom2Seed = 1818;
    
    // Memory profiling options
    private const bool EnableDetailedMemoryProfiling = false; // Set to true for detailed memory analysis
    private const bool SaveMemoryProfileToCsv = false; // Set to true to save detailed CSV logs

    public void Start(DriverMode mode)
    {
        switch (mode)
        {
            case DriverMode.RandomVsRandom:
                throw new NotImplementedException();
                //RunRandomTest();
                break;
            case DriverMode.RandomVsRandomEvaluation:
                RunRandomVsRandomEvaluationTest(BattleFormat.Singles).GetAwaiter().GetResult();
                break;
            case DriverMode.RandomVsRandomEvaluationDoubles:
                RunRandomVsRandomEvaluationTest(BattleFormat.Doubles).GetAwaiter().GetResult();
                break;
            case DriverMode.ConsoleVsRandom:
                RunConsoleVsRandomTest().GetAwaiter().GetResult();
                break;
            case DriverMode.ConsoleVsRandomDoubles:
                RunConsoleVsRandomDoublesTest().GetAwaiter().GetResult();
                break;
            case DriverMode.ConsoleVsConsole:
                throw new NotImplementedException("Console vs Console mode is not implemented yet.");
            case DriverMode.ConsoleVsMcts:
                RunConsoleVsMctsTest().GetAwaiter().GetResult();
                break;
            case DriverMode.MctsVsRandom:
                RunMctsVsRandom().GetAwaiter().GetResult();
                break;
            case DriverMode.MctsVsRandomEvaluation:
                 RunMctsVsRandomEvaluation(BattleFormat.Singles).GetAwaiter().GetResult();
                break;
            case DriverMode.MctsVsRandomEvaluationDoubles:
                RunMctsVsRandomEvaluation(BattleFormat.Doubles).GetAwaiter().GetResult();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private async Task RunConsoleVsMctsTest()
    {
        IPlayerNew player1 = new PlayerConsoleNew(PlayerId.Player1);
        IPlayerNew player2 = new PlayerMcts(PlayerId.Player2, MctsMaxIterations,
            MctsExplorationParameter, Library, PlayerRandom1Seed, NumThreads, _mctsMaxTimer);

        BattleNew battle = BattleGenerator.GenerateTestBattleNew(Library, player1, player2, "Matt",
            "MCTS", BattleFormat.Doubles);

        var simulator = new SimulatorNew
        {
            Battle = battle,
            Player1 = player1,
            Player2 = player2,
        };

        SimulatorResult result = await simulator.Run();

        string winner = result switch
        {
            SimulatorResult.Player1Win => "Matt",
            SimulatorResult.Player2Win => "MCTS",
            _ => "Unknown",
        };

        Console.WriteLine($"Battle finished. Winner: {winner}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private async Task RunMctsVsRandom()
    {
        IPlayerNew player1 = new PlayerMcts(PlayerId.Player1, MctsMaxIterations,
            MctsExplorationParameter, Library, PlayerRandom1Seed, NumThreads, _mctsMaxTimer);
        IPlayerNew player2 = new PlayerRandomNew(PlayerId.Player2, PlayerRandom2Seed);

        BattleNew battle = BattleGenerator.GenerateTestBattleNew(Library, player1, player2, "MCTS",
            "Random", BattleFormat.Doubles);

        var simulator = new SimulatorNew
        {
            Battle = battle,
            Player1 = player1,
            Player2 = player2,
        };

        SimulatorResult result = await simulator.Run();

        string winner = result switch
        {
            SimulatorResult.Player1Win => "MCTS",
            SimulatorResult.Player2Win => "Random",
            _ => "Unknown",
        };

        Console.WriteLine($"Battle finished. Winner: {winner}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    //private void RunMctsVsRandomEvaluation()
    //{
    //    var simResults = new ConcurrentBag<SimulatorResult>();
    //    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    //    for (int i = 0; i < MctsEvaluationNumTest; i++)
    //    {
    //            int player1Seed = PlayerRandom1Seed + i;
    //            int player2Seed = PlayerRandom2Seed + i;

    //            Battle battle = BattleGenerator.GenerateTestBattle(Library, "Mcts",
    //                "Random", BattleFormat.Singles);
    //            var simulator = new Simulator
    //            {
    //                Battle = battle,
    //                Player1 = new PlayerMcts(PlayerId.Player1, battle, MctsMaxIterations,
    //                    MctsExplorationParameter, Library, player1Seed, NumThreads, _mctsMaxTimer),
    //                Player2 = new PlayerRandom(PlayerId.Player2, battle, Library,
    //                    MctsEvaluationChoiceFilterStrategy, player2Seed),
    //            };
    //            simResults.Add(simulator.Run());
    //    }

    //    stopwatch.Stop();

    //    // Convert to list for counting
    //    var resultsList = simResults.ToList();
    //    int player1Wins = resultsList.Count(result => result == SimulatorResult.Player1Win);
    //    int player2Wins = resultsList.Count(result => result == SimulatorResult.Player2Win);

    //    // Calculate timing metrics
    //    double totalSeconds = stopwatch.Elapsed.TotalSeconds;
    //    double timePerSimulation = totalSeconds / MctsEvaluationNumTest;
    //    double simulationsPerSecond = MctsEvaluationNumTest / totalSeconds;

    //    // Rest of the method with timing information added...
    //    StringBuilder sb = new();
    //    sb.AppendLine($"MCTS vs Random Evaluation Results ({MctsEvaluationNumTest} battles):");
    //    sb.AppendLine($"MCTS Wins: {player1Wins}");
    //    sb.AppendLine($"Random Wins: {player2Wins}");
    //    sb.AppendLine($"Win Rate for MCTS: {(double)player1Wins / MctsEvaluationNumTest:P2}");
    //    sb.AppendLine($"Win Rate for Random: {(double)player2Wins / MctsEvaluationNumTest:P2}");
    //    sb.AppendLine();
    //    sb.AppendLine("MCTS Parameters:");
    //    sb.AppendLine($"Max Iterations: {MctsMaxIterations}");
    //    sb.AppendLine($"Exploration Parameter: {MctsExplorationParameter}");
    //    sb.AppendLine($"Choice Filter Strategy: {MctsEvaluationChoiceFilterStrategy}");
    //    sb.AppendLine($"Max turn timer: {_mctsMaxTimer} ms");
    //    sb.AppendLine();
    //    sb.AppendLine("Performance Metrics:");
    //    sb.AppendLine($"Number of threads: {NumThreads}");
    //    sb.AppendLine($@"Total Execution Time: {stopwatch.Elapsed:hh\:mm\:ss\.fff}");
    //    sb.AppendLine($"Total Execution Time (seconds): {totalSeconds:F3}");
    //    sb.AppendLine($"Time per Simulation: {timePerSimulation * 1000:F3} ms");
    //    sb.AppendLine($"Simulations per Second: {simulationsPerSecond:F1}");
    //    Console.WriteLine(sb.ToString());

    //    Console.WriteLine("Press any key to exit...");
    //    Console.ReadKey();
    //}

    private async Task RunMctsVsRandomEvaluation(BattleFormat format)
    {
        var simResults = new ConcurrentBag<SimulatorResult>();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        int seedCounter = 0;
        var turnOnBattleEnd = new ConcurrentBag<int>();
        var mctsExecutionTimes = new ConcurrentBag<double>(); // Track MCTS execution times
        var memoryUsageSamples = new ConcurrentBag<long>(); // Track memory usage samples

        // Initialize optional detailed memory profiler
        MemoryProfiler? detailedProfiler = null;
        if (EnableDetailedMemoryProfiling)
        {
            detailedProfiler = new MemoryProfiler($"MCTS_vs_Random_{format}_{MctsEvaluationNumTest}_battles");
            detailedProfiler.TakeSnapshot("Evaluation_Start");
        }

        // Record initial memory usage
        GC.Collect(); // Force garbage collection for accurate baseline
        GC.WaitForPendingFinalizers();
        GC.Collect();
        long initialMemory = GC.GetTotalMemory(false);
        long peakMemory = initialMemory;
        
        Console.WriteLine($"Starting MCTS evaluation with initial memory: {initialMemory / (1024 * 1024):F2} MB");

        // Run simulations sequentially instead of in parallel
        for (int i = 0; i < MctsEvaluationNumTest; i++)
        {
            int currentSeed = ++seedCounter;
            int player1Seed = PlayerRandom1Seed + currentSeed;
            int player2Seed = PlayerRandom2Seed + currentSeed;

            // Take detailed snapshot before battle setup
            detailedProfiler?.TakeSnapshot($"Before_Battle_{i + 1}_Setup");

            var mctsPlayer = new PlayerMcts(PlayerId.Player1, MctsMaxIterations,
                MctsExplorationParameter, Library, player1Seed, NumThreads, _mctsMaxTimer);
            IPlayerNew player2 = new PlayerRandomNew(PlayerId.Player2, player2Seed);

            BattleNew battle = BattleGenerator.GenerateTestBattleNew(Library, mctsPlayer, player2,
                "MCTS", "Random", format, false, currentSeed);

            var simulator = new SimulatorNew
            {
                Battle = battle,
                Player1 = mctsPlayer,
                Player2 = player2,
            };

            // Track MCTS execution times during the simulation
            var battleMctsExecutionTimes = new List<double>();
            
            // Subscribe to choice submission events to capture MCTS timing
            mctsPlayer.ChoiceSubmitted += (sender, choice) =>
            {
                if (sender is PlayerMcts player && player.LastMctsExecutionTimeMs > 0)
                {
                    battleMctsExecutionTimes.Add(player.LastMctsExecutionTimeMs);
                }
            };

            // Record memory usage before simulation
            long memoryBeforeBattle = GC.GetTotalMemory(false);
            memoryUsageSamples.Add(memoryBeforeBattle);

            // Take detailed snapshot before battle execution
            detailedProfiler?.TakeSnapshot($"Before_Battle_{i + 1}_Execution");

            SimulatorResult result = await simulator.Run();
            simResults.Add(result);
            turnOnBattleEnd.Add(battle.TurnCounter);
            
            // Add all MCTS execution times from this battle
            foreach (var time in battleMctsExecutionTimes)
            {
                mctsExecutionTimes.Add(time);
            }
            
            // Record memory usage after simulation
            long memoryAfterBattle = GC.GetTotalMemory(false);
            memoryUsageSamples.Add(memoryAfterBattle);
            
            // Track peak memory usage
            if (memoryAfterBattle > peakMemory)
            {
                peakMemory = memoryAfterBattle;
            }
            
            //simulator.Battle.ClearTurns();

            // Take detailed snapshot after battle cleanup
            detailedProfiler?.TakeSnapshot($"After_Battle_{i + 1}_Cleanup");
            
            // Optional: Progress reporting for long-running sequential tests
            if ((i + 1) % 10 == 0 || i == MctsEvaluationNumTest - 1)
            {
                long currentMemory = GC.GetTotalMemory(false);
                Console.WriteLine($"Completed {i + 1}/{MctsEvaluationNumTest} simulations - Current memory: {currentMemory / (1024 * 1024):F2} MB");
                
                // Take detailed snapshot at progress checkpoints
                detailedProfiler?.TakeSnapshot($"Progress_Checkpoint_{i + 1}");
            }
            
            // Optional: Force garbage collection every 50 battles to prevent excessive memory buildup
            if ((i + 1) % 50 == 0)
            {
                detailedProfiler?.TakeSnapshot($"Before_GC_Cycle_{i + 1}");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                detailedProfiler?.TakeSnapshot($"After_GC_Cycle_{i + 1}");
            }
        }

        stopwatch.Stop();

        // Final memory measurement
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        long finalMemory = GC.GetTotalMemory(false);

        // Take final detailed snapshot
        detailedProfiler?.TakeSnapshot("Evaluation_End");

        // Convert to list for counting (now from ConcurrentBag)
        var resultsList = simResults.ToList();
        var turnsList = turnOnBattleEnd.ToList();
        var mctsTimesList = mctsExecutionTimes.ToList();
        var memoryList = memoryUsageSamples.ToList();
        
        int player1Wins = resultsList.Count(result => result == SimulatorResult.Player1Win);
        int player2Wins = resultsList.Count(result => result == SimulatorResult.Player2Win);

        // Calculate timing metrics
        double totalSeconds = stopwatch.Elapsed.TotalSeconds;
        double timePerSimulation = totalSeconds / MctsEvaluationNumTest;
        double simulationsPerSecond = MctsEvaluationNumTest / totalSeconds;

        // Calculate turn statistics
        double meanTurns = turnOnBattleEnd.Mean();
        double stdDevTurns = turnOnBattleEnd.StandardDeviation();
        double medianTurns = turnOnBattleEnd.Median();
        int minTurns = turnOnBattleEnd.Minimum();
        int maxTurns = turnOnBattleEnd.Maximum();
        
        // Calculate MCTS timing statistics
        double meanMctsTime = mctsTimesList.Count > 0 ? mctsTimesList.Average() : 0.0;
        double stdDevMctsTime = mctsTimesList.Count > 0 ? mctsTimesList.StandardDeviation() : 0.0;
        double medianMctsTime = mctsTimesList.Count > 0 ? mctsTimesList.Median() : 0.0;
        double minMctsTime = mctsTimesList.Count > 0 ? mctsTimesList.Min() : 0.0;
        double maxMctsTime = mctsTimesList.Count > 0 ? mctsTimesList.Max() : 0.0;
        double totalMctsTime = mctsTimesList.Count > 0 ? mctsTimesList.Sum() : 0.0;
        int totalMctsChoices = mctsTimesList.Count;

        // Calculate memory statistics
        double meanMemoryMB = memoryList.Count > 0 ? memoryList.Select(m => m / (1024.0 * 1024.0)).Average() : 0.0;
        double stdDevMemoryMB = memoryList.Count > 0 ? memoryList.Select(m => m / (1024.0 * 1024.0)).StandardDeviation() : 0.0;
        double medianMemoryMB = memoryList.Count > 0 ? memoryList.Select(m => m / (1024.0 * 1024.0)).Median() : 0.0;
        double minMemoryMB = memoryList.Count > 0 ? memoryList.Min() / (1024.0 * 1024.0) : 0.0;
        double maxMemoryMB = memoryList.Count > 0 ? memoryList.Max() / (1024.0 * 1024.0) : 0.0;
        double initialMemoryMB = initialMemory / (1024.0 * 1024.0);
        double finalMemoryMB = finalMemory / (1024.0 * 1024.0);
        double peakMemoryMB = peakMemory / (1024.0 * 1024.0);
        double memoryDeltaMB = finalMemoryMB - initialMemoryMB;

        // Generate comprehensive report
        StringBuilder sb = new();
        sb.AppendLine($"MCTS vs Random Evaluation Results ({MctsEvaluationNumTest} battles):");
        switch (format)
        {
            case BattleFormat.Singles:
                sb.AppendLine("Format: Singles");
                break;
            case BattleFormat.Doubles:
                sb.AppendLine("Format: Doubles");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
        sb.AppendLine($"MCTS Wins: {player1Wins}");
        sb.AppendLine($"Random Wins: {player2Wins}");
        sb.AppendLine($"Win Rate for MCTS: {(double)player1Wins / MctsEvaluationNumTest:P2}");
        sb.AppendLine($"Win Rate for Random: {(double)player2Wins / MctsEvaluationNumTest:P2}");
        sb.AppendLine();
        sb.AppendLine("MCTS Parameters:");
        sb.AppendLine($"Max Iterations: {MctsMaxIterations}");
        sb.AppendLine($"Exploration Parameter: {MctsExplorationParameter}");
        sb.AppendLine($"Max Timer: {_mctsMaxTimer?.ToString() ?? "None"} ms");
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
        sb.AppendLine();
        sb.AppendLine("MCTS Execution Time Statistics:");
        sb.AppendLine($"Total MCTS Choices Made: {totalMctsChoices}");
        sb.AppendLine($"Total MCTS Time: {totalMctsTime:F3} ms");
        sb.AppendLine($"Mean MCTS Time per Choice: {meanMctsTime:F3} ms");
        sb.AppendLine($"Standard Deviation of MCTS Time: {stdDevMctsTime:F3} ms");
        sb.AppendLine($"Median MCTS Time per Choice: {medianMctsTime:F3} ms");
        sb.AppendLine($"Minimum MCTS Time per Choice: {minMctsTime:F3} ms");
        sb.AppendLine($"Maximum MCTS Time per Choice: {maxMctsTime:F3} ms");
        if (totalMctsChoices > 0)
        {
            sb.AppendLine($"Average MCTS Choices per Battle: {(double)totalMctsChoices / MctsEvaluationNumTest:F1}");
        }
        sb.AppendLine();
        sb.AppendLine("Memory Usage Statistics:");
        sb.AppendLine($"Initial Memory: {initialMemoryMB:F2} MB");
        sb.AppendLine($"Final Memory: {finalMemoryMB:F2} MB");
        sb.AppendLine($"Memory Delta: {memoryDeltaMB:+F2;-F2;0} MB");
        sb.AppendLine($"Peak Memory: {peakMemoryMB:F2} MB");
        sb.AppendLine($"Mean Memory Usage: {meanMemoryMB:F2} MB");
        sb.AppendLine($"Standard Deviation of Memory: {stdDevMemoryMB:F2} MB");
        sb.AppendLine($"Median Memory Usage: {medianMemoryMB:F2} MB");
        sb.AppendLine($"Minimum Memory Usage: {minMemoryMB:F2} MB");
        sb.AppendLine($"Maximum Memory Usage: {maxMemoryMB:F2} MB");
        sb.AppendLine($"Memory Samples Collected: {memoryList.Count}");
        
        // Add detailed memory profiling information if enabled
        if (detailedProfiler != null)
        {
            sb.AppendLine();
            sb.AppendLine("=== DETAILED MEMORY PROFILE ===");
            sb.AppendLine(detailedProfiler.GenerateReport());
            
            // Save to CSV if enabled
            if (SaveMemoryProfileToCsv)
            {
                string csvFilename = $"memory_profile_{format}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                detailedProfiler.SaveToCsv(csvFilename);
                sb.AppendLine($"Detailed memory profile saved to: {csvFilename}");
            }
        }
        
        Console.WriteLine(sb.ToString());

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private async Task RunRandomVsRandomEvaluationTest(BattleFormat format)
    {
        var simResults = new ConcurrentBag<SimulatorResult>();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

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

                    IPlayerNew player1 = new PlayerRandomNew(PlayerId.Player1, player1Seed);
                    IPlayerNew player2 = new PlayerRandomNew(PlayerId.Player2, player2Seed);

                    BattleNew battle = BattleGenerator.GenerateTestBattleNew(Library, player1, player2,
                        "Random1", "Random2", format, false, currentSeed);

                    var simulator = new SimulatorNew
                    {
                        Battle = battle,
                        Player1 = player1,
                        Player2 = player2,
                    };

                    SimulatorResult result = await simulator.Run();
                    simResults.Add(result);
                    turnOnBattleEnd.Add(battle.TurnCounter);
                    simulator.Battle.ClearTurns();
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

        // Calculate timing metrics
        double totalSeconds = stopwatch.Elapsed.TotalSeconds;
        double timePerSimulation = totalSeconds / RandomEvaluationNumTest;
        double simulationsPerSecond = RandomEvaluationNumTest / totalSeconds;

        // Calculate turn statistics
        double meanTurns = turnOnBattleEnd.Mean();
        double stdDevTurns = turnOnBattleEnd.StandardDeviation();
        double medianTurns = turnOnBattleEnd.Median();
        int minTurns = turnOnBattleEnd.Minimum();
        int maxTurns = turnOnBattleEnd.Maximum();

        // Rest of the method with timing information added...
        StringBuilder sb = new();
        sb.AppendLine($"Random vs Random Evaluation Results ({RandomEvaluationNumTest} battles):");
        switch (format)
        {
            case BattleFormat.Singles:
                sb.AppendLine("Format: Singles");
                break;
            case BattleFormat.Doubles:
                sb.AppendLine("Format: Doubles");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
        sb.AppendLine($"Player 1 Wins: {player1Wins}");
        sb.AppendLine($"Player 2 Wins: {player2Wins}");
        sb.AppendLine($"Win Rate for Player 1: {(double)player1Wins / RandomEvaluationNumTest:P2}");
        sb.AppendLine($"Win Rate for Player 2: {(double)player2Wins / RandomEvaluationNumTest:P2}");
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

    //private void RunRandomTest()
    //{
    //    Battle battle = BattleGenerator.GenerateTestBattle(Library, "Random1",
    //        "Random2", BattleFormat.Singles, true);
    //    Simulator = new Simulator
    //    {
    //        Battle = battle,
    //        Player1 = new PlayerRandom(PlayerId.Player1, battle, Library,
    //            ChoiceFilterStrategy.None,
    //            PlayerRandom1Seed),
    //        Player2 = new PlayerRandom(PlayerId.Player2, battle, Library,
    //            ChoiceFilterStrategy.None,
    //            PlayerRandom2Seed),
    //    };
    //    SimulatorResult result = Simulator.Run();
    //    string winner = result switch
    //    {
    //        SimulatorResult.Player1Win => "Player 1",
    //        SimulatorResult.Player2Win => "Player 2",
    //        _ => "Unknown"
    //    };
    //    Console.WriteLine($"Battle finished. Winner: {winner}");
    //    Console.WriteLine("Press any key to exit...");
    //    Console.ReadKey();
    //}

        //private void RunConsoleVsRandomTest()
        //{
        //    Battle battle = BattleGenerator.GenerateTestBattle(Library, "Matt", 
        //        "Random", BattleFormat.Singles, true);

        //    Simulator = new Simulator
        //    {
        //        Battle = battle,
        //        Player1 = new PlayerConsole(PlayerId.Player1, battle),
        //        Player2 = new PlayerRandom(PlayerId.Player2, battle, Library,
        //            ChoiceFilterStrategy.ReducedSwitching, PlayerRandom2Seed),
        //    };

        //    Simulator.Run();

        //    Console.WriteLine("Press any key to exit...");
        //    Console.ReadKey();
        //}

    private async Task RunConsoleVsRandomTest()
    {
        IPlayerNew player1 = new PlayerConsoleNew(PlayerId.Player1);
        IPlayerNew player2 = new PlayerRandomNew(PlayerId.Player2, PlayerRandom2Seed);

        BattleNew battle = BattleGenerator.GenerateTestBattleNew(Library, player1, player2, "Matt",
            "Random", BattleFormat.Singles, true);

        var simulator = new SimulatorNew
        {
            Battle = battle,
            Player1 = player1,
            Player2 = player2,
        };

        SimulatorResult result = await simulator.Run();

        string winner = result switch
        {
            SimulatorResult.Player1Win => "Matt",
            SimulatorResult.Player2Win => "Random",
            _ => "Unknown",
        };

        Console.WriteLine($"Battle finished. Winner: {winner}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private async Task RunConsoleVsRandomDoublesTest()
    {
        IPlayerNew player1 = new PlayerConsoleNew(PlayerId.Player1);
        IPlayerNew player2 = new PlayerRandomNew(PlayerId.Player2, PlayerRandom2Seed);

        BattleNew battle = BattleGenerator.GenerateTestBattleNew(Library, player1, player2, "Matt",
            "Random", BattleFormat.Doubles, true);

        var simulator = new SimulatorNew
        {
            Battle = battle,
            Player1 = player1,
            Player2 = player2,
        };

        SimulatorResult result = await simulator.Run();

        string winner = result switch
        {
            SimulatorResult.Player1Win => "Matt",
            SimulatorResult.Player2Win => "Random",
            _ => "Unknown",
        };

        Console.WriteLine($"Battle finished. Winner: {winner}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}