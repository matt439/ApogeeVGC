using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.BattleClasses.Messages;

/// <summary>
/// Message emitted when the battle has UI updates for players.
/// </summary>
public record UpdateMessage(
    SideId SideId,
    BattlePerspective Perspective,
    IEnumerable<BattleMessage> Messages) : IBattleMessage;
