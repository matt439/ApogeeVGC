using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public record EventListenerWithoutPriority
{
    public required IEffect Effect { get; init; }
    public Pokemon? Target { get; set; }
    public int? Index { get; set; }
    public Delegate? Callback { get; init; }
    public EffectState? State { get; init; }
    public Delegate? End { get; init; }
    public List<object>? EndCallArgs { get; init; }
    public required EffectHolder EffectHolder { get; init; } 
}