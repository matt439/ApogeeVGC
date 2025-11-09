using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Player;

public enum PlayerUiType
{
    None,
    Gui,
}

public interface IPlayer
{
    SideId SideId { get; }
    PlayerOptions Options { get; }
    PlayerUiType UiType { get; }
    IBattleController BattleController { get; }

    // Async choice submission
    Task<Choice> GetNextChoiceAsync(IChoiceRequest choice, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken);

    void UpdateUi(BattlePerspective perspective);
    
    void UpdateMessages(IEnumerable<BattleMessage> messages);

    // Events for notifications
    event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    event EventHandler<Choice>? ChoiceSubmitted;

    // Optional: For handling timeouts gracefully
    Task NotifyTimeoutWarningAsync(TimeSpan remainingTime);
    Task NotifyChoiceTimeoutAsync();
}

//public interface IPlayer : IPlayer
//{
//    // Sync version for MCTS (fast)
//    Choice GetNextChoiceSync(List<IChoiceRequest> choices, BattlePerspective perspective);

//    // Simplified async version for human players (with events, timeouts)
//    Task<Choice> GetNextChoiceAsync(List<IChoiceRequest> choices, BattlePerspective perspective,
//        CancellationToken cancellationToken);
//}