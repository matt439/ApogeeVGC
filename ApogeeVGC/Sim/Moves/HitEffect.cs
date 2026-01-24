using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Moves;

public record HitEffect
{
    public ResultMoveHandler? OnHit { get; set; }
    public SparseBoostsTable? Boosts { get; set; }
    public ConditionId? Status { get; init; }
    public ConditionId? VolatileStatus { get; set; }
    public ConditionId? SideCondition { get; init; }
    public ConditionId? SlotCondition { get; init; }
    public ConditionId? PseudoWeather { get; init; }
    public ConditionId? Terrain { get; init; }
    public ConditionId? Weather { get; init; }
}