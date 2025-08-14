namespace ApogeeVGC.Player;

public class Choice
{

}

public class PlayerChoices
{
    public int PlayerId { get; set; }
    public List<Choice> Choices { get; set; } = [];
}


public interface IPlayer
{
    //int GetInputCommand();  
    //void InputCurrentPlayerChoices(PlayerChoices choices);

    int GetNextCommand(PlayerChoices choices);

    //void GetBattle(Battle battle);
}