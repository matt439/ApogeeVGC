using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Represents the perspective of a battle from one player's point of view
/// Some of the opponent's information may be hidden (e.g., moves, items)
/// </summary>
public record BattlePerspective
{
    public required SidePlayerPerspective PlayerSide { get; init; }
    public required SideOpponentPerspective OpponentSide { get; init; }
    public required FieldPerspective Field { get; init; }
    public required int TurnCounter { get; init; }
}
