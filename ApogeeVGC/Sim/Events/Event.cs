using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Events;

public class Event
{
    //public required EventId EventId { get; init; }
    //public double Modifier { get; set; } = 1.0;
    //public Pokemon? Target { get; set; }

    public EventId Id { get; set; }
    public SingleEventSource? Source { get; set; }
    public SingleEventTarget? Target { get; set; }
    public IEffect? Effect { get; set; }
    public double? Modifier { get; set; }

    public EffectDelegate? GetDelegate(EventId id)
    {
        return null;
        // TODO: Implement logic to return the appropriate delegate based on the EventId
    }
}