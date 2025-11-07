using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Player;

public class PlayerRandom(SideId sideId, PlayerOptions options, IBattleController battleController) : IPlayer
{
    public SideId SideId { get; } = sideId;
    public PlayerOptions Options { get; } = options;
    public PlayerUiType UiType => PlayerUiType.None;
    public IBattleController BattleController { get; } = battleController;

    private readonly Prng _random = options.Seed is null ? new Prng(null) : new Prng(options.Seed);

    // Fast sync version for MCTS rollouts (IPlayer)
    public Choice GetNextChoiceSync(IChoiceRequest choice, BattlePerspective perspective)
    {
        return GetNextChoiceFromAll(choice);
    }

    // Simplified async version (IPlayer)
    public Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest, BattlePerspective perspective,
        CancellationToken cancellationToken)
    {
        Choice choice = GetNextChoiceFromAll(choiceRequest);
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    // Full async version for backward compatibility (IPlayer)
    public Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        Choice choice = GetNextChoiceFromAll(choiceRequest);
        ChoiceRequested?.Invoke(this, new ChoiceRequestEventArgs
        {
            Choice = choiceRequest,
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

    private Choice GetNextChoiceFromAll(IChoiceRequest choice)
    {
        throw new NotImplementedException();
    }
}