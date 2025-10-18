using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Moves;

public record HitEffect
{
    public ResultMoveHandler? OnHit { get; init; }
    public SparseBoostsTable? Boosts { get; init; }
    public ConditionId? Status { get; init; }
    public ConditionId? VolatileStatus { get; init; }
    public ConditionId? SideCondition { get; init; }
    public ConditionId? SlotCondition { get; init; }
    public ConditionId? PseudoWeather { get; init; }
    public ConditionId? Terrain { get; init; }
    public ConditionId? Weather { get; init; }
}