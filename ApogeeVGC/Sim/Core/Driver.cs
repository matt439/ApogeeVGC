using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Generators;
using System.Collections.Concurrent;
using System.Text;
using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.Sim.Core;

public enum DriverMode
{
    RandomVsRandom,
    RandomVsRandomEvaluation,
    ConsoleVsRandom,
    ConsoleVsRandomNew,
    ConsoleVsConsole,
    ConsoleVsMcts,
    MctsVsRandom,
    MctsVsRandomEvaluation,
}

public class Driver
{
    private const double Root2 = 1.4142135623730951; // sqrt of 2
    private Library Library { get; } = new();

    private const int RandomEvaluationNumTest = 500;

    private const int MctsEvaluationNumTest = 100;
    private const int MctsMaxIterations = 1000000;
    private const double MctsExplorationParameter = 0.0;
    private const ChoiceFilterStrategy MctsEvaluationChoiceFilterStrategy =
        ChoiceFilterStrategy.ReducedSwitching;
    private readonly int? _mctsMaxTimer = null; // in milliseconds

    private static readonly int NumThreads = Environment.ProcessorCount;

    private const int PlayerRandom1Seed = 439;
    private const int PlayerRandom2Seed = 1818;

    public void Start(DriverMode mode)
    {
        switch (mode)
        {
            case DriverMode.RandomVsRandom:
                throw new NotImplementedException();
                //RunRandomTest();
                break;
            case DriverMode.RandomVsRandomEvaluation:
                RunRandomVsRandomEvaluationTest().GetAwaiter().GetResult();
                break;
            case DriverMode.ConsoleVsRandom:
                throw new NotImplementedException();
                //RunConsoleVsRandomTest();
                break;
            case DriverMode.ConsoleVsRandomNew:
                RunConsoleVsRandomNewTest().GetAwaiter().GetResult();
                break;
            case DriverMode.ConsoleVsConsole:
                throw new NotImplementedException("Console vs Console mode is not implemented yet.");
            case DriverMode.ConsoleVsMcts:
                throw new NotImplementedException();
                //RunConsoleVsMctsTest();
                break;
            case DriverMode.MctsVsRandom:
                throw new NotImplementedException();
                //RunMctsVsRandom();
                break;
            case DriverMode.MctsVsRandomEvaluation:
                throw new NotImplementedException();
                //RunMctsVsRandomEvaluation();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    //private void RunConsoleVsMctsTest()
    //{
    //    Battle battle = BattleGenerator.GenerateTestBattle(Library, "Matt", 
    //        "MCTS", BattleFormat.Singles, true);
    //    Simulator = new Simulator
    //    {
    //        Battle = battle,
    //        Player1 = new PlayerConsole(PlayerId.Player1, battle),
    //        Player2 = new PlayerMcts(PlayerId.Player2, battle, MctsMaxIterations,
    //            MctsExplorationParameter, Library, PlayerRandom2Seed, NumThreads, _mctsMaxTimer),
    //    };
    //    Simulator.Run();
    //    Console.WriteLine("Press any key to exit...");
    //    Console.ReadKey();
    //}

    //private void RunMctsVsRandom()
    //{
    //    Battle battle = BattleGenerator.GenerateTestBattle(Library, "MCTS", "Random",
    //        BattleFormat.Singles);
    //    Simulator = new Simulator
    //    {
    //        Battle = battle,
    //        Player1 = new PlayerMcts(PlayerId.Player1, battle, MctsMaxIterations,
    //            MctsExplorationParameter, Library, PlayerRandom1Seed, NumThreads),
    //        Player2 = new PlayerRandom(PlayerId.Player2, battle, Library,
    //            ChoiceFilterStrategy.None,
    //            PlayerRandom2Seed),
    //    };
    //    SimulatorResult result = Simulator.Run();
    //    string winner = result switch
    //    {
    //        SimulatorResult.Player1Win => "MCTS",
    //        SimulatorResult.Player2Win => "Random",
    //        _ => "Unknown"
    //    };
    //    Console.WriteLine($"Battle finished. Winner: {winner}");
    //    Console.WriteLine("Press any key to exit...");
    //    Console.ReadKey();
    //}

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

    private async Task RunRandomVsRandomEvaluationTest()
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
                        "Random1", "Random2", BattleFormat.Singles, false, currentSeed);

                    var simulator = new SimulatorNew
                    {
                        Battle = battle,
                        Player1 = player1,
                        Player2 = player2,
                    };

                    SimulatorResult result = await simulator.Run();
                    simResults.Add(result);
                    turnOnBattleEnd.Add(battle.TurnCounter);
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

    private async Task RunConsoleVsRandomNewTest()
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