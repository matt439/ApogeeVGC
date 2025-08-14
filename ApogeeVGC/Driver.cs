using ApogeeVGC.Player;
using ApogeeVGC.Sim;

namespace ApogeeVGC;

internal enum DriverMode
{
    RandomVsRandom,
    ConsoleVsRandom,
    ConsoleVsConsole,
    ConsoleVsMcts,
    MctsVsRandom,
}

internal class Driver
{
    private ISimulator? Simulator { get; set; }
    private IPlayer? Player1 { get; set; }
    private IPlayer? Player2 { get; set; }

    internal void Start(DriverMode mode)
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

    internal void RunRandomTest()
    {
        if (Simulator == null)
        {
            Simulator = new Simulator();
        }
        else
        {
            Simulator.Reset();
        }
    }
}