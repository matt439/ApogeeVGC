using ApogeeVGC.Player;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public record PlayerOptions
{
    public PlayerType Type { get; init; } = PlayerType.Random;
    public required string Name { get; init; }
    public required IReadOnlyList<PokemonSet> Team { get; init; }
    public PrngSeed? Seed { get; init; }
}