using ApogeeVGC.Data;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.Player;

public class PlayerMcts : IPlayerNew
{
    public PlayerId PlayerId { get; }
    
    private readonly int _seed;
    private readonly PokemonMcts _mcts;
    private readonly Random _fallbackRandom;

    // Events for IPlayerNew interface
    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<BattleChoice>? ChoiceSubmitted;

    // Property to expose the last MCTS execution time
    public double LastMctsExecutionTimeMs { get; private set; }

    public PlayerMcts(PlayerId playerId, int maxIterations, double explorationParameter,
        Library library, int? seed = null, int? maxDegreeOfParallelism = null, int? maxTimer = null)
    {
        PlayerId = playerId;
        
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
        _fallbackRandom = new Random(_seed);
        
        _mcts = new PokemonMcts(maxIterations, explorationParameter, playerId, library,
            _seed, maxDegreeOfParallelism, maxTimer);
    }

    public Task<BattleChoice> GetNextChoiceAsync(BattleChoice[] availableChoices, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        // Notify that a choice has been requested
        var requestEventArgs = new ChoiceRequestEventArgs
        {
            AvailableChoices = availableChoices,
            TimeLimit = TimeSpan.FromSeconds(45), // Default timeout
            RequestTime = DateTime.UtcNow,
        };
        ChoiceRequested?.Invoke(this, requestEventArgs);

        BattleChoice choice = GetNextChoice(availableChoices, perspective);
        
        // Notify that a choice has been submitted
        ChoiceSubmitted?.Invoke(this, choice);
        
        return Task.FromResult(choice);
    }

    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime)
    {
        // MCTS player could potentially use this to reduce search time or iterations
        // For now, just acknowledge the warning
        return Task.CompletedTask;
    }

    public Task NotifyChoiceTimeoutAsync()
    {
        // MCTS search was interrupted by timeout
        // The GetNextChoiceAsync method should handle this gracefully
        return Task.CompletedTask;
    }

    private BattleChoice GetNextChoice(BattleChoice[] availableChoices, BattlePerspective perspective)
    {
        switch (availableChoices.Length)
        {
            case 0:
                throw new ArgumentException("No available choices provided.", nameof(availableChoices));
            case 1:
                // Only one choice available, no need for MCTS
                LastMctsExecutionTimeMs = 0.0; // No MCTS time needed for single choice
                return availableChoices[0];
            default:
                PokemonMcts.MoveResult result = _mcts.FindBestChoice(perspective.Battle, availableChoices);
                LastMctsExecutionTimeMs = result.ExecutionTimeMs; // Store the execution time
                return result.OptimalChoice;

                //try
                //{
                //    PokemonMcts.MoveResult result = _mcts.FindBestChoice(perspective.Battle, availableChoices);
                //    return result.OptimalChoice;
                //}
                //catch (Exception ex)
                //{
                //    // Log the exception if you have logging
                //    Console.WriteLine($"MCTS failed with exception: {ex.Message}");
            
                //    // Fallback to random choice
                //    return availableChoices[_fallbackRandom.Next(availableChoices.Length)];
                //}
        }
    }
}