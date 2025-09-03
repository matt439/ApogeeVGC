using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.GameObjects;

public class EventInfo
{
    public int Generation { get; set; }
    public int? Level { get; set; }
    public bool? Shiny { get; set; } // true: always shiny, 1: sometimes shiny, false/null: never shiny
    public int? ShinySometimes { get; set; } // Use this if you need to distinguish '1'
    public GenderId? Gender { get; set; }
    public string? Nature { get; set; }
    public Dictionary<StatId, int>? Ivs { get; set; }
    public int? PerfectIvs { get; set; }
    public bool? IsHidden { get; set; }
    public List<AbilityId>? Abilities { get; set; }
    public int? MaxEggMoves { get; set; }
    public List<string>? Moves { get; set; }
    public string? Pokeball { get; set; }
    public string? From { get; set; }
    public bool? Japan { get; set; }
    public bool? EmeraldEventEgg { get; set; }
}