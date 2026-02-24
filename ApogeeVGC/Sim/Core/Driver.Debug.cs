using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    /// <summary>
    /// Runs a single VGC Reg I battle with specific seeds for debugging.
    /// Edit the seed constants in this method to reproduce specific battle
    /// scenarios from RndVsRndVgcRegIEvaluation failures
    /// </summary>
    private void RunSingleBattleDebugVgcRegI()
    {
        // !! EDIT THESE VALUES TO DEBUG SPECIFIC BATTLES !!
        // Copy the 5 seeds from the RndVsRndVgcRegIEvaluation exception output.
        const int debugTeam1Seed = 82822;
        const int debugTeam2Seed = 96392;
        const int debugPlayer1Seed = 40848;
        const int debugPlayer2Seed = 30322;
        const int debugBattleSeed = 38381;
        const bool debug = true;

        Console.WriteLine("[Driver] Starting Single Battle Debug (VGC Reg I)");
        Console.WriteLine($"[Driver] Seeds - Team1: {debugTeam1Seed}, Team2: {debugTeam2Seed}, " +
                          $"P1: {debugPlayer1Seed}, P2: {debugPlayer2Seed}, Battle: {debugBattleSeed}");
        Console.WriteLine();

        try
        {
            var team1Generator = new RandomTeamGenerator(Library, FormatId.Gen9VgcRegulationI, debugTeam1Seed);
            var team2Generator = new RandomTeamGenerator(Library, FormatId.Gen9VgcRegulationI, debugTeam2Seed);

            var team1 = team1Generator.GenerateTeam();
            var team2 = team2Generator.GenerateTeam();

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
                Id = FormatId.Gen9VgcRegulationI,
                Player1Options = player1Options,
                Player2Options = player2Options,
                Debug = debug,
                Sync = true,
                Seed = new PrngSeed(debugBattleSeed),
                MaxTurns = 1000,
            };

            var simulator = new SyncSimulator();
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
