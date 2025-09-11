using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Player;

public class PlayerRandomNew(PlayerId playerId, int? seed = null) : IPlayerNew
{
    public PlayerId PlayerId { get; } = playerId;

    private readonly Random _random = seed is null ? new Random() : new Random(seed.Value);

    public Task<BattleChoice> GetNextChoiceAsync(BattleChoice[] availableChoices, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        if (availableChoices.Length == 0)
        {
            throw new ArgumentException("No available choices to select from.", nameof(availableChoices));
        }

        BattleChoice choice = GetNextChoiceFromAll(availableChoices);
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<BattleChoice>? ChoiceSubmitted;

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