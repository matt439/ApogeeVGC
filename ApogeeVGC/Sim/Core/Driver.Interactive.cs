using ApogeeVGC.Data;
using ApogeeVGC.Gui;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    private void RunConsoleVsRandomSinglesTest()
    {
        const bool debug = true;

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Console,
            Name = "Matt",
            Team = TeamGenerator.GenerateTestTeam(Library),
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
            PrintDebug = debug,
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomSingles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = debug,
            Sync = false,
            Seed = new PrngSeed(BattleSeed),
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Simulator created");

        // Run the battle synchronously on the main thread
        SimulatorResult result =
            simulator.RunAsync(Library, battleOptions, printDebug: debug).Result;

        Console.WriteLine($"[Driver] Battle completed with result: {result}");
    }

    private void RunConsoleVsRandomDoublesTest()
    {
        const bool debug = true;

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Console,
            Name = "Matt",
            Team = TeamGenerator.GenerateTestTeam(Library),
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
            PrintDebug = debug,
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomDoubles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = debug,
            Sync = false,
            Seed = new PrngSeed(BattleSeed),
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Simulator created");

        // Run the battle asynchronously on the main thread
        SimulatorResult result =
            simulator.RunAsync(Library, battleOptions, printDebug: debug).Result;

        Console.WriteLine($"[Driver] Battle completed with result: {result}");
    }

    private void RunGuiVsRandomSinglesTest()
    {
        Console.WriteLine("[Driver] Starting GUI vs Random Singles test");
        const bool debug = true;

        // Create the MonoGame window
        using var game = new BattleGame();

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Gui,
            Name = "Matt",
            Team = TeamGenerator.GenerateTestTeam(Library),
            GuiWindow = game,
            GuiChoiceCoordinator = game.GetChoiceCoordinator(),
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
            PrintDebug = debug,
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomSingles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = debug,
            Sync = false,
            Seed = new PrngSeed(BattleSeed),
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Simulator created");

        // Start the battle (will defer until LoadContent completes)
        game.StartBattle(Library, battleOptions, simulator);
        Console.WriteLine("[Driver] Battle start queued");

        // Run MonoGame on main thread (blocking call)
        game.Run();

        Console.WriteLine("[Driver] Battle completed");
    }

    private void RunGuiVsRandomDoublesTest()
    {
        Console.WriteLine("[Driver] Starting GUI vs Random Doubles test");
        const bool debug = true;

        // Create the MonoGame window
        using var game = new BattleGame();

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Gui,
            Name = "Matt",
            Team = TeamGenerator.GenerateTestTeam(Library),
            GuiWindow = game,
            GuiChoiceCoordinator = game.GetChoiceCoordinator(),
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
            PrintDebug = debug,
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomDoubles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = debug,
            Sync = false,
            Seed = new PrngSeed(BattleSeed),
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Simulator created");

        // Start the battle (will defer until LoadContent completes)
        game.StartBattle(Library, battleOptions, simulator);
        Console.WriteLine("[Driver] Battle start queued");

        // Run MonoGame on main thread (blocking call)
        game.Run();

        Console.WriteLine("[Driver] Battle completed");
    }
}
