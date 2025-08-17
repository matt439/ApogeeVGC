using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

//public enum SimState
//{
//    RequestingPlayer1Input,
//    RequestingPlayer2Input,
//    RequestingBothPlayersInput,
//    Player1Win,
//    Player2Win,
//}

//public interface ISimulator
//{
//    Battle? Battle { get; }
//    Dictionary<PlayerId, PlayerChoices> PlayerChoices { get; }
//    SimState RequestState { get; }

//    void PerformCommand(Choice choice);
//    void Start();
//}

public class Simulator
{
    public required Battle Battle { get; init; }
    public required IPlayer Player1 { get; init; }
    public required IPlayer Player2 { get; init; }
    //public PlayerChoices? PlayerChoices { get; }
    //public SimState RequestState { get; }

    public void Run()
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

        Console.WriteLine("Battle finished.");
        switch (battleState)
        {
            case BattleRequestState.Player1Win:
                Console.WriteLine("Player 1 wins!");
                break;
            case BattleRequestState.Player2Win:
                Console.WriteLine("Player 2 wins!");
                break;
            case BattleRequestState.RequestingPlayer1Input:
            case BattleRequestState.RequestingPlayer2Input:
            case BattleRequestState.RequestingBothPlayersInput:
            default:
                Console.WriteLine("Battle ended unexpectedly.");
                break;
        }
    }

    //private SimulatorOutput PerformCommand(SimulatorInput input)
    //{
    //    PlayerId winner = Battle.IsWinner();
    //    if (winner != PlayerId.None)
    //    {
    //        return new SimulatorOutput
    //        {
    //            RequestState = winner == PlayerId.Player1 ? SimState.Player1Win : SimState.Player2Win,
    //            Player1Choices = [],
    //            Player2Choices = []
    //        };
    //    }

    //    BattleRequestState battleState = Battle.RequestState;
    //    switch (battleState)
    //    {
    //        case BattleRequestState.WaitingForPlayer1:
    //            Battle.ApplyChoice(PlayerId.Player1, input.Player1Choice);
    //            return new SimulatorOutput()
    //            {
    //                RequestState = SimState.RequestingPlayer1Input,
    //                Player1Choices = Battle.GetAvailableChoices(PlayerId.Player1),
    //                Player2Choices = []
    //            };
    //        case BattleRequestState.WaitingForPlayer2:
    //            Battle.ApplyChoice(PlayerId.Player2, input.Player2Choice);
    //            return new SimulatorOutput()
    //            {
    //                RequestState = SimState.RequestingPlayer2Input,
    //                Player1Choices = [],
    //                Player2Choices = Battle.GetAvailableChoices(PlayerId.Player2)
    //            };
    //        case BattleRequestState.WaitingForBothPlayers:
    //            Battle.ApplyChoices(input.Player1Choice, input.Player2Choice);
    //            return new SimulatorOutput()
    //            {
    //                RequestState = SimState.RequestingBothPlayersInput,
    //                Player1Choices = Battle.GetAvailableChoices(PlayerId.Player1),
    //                Player2Choices = Battle.GetAvailableChoices(PlayerId.Player2)
    //            };
    //        default:
    //            throw new InvalidOperationException($"Unexpected battle state: {battleState}");
    //    }
    //}

    //private SimulatorOutput Start()
    //{
    //    return new SimulatorOutput()
    //    {
    //        RequestState = SimState.RequestingBothPlayersInput,
    //        Player1Choices = Battle.GetAvailableChoices(PlayerId.Player1),
    //        Player2Choices = Battle.GetAvailableChoices(PlayerId.Player2)
    //    };
    //}
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