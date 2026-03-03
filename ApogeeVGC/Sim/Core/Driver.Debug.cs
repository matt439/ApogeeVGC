using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    /// <summary>
    /// Runs a single battle with specific seeds for debugging.
    /// Edit the seed constants below to reproduce specific battle
    /// scenarios from evaluation failures.
    /// </summary>
    private void RunSingleBattleDebug(FormatId formatId)
    {
        // !! EDIT THESE VALUES TO DEBUG SPECIFIC BATTLES !!
        // Copy the 5 seeds from the evaluation exception output.
        const int debugTeam1Seed = 761837;
        const int debugTeam2Seed = 775407;
        const int debugPlayer1Seed = 719863;
        const int debugPlayer2Seed = 709337;
        const int debugBattleSeed = 717396;

        string formatLabel = Library.Formats[formatId].Name;
        const bool debug = true;

        Console.WriteLine($"[Driver] Starting Single Battle Debug ({formatLabel})");
        Console.WriteLine($"[Driver] Seeds - Team1: {debugTeam1Seed}, Team2: {debugTeam2Seed}, " +
                          $"P1: {debugPlayer1Seed}, P2: {debugPlayer2Seed}, Battle: {debugBattleSeed}");
        Console.WriteLine();

        try
        {
            var team1 = new RandomTeamGenerator(Library, formatId, debugTeam1Seed).GenerateTeam();
            var team2 = new RandomTeamGenerator(Library, formatId, debugTeam2Seed).GenerateTeam();

            PlayerOptions player1Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Debug1",
                Team = team1,
                Seed = new PrngSeed(debugPlayer1Seed),
                PrintDebug = debug,
            };

            PlayerOptions player2Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Debug2",
                Team = team2,
                Seed = new PrngSeed(debugPlayer2Seed),
                PrintDebug = debug,
            };

            BattleOptions battleOptions = new()
            {
                Id = formatId,
                Player1Options = player1Options,
                Player2Options = player2Options,
                Debug = debug,
                Sync = true,
                Seed = new PrngSeed(debugBattleSeed),
                MaxTurns = 2000,
            };

            var simulator = new SimulatorSync();
            SimulatorResult result = simulator.Run(Library, battleOptions, printDebug: debug);

            Console.WriteLine();
            Console.WriteLine("=== Battle Result ===");
            Console.WriteLine($"Winner: {result}");
            Console.WriteLine($"Turns: {simulator.Battle?.Turn ?? 0}");
            Console.WriteLine("Battle completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("=== Battle Failed ===");
            Console.WriteLine($"Exception Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Stack Trace:");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine();
        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
    }
}
