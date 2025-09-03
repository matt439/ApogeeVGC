using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

public static class BattleTools
{
    public static bool CanSubmitChoice(this PlayerState playerState)
    {
        return playerState switch
        {
            PlayerState.TeamPreviewSelect => true,
            PlayerState.TeamPreviewLocked => false,
            PlayerState.MoveSwitchSelect => true,
            PlayerState.MoveSwitchLocked => false,
            PlayerState.FaintedSelect => true,
            PlayerState.FaintedLocked => false,
            PlayerState.ForceSwitchSelect => true,
            PlayerState.ForceSwitchLocked => false,
            PlayerState.Idle => false,
            _ => throw new ArgumentOutOfRangeException(nameof(playerState), playerState, null)
        };
    }

    public static PlayerId OpposingPlayerId(this PlayerId playerId)
    {
        return playerId switch
        {
            PlayerId.Player1 => PlayerId.Player2,
            PlayerId.Player2 => PlayerId.Player1,
            PlayerId.None => throw new ArgumentException("PlayerId cannot be 'None'", nameof(playerId)),
            _ => throw new ArgumentException("Invalid player ID", nameof(playerId))
        };
    }
}
