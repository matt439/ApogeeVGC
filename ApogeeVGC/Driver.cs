using ApogeeVGC.Player;
using ApogeeVGC.Sim;

namespace ApogeeVGC;

public enum DriverMode
{
    RandomVsRandom,
    ConsoleVsRandom,
    ConsoleVsConsole,
    ConsoleVsMcts,
    MctsVsRandom,
}

public class Driver
{
    private ISimulator? Simulator { get; set; }
    private IPlayer? Player1 { get; set; }
    private IPlayer? Player2 { get; set; }

    public void Start(DriverMode mode)
    {
        switch (mode)
        {
            case DriverMode.RandomVsRandom:
                RunRandomTest();
                break;
            case DriverMode.ConsoleVsRandom:
                throw new NotImplementedException("Console vs Random mode is not implemented yet.");
                break;
            case DriverMode.ConsoleVsConsole:
                throw new NotImplementedException("Console vs Console mode is not implemented yet.");
                break;
            case DriverMode.ConsoleVsMcts:
                throw new NotImplementedException("Console vs MCTS mode is not implemented yet.");
                break;
            case DriverMode.MctsVsRandom:
                throw new NotImplementedException("MCTS vs Random mode is not implemented yet.");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private void RunRandomTest()
    {
        Simulator = new Simulator();
        Player1 = new PlayerRandom(PlayerId.Player1);
        Player2 = new PlayerRandom(PlayerId.Player2);

        Simulator.Start();

        while (Simulator.State != SimState.Player1Win && Simulator.State != SimState.Player2Win)
        {
            PlayerChoices currentChoices = Simulator.PlayerChoices;
            if (currentChoices == null)
            {
                throw new InvalidOperationException("Current player choices cannot be null.");
            }
            switch (currentChoices.PlayerId)
            {
                case PlayerId.Player1:
                {
                    Choice command = Player1.GetNextChoice(currentChoices);
                    Simulator.InputCommand(command);

                    break;
                }
                case PlayerId.Player2:
                {
                    Choice command = Player2.GetNextChoice(currentChoices);
                    Simulator.InputCommand(command);

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Console.WriteLine("Battle finished.");
        Console.WriteLine($"Player {Simulator.State} wins");
    }
}