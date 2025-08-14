using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

public interface ISimulator
{
    PlayerChoices GetCurrentPlayerChoices();
    void InputCommand(int commandId);

    Battle GetBattle();
}

//public class Simulator : ISimulator
//{

//}