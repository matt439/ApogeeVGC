using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

public enum SimState
{
    Initialised,
    Running,
    Player1Win,
    Player2Win,
}

public interface ISimulator
{
    Battle Battle { get; }
    PlayerChoices PlayerChoices { get; }
    SimState State { get; }

    void InputCommand(Choice choice);
    void Start();
}

public class Simulator : ISimulator
{
    public Battle Battle { get; }
    public PlayerChoices PlayerChoices { get; }
    public SimState State { get; }
    public void InputCommand(Choice choice)
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }
}