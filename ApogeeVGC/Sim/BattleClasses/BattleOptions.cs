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
    public required IPlayer P1 { get; init; }
    public required IPlayer P2 { get; init; }
    public bool Debug { get; set; } // show debug mode option
}