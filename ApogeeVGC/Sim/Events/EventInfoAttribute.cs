namespace ApogeeVGC.Sim.Events;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EventInfoAttribute(EventScope scope) : Attribute
{
    public EventScope Scope { get; set; } = scope;
    public bool HasOnPrefix { get; set; }
    public bool HasFoePrefix { get; set; }
    public string Description { get; set; } = string.Empty;
}