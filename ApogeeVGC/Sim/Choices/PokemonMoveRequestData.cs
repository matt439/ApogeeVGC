using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;
using System.Text.Json.Serialization;

namespace ApogeeVGC.Sim.Choices;

public record PokemonMoveData
{
    public required MoveDto Move { get; init; }
    public MoveId Id => Move.Id;
    public MoveTarget? Target { get; init; }
    
    /// <summary>
    /// Indicates if the move is disabled. Can be true, false, or a MoveId indicating why it's disabled.
    /// For JSON serialization, we simplify this to bool? where null means not disabled.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Disabled { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEffect? DisabledSource { get; init; }
    
    /// <summary>
 /// Internal property for working with the union type in code.
 /// </summary>
    [JsonIgnore]
    public MoveIdBoolUnion? DisabledUnion { get; set; }
}

public record PokemonMoveRequestData
{
    public required IReadOnlyList<PokemonMoveData> Moves { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? MaybeDisabled { get; set; }
  
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public bool? MaybeLocked { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Trapped { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? MaybeTrapped { get; set; }
    
  /// <summary>
    /// The Tera type the Pokemon can terastallize into, or null if it cannot terastallize.
    /// Represents MoveTypeFalseUnion where false means cannot terastallize (null in DTO).
    /// </summary>
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MoveType? CanTerastallize { get; set; }
}
