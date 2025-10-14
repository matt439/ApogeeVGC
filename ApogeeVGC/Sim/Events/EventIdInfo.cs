namespace ApogeeVGC.Sim.Events;

public record EventIdInfo
{
    public EventId Id { get; init; }
    public EventPrefix Prefix { get; init; } = EventPrefix.None;
    public EventSuffix Suffix { get; init; } = EventSuffix.None;
    public EventType Type { get; init; } = EventType.None;
}