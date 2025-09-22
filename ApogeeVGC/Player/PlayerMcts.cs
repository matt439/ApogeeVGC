using ApogeeVGC.Data;
using ApogeeVGC.Sim;
using ApogeeVGC.Mcts;

namespace ApogeeVGC.Player;

public class PlayerMcts : IPlayer
{
    public PlayerId PlayerId { get; }
    public Battle Battle { get; }
    private readonly int _seed;
    private readonly PokemonMonteCarloTreeSearch _mcts;
    private readonly List<double> _choiceTimings = new List<double>();

    public PlayerMcts(PlayerId playerId, Battle battle, int maxIterations, double explorationParameter,
        Library library, int? seed = null, int? maxDegreeOfParallelism = null, int? maxTimer = null)
    {
        PlayerId = playerId;
        Battle = battle;
        if (maxIterations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxIterations),
                "Max iterations must be greater than 0.");
        }
        if (explorationParameter < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(explorationParameter),
                "Exploration parameter must be 0 or greater.");
        }
        _seed = seed ?? Environment.TickCount;
        _mcts = new PokemonMonteCarloTreeSearch(maxIterations, explorationParameter, playerId, library,
            _seed, maxDegreeOfParallelism, maxTimer);
    }

    public Choice GetNextChoice(Choice[] availableChoices)
    {
        switch (availableChoices.Length)
        {
            case 0:
                throw new ArgumentException("No available choices provided.", nameof(availableChoices));
            case 1:
                // Only one choice available, no need for MCTS
                _choiceTimings.Add(0.0); // No time spent on MCTS
                return availableChoices[0];
            default:
                try
                {
                    // Use MCTS to find the best choice
                    PokemonMonteCarloTreeSearch.MoveResult result = _mcts.FindBestChoice(Battle, availableChoices);

                    // Record the timing
                    _choiceTimings.Add(result.ExecutionTimeMs);

                    if (result.OptimalChoice == Choice.Invalid)
                    {
                        throw new InvalidOperationException("MCTS returned an invalid optimal choice." +
                                                            "This should not happen.");
                    }
                    return result.OptimalChoice;
                }
                catch (Exception ex)
                {
                    // Log the exception if you have logging
                    Console.WriteLine($"MCTS failed with exception: {ex.Message}");
            
                    // Record fallback timing (essentially 0)
                    _choiceTimings.Add(0.0);
                    
                    // Fallback to random choice
                    var random = new Random(_seed);
                    return availableChoices[random.Next(availableChoices.Length)];
                }
        }
    }

    /// <summary>
    /// Gets timing statistics for all MCTS choice selections made by this player
    /// </summary>
    public MctsTimingStats GetTimingStats()
    {
        if (_choiceTimings.Count == 0)
        {
            return new MctsTimingStats();
        }

        return new MctsTimingStats
        {
            TotalChoices = _choiceTimings.Count,
            AverageTimeMs = _choiceTimings.Average(),
            MinTimeMs = _choiceTimings.Min(),
            MaxTimeMs = _choiceTimings.Max(),
            TotalTimeMs = _choiceTimings.Sum(),
            StandardDeviationMs = CalculateStandardDeviation(_choiceTimings)
        };
    }

    /// <summary>
    /// Resets the timing statistics
    /// </summary>
    public void ResetTimingStats()
    {
        _choiceTimings.Clear();
    }

    private static double CalculateStandardDeviation(IEnumerable<double> values)
    {
        var list = values.ToList();
        if (list.Count <= 1) return 0.0;

        double mean = list.Average();
        double variance = list.Select(x => Math.Pow(x - mean, 2)).Average();
        return Math.Sqrt(variance);
    }
}

/// <summary>
/// Statistics for MCTS timing performance
/// </summary>
public struct MctsTimingStats
{
    public int TotalChoices { get; set; }
    public double AverageTimeMs { get; set; }
    public double MinTimeMs { get; set; }
    public double MaxTimeMs { get; set; }
    public double TotalTimeMs { get; set; }
    public double StandardDeviationMs { get; set; }

    public override string ToString()
    {
        return $"MCTS Timing Stats - Choices: {TotalChoices}, Avg: {AverageTimeMs:F3}ms, " +
               $"Min: {MinTimeMs:F3}ms, Max: {MaxTimeMs:F3}ms, Total: {TotalTimeMs:F3}ms, " +
               $"StdDev: {StandardDeviationMs:F3}ms";
    }
}