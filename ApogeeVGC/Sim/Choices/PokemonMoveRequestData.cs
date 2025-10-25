using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Choices;

public record PokemonMoveData
{
    public required Move Move { get; init; }
    public MoveId Id => Move.Id;
    public MoveTarget? Target { get; init; }
    public MoveIdBoolUnion? Disabled { get; set; }
    public IEffect? DisabledSource { get; init; }
}

public record PokemonMoveRequestData
{
    public required IReadOnlyList<PokemonMoveData> Moves { get; init; }
    public bool? MaybeDisabled { get; set; }
    public bool? MaybeLocked { get; set; }
    public bool? Trapped { get; set; }
    public bool? MaybeTrapped { get; set; }
    public MoveTypeFalseUnion? CanTerastallize { get; set; }
}