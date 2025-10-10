using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public record PlayerOptions
{
    public string? Name { get; init; }
    public string? Avatar { get; init; }
    public int? Rating { get; init; }
    public IReadOnlyList<PokemonSet>? Team { get; init; }
    public PrngSeed? Seed { get; init; }
}