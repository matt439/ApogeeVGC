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
        const int debugTeam1Seed = 54322;
        const int debugTeam2Seed = 67892;
        const int debugPlayer1Seed = 12348;
        const int debugPlayer2Seed = 1822;
        const int debugBattleSeed = 9881;

        RunSingleBattleDebug(
            FormatId.Gen9VgcRegulationI,
            "VGC Reg I",
            debugTeam1Seed, debugTeam2Seed,
            debugPlayer1Seed, debugPlayer2Seed,
            debugBattleSeed);
    }

    /// <summary>
    /// Runs a single Mega Evolution battle with specific seeds for debugging.
    /// Edit the seed constants in this method to reproduce specific battle
    /// scenarios from RndVsRndMegaEvaluation failures
    /// </summary>
    private void RunSingleBattleDebugMega()
    {
        // !! EDIT THESE VALUES TO DEBUG SPECIFIC BATTLES !!
        // Copy the 5 seeds from the RndVsRndMegaEvaluation exception output.
        const int debugTeam1Seed = 54322;
        const int debugTeam2Seed = 67892;
        const int debugPlayer1Seed = 12348;
        const int debugPlayer2Seed = 1822;
        const int debugBattleSeed = 9881;

        RunSingleBattleDebug(
            FormatId.Gen9VgcMega,
            "Mega Evolution",
            debugTeam1Seed, debugTeam2Seed,
            debugPlayer1Seed, debugPlayer2Seed,
            debugBattleSeed);
    }

    /// <summary>
    /// Shared implementation for single-battle debug methods.
    /// Generates teams from seeds, runs a single battle, and prints results or exception details.
    /// </summary>
    private void RunSingleBattleDebug(
        FormatId formatId,
        string formatLabel,
        int team1Seed, int team2Seed,
        int player1Seed, int player2Seed,
        int battleSeed)
    {
        const bool debug = true;

        Console.WriteLine($"[Driver] Starting Single Battle Debug ({formatLabel})");
        Console.WriteLine($"[Driver] Seeds - Team1: {team1Seed}, Team2: {team2Seed}, " +
                          $"P1: {player1Seed}, P2: {player2Seed}, Battle: {battleSeed}");
        Console.WriteLine();

        try
        {
            var team1 = new RandomTeamGenerator(Library, formatId, team1Seed).GenerateTeam();
            var team2 = new RandomTeamGenerator(Library, formatId, team2Seed).GenerateTeam();

            PlayerOptions player1Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Debug1",
                Team = team1,
                Seed = new PrngSeed(player1Seed),
                PrintDebug = debug,
            };

            PlayerOptions player2Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Debug2",
                Team = team2,
                Seed = new PrngSeed(player2Seed),
                PrintDebug = debug,
            };

            BattleOptions battleOptions = new()
            {
                Id = formatId,
                Player1Options = player1Options,
                Player2Options = player2Options,
                Debug = debug,
                Sync = true,
                Seed = new PrngSeed(battleSeed),
                MaxTurns = 2000,
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
