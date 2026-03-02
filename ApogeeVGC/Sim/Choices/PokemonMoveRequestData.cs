using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;
using System.Text.Json.Serialization;

namespace ApogeeVGC.Sim.Choices;

public record struct PokemonMoveData
{
    [JsonIgnore]
    public required Move Move { get; init; }

    [JsonPropertyName("move")]
    public readonly string Name => Move.Name;

    public readonly MoveId Id => Move.Id;
    public MoveTarget? Target { get; init; }
    public MoveIdBoolUnion? Disabled { get; set; }
    public EffectStateId? DisabledSource { get; set; }
    public int Pp { get; init; }
    public int MaxPp { get; init; }
}

public record PokemonMoveRequestData
{
    public required PokemonMoveData[] Moves { get; init; }
    public bool? MaybeDisabled { get; set; }
    public bool? MaybeLocked { get; set; }
    public bool? Trapped { get; set; }
    public bool? MaybeTrapped { get; set; }
    public MoveTypeFalseUnion? CanTerastallize { get; set; }
    public SpecieId? CanMegaEvo { get; set; }
}