namespace ApogeeVGC.Sim.BattleClasses.Messages;

/// <summary>
/// Message emitted when the battle has ended.
/// </summary>
public record BattleEndedMessage(string? Winner) : IBattleMessage;
