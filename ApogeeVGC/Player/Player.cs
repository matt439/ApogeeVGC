using ApogeeVGC.Sim;

namespace ApogeeVGC.Player;

public enum PlayerId
{
    Player1 = 1,
    Player2 = 2,
}

public enum Choice
{
    Move1 = 1,
    Move2 = 2,
    Move3 = 3,
    Move4 = 4,
    Switch1 = 5,
    Switch2 = 6,
    Switch3 = 7,
    Switch4 = 8,
    Switch5 = 9,
    Quit = 10,
    None = 11,
    Invalid = 12,
}

public class ChoiceDetails
{

}

public class PlayerChoices
{
    public PlayerId PlayerId { get; init; }
    public Dictionary<Choice, ChoiceDetails> Choices { get; init; } = [];
}


public interface IPlayer
{
    //int GetInputCommand();  
    //void InputCurrentPlayerChoices(PlayerChoices choices);

    PlayerId PlayerId { get; }

    Choice GetNextChoice(PlayerChoices availableChoices);

    void InputBattle(Battle battle);
}