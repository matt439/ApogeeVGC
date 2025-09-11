using ApogeeVGC.Player;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Turns;

public sealed record PostGameTurn : Turn
{
    public override TimeSpan TurnTimeLimit => TimeSpan.Zero; // No timeout for completed games

    public required PlayerId Winner { get; init; }
    public PlayerId Loser => Winner.OpposingPlayerId();

    // The end states are the same as the start states, as no further changes occur after the game ends.
    public override Side Side1End
    {
        get => Side1Start;
        init { }
    }
    public override Side Side2End
    {
        get => Side2Start;
        init { }
    }
}