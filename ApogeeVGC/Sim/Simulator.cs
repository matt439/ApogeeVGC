using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

//public interface ISimulator
//{
//    Battle? Battle { get; }
//    Dictionary<PlayerId, PlayerChoices> PlayerChoices { get; }
//    SimState RequestState { get; }

//    void PerformCommand(Choice choice);
//    void Start();
//}

public enum SimulatorResult
{
    Player1Win,
    Player2Win,
}

public class Simulator
{
    public required Battle Battle { get; init; }
    public required IPlayer Player1 { get; init; }
    public required IPlayer Player2 { get; init; }
    public bool PrintDebug { get; set; }

    public SimulatorResult Run()
    {
        BattleRequestState battleState = Battle.GetRequestState();

        while (battleState != BattleRequestState.Player1Win &&
               battleState != BattleRequestState.Player2Win)
        {
            Choice player1Choice;
            Choice player2Choice;

            switch (battleState)
            {
                case BattleRequestState.RequestingPlayer1Input:
                    player1Choice = Player1.GetNextChoice(Battle.GetAvailableChoices(PlayerId.Player1));
                    Battle.SubmitChoice(PlayerId.Player1, player1Choice);
                    break;
                case BattleRequestState.RequestingPlayer2Input:
                    player2Choice = Player2.GetNextChoice(Battle.GetAvailableChoices(PlayerId.Player2));
                    Battle.SubmitChoice(PlayerId.Player2, player2Choice);
                    break;
                case BattleRequestState.RequestingBothPlayersInput:
                    player1Choice = Player1.GetNextChoice(Battle.GetAvailableChoices(PlayerId.Player1));
                    player2Choice = Player2.GetNextChoice(Battle.GetAvailableChoices(PlayerId.Player2));
                    Battle.SubmitChoice(PlayerId.Player1, player1Choice);
                    Battle.SubmitChoice(PlayerId.Player2, player2Choice);
                    break;
                case BattleRequestState.Player1Win:
                case BattleRequestState.Player2Win:
                    throw new InvalidOperationException("Battle has already ended.");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            battleState = Battle.GetRequestState();
        }

        string winner;
        SimulatorResult result;
        switch (battleState)
        {
            case BattleRequestState.Player1Win:
                winner = Battle.Side1.Team.Trainer.Name;
                result = SimulatorResult.Player1Win;
                break;
            case BattleRequestState.Player2Win:
                winner = Battle.Side2.Team.Trainer.Name;
                result = SimulatorResult.Player2Win;
                break;
            case BattleRequestState.RequestingPlayer1Input:
            case BattleRequestState.RequestingPlayer2Input:
            case BattleRequestState.RequestingBothPlayersInput:
                throw new InvalidOperationException("Battle has not ended yet.");
            default:
                throw new ArgumentOutOfRangeException(nameof(battleState), battleState,
                    "Unexpected battle state.");
        }

        if (PrintDebug)
        {
            UiGenerator.PrintBattleEnd(winner);
        }
        return result;
    }
}

//public record SimulatorOutput
//{
//    public SimState RequestState { get; init; }
//    public required Choice[] Player1Choices { get; init; }
//    public required Choice[] Player2Choices { get; init; }
//}

//public record SimulatorInput
//{
//    public Choice Player1Choice { get; init; }
//    public Choice Player2Choice { get; init; }
//}