using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public record EventListenerWithoutPriority : IPriorityComparison
{
    public required IEffect Effect { get; init; }
    public Pokemon? Target { get; init; }
    public int? Index { get; init; }
    public Delegate? Callback { get; init; }
    public EffectState? State { get; init; }
    public Delegate? End { get; init; }
    public List<object>? EndCallArgs { get; init; }
    public required EffectHolder EffectHolder { get; init; }
    public ActionOrder Order { get; init; } = ActionOrder.Max;
    public int Priority { get; init; }
    public int Speed { get; init; }
    public int SubOrder { get; init; }
    public int EffectOrder { get; init; }
}