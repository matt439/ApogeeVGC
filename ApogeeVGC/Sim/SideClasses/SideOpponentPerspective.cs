using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.SideClasses;

/// <summary>
/// Represents the opponent's side with full information visible (full observability mode).
/// </summary>
public record SideOpponentPerspective
{
    public IReadOnlyList<PokemonPerspective> Pokemon { get; init; } = [];
    public IReadOnlyList<PokemonPerspective?> Active { get; init; } = [];
    public IReadOnlyDictionary<ConditionId, int?> SideConditionsWithDuration { get; init; } = new Dictionary<ConditionId, int?>();
}