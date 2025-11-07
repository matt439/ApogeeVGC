using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Player;

public class PlayerRandom(SideId sideId, PlayerOptions options) : IPlayer
{
    public SideId SideId { get; } = sideId;
    public PlayerOptions Options { get; } = options;
    public PlayerUiType UiType => PlayerUiType.None;

    private readonly Prng _random = options.Seed is null ? new Prng(null) : new Prng(options.Seed);

    // Fast sync version for MCTS rollouts (IPlayer)
    public Choice GetNextChoiceSync(List<IChoiceRequest> choices, BattlePerspective perspective)
    {
        return GetNextChoiceFromAll(choices);
    }

    // Simplified async version (IPlayer)
    public Task<Choice> GetNextChoiceAsync(List<IChoiceRequest> choices, BattlePerspective perspective,
        CancellationToken cancellationToken)
    {
        if (choices.Count == 0)
        {
            throw new ArgumentException("No available choices to select from.", nameof(choices));
        }

        Choice choice = GetNextChoiceFromAll(choices);
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    // Full async version for backward compatibility (IPlayer)
    public Task<Choice> GetNextChoiceAsync(List<IChoiceRequest> availableChoices, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        if (availableChoices.Count == 0)
        {
            throw new ArgumentException("No available choices to select from.", nameof(availableChoices));
        }

        Choice choice = GetNextChoiceFromAll(availableChoices);
        ChoiceRequested?.Invoke(this, new ChoiceRequestEventArgs
        {
            AvailableChoices = availableChoices,
            TimeLimit = TimeSpan.FromSeconds(45),
            RequestTime = DateTime.UtcNow,
        });
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    public void UpdateUi(BattlePerspective perspective)
    {
        // Random player doesn't have a UI to update so jut return
    }

    // Events from interfaces
    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;

    // Timeout methods from IPlayer
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

    private Choice GetNextChoiceFromAll(List<IChoiceRequest> availableChoices)
    {
        // Select a random choice from all available choices
        if (availableChoices.Count == 0)
        {
            throw new ArgumentException("No available choices to select from.", nameof(availableChoices));
        }
        int randomIndex = _random.Random(availableChoices.Count);
        //return availableChoices[randomIndex];
        throw new NotImplementedException();
    }
}