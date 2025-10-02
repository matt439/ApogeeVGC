using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Effects;

public enum EffectStateId
{
    None,
}

public class EffectState
{
    public EffectStateId Id { get; init; } = EffectStateId.None;
    public int EffectOrder { get; init; }
    public int? Duration { get; init; }

    // other properties that might be relevant to effect state

    public bool? FromBooster { get; set; }
    public StatIdExceptHp? BestStat { get; set; }
}