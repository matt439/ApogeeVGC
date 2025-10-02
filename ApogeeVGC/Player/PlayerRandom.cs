using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Player;

public class PlayerRandom(PlayerId playerId, int? seed = null) : IPlayer
{
    public PlayerId PlayerId { get; } = playerId;

    private readonly Random _random = seed is null ? new Random() : new Random(seed.Value);

    // Fast sync version for MCTS rollouts (IPlayer)
    public BattleChoice GetNextChoiceSync(BattleChoice[] choices, BattlePerspective perspective)
    {
        return GetNextChoiceFromAll(choices);
    }

    // Simplified async version (IPlayer)
    public Task<BattleChoice> GetNextChoiceAsync(BattleChoice[] choices, BattlePerspective perspective,
        CancellationToken cancellationToken)
    {
        if (choices.Length == 0)
        {
            throw new ArgumentException("No available choices to select from.", nameof(choices));
        }

        BattleChoice choice = GetNextChoiceFromAll(choices);
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    // Full async version for backward compatibility (IPlayerNew)
    public Task<BattleChoice> GetNextChoiceAsync(BattleChoice[] availableChoices, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        if (availableChoices.Length == 0)
        {
            throw new ArgumentException("No available choices to select from.", nameof(availableChoices));
        }

        BattleChoice choice = GetNextChoiceFromAll(availableChoices);
        //ChoiceRequested?.Invoke(this, new ChoiceRequestEventArgs
        //{
        //    AvailableChoices = availableChoices,
        //    TimeLimit = TimeSpan.FromSeconds(45),
        //    RequestTime = DateTime.UtcNow,
        //});
        //ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    // Events from interfaces
    //public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<BattleChoice>? ChoiceSubmitted;

    // Timeout methods from IPlayerNew
    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime)
    {
        // Random player doesn't need timeout warnings since it's automated
        return Task.CompletedTask;
    }

    public Task NotifyChoiceTimeoutAsync()
    {
        // Random player doesn't need timeout notifications since it's automated
        return Task.CompletedTask;
    }

    private BattleChoice GetNextChoiceFromAll(BattleChoice[] availableChoices)
    {
        // Select a random choice from all available choices
        if (availableChoices.Length == 0)
        {
            throw new ArgumentException("No available choices to select from.", nameof(availableChoices));
        }
        int randomIndex = _random.Next(availableChoices.Length);
        return availableChoices[randomIndex];
    }
}