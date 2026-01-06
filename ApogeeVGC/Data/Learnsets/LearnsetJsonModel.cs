using System.Text.Json.Serialization;

namespace ApogeeVGC.Data.Json;

/// <summary>
/// JSON-serializable model for learnset data.
/// Optimized for file size by using string arrays for move sources.
/// </summary>
public sealed class LearnsetJsonModel
{
    /// <summary>
    /// Move ID (string) -> array of source codes (e.g., ["9M", "8L40"])
    /// </summary>
    [JsonPropertyName("learnset")]
    public Dictionary<string, string[]>? Learnset { get; set; }

    [JsonPropertyName("eventData")]
    public List<EventInfoJsonModel>? EventData { get; set; }

    [JsonPropertyName("eventOnly")]
    public bool? EventOnly { get; set; }

    [JsonPropertyName("encounters")]
    public List<EventInfoJsonModel>? Encounters { get; set; }
}

/// <summary>
/// JSON-serializable model for event info data.
/// </summary>
public sealed class EventInfoJsonModel
{
    [JsonPropertyName("generation")]
    public int Generation { get; set; }

    [JsonPropertyName("level")]
    public int? Level { get; set; }

    [JsonPropertyName("shiny")]
    public bool? Shiny { get; set; }

    [JsonPropertyName("shinySometimes")]
    public int? ShinySometimes { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("nature")]
    public string? Nature { get; set; }

    [JsonPropertyName("ivs")]
    public Dictionary<string, int>? Ivs { get; set; }

    [JsonPropertyName("perfectIvs")]
    public int? PerfectIvs { get; set; }

    [JsonPropertyName("isHidden")]
    public bool? IsHidden { get; set; }

    [JsonPropertyName("abilities")]
    public List<string>? Abilities { get; set; }

    [JsonPropertyName("maxEggMoves")]
    public int? MaxEggMoves { get; set; }

    [JsonPropertyName("moves")]
    public List<string>? Moves { get; set; }

    [JsonPropertyName("pokeball")]
    public string? Pokeball { get; set; }

    [JsonPropertyName("from")]
    public string? From { get; set; }

    [JsonPropertyName("japan")]
    public bool? Japan { get; set; }

    [JsonPropertyName("emeraldEventEgg")]
    public bool? EmeraldEventEgg { get; set; }
}

/// <summary>
/// Root model for the learnsets JSON file.
/// Maps species ID (string) -> LearnsetJsonModel
/// </summary>
public sealed class LearnsetsJsonRoot
{
    [JsonPropertyName("learnsets")]
    public Dictionary<string, LearnsetJsonModel> Learnsets { get; set; } = new();
}
