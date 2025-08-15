using ApogeeVGC.Sim;

namespace ApogeeVGC.Player;

public class PlayerRandom(PlayerId playerId, Battle battle) : IPlayer
{
    public PlayerId PlayerId { get; } = playerId;
    public Battle Battle { get; } = battle;

    public Choice GetNextChoice(IReadOnlyList<Choice> availableChoices)
    {
        // Select a random choice from the available choices
        if (availableChoices.Count == 0)
        {
            return Choice.Invalid; // No choices available
        }
        int randomIndex = new Random().Next(availableChoices.Count);
        return availableChoices[randomIndex];
    }

    //public void InputBattle(Battle battle)
    //{
    //    throw new NotImplementedException();
    //}
}