using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Turns;

public sealed record TeamPreviewTurn : Turn //(Side Side1Start, Side Side2Start, Field FieldStart, int TurnCounter)
    //: Turn(Side1Start, Side2Start, FieldStart, TurnCounter)
{
    public static TimeSpan TurnTimeLimit => TimeSpan.FromSeconds(90);
}