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
}