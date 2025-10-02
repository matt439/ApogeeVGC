using ApogeeVGC.Data;
using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.Player;

public class PlayerMcts : IPlayer
{
    public PlayerId PlayerId { get; }
    
    private readonly int _seed;
    private readonly PokemonMcts _mcts;
    private readonly Random _fallbackRandom;

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

    // Fast sync version for MCTS rollouts (IPlayer)
    public BattleChoice GetNextChoiceSync(BattleChoice[] choices, BattlePerspective perspective)
    {
        return GetNextChoice(choices, perspective);
    }

    // Simplified async version (IPlayer)
    public Task<BattleChoice> GetNextChoiceAsync(BattleChoice[] choices, BattlePerspective perspective,
        CancellationToken cancellationToken)
    {
        BattleChoice choice = GetNextChoice(choices, perspective);
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    // Full async version for backward compatibility (IPlayerNew)
    public Task<BattleChoice> GetNextChoiceAsync(BattleChoice[] availableChoices, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        //// Notify that a choice has been requested
        //var requestEventArgs = new ChoiceRequestEventArgs
        //{
        //    AvailableChoices = availableChoices,
        //    TimeLimit = TimeSpan.FromSeconds(45), // Default timeout
        //    RequestTime = DateTime.UtcNow,
        //};
        //ChoiceRequested?.Invoke(this, requestEventArgs);

        BattleChoice choice = GetNextChoice(availableChoices, perspective);
        
        // Notify that a choice has been submitted
        ChoiceSubmitted?.Invoke(this, choice);
        
        return Task.FromResult(choice);
    }

    // Events from interfaces
    //public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<BattleChoice>? ChoiceSubmitted;

    // Timeout methods from IPlayerNew
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
                return availableChoices[0];
            default:
                PokemonMcts.MoveResult result = _mcts.FindBestChoice(perspective.Battle, availableChoices);
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