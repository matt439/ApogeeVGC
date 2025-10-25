using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.GameObjects;

public record Learnset : ICopyable<Learnset>
{
    public IReadOnlyDictionary<string, List<MoveSource>>? LearnsetData { get; init; }
    public IReadOnlyList<EventInfo>? EventData { get; init; }
    public bool? EventOnly { get; init; }
    public IReadOnlyList<EventInfo>? Encounters { get; init; }
    public bool? Exists { get; init; }

    public Learnset Copy()
    {
        return this with
        {
            LearnsetData = LearnsetData == null
                ? null
                : new Dictionary<string, List<MoveSource>>(
                    LearnsetData.Select(kvp =>
                        new KeyValuePair<string, List<MoveSource>>(
                            kvp.Key,
                            [..kvp.Value]
                        )
                    )
                ),
            EventData = EventData == null
                ? null
                : new List<EventInfo>(EventData),
            Encounters = Encounters == null
                ? null
                : new List<EventInfo>(Encounters),
            // EventOnly and Exists are value types, copied automatically by 'with'
        };
    }
}