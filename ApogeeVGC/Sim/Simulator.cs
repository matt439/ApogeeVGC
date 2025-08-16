using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

public enum SimState
{
    RequestingPlayer1Input,
    RequestingPlayer2Input,
    RequestingBothPlayersInput,
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
        PlayerId winner = Battle.IsWinner();
        if (winner != PlayerId.None)
        {
            return new SimulatorOutput
            {
                State = winner == PlayerId.Player1 ? SimState.Player1Win : SimState.Player2Win,
                Player1Choices = [],
                Player2Choices = []
            };
        }

        BattleState battleState = Battle.State;
        switch (battleState)
        {
            case BattleState.WaitingForPlayer1:
                Battle.ApplyChoice(PlayerId.Player1, input.Player1Choice);
                return new SimulatorOutput()
                {
                    State = SimState.RequestingPlayer1Input,
                    Player1Choices = Battle.GetAvailableChoices(PlayerId.Player1),
                    Player2Choices = []
                };
            case BattleState.WaitingForPlayer2:
                Battle.ApplyChoice(PlayerId.Player2, input.Player2Choice);
                return new SimulatorOutput()
                {
                    State = SimState.RequestingPlayer2Input,
                    Player1Choices = [],
                    Player2Choices = Battle.GetAvailableChoices(PlayerId.Player2)
                };
            case BattleState.WaitingForBothPlayers:
                Battle.ApplyChoices(input.Player1Choice, input.Player2Choice);
                return new SimulatorOutput()
                {
                    State = SimState.RequestingBothPlayersInput,
                    Player1Choices = Battle.GetAvailableChoices(PlayerId.Player1),
                    Player2Choices = Battle.GetAvailableChoices(PlayerId.Player2)
                };
            default:
                throw new InvalidOperationException($"Unexpected battle state: {battleState}");
        }
    }

    public SimulatorOutput Start()
    {
        return new SimulatorOutput()
        {
            State = SimState.RequestingBothPlayersInput,
            Player1Choices = Battle.GetAvailableChoices(PlayerId.Player1),
            Player2Choices = Battle.GetAvailableChoices(PlayerId.Player2)
        };
    }
}

public record SimulatorOutput
{
    public SimState State { get; init; }
    public required Choice[] Player1Choices { get; init; }
    public required Choice[] Player2Choices { get; init; }
}

public record SimulatorInput
{
    public Choice Player1Choice { get; init; }
    public Choice Player2Choice { get; init; }
}