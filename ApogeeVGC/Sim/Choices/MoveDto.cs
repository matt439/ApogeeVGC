using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using System.Text.Json.Serialization;

namespace ApogeeVGC.Sim.Choices;

/// <summary>
/// Data Transfer Object for Move that excludes delegate properties for JSON serialization.
/// </summary>
public record MoveDto
{
    public required MoveId Id { get; init; }
    public required string Name { get; init; }
    public int BasePower { get; init; }
    
    /// <summary>
    /// Accuracy as an integer (1-100) or null if the move always hits.
    /// Represents IntTrueUnion where true means "always hits" (null in DTO).
    /// </summary>
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Accuracy { get; init; }
 
 public int BasePp { get; init; }
    public MoveCategory Category { get; init; }
 public MoveType Type { get; init; }
   public int Priority { get; init; }
   public MoveTarget Target { get; init; }
    
    public static MoveDto FromMove(Move move)
    {
   return new MoveDto
  {
      Id = move.Id,
       Name = move.Name,
      BasePower = move.BasePower,
   // Convert IntTrueUnion to int? - true means always hit (null), int means accuracy value
Accuracy = move.Accuracy switch
         {
    Utils.Unions.TrueIntTrueUnion => null, // Always hits
    Utils.Unions.IntIntTrueUnion { Value: var acc } => acc,
    _ => null
     },
    BasePp = move.BasePp,
          Category = move.Category,
Type = move.Type,
    Priority = move.Priority,
  Target = move.Target,
        };
    }
}
