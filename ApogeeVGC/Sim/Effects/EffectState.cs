using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Effects;

public class EffectState
{
    public required EffectStateId Id { get; init; }
    public int EffectOrder { get; init; }
    public int? Duration { get; init; }

    // other properties that might be relevant to effect state

    public bool? FromBooster { get; set; }
    public StatIdExceptHp? BestStat { get; set; }
    public Pokemon? Target { get; init; }
}