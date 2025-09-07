using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.GameObjects;

public record Learnset
{
    public IReadOnlyDictionary<string, List<MoveSource>>? LearnsetData { get; init; }
    public IReadOnlyList<EventInfo>? EventData { get; init; }
    public bool? EventOnly { get; init; }
    public IReadOnlyList<EventInfo>? Encounters { get; init; }
    public bool? Exists { get; init; }
}