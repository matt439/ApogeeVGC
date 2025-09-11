using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Turns;

public abstract record Turn
{
    public required Side Side1Start { get; init; }
    public required Side Side2Start { get; init; }
    public abstract Side Side1End { get; init; }
    public abstract Side Side2End { get; init; }
    public required Field Field { get; init; }

    public abstract TimeSpan TurnTimeLimit { get; }
    public DateTime TurnStartTime { get; init; } = DateTime.UtcNow;

    public bool HasTurnTimedOut => DateTime.UtcNow - TurnStartTime > TurnTimeLimit;
}