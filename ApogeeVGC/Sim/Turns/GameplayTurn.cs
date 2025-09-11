using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Turns;

public sealed record GameplayTurn : Turn
{
    public override Side Side1End { get; init; }
    public override Side Side2End { get; init; }
    public override TimeSpan TurnTimeLimit => TimeSpan.FromSeconds(45);
    public required int TurnCounter { get; init; }

    public GameplayTurn(Side side1Start, Side side2Start)
    {
        Side1Start = side1Start;
        Side2Start = side2Start;

        // Initialise the end states to be the same as the start states.
        // They will be modified later in the turn processing.
        Side1End = side1Start;
        Side2End = side2Start;
    }
}