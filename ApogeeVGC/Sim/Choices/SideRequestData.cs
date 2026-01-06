using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.Choices;

public record SideRequestData
{
    public required string Name { get; init; }
    public SideId Id { get; init; }
    public required IReadOnlyList<PokemonSwitchRequestData> Pokemon { get; init; }
    public bool? NoCancel { get; init; }
}