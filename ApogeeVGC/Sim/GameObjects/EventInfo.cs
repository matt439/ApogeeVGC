using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.GameObjects;

public record EventInfo
{
    public int Generation { get; init; }
    public int? Level { get; init; }
    public bool? Shiny { get; init; } // true: always shiny, 1: sometimes shiny, false/null: never shiny
    public int? ShinySometimes { get; init; } // Use this if you need to distinguish '1'
    public GenderId? Gender { get; init; }
    public string? Nature { get; init; }
    public Dictionary<StatId, int>? Ivs { get; init; }
    public int? PerfectIvs { get; init; }
    public bool? IsHidden { get; init; }
    public List<AbilityId>? Abilities { get; init; }
    public int? MaxEggMoves { get; init; }
    public List<string>? Moves { get; init; }
    public string? Pokeball { get; init; }
    public string? From { get; init; }
    public bool? Japan { get; init; }
    public bool? EmeraldEventEgg { get; init; }
}