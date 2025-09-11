using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Turns;

public sealed record TeamPreviewTurn : Turn
{
    public override TimeSpan TurnTimeLimit => TimeSpan.FromSeconds(90);

    public TeamPreviewTurn(Side side1Start, Side side2Start, Field field)
    {
        Side1Start = side1Start;
        Side2Start = side2Start;
        Field = field;

        // Initialise the end states to be the same as the start states.
        // They will be modified later in the turn processing.
        Side1End = side1Start;
        Side2End = side2Start;
    }

    public override Side Side1End { get; init; }
    public override Side Side2End { get; init; }
}