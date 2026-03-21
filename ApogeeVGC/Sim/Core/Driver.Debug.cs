using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.PokemonClasses;
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
        const int debugTeam1Seed = 1425057;
        const int debugTeam2Seed = 1438627;
        const int debugPlayer1Seed = 1383083;
        const int debugPlayer2Seed = 1372557;
        const int debugBattleSeed = 1380616;

        string formatLabel = Library.Formats[formatId].Name;
        const bool debug = true;

        Console.WriteLine($"[Driver] Starting Single Battle Debug ({formatLabel})");
        Console.WriteLine($"[Driver] Seeds - Team1: {debugTeam1Seed}, Team2: {debugTeam2Seed}, " +
                          $"P1: {debugPlayer1Seed}, P2: {debugPlayer2Seed}, Battle: {debugBattleSeed}");
        Console.WriteLine();

        try
        {
            List<PokemonSet> team1 = new RandomTeamGenerator(Library, formatId, debugTeam1Seed).GenerateTeam();
            List<PokemonSet> team2 = new RandomTeamGenerator(Library, formatId, debugTeam2Seed).GenerateTeam();

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

            SimulatorSync simulator = new();
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
        if (WaitForInput) Console.ReadLine();
    }

    /// <summary>
    /// Runs a single MCTS-DL vs Random battle with verbose decision logging.
    /// At every decision point, prints all legal actions with DL policy priors,
    /// MCTS visit counts, Q-values, and the final selected action.
    /// Edit the seed constants below to reproduce specific battle scenarios.
    /// </summary>
    private void RunMctsDlSingleBattleDebug(FormatId formatId)
    {
        // !! EDIT THESE VALUES TO DEBUG SPECIFIC BATTLES !!
        const int debugTeam1Seed = 57357;
        const int debugTeam2Seed = 70927;
        const int debugPlayer1Seed = 15383;
        const int debugPlayer2Seed = 4857;
        const int debugBattleSeed = 12916;
        const int mctsIterations = 10_000;

        string formatLabel = Library.Formats[formatId].Name;
        const bool debug = true;

        Console.WriteLine($"[Driver] Starting MCTS-DL Single Battle Debug ({formatLabel})");
        Console.WriteLine($"[Driver] Seeds - Team1: {debugTeam1Seed}, Team2: {debugTeam2Seed}, " +
                          $"P1: {debugPlayer1Seed}, P2: {debugPlayer2Seed}, Battle: {debugBattleSeed}");
        Console.WriteLine($"[Driver] MCTS iterations per search: {mctsIterations}");
        Console.WriteLine();

        // Initialize MCTS resources
        if (!File.Exists(MctsModelPath))
        {
            Console.WriteLine($"[Driver] ERROR: MCTS model not found at: {MctsModelPath}");
            Console.WriteLine("[Driver] Export the model to ONNX format using Tools/DLModel/export_onnx.py");
            return;
        }

        if (!File.Exists(MctsVocabPath))
        {
            Console.WriteLine($"[Driver] ERROR: MCTS vocab not found at: {MctsVocabPath}");
            return;
        }

        MctsConfig mctsConfig = new()
        {
            NumIterations = mctsIterations,
        };

        MctsResources.Initialize(MctsModelPath, MctsVocabPath, Library, mctsConfig,
            teamPreviewModelPath: MctsTeamPreviewModelPath);
        Console.WriteLine("[Driver] MCTS resources initialized");
        Console.WriteLine();

        try
        {
            List<PokemonSet> team1 = new RandomTeamGenerator(Library, formatId, debugTeam1Seed).GenerateTeam();
            List<PokemonSet> team2 = new RandomTeamGenerator(Library, formatId, debugTeam2Seed).GenerateTeam();

            PlayerOptions player1Options = new()
            {
                Type = Player.PlayerType.MctsDL,
                Name = "MCTS-DL",
                Team = team1,
                Seed = new PrngSeed(debugPlayer1Seed),
                PrintDebug = debug,
                MctsConfig = mctsConfig,
            };

            PlayerOptions player2Options = new()
            {
                Type = Player.PlayerType.Random,
                Name = "Random",
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

            SimulatorSync simulator = new();
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
        finally
        {
            MctsResources.Shutdown();
        }

        Console.WriteLine();
        Console.WriteLine("Press Enter key to exit...");
        if (WaitForInput) Console.ReadLine();
    }
}
