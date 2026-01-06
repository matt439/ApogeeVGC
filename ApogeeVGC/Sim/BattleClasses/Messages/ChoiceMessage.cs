using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.BattleClasses.Messages;

/// <summary>
/// Message sent when a player submits a choice (move, switch, etc.).
/// </summary>
public record ChoiceMessage(SideId SideId, Choice Choice) : IPlayerMessage;
