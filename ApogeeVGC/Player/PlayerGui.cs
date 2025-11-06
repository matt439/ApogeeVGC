using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Player;

public class PlayerGui(SideId sideId) : IPlayer
{
    public SideId SideId { get; } = sideId;

    public Task<Choice> GetNextChoiceAsync(List<IChoiceRequest> availableChoices, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;
    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime)
    {
        throw new NotImplementedException();
    }

    public Task NotifyChoiceTimeoutAsync()
    {
        throw new NotImplementedException();
    }
}