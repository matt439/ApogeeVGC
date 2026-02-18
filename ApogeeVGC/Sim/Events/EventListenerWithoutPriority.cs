using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

public class EventListenerWithoutPriority
{
    public required IEffect Effect { get; set; }
    public Pokemon? Target { get; set; }
    public int? Index { get; set; }
    public EventHandlerInfo? HandlerInfo { get; set; }
    public EffectState? State { get; set; }
    public EffectDelegate? End { get; set; }
    public List<object>? EndCallArgs { get; set; }
    public required EffectHolder EffectHolder { get; set; }
}