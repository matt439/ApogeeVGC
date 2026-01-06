using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.BattleClasses.Messages;

/// <summary>
/// Message emitted when the battle requests a choice from a player.
/// </summary>
public record ChoiceRequestMessage(
    SideId SideId,
    IChoiceRequest Request,
    BattleRequestType RequestType,
    BattlePerspective Perspective) : IBattleMessage;
