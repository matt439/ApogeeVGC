using System.Text.Json.Serialization;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Moves;

public record HitEffect
{
    [JsonIgnore]
    public ResultMoveHandler? OnHit { get; set; }
    public SparseBoostsTable? Boosts { get; set; }
    public ConditionId? Status { get; init; }
    public ConditionId? VolatileStatus { get; set; }
    public ConditionId? SideCondition { get; set; } // Settable for pledge combo side conditions
    public ConditionId? SlotCondition { get; init; }
    public ConditionId? PseudoWeather { get; init; }
    public ConditionId? Terrain { get; init; }
    public ConditionId? Weather { get; init; }
}