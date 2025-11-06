using ApogeeVGC.Sim.Conditions;

namespace ApogeeVGC.Sim.FieldClasses;

public record FieldPerspective
{
    public ConditionId Weather { get; init; }
    public ConditionId Terrain { get; init; }
    public IReadOnlyList<ConditionId> PseudoWeather { get; init; } = [];
}