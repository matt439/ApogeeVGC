using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.SideClasses;

public record SidePlayerPerspective
{
    public IReadOnlyList<PokemonSet> Team { get; init; } = [];
    public IReadOnlyList<PokemonPerspective> Pokemon { get; init; } = [];
    public IReadOnlyList<PokemonPerspective?> Active { get; init; } = [];
    public IReadOnlyDictionary<ConditionId, int?> SideConditionsWithDuration { get; init; } = new Dictionary<ConditionId, int?>();
}