using ApogeeVGC.Gui;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    private void RunConsoleVsRandom(FormatId formatId)
    {
        Console.WriteLine($"[Driver] Starting Console vs Random ({Library.Formats[formatId].Name})");
        const bool debug = true;

        var team1 = new RandomTeamGenerator(Library, formatId, Team1EvalSeed).GenerateTeam();
        var team2 = new RandomTeamGenerator(Library, formatId, Team2EvalSeed).GenerateTeam();

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Console,
            Name = "Player",
            Team = team1,
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random",
            Team = team2,
            Seed = new PrngSeed(PlayerRandom2Seed),
            PrintDebug = debug,
        };

        BattleOptions battleOptions = new()
        {
            Id = formatId,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = debug,
            Sync = false,
            Seed = new PrngSeed(BattleSeed),
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Simulator created");

        SimulatorResult result =
            simulator.RunAsync(Library, battleOptions, printDebug: debug).Result;

        Console.WriteLine($"[Driver] Battle completed with result: {result}");
    }

    private void RunGuiVsRandom(FormatId formatId)
    {
        Console.WriteLine($"[Driver] Starting GUI vs Random ({Library.Formats[formatId].Name})");
        const bool debug = true;

        var team1 = new RandomTeamGenerator(Library, formatId, Team1EvalSeed).GenerateTeam();
        var team2 = new RandomTeamGenerator(Library, formatId, Team2EvalSeed).GenerateTeam();

        using var game = new BattleGame();

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Gui,
            Name = "Player",
            Team = team1,
            GuiWindow = game,
            GuiChoiceCoordinator = game.GetChoiceCoordinator(),
            PrintDebug = debug,
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random",
            Team = team2,
            Seed = new PrngSeed(PlayerRandom2Seed),
            PrintDebug = debug,
        };

        BattleOptions battleOptions = new()
        {
            Id = formatId,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = debug,
            Sync = false,
            Seed = new PrngSeed(BattleSeed),
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Simulator created");

        game.StartBattle(Library, battleOptions, simulator);
        Console.WriteLine("[Driver] Battle start queued");

        game.Run();

        Console.WriteLine("[Driver] Battle completed");
    }
}
