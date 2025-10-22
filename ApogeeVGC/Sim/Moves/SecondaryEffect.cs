using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Moves;

public record SecondaryEffect : HitEffect
{
    //public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; init; }
    //public SparseBoostsTable? Boosts { get; init; }
    //public Choice? Status { get; init; }
    //public Choice? VolatileStatus { get; init; }
    //public string? SideCondition { get; init; }
    //public string? SlotCondition { get; init; }
    //public string? PseudoWeather { get; init; }
    //public string? Terrain { get; init; }
    //public Choice? Weather { get; init; }
    //public int? Chance { get; init; }
    //public Ability? Ability { get; init; }
    //public bool? KingsRock { get; init; }
    //public IHitEffect? Self { get; init; }
    public SparseBoostsTable? Boosts { get; init; }
    public int? Chance { get; init; }
    public ConditionId? VolatileStatus { get; init; }
}