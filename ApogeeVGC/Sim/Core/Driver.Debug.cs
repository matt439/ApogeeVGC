using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    /// <summary>
    /// Runs the incremental debug test to identify problematic elements.
    /// Starts with a known-good baseline and incrementally adds elements.
    /// </summary>
    private void RunIncrementalDebugTest()
    {
        Console.WriteLine("[Driver] Starting Incremental Debug Test");
        Console.WriteLine("[Driver] This mode incrementally tests elements to identify bugs.");
        Console.WriteLine();

        var runner = new IncrementalTestRunner(
            Library,
            formatId: FormatId.CustomDoubles,
            battlesPerVerification: IncrementalDebugMaxIterations,
            battleTimeoutMs: BattleTimeoutMilliseconds,
            printDebug: false,
            stateDirectory: ".");

        // Run unlimited iterations (0 = test all elements)
        IncrementalTestRunner.TestingSummary summary;
        try
        {
            summary = runner.Run(maxIterations: 0);
        }
        catch (Exception ex)
        {
            // Each battle consumes 3 consecutive seeds, so the last battle used these:
            int lastBattleSeed = runner.SeedCounter;
            int lastP2Seed = lastBattleSeed - 1;
            int lastP1Seed = lastBattleSeed - 2;

            Console.WriteLine();
            Console.WriteLine("[Driver] UNHANDLED EXCEPTION during incremental debug test!");
            Console.WriteLine($"[Driver] Exception: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"[Driver] Last seeds: P1={lastP1Seed}, P2={lastP2Seed}, Battle={lastBattleSeed}");
            Console.WriteLine("[Driver] Use these seeds in RunSingleBattleDebug to reproduce the failure.");
            throw;
        }

        Console.WriteLine();
        Console.WriteLine("Incremental debug test complete.");
        Console.WriteLine($"Total iterations: {summary.IterationsCompleted}");
        Console.WriteLine($"Failed elements: {summary.FailedMoves.Count} moves, " +
                          $"{summary.FailedAbilities.Count} abilities, " +
                          $"{summary.FailedItems.Count} items, " +
                          $"{summary.FailedSpecies.Count} species");

        Console.WriteLine();
        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
    }

    /// <summary>
    /// Runs a single battle with specific seeds for debugging.
    /// Edit the seed constants and testing element in this method to reproduce
    /// specific battle scenarios from IncrementalDebugTest failures.
    /// </summary>
    private void RunSingleBattleDebug()
    {
        // !! EDIT THESE VALUES TO DEBUG SPECIFIC BATTLES !!
        // Copy the seeds and testing element from the IncrementalDebugTest failure output.
        const int debugPlayer1Seed = 52;
        const int debugPlayer2Seed = debugPlayer1Seed + 1;
        const int debugBattleSeed = debugPlayer2Seed + 1;
        const bool debug = true;

        // Set the element that was being tested when the failure occurred.
        // Only ONE of these should be set; the rest must remain None/default.
        // Copy the value from the "Testing Moves/Abilities/Items:" output line.
        const MoveId debugTestingMove = MoveId.LunarDance;
        const AbilityId debugTestingAbility = AbilityId.None;
        const ItemId debugTestingItem = ItemId.None;
        const SpecieId debugTestingSpecies = default; // SpecieId has no None; default (Bulbasaur) means "not set"

        Console.WriteLine("[Driver] Starting Single Battle Debug");
        Console.WriteLine($"[Driver] Seeds - Player1: {debugPlayer1Seed}, Player2: {debugPlayer2Seed}, Battle: {debugBattleSeed}");
        Console.WriteLine();

        // Load the saved pools state (after failure, the testing element is in the Failed set).
        // We restore it to Testing so Available = Allowed + Testing matches the failing battle.
        var pools = new DebugElementPools();
        if (!pools.LoadState())
        {
            Console.WriteLine("WARNING: No debug-element-pools-state.json found!");
            Console.WriteLine("This file is required to recreate battles from IncrementalDebugTest.");
            Console.WriteLine("Make sure the state file exists in the working directory.");
            Console.WriteLine();
            Console.WriteLine("Press Enter key to exit...");
            Console.ReadLine();
            return;
        }

        // Restore the testing element from Failed back to Testing
        if (debugTestingMove != MoveId.None)
        {
            pools.RestoreFailedToTesting(debugTestingMove);
            Console.WriteLine($"Restored testing move: {debugTestingMove}");
        }
        if (debugTestingAbility != AbilityId.None)
        {
            pools.RestoreFailedToTesting(debugTestingAbility);
            Console.WriteLine($"Restored testing ability: {debugTestingAbility}");
        }
        if (debugTestingItem != ItemId.None)
        {
            pools.RestoreFailedToTesting(debugTestingItem);
            Console.WriteLine($"Restored testing item: {debugTestingItem}");
        }
        if (debugTestingSpecies != default)
        {
            pools.RestoreFailedToTesting(debugTestingSpecies);
            Console.WriteLine($"Restored testing species: {debugTestingSpecies}");
        }

        Console.WriteLine("Loaded debug element pools state:");
        Console.WriteLine(pools.GetSummary());
        Console.WriteLine();

        try
        {
            // Create teams using the same DebugTeamGenerator as IncrementalTestRunner
            var team1Gen = new DebugTeamGenerator(Library, pools, debugPlayer1Seed);
            var team2Gen = new DebugTeamGenerator(Library, pools, debugPlayer2Seed);

            var team1 = team1Gen.GenerateTeam();
            var team2 = team2Gen.GenerateTeam();

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
                Id = FormatId.CustomDoubles, // Same format as IncrementalTestRunner
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

    /// <summary>
    /// Runs a single VGC Reg I battle with specific seeds for debugging.
    /// Edit the seed constants in this method to reproduce specific battle
    /// scenarios from RndVsRndVgcRegIEvaluation failures
    /// </summary>
    private void RunSingleBattleDebugVgcRegI()
    {
        // !! EDIT THESE VALUES TO DEBUG SPECIFIC BATTLES !!
        // Copy the 5 seeds from the RndVsRndVgcRegIEvaluation exception output.
        const int debugTeam1Seed = 117817;
        const int debugTeam2Seed = 131387;
        const int debugPlayer1Seed = 75843;
        const int debugPlayer2Seed = 65317;
        const int debugBattleSeed = 73376;
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
