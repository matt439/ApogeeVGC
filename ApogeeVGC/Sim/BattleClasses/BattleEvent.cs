namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Represents a battle event: a message and the resulting perspective after that message was processed
/// </summary>
public record BattleEvent
{
    /// <summary>
    /// The message describing what happened
    /// </summary>
    public required BattleMessage Message { get; init; }
    
    /// <summary>
    /// The battle perspective AFTER this message was processed
    /// This is the authoritative state - GUI renders this directly
    /// </summary>
    public required BattlePerspective Perspective { get; init; }
}
