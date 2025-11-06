using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.SideClasses;

public record SideOpponentPerspective
{
    public IReadOnlyList<PokemonOpponentPerspective> Pokemon { get; init; } = [];
    public IReadOnlyList<PokemonOpponentPerspective?> Active { get; init; } = [];
}