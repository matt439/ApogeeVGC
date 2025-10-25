using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Choices;

/// <summary>
/// A single action that can be chosen. Choices will have one Action for each pokemon.
/// </summary>
public record ChosenAction : IActionChoice
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


    public IntFalseUnion Order { get; init; } = int.MaxValue;
    public int Priority { get; init; }
    public int Speed { get; init; }
    public int SubOrder { get; init; }
    public int EffectOrder { get; init; }
}