using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Player;

public interface IPlayer
{
    //int GetInputCommand();  
    //void InputCurrentPlayerChoices(PlayerChoices choices);

    PlayerId PlayerId { get; }

    BattleChoice GetNextChoice(BattleChoice[] availableChoices);

    //void InputBattle(Battle battle);
}