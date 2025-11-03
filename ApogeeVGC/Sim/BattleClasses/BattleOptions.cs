using System.Text.Json.Serialization;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public record BattleOptions
{
    public Format? Format { get; init; }

    [JsonConverter(typeof(FormatIdJsonConverter))]
    [JsonPropertyName("formatid")]
    public FormatId Id { get; init; }
    
    public Action<SendType, IEnumerable<string>>? Send { get; set; } // Output callback
    public Prng? Prng { get; init; } // PRNG override (you usually don't need this, just pass a seed)
    public PrngSeed? Seed { get; init; } // PRNG seed
    public bool? Rated { get; init; } // Rated string
    public PlayerOptions? P1 { get; init; } // Player 1 data
    public PlayerOptions? P2 { get; init; } // Player 2 data
    //public PlayerOptions? P3 { get; init; } // Player 3 data
    //public PlayerOptions? P4 { get; init; } // Player 4 data
    public bool Debug { get; set; } // show debug mode option
    public bool? ForceRandomChance { get; init; } // force Battle#randomChance to always return true or false (used in some tests)
    public bool Deserialized { get; init; }
    public bool StrictChoices { get; init; } // whether invalid choices should throw
}