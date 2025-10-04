using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Choices;

public record PokemonMoveData
{
    public required Move Move { get; init; }
    public MoveId Id => Move.Id;
    public Pokemon? Target { get; init; }
    public MoveIdBoolUnion? Disabled { get; init; }
    public Pokemon? DisabledSource { get; init; }
}

public record PokemonMoveRequestData
{
    public required IReadOnlyList<PokemonMoveData> Moves { get; init; }
    public bool? MaybeDisabled { get; init; }
    public bool? MaybeLocked { get; init; }
    public bool? Trapped { get; init; }
    public bool? MaybeTrapped { get; init; }
    public bool? CanTerastallize { get; init; }
}