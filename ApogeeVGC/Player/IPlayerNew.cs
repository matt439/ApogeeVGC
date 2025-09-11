using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Player;

public interface IPlayerNew
{
    PlayerId PlayerId { get; }

    // Async choice submission
    Task<BattleChoice> GetNextChoiceAsync(BattleChoice[] availableChoices, BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken);

    // Events for notifications
    event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    event EventHandler<BattleChoice>? ChoiceSubmitted;

    // Optional: For handling timeouts gracefully
    Task NotifyTimeoutWarningAsync(TimeSpan remainingTime);
    Task NotifyChoiceTimeoutAsync();
}