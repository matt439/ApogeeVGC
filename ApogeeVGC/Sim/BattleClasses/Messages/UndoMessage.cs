using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.BattleClasses.Messages;

/// <summary>
/// Message sent when a player requests to undo their choice.
/// </summary>
public record UndoMessage(SideId SideId) : IPlayerMessage;
