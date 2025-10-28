using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.Choices;

/// <summary>
/// Data Transfer Object for Move - contains only serializable data needed for requests.
/// </summary>
public record MoveDto
{
    public required MoveId Id { get; init; }
    public required string Name { get; init; }
    public int Num { get; init; }
    public int BasePower { get; init; }
    public MoveCategory Category { get; init; }
    public MoveType Type { get; init; }
    public int Priority { get; init; }
    public MoveTarget Target { get; init; }
    public int BasePp { get; init; }

    public static MoveDto FromMove(Move move)
    {
        return new MoveDto
        {
       Id = move.Id,
     Name = move.Name,
            Num = move.Num,
   BasePower = move.BasePower,
            Category = move.Category,
         Type = move.Type,
            Priority = move.Priority,
   Target = move.Target,
      BasePp = move.BasePp,
        };
    }
}
