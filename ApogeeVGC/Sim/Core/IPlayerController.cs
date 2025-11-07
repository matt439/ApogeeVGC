using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Sim.Core;

public interface IPlayerController
{
    Task<Choice> RequestChoiceAsync(SideId sideId, IChoiceRequest choiceRequest,
        BattleRequestType requestType, BattlePerspective perspective, CancellationToken cancellationToken);

    void UpdatePlayerUi(SideId sideId, BattlePerspective perspective);

    PlayerUiType GetPlayerUiType(SideId sideId);
}