using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Player;

public interface IPlayer
{
    //int GetInputCommand();  
    //void InputCurrentPlayerChoices(PlayerChoices choices);

    PlayerId PlayerId { get; }

    Choice GetNextChoice(Choice[] availableChoices);

    //void InputBattle(Battle battle);
}