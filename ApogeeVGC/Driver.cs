using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim;
using System.Collections.Concurrent;
using System.Text;

namespace ApogeeVGC;

public enum DriverMode
{
    RandomVsRandom,
    RandomVsRandomEvaluation,
    ConsoleVsRandom,
    ConsoleVsConsole,
    ConsoleVsMcts,
    MctsVsRandom,
    MctsVsRandomEvaluation,
    MctsIterationComparison,
}

public class Driver
{
    private const double Root2 = 1.4142135623730951; // sqrt of 2
    private Library Library { get; } = new();
    private Simulator? Simulator { get; set; }

    private const int RandomEvaluationNumTest = 100000;

    private const int MctsEvaluationNumTest = 100;
    private const int MctsMaxIterations = 10000;
    private const double MctsExplorationParameter = Root2;
    private const ChoiceFilterStrategy MctsEvaluationChoiceFilterStrategy =
        ChoiceFilterStrategy.ReducedSwitching;
    private readonly int? _mctsMaxTimer = null; // in milliseconds

    private static readonly int NumThreads = Environment.ProcessorCount;

    private const int PlayerRandom1Seed = 439;
    private const int PlayerRandom2Seed = 1815;

    public void Start(DriverMode mode)
    {
        switch (mode)
        {
            case DriverMode.RandomVsRandom:
                RunRandomTest();
                break;
            case DriverMode.RandomVsRandomEvaluation:
                RunRandomVsRandomEvaluationTest();
                break;
            case DriverMode.ConsoleVsRandom:
                RunConsoleVsRandomTest();
                break;
            case DriverMode.ConsoleVsConsole:
                throw new NotImplementedException("Console vs Console mode is not implemented yet.");
            case DriverMode.ConsoleVsMcts:
                RunConsoleVsMctsTest();
                break;
            case DriverMode.MctsVsRandom:
                RunMctsVsRandom();
                break;
            case DriverMode.MctsVsRandomEvaluation:
                RunMctsVsRandomEvaluation();
                break;
            case DriverMode.MctsIterationComparison:
                RunMctsIterationComparison();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private void RunConsoleVsMctsTest()
    {
        Battle battle = BattleGenerator.GenerateTestBattle(Library, "Matt", 
            "MCTS", true);
        Simulator = new Simulator
        {
            Battle = battle,
            Player1 = new PlayerConsole(PlayerId.Player1, battle),
            Player2 = new PlayerMcts(PlayerId.Player2, battle, MctsMaxIterations,
                MctsExplorationParameter, Library, PlayerRandom2Seed, NumThreads, _mctsMaxTimer),
        };
        Simulator.Run();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private void RunMctsVsRandom()
    {
        Battle battle = BattleGenerator.GenerateTestBattle(Library, "MCTS", "Random");
        
        var mctsPlayer = new PlayerMcts(PlayerId.Player1, battle, MctsMaxIterations,
            MctsExplorationParameter, Library, PlayerRandom1Seed, NumThreads);
        
        Simulator = new Simulator
        {
            Battle = battle,
            Player1 = mctsPlayer,
            Player2 = new PlayerRandom(PlayerId.Player2, battle, Library,
                ChoiceFilterStrategy.None,
                PlayerRandom2Seed),
        };
        SimulatorResult result = Simulator.Run();
        
        // Get timing statistics
        var timingStats = mctsPlayer.GetTimingStats();
        
        string winner = result switch
        {
            SimulatorResult.Player1Win => "MCTS",
            SimulatorResult.Player2Win => "Random",
            _ => "Unknown"
        };
        Console.WriteLine($"Battle finished. Winner: {winner}");
        Console.WriteLine($"MCTS Timing Statistics:");
        Console.WriteLine($"  Total choices made: {timingStats.TotalChoices}");
        Console.WriteLine($"  Average time per choice: {timingStats.AverageTimeMs:F3} ms");
        Console.WriteLine($"  Min time: {timingStats.MinTimeMs:F3} ms");
        Console.WriteLine($"  Max time: {timingStats.MaxTimeMs:F3} ms");
        Console.WriteLine($"  Total MCTS time: {timingStats.TotalTimeMs:F3} ms");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private void RunMctsVsRandomEvaluation()
    {
        // Run evaluation with current settings
        RunMctsVsRandomEvaluationWithIterations(MctsMaxIterations);
    }

    private void RunMctsVsRandomEvaluationWithIterations(int maxIterations)
    {
        var simResults = new ConcurrentBag<SimulatorResult>();
        var mctsTimingStats = new ConcurrentBag<MctsTimingStats>();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < MctsEvaluationNumTest; i++)
        {
                int player1Seed = PlayerRandom1Seed + i;
                int player2Seed = PlayerRandom2Seed + i;

                Battle battle = BattleGenerator.GenerateTestBattle(Library, "Mcts",
                    "Random");
                
                var mctsPlayer = new PlayerMcts(PlayerId.Player1, battle, maxIterations,
                    MctsExplorationParameter, Library, player1Seed, NumThreads, _mctsMaxTimer);
                
                var simulator = new Simulator
                {
                    Battle = battle,
                    Player1 = mctsPlayer,
                    Player2 = new PlayerRandom(PlayerId.Player2, battle, Library,
                        MctsEvaluationChoiceFilterStrategy, player2Seed),
                };
                
                simResults.Add(simulator.Run());
                
                // Collect timing statistics from the MCTS player
                mctsTimingStats.Add(mctsPlayer.GetTimingStats());
        }

        stopwatch.Stop();

        // Convert to list for counting
        var resultsList = simResults.ToList();
        var timingStatsList = mctsTimingStats.ToList();
        int player1Wins = resultsList.Count(result => result == SimulatorResult.Player1Win);
        int player2Wins = resultsList.Count(result => result == SimulatorResult.Player2Win);

        // Calculate timing metrics
        double totalSeconds = stopwatch.Elapsed.TotalSeconds;
        double timePerSimulation = totalSeconds / MctsEvaluationNumTest;
        double simulationsPerSecond = MctsEvaluationNumTest / totalSeconds;

        // Calculate MCTS timing statistics
        var allChoiceTimes = timingStatsList.Where(stats => stats.TotalChoices > 0).ToList();
        double avgMctsChoiceTime = 0.0;
        double totalMctsChoices = 0;
        double totalMctsTime = 0.0;
        double minMctsChoiceTime = double.MaxValue;
        double maxMctsChoiceTime = 0.0;
        
        if (allChoiceTimes.Count > 0)
        {
            totalMctsChoices = allChoiceTimes.Sum(stats => stats.TotalChoices);
            totalMctsTime = allChoiceTimes.Sum(stats => stats.TotalTimeMs);
            avgMctsChoiceTime = totalMctsTime / totalMctsChoices;
            minMctsChoiceTime = allChoiceTimes.Min(stats => stats.MinTimeMs);
            maxMctsChoiceTime = allChoiceTimes.Max(stats => stats.MaxTimeMs);
        }

        // Rest of the method with timing information added...
        StringBuilder sb = new();
        sb.AppendLine($"MCTS vs Random Evaluation Results ({MctsEvaluationNumTest} battles):");
        sb.AppendLine($"MCTS Wins: {player1Wins}");
        sb.AppendLine($"Random Wins: {player2Wins}");
        sb.AppendLine($"Win Rate for MCTS: {(double)player1Wins / MctsEvaluationNumTest:P2}");
        sb.AppendLine($"Win Rate for Random: {(double)player2Wins / MctsEvaluationNumTest:P2}");
        sb.AppendLine();
        sb.AppendLine("MCTS Parameters:");
        sb.AppendLine($"Max Iterations: {maxIterations}");
        sb.AppendLine($"Exploration Parameter: {MctsExplorationParameter}");
        sb.AppendLine($"Choice Filter Strategy: {MctsEvaluationChoiceFilterStrategy}");
        sb.AppendLine($"Max turn timer: {_mctsMaxTimer} ms");
        sb.AppendLine();
        sb.AppendLine("Performance Metrics:");
        sb.AppendLine($"Number of threads: {NumThreads}");
        sb.AppendLine($@"Total Execution Time: {stopwatch.Elapsed:hh\:mm\:ss\.fff}");
        sb.AppendLine($"Total Execution Time (seconds): {totalSeconds:F3}");
        sb.AppendLine($"Time per Simulation: {timePerSimulation * 1000:F3} ms");
        sb.AppendLine($"Simulations per Second: {simulationsPerSecond:F1}");
        sb.AppendLine();
        sb.AppendLine("MCTS Choice Selection Timing:");
        sb.AppendLine($"Total MCTS Choices Made: {totalMctsChoices:F0}");
        sb.AppendLine($"Average Time per MCTS Choice: {avgMctsChoiceTime:F3} ms");
        sb.AppendLine($"Minimum MCTS Choice Time: {minMctsChoiceTime:F3} ms");
        sb.AppendLine($"Maximum MCTS Choice Time: {maxMctsChoiceTime:F3} ms");
        sb.AppendLine($"Total MCTS Time: {totalMctsTime:F3} ms");
        sb.AppendLine($"MCTS Time as % of Total: {(totalMctsTime / (totalSeconds * 1000)):P2}");
        Console.WriteLine(sb.ToString());

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Runs MCTS evaluation with multiple iteration counts to compare performance
    /// </summary>
    private void RunMctsIterationComparison()
    {
        int[] iterationCounts = { 1, 5, 10, 20, 50, 100 };
        
        Console.WriteLine("Running MCTS iteration comparison...");
        Console.WriteLine($"Testing iteration counts: [{string.Join(", ", iterationCounts)}]");
        Console.WriteLine($"Number of battles per test: {MctsEvaluationNumTest}");
        Console.WriteLine();

        var results = new List<(int iterations, double avgChoiceTime, double winRate, int totalChoices)>();

        foreach (int iterations in iterationCounts)
        {
            Console.WriteLine($"Testing with {iterations} iterations...");
            
            var simResults = new List<SimulatorResult>();
            var mctsTimingStats = new List<MctsTimingStats>();

            for (int i = 0; i < MctsEvaluationNumTest; i++)
            {
                int player1Seed = PlayerRandom1Seed + i;
                int player2Seed = PlayerRandom2Seed + i;

                Battle battle = BattleGenerator.GenerateTestBattle(Library, "Mcts", "Random");
                
                var mctsPlayer = new PlayerMcts(PlayerId.Player1, battle, iterations,
                    MctsExplorationParameter, Library, player1Seed, NumThreads, _mctsMaxTimer);
                
                var simulator = new Simulator
                {
                    Battle = battle,
                    Player1 = mctsPlayer,
                    Player2 = new PlayerRandom(PlayerId.Player2, battle, Library,
                        MctsEvaluationChoiceFilterStrategy, player2Seed),
                };
                
                simResults.Add(simulator.Run());
                mctsTimingStats.Add(mctsPlayer.GetTimingStats());
            }

            // Calculate statistics
            int wins = simResults.Count(result => result == SimulatorResult.Player1Win);
            double winRate = (double)wins / MctsEvaluationNumTest;
            
            var validStats = mctsTimingStats.Where(stats => stats.TotalChoices > 0).ToList();
            double avgChoiceTime = 0.0;
            int totalChoices = 0;
            
            if (validStats.Count > 0)
            {
                totalChoices = validStats.Sum(stats => stats.TotalChoices);
                double totalTime = validStats.Sum(stats => stats.TotalTimeMs);
                avgChoiceTime = totalTime / totalChoices;
            }

            results.Add((iterations, avgChoiceTime, winRate, totalChoices));
            Console.WriteLine($"  Win rate: {winRate:P2}, Avg choice time: {avgChoiceTime:F3} ms");
        }

        // Display summary
        Console.WriteLine();
        Console.WriteLine("MCTS Iteration Comparison Summary:");
        Console.WriteLine($"{"Iterations",-12} {"Win Rate",-10} {"Avg Choice Time",-16} {"Total Choices",-14}");
        Console.WriteLine(new string('-', 60));
        
        foreach (var (iterations, avgChoiceTime, winRate, totalChoices) in results)
        {
            Console.WriteLine($"{iterations,-12} {winRate:P2,-10} {avgChoiceTime:F3} ms{"",-8} {totalChoices,-14}");
        }
    }

    private void RunRandomVsRandomEvaluationTest()
    {
        var simResults = new ConcurrentBag<SimulatorResult>();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Thread-safe counter for seed generation
        int seedCounter = 0;//538;
        List<int> turnOnBattleEnd = [];

        Parallel.For(0, RandomEvaluationNumTest, new ParallelOptions { MaxDegreeOfParallelism = NumThreads },
            _ =>
                //for (int i = 0; i < RandomEvaluationNumTest; i++)
            {
                int currentSeed = Interlocked.Increment(ref seedCounter);
                int player1Seed = PlayerRandom1Seed + currentSeed;
                int player2Seed = PlayerRandom2Seed + currentSeed;

                Battle battle = BattleGenerator.GenerateTestBattle(Library, "Random1",
                    "Random2", false, currentSeed);
                var simulator = new Simulator
                {
                    Battle = battle,
                    Player1 = new PlayerRandom(PlayerId.Player1, battle, Library,
                        ChoiceFilterStrategy.None,
                        player1Seed),
                    Player2 = new PlayerRandom(PlayerId.Player2, battle, Library,
                        ChoiceFilterStrategy.None,
                        player2Seed),
                };
                simResults.Add(simulator.Run());
                turnOnBattleEnd.Add(battle.Turn);
            });

        stopwatch.Stop();

        // Convert to list for counting
        var resultsList = simResults.ToList();
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

    private void RunRandomTest()
    {
        Battle battle = BattleGenerator.GenerateTestBattle(Library, "Random1",
            "Random2", true);
        Simulator = new Simulator
        {
            Battle = battle,
            Player1 = new PlayerRandom(PlayerId.Player1, battle, Library,
                ChoiceFilterStrategy.None,
                PlayerRandom1Seed),
            Player2 = new PlayerRandom(PlayerId.Player2, battle, Library,
                ChoiceFilterStrategy.None,
                PlayerRandom2Seed),
        };
        SimulatorResult result = Simulator.Run();
        string winner = result switch
        {
            SimulatorResult.Player1Win => "Player 1",
            SimulatorResult.Player2Win => "Player 2",
            _ => "Unknown"
        };
        Console.WriteLine($"Battle finished. Winner: {winner}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private void RunConsoleVsRandomTest()
    {
        Battle battle = BattleGenerator.GenerateTestBattle(Library, "Matt", 
            "Random", true);

        Simulator = new Simulator
        {
            Battle = battle,
            Player1 = new PlayerConsole(PlayerId.Player1, battle),
            Player2 = new PlayerRandom(PlayerId.Player2, battle, Library,
                ChoiceFilterStrategy.ReducedSwitching, PlayerRandom2Seed),
        };

        Simulator.Run();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}

public static class Statistics
{
    public static double Mean(this IEnumerable<int> source)
    {
        if (source == null)
            throw new ArgumentException("Source cannot be null.");
        
        var list = source.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Source cannot be empty.");
        
        return list.Average();
    }
    
    public static double StandardDeviation(this IEnumerable<int> source)
    {
        if (source == null)
            throw new ArgumentException("Source cannot be null.");
        
        var list = source.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Source cannot be empty.");
        
        double mean = list.Average();
        double variance = list.Select(x => Math.Pow(x - mean, 2)).Average();
        return Math.Sqrt(variance);
    }
    
    public static double Median(this IEnumerable<int> source)
    {
        if (source == null)
            throw new ArgumentException("Source cannot be null.");
        
        var list = source.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Source cannot be empty.");
        
        var sorted = list.OrderBy(x => x).ToList();
        int count = sorted.Count;
        
        if (count % 2 == 0)
        {
            // Even number of elements - average of middle two
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
        }
        else
        {
            // Odd number of elements - middle element
            return sorted[count / 2];
        }
    }
    
    public static int Minimum(this IEnumerable<int> source)
    {
        if (source == null)
            throw new ArgumentException("Source cannot be null.");
        
        var list = source.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Source cannot be empty.");
        
        return list.Min();
    }
    
    public static int Maximum(this IEnumerable<int> source)
    {
        if (source == null)
            throw new ArgumentException("Source cannot be null.");
        
        var list = source.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Source cannot be empty.");
        
        return list.Max();
    }
}