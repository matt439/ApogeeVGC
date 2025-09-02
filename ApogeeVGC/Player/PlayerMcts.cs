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

    public PlayerMcts(PlayerId playerId, Battle battle, int maxIterations, double explorationParameter,
        Library library, int? seed = null, int? maxDegreeOfParallelism = null)
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
            _seed, maxDegreeOfParallelism);
    }

    public Choice GetNextChoice(Choice[] availableChoices)
    {
        switch (availableChoices.Length)
        {
            case 0:
                throw new ArgumentException("No available choices provided.", nameof(availableChoices));
            case 1:
                // Only one choice available, no need for MCTS
                return availableChoices[0];
            default:
                try
                {
                    // Use MCTS to find the best choice
                    PokemonMonteCarloTreeSearch.MoveResult result = _mcts.FindBestChoice(Battle, availableChoices);

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
            
                    // Fallback to random choice
                    var random = new Random(_seed);
                    return availableChoices[random.Next(availableChoices.Length)];
                }
        }
    }
}