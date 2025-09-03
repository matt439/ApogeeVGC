using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.GameObjects;

public record Learnset
{
    public Dictionary<string, List<MoveSource>>? LearnsetData { get; init; }
    public List<EventInfo>? EventData { get; init; }
    public bool? EventOnly { get; init; }
    public List<EventInfo>? Encounters { get; init; }
    public bool? Exists { get; init; }
}