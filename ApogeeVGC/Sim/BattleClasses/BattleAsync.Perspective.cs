using ApogeeVGC.Player;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    private BattlePerspective GetPerspective(PlayerId playerId)
    {
        // TODO: Hide information based on game rules
        return playerId switch
        {
            PlayerId.Player1 => BattlePerspective.CreateSafe(Side1, Side2, Field, TurnCounter, this),
            PlayerId.Player2 => BattlePerspective.CreateSafe(Side2, Side1, Field, TurnCounter, this),
            _ => throw new ArgumentOutOfRangeException(nameof(playerId), "Invalid player ID"),
        };
    }
}