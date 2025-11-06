using ApogeeVGC.Gui;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Player;

public class PlayerGui(SideId sideId, PlayerOptions options) : IPlayer
{
    public SideId SideId { get; } = sideId;
    public PlayerOptions Options { get; } = options;
    public PlayerUiType UiType => PlayerUiType.Graphical;
    public BattleGame GuiWindow { get; set; } = new();

    public Task<Choice> GetNextChoiceAsync(List<IChoiceRequest> availableChoices, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUi(BattlePerspective perspective)
    {
        // TODO: Implement GUI update logic here
        return Task.CompletedTask;
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