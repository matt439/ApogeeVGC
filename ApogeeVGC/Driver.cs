using ApogeeVGC.Data;
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
    private Library Library { get; } = new();
    private Simulator? Simulator { get; set; }
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
                RunConsoleVsRandomTest();
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
        Simulator = new Simulator
        {
            Battle = BattleGenerator.GenerateTestBattle(Library)
        };

        Player1 = new PlayerRandom(PlayerId.Player1, Simulator.Battle);
        Player2 = new PlayerRandom(PlayerId.Player2, Simulator.Battle);

        SimulatorOutput output = Simulator.Start();

        while (output.State != SimState.Player1Win && output.State != SimState.Player2Win)
        {
            // Get choices from players
            Choice player1Choice = Player1.GetNextChoice(output.Player1Choices);
            Choice player2Choice = Player2.GetNextChoice(output.Player2Choices);
            // Create input for the simulator
            SimulatorInput input = new()
            {
                Player1Choice = player1Choice,
                Player2Choice = player2Choice
            };
            // Perform the command in the simulator
            output = Simulator.PerformCommand(input);
            // Output the current state of the battle


            Console.Write($"Player 1 chose: {player1Choice.GetChoiceName()}, ");
            Console.WriteLine($"Player 2 chose: {player2Choice.GetChoiceName()}\n");
        }

        Console.WriteLine("Battle finished.");
        switch (output.State)
        {
            case SimState.Player1Win:
                Console.WriteLine("Player 1 wins!");
                break;
            case SimState.Player2Win:
                Console.WriteLine("Player 2 wins!");
                break;
            case SimState.Running:
            default:
                Console.WriteLine("Battle ended unexpectedly.");
                break;
        }
    }

    private void RunConsoleVsRandomTest()
    {
        Simulator = new Simulator
        {
            Battle = BattleGenerator.GenerateTestBattle(Library)
        };

        Player1 = new PlayerConsole(PlayerId.Player1, Simulator.Battle);
        Player2 = new PlayerRandom(PlayerId.Player2, Simulator.Battle);

        SimulatorOutput output = Simulator.Start();

        while (output.State != SimState.Player1Win && output.State != SimState.Player2Win)
        {
            // Get choices from players
            Choice player1Choice = Player1.GetNextChoice(output.Player1Choices);
            Choice player2Choice = Player2.GetNextChoice(output.Player2Choices);
            // Create input for the simulator
            SimulatorInput input = new()
            {
                Player1Choice = player1Choice,
                Player2Choice = player2Choice
            };
            // Perform the command in the simulator
            output = Simulator.PerformCommand(input);
            // Output the current state of the battle

            Console.Write($"Player 1 chose: {player1Choice.GetChoiceName()}, ");
            Console.WriteLine($"Player 2 chose: {player2Choice.GetChoiceName()}\n");
        }

        Console.WriteLine("Battle finished.");
        switch (output.State)
        {
            case SimState.Player1Win:
                Console.WriteLine("Player 1 wins!");
                break;
            case SimState.Player2Win:
                Console.WriteLine("Player 2 wins!");
                break;
            case SimState.Running:
            default:
                Console.WriteLine("Battle ended unexpectedly.");
                break;
        }
    }
}