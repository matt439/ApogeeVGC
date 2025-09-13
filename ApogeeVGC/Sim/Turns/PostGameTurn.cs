using ApogeeVGC.Player;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.Turns;

public sealed record PostGameTurn: Turn  //(Side Side1Start, Side Side2Start, Field FieldStart, int TurnCounter,
    //PlayerId Winner)
    //: Turn(Side1Start, Side2Start, FieldStart, TurnCounter)
{
    public required PlayerId Winner { get; init; }
    public PlayerId Loser => Winner.OpposingPlayerId();

    // The end states are the same as the start states, as no further changes occur after the game ends.
    //public override Side Side1End
    //{
    //    get => Side1Start;
    //    init { }
    //}
    //public override Side Side2End
    //{
    //    get => Side2Start;
    //    init { }
    //}

    /// <summary>
    /// Creates a deep copy of this PostGameTurn for simulation purposes.
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
            // Winner is copied by value (enum)
        };
    }
}