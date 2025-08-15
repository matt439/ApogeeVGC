using ApogeeVGC.Sim;

namespace ApogeeVGC.Player;

public class PlayerRandom(PlayerId playerId) : IPlayer
{
    public PlayerId PlayerId { get; } = playerId;


    public Choice GetNextChoice(PlayerChoices availableChoices)
    {
        throw new NotImplementedException();
    }

    public void InputBattle(Battle battle)
    {
        throw new NotImplementedException();
    }
}