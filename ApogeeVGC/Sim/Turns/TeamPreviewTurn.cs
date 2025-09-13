using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Turns;

public sealed record TeamPreviewTurn : Turn //(Side Side1Start, Side Side2Start, Field FieldStart, int TurnCounter)
    //: Turn(Side1Start, Side2Start, FieldStart, TurnCounter)
{
    public static TimeSpan TurnTimeLimit => TimeSpan.FromSeconds(90);

    /// <summary>
    /// Creates a deep copy of this TeamPreviewTurn for simulation purposes.
    /// </summary>
    public override Turn Copy()
    {
        return this with
        {
            Side1Start = Side1Start.Copy(),
            Side1End = Side1End?.Copy(),
            Side2Start = Side2Start.Copy(),
            Side2End = Side2End?.Copy(),
            FieldStart = FieldStart.Copy(),
            FieldEnd = FieldEnd?.Copy(),
        };
    }
}