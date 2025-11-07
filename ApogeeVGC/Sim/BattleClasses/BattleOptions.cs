using ApogeeVGC.Player;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public record BattleOptions
{
    public Format? Format { get; init; }
    public FormatId Id { get; init; }
    public Action<SendType, IEnumerable<string>>? Send { get; set; } // Output callback
    public PrngSeed? Seed { get; init; } // PRNG seed
    public bool? Rated { get; init; } // Rated string
    public required PlayerOptions Player1Options { get; init; }
    public required PlayerOptions Player2Options { get; init; }
    public bool Debug { get; set; } // show debug mode option
}