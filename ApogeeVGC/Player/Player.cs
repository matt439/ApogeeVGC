using ApogeeVGC.Sim;

namespace ApogeeVGC.Player;

public enum PlayerId
{
    Player1,
    Player2,
}

public enum Choice
{
    Move1,
    Move2,
    Move3,
    Move4,

    Switch1,
    Switch2,
    Switch3,
    Switch4,
    Switch5,
    Switch6,

    Select1,
    Select2,
    Select3,
    Select4,
    Select5,
    Select6,

    Quit,
    None,
    Invalid,
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

    Choice GetNextChoice(IReadOnlyList<Choice> availableChoices);

    //void InputBattle(Battle battle);
}