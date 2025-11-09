using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Sim.Core;

public interface IPlayerController
{
    Task<Choice> RequestChoiceAsync(SideId sideId, IChoiceRequest choiceRequest,
        BattleRequestType requestType, BattlePerspective perspective, CancellationToken cancellationToken);

    void UpdatePlayerUi(SideId sideId, BattlePerspective perspective);
    
    void UpdateMessages(SideId sideId, IEnumerable<BattleMessage> messages);
    
    /// <summary>
    /// Clear all messages from the message display.
    /// Typically called at the end of each turn to prevent message overflow.
    /// </summary>
    void ClearMessages();

    PlayerUiType GetPlayerUiType(SideId sideId);
}