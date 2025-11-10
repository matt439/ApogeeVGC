using ApogeeVGC.Data;
using ApogeeVGC.Gui;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Core;

public class Driver
{
    private Library Library { get; } = new();

    private const int PlayerRandom2Seed = 1818;

    public void Start(DriverMode mode)
    {
        switch (mode)
        {
            case DriverMode.GuiVsRandomSingles:
                RunGuiVsRandomSinglesTest();
                break;
            case DriverMode.GuiVsRandomDoubles:
                RunGuiVsRandomDoublesTest();
                break;
            default:
                throw new NotImplementedException($"Driver mode {mode} is not implemented.");
        }
    }

    private void RunGuiVsRandomSinglesTest()
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

        // Start the battle asynchronously but don't await it yet
        // The battle will run in the background while MonoGame processes events
        Task<SimulatorResult> battleTask = simulator.RunAsync(Library, battleOptions, printDebug: true);

        // Set up callback for when battle completes
        battleTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Console.WriteLine($"[Driver] Battle failed: {task.Exception?.GetBaseException().Message}");
            }
            else
            {
                Console.WriteLine($"[Driver] Battle completed with result: {task.Result}");
            }
        }, TaskScheduler.Default);

        Console.WriteLine("[Driver] Starting BattleGame.Run()");

        // Run MonoGame on main thread - this blocks until game window closes
        battleGame.Run();

        Console.WriteLine("[Driver] BattleGame.Run() exited");

        // Wait for battle to complete if it's still running
        if (!battleTask.IsCompleted)
        {
            Console.WriteLine("[Driver] Waiting for battle to complete...");
            try
            {
                battleTask.Wait(TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Driver] Error waiting for battle: {ex.Message}");
            }
        }
    }

    private void RunGuiVsRandomDoublesTest()
    {
        throw new NotImplementedException();
    }
}