using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.SideClasses;

public record SidePlayerPerspective
{
    public IReadOnlyList<PokemonSet> Team { get; init; } = [];
    public IReadOnlyList<PokemonPlayerPerspective> Pokemon { get; init; } = [];
    public IReadOnlyList<PokemonPlayerPerspective?> Active { get; init; } = [];
}