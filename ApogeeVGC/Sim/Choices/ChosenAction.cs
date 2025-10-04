using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Choices;

public record ChosenAction
{
    public required ChoiceType Choice { get; init; }
    public Pokemon? Pokemon { get; init; }
    public int? TargetLoc { get; init; }
    public required MoveId MoveId { get; init; }
    public Move? Move { get; init; }
    public Pokemon? Target { get; init; }
    public int? Index { get; init; }
    public Side? Side { get; init; }
    public MoveType? Terastallize { get; init; }
    public int? Priority { get; init; }
}