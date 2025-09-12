
using ApogeeVGC.Player;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class PlayerIdTools
{
    public static PlayerId OpposingPlayerId(this PlayerId player)
    {
        return player switch
        {
            PlayerId.Player1 => PlayerId.Player2,
            PlayerId.Player2 => PlayerId.Player1,
            _ => throw new ArgumentOutOfRangeException(nameof(player), player, null),
        };
    }
}