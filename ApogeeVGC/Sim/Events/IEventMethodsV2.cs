using ApogeeVGC.Sim.Events.Handlers.EventMethods;

namespace ApogeeVGC.Sim.Events;

public interface IEventMethodsV2
{
    OnDamagingHitEventInfo? OnDamagingHit { get; }
}
