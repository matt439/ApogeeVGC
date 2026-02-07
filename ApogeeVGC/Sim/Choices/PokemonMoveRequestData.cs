using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;
using System.Text.Json.Serialization;

namespace ApogeeVGC.Sim.Choices;

public record PokemonMoveData
{
    [JsonIgnore]
    public required Move Move { get; init; }
    
    [JsonPropertyName("move")]
    public string Name => Move.Name;
    
    public MoveId Id => Move.Id;
    public MoveTarget? Target { get; init; }
    public MoveIdBoolUnion? Disabled { get; set; }
    public EffectStateId? DisabledSource { get; init; }
    public int Pp { get; init; }
    public int MaxPp { get; init; }
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