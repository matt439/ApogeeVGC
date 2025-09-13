using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Turns;

public abstract record Turn//(Side Side1Start, Side Side2Start, Field FieldStart, int TurnCounter)
{
    public required Side Side1Start { get; init; }
    public Side? Side1End { get; init; } = null;
    public required Side Side2Start { get; init; }
    public Side? Side2End { get; init; } = null;
    public required Field FieldStart { get; init; }
    public Field? FieldEnd { get; init; } = null;

    // public abstract TimeSpan TurnTimeLimit { get; }
    public DateTime TurnStartTime { get; init; } = DateTime.UtcNow;
    public DateTime? TurnEndTime { get; init; } = null;

    public required int TurnCounter { get; init; }

    //public bool HasTurnTimedOut => DateTime.UtcNow - TurnStartTime > TurnTimeLimit;

    /// <summary>
    /// Creates a deep copy of this Turn for simulation purposes.
    /// Since Turn is a record, this uses the 'with' expression for copying.
    /// </summary>
    public virtual Turn Copy()
    {
        return this with
        {
            Side1Start = Side1Start.Copy(),
            Side1End = Side1End?.Copy(),
            Side2Start = Side2Start.Copy(),
            Side2End = Side2End?.Copy(),
            FieldStart = FieldStart.Copy(),
            FieldEnd = FieldEnd?.Copy(),
            // DateTime values are copied by value
            // TurnCounter is copied by value
        };
    }
}