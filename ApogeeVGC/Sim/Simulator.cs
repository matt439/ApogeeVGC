using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

public enum SimState
{
    Running,
    Player1Win,
    Player2Win,
}

//public interface ISimulator
//{
//    Battle? Battle { get; }
//    Dictionary<PlayerId, PlayerChoices> PlayerChoices { get; }
//    SimState State { get; }

//    void PerformCommand(Choice choice);
//    void Start();
//}

public class Simulator
{
    public required Battle Battle { get; init; }
    //public PlayerChoices? PlayerChoices { get; }
    //public SimState State { get; }

    public SimulatorOutput PerformCommand(SimulatorInput input)
    {
        throw new NotImplementedException();
    }

    public SimulatorOutput Start()
    {
        throw new NotImplementedException();
    }

    private List<Choice> GetAvailableChoices(PlayerId playerId)
    {
        // This method should return the available choices for the given player.
        // For now, we return an empty list as a placeholder.
        return new List<Choice>();
    }

    private PlayerId IsWinner()
    {
        throw new NotImplementedException("This method should determine if there is a winner.");
    }


}

public record SimulatorOutput
{
    public SimState State { get; init; }
    public required IReadOnlyList<Choice> Player1Choices { get; init; }
    public required IReadOnlyList<Choice> Player2Choices { get; init; }
}

public record SimulatorInput
{
    public Choice Player1Choice { get; init; }
    public Choice Player2Choice { get; init; }
}