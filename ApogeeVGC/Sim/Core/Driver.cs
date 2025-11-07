using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Gui;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Core;

public enum DriverMode
{
    //RandomVsRandom,
    //RandomVsRandomEvaluation,
    //RandomVsRandomEvaluationDoubles,
    GuiVsRandomSingles,
    GuiVsRandomDoubles,
    //ConsoleVsConsole,
    //ConsoleVsMcts,
    //MctsVsRandom,
    //MctsVsRandomEvaluation,
    //MctsVsRandomEvaluationDoubles,
}

public class Driver
{
    //private const double Root2 = 1.4142135623730951; // sqrt of 2
    private Library Library { get; } = new();

    //private const int RandomEvaluationNumTest = 1000;

    //private const int MctsEvaluationNumTest = 100;
    //private const int MctsMaxIterations = 10000;
    //private const double MctsExplorationParameter = Root2;
    //private readonly int? _mctsMaxTimer = null; // in milliseconds

    //private static readonly int NumThreads = Environment.ProcessorCount;

    //private const int PlayerRandom1Seed = 439;
    private const int PlayerRandom2Seed = 1818;

    public void Start(DriverMode mode)
    {
        switch (mode)
        {
            case DriverMode.GuiVsRandomSingles:
                RunGuiVsRandomSinglesTest(); // No longer async
                break;
            case DriverMode.GuiVsRandomDoubles:
                RunGuiVsRandomDoublesTest(); // No longer async
                break;
            default:
                throw new NotImplementedException($"Driver mode {mode} is not implemented.");
        }
    }

    private void RunGuiVsRandomSinglesTest() // Changed from async Task
    {
        // Create BattleGame instance on main thread (required by MonoGame)
        using var battleGame = new BattleGame();
        Console.WriteLine($"[Driver] BattleGame created, instance: {battleGame.GetHashCode()}");

        PlayerOptions player1Options = new()
        {
            Type = PlayerType.Gui,
            Name = "Matt",
            Team = TeamGenerator.GenerateTestTeam(Library),
            GuiWindow = battleGame, // Pass BattleGame to PlayerGui
        };

        PlayerOptions player2Options = new()
        {
            Type = PlayerType.Random,
            Name = "Random",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomSingles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = true,
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Simulator created");

        // Start the battle using BattleGame's deferred start mechanism
        // This will queue the battle to start after LoadContent()
        battleGame.StartBattle(Library, battleOptions, simulator);
        Console.WriteLine("[Driver] Battle queued, calling battleGame.Run()");

        // Run MonoGame on main thread - this blocks until game window closes
        // LoadContent() will be called during initialization and will start the queued battle
        battleGame.Run();

        Console.WriteLine("[Driver] BattleGame.Run() exited");
    }

    private void RunGuiVsRandomDoublesTest() // Changed from async Task
    {
        throw new NotImplementedException();
    }
}