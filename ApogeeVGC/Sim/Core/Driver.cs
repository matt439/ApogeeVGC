using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Core;

public class Driver
{
    private Library Library { get; } = new();

    private const int PlayerRandom1Seed = 12345;
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
            case DriverMode.RandomVsRandomSingles:
                RunRandomVsRandomSinglesTest();
                break;
            case DriverMode.AsyncRandomVsRandomSingles:
                RunAsyncRandomVsRandomSinglesTest();
                break;
            default:
                throw new NotImplementedException($"Driver mode {mode} is not implemented.");
        }
    }

    private void RunGuiVsRandomSinglesTest()
    {
        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Console, // Use Player namespace
            Name = "Matt",
            Team = TeamGenerator.GenerateTestTeam(Library),
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random, // Use Player namespace
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

        // Run the battle synchronously on the main thread
        var result = simulator.RunAsync(Library, battleOptions, printDebug: true).Result;

        Console.WriteLine($"[Driver] Battle completed with result: {result}");
    }

    private void RunGuiVsRandomDoublesTest()
    {
        throw new NotImplementedException();
    }

    private void RunRandomVsRandomSinglesTest()
    {
        Console.WriteLine("[Driver] Starting Random vs Random Singles test (SYNCHRONOUS)");

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random 1",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom1Seed),
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random 2",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomSingles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = true,
            Sync = true, // Enable synchronous mode
        };

        var simulator = new SyncSimulator();
        Console.WriteLine("[Driver] SyncSimulator created");

        // Run the battle completely synchronously - no async/await needed!
        var result = simulator.Run(Library, battleOptions, printDebug: true);

        Console.WriteLine($"[Driver] Battle completed with result: {result}");

        // Show final statistics
        Console.WriteLine("\n=== Battle Complete ===");
        Console.WriteLine($"Winner: {result}");
    }

    private void RunAsyncRandomVsRandomSinglesTest()
    {
        Console.WriteLine("[Driver] Starting Random vs Random Singles test (ASYNCHRONOUS)");

        PlayerOptions player1Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random 1",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom1Seed),
        };

        PlayerOptions player2Options = new()
        {
            Type = Player.PlayerType.Random,
            Name = "Random 2",
            Team = TeamGenerator.GenerateTestTeam(Library),
            Seed = new PrngSeed(PlayerRandom2Seed),
        };

        BattleOptions battleOptions = new()
        {
            Id = FormatId.CustomSingles,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = true,
            Sync = false, // Ensure this is false for async
        };

        var simulator = new Simulator();
        Console.WriteLine("[Driver] Async Simulator created");

        // Run the battle asynchronously
        var result = simulator.RunAsync(Library, battleOptions, printDebug: true).Result;

        Console.WriteLine($"[Driver] Battle completed with result: {result}");

        // Show final statistics
        Console.WriteLine("\n=== Battle Complete ===");
        Console.WriteLine($"Winner: {result}");
    }
}