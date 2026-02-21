using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    private void RunRandomVsRandomSinglesEvaluationTest()
    {
        Console.WriteLine("[Driver] Starting Random vs Random Singles Evaluation Test");
        Console.WriteLine($"[Driver] Running {RandomEvaluationNumTest} battles with {NumThreads} threads");

        const bool debug = false;

        var simResults = new ConcurrentBag<SimulatorResult>();
        var stopwatch = Stopwatch.StartNew();

        var seedCounter = 0;
        var completedBattles = 0;
        var turnOnBattleEnd = new ConcurrentBag<int>();
        var exceptions = new ConcurrentBag<(int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)>();

        // Run simulations in parallel with specified number of threads
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = NumThreads,
        };

        Parallel.For(0, RandomEvaluationNumTest, parallelOptions, _ =>
        {
            // Calculate unique seeds for this battle using atomic operations
            int offset1 = Interlocked.Increment(ref seedCounter);
            int offset2 = Interlocked.Increment(ref seedCounter);
            int offset3 = Interlocked.Increment(ref seedCounter);

            int localPlayer1Seed = PlayerRandom1EvalSeed + offset1;
            int localPlayer2Seed = PlayerRandom2EvalSeed + offset2;
            int localBattleSeed = BattleEvalSeed + offset3;

            try
            {
                (SimulatorResult result, int turn) = RunBattleWithTimeout(
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    FormatId.CustomSingles,
                    debug);

                simResults.Add(result);
                turnOnBattleEnd.Add(turn);

                int completed = Interlocked.Increment(ref completedBattles);
                if (completed % 100 == 0)
                {
                    Console.WriteLine($"[Driver] Completed {completed}/{RandomEvaluationNumTest} battles");
                }
            }
            catch (Exception ex)
            {
                // Store exception with seed information for debugging
                exceptions.Add((localPlayer1Seed, localPlayer2Seed, localBattleSeed, ex));

                // Log immediately to console
                LogExceptionWithSeeds(
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    ex,
                    "RandomVsRandomSingles");
            }
        });

        stopwatch.Stop();

        // Convert to list for counting (now from ConcurrentBag)
        var resultsList = simResults.ToList();
        var turnsList = turnOnBattleEnd.ToList();
        var exceptionsList = exceptions.ToList();

        int successfulBattles = resultsList.Count;
        int failedBattles = exceptionsList.Count;
        int player1Wins = resultsList.Count(result => result == SimulatorResult.Player1Win);
        int player2Wins = resultsList.Count(result => result == SimulatorResult.Player2Win);
        int ties = resultsList.Count(result => result == SimulatorResult.Tie);

        // Calculate timing metrics
        double totalSeconds = stopwatch.Elapsed.TotalSeconds;
        double timePerSimulation = totalSeconds / RandomEvaluationNumTest;
        double simulationsPerSecond = RandomEvaluationNumTest / totalSeconds;

        // Calculate turn statistics (only if there are successful battles)
        double meanTurns = 0;
        double stdDevTurns = 0;
        double medianTurns = 0;
        var minTurns = 0;
        var maxTurns = 0;

        if (turnsList.Count > 0)
        {
            meanTurns = turnsList.Mean();
            stdDevTurns = turnsList.StandardDeviation();
            medianTurns = turnsList.Median();
            minTurns = turnsList.Minimum();
            maxTurns = turnsList.Maximum();
        }

        // Print results
        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine($"Random vs Random Evaluation Results ({RandomEvaluationNumTest} battles):");
        sb.AppendLine("Format: Singles");
        sb.AppendLine($"Git Commit: {GetGitCommitId()}");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {RandomEvaluationNumTest}");
        sb.AppendLine();

        if (failedBattles > 0)
        {
            sb.AppendLine("??  EXCEPTION SUMMARY:");
            sb.AppendLine("-----------------------------------------------------------");
            for (var i = 0; i < exceptionsList.Count; i++)
            {
                (int p1Seed, int p2Seed, int bSeed, Exception ex) = exceptionsList[i];
                sb.AppendLine($"Exception #{i + 1}:");
                sb.AppendLine($"  Player 1 Seed: {p1Seed}");
                sb.AppendLine($"  Player 2 Seed: {p2Seed}");
                sb.AppendLine($"  Battle Seed:   {bSeed}");
                sb.AppendLine($"  Exception:     {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine();
            }

            sb.AppendLine("-----------------------------------------------------------");
            sb.AppendLine();
        }

        sb.AppendLine("Battle Results (Successful Battles Only):");
        sb.AppendLine($"Player 1 Wins: {player1Wins}");
        sb.AppendLine($"Player 2 Wins: {player2Wins}");
        sb.AppendLine($"Ties: {ties}");
        if (successfulBattles > 0)
        {
            sb.AppendLine($"Win Rate for Player 1: {(double)player1Wins / successfulBattles:P2}");
            sb.AppendLine($"Win Rate for Player 2: {(double)player2Wins / successfulBattles:P2}");
            sb.AppendLine($"Tie Rate: {(double)ties / successfulBattles:P2}");
        }

        sb.AppendLine();
        sb.AppendLine("Performance Metrics:");
        sb.AppendLine($"Number of threads: {NumThreads}");
        sb.AppendLine($@"Total Execution Time: {stopwatch.Elapsed:hh\:mm\:ss\.fff}");
        sb.AppendLine($"Total Execution Time (seconds): {totalSeconds:F3}");
        sb.AppendLine($"Time per Simulation: {timePerSimulation * 1000:F3} ms");
        sb.AppendLine($"Simulations per Second: {simulationsPerSecond:F0}");
        sb.AppendLine();
        sb.AppendLine("Turn Statistics:");
        sb.AppendLine($"Mean Turns: {meanTurns:F2}");
        sb.AppendLine($"Standard Deviation of Turns: {stdDevTurns:F2}");
        sb.AppendLine($"Median Turns: {medianTurns:F2}");
        sb.AppendLine($"Minimum Turns: {minTurns}");
        sb.AppendLine($"Maximum Turns: {maxTurns}");
        Console.WriteLine(sb.ToString());

        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
    }

    private void RunRandomVsRandomDoublesEvaluationTest()
    {
        Console.WriteLine("[Driver] Starting Random vs Random Doubles Evaluation Test");
        Console.WriteLine($"[Driver] Running {RandomEvaluationNumTest} battles with {NumThreads} threads");

        const bool debug = false;

        var simResults = new ConcurrentBag<SimulatorResult>();
        var stopwatch = Stopwatch.StartNew();

        var seedCounter = 0;
        var completedBattles = 0;
        var turnOnBattleEnd = new ConcurrentBag<int>();
        var exceptions = new ConcurrentBag<(int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)>();

        // Run simulations in parallel with specified number of threads
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = NumThreads,
        };

        Parallel.For(0, RandomEvaluationNumTest, parallelOptions, _ =>
        {
            // Calculate unique seeds for this battle using atomic operations
            int offset1 = Interlocked.Increment(ref seedCounter);
            int offset2 = Interlocked.Increment(ref seedCounter);
            int offset3 = Interlocked.Increment(ref seedCounter);

            int localPlayer1Seed = PlayerRandom1EvalSeed + offset1;
            int localPlayer2Seed = PlayerRandom2EvalSeed + offset2;
            int localBattleSeed = BattleEvalSeed + offset3;

            try
            {
                (SimulatorResult result, int turn) = RunBattleWithTimeout(
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    FormatId.Gen9VgcRegulationA,
                    debug);

                simResults.Add(result);
                turnOnBattleEnd.Add(turn);

                int completed = Interlocked.Increment(ref completedBattles);
                if (completed % 100 == 0)
                {
                    Console.WriteLine($"[Driver] Completed {completed}/{RandomEvaluationNumTest} battles");
                }
            }
            catch (Exception ex)
            {
                // Store exception with seed information for debugging
                exceptions.Add((localPlayer1Seed, localPlayer2Seed, localBattleSeed, ex));

                // Log immediately to console
                LogExceptionWithSeeds(
                    localPlayer1Seed,
                    localPlayer2Seed,
                    localBattleSeed,
                    ex,
                    "RandomVsRandomDoubles");
            }
        });

        stopwatch.Stop();

        // Convert to list for counting (now from ConcurrentBag)
        var resultsList = simResults.ToList();
        var turnsList = turnOnBattleEnd.ToList();
        var exceptionsList = exceptions.ToList();

        int successfulBattles = resultsList.Count;
        int failedBattles = exceptionsList.Count;
        int player1Wins = resultsList.Count(result => result == SimulatorResult.Player1Win);
        int player2Wins = resultsList.Count(result => result == SimulatorResult.Player2Win);
        int ties = resultsList.Count(result => result == SimulatorResult.Tie);

        // Calculate timing metrics
        double totalSeconds = stopwatch.Elapsed.TotalSeconds;
        double timePerSimulation = totalSeconds / RandomEvaluationNumTest;
        double simulationsPerSecond = RandomEvaluationNumTest / totalSeconds;

        // Calculate turn statistics (only if there are successful battles)
        double meanTurns = 0;
        double stdDevTurns = 0;
        double medianTurns = 0;
        var minTurns = 0;
        var maxTurns = 0;

        if (turnsList.Count > 0)
        {
            meanTurns = turnsList.Mean();
            stdDevTurns = turnsList.StandardDeviation();
            medianTurns = turnsList.Median();
            minTurns = turnsList.Minimum();
            maxTurns = turnsList.Maximum();
        }

        // Print results
        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine($"Random vs Random Evaluation Results ({RandomEvaluationNumTest} battles):");
        sb.AppendLine("Format: Doubles");
        sb.AppendLine($"Git Commit: {GetGitCommitId()}");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {RandomEvaluationNumTest}");
        sb.AppendLine();

        if (failedBattles > 0)
        {
            sb.AppendLine("??  EXCEPTION SUMMARY:");
            sb.AppendLine("-----------------------------------------------------------");
            for (var i = 0; i < exceptionsList.Count; i++)
            {
                (int p1Seed, int p2Seed, int bSeed, Exception ex) = exceptionsList[i];
                sb.AppendLine($"Exception #{i + 1}:");
                sb.AppendLine($"  Player 1 Seed: {p1Seed}");
                sb.AppendLine($"  Player 2 Seed: {p2Seed}");
                sb.AppendLine($"  Battle Seed:   {bSeed}");
                sb.AppendLine($"  Exception:     {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine();
            }

            sb.AppendLine("-----------------------------------------------------------");
            sb.AppendLine();
        }

        sb.AppendLine("Battle Results (Successful Battles Only):");
        sb.AppendLine($"Player 1 Wins: {player1Wins}");
        sb.AppendLine($"Player 2 Wins: {player2Wins}");
        sb.AppendLine($"Ties: {ties}");
        if (successfulBattles > 0)
        {
            sb.AppendLine($"Win Rate for Player 1: {(double)player1Wins / successfulBattles:P2}");
            sb.AppendLine($"Win Rate for Player 2: {(double)player2Wins / successfulBattles:P2}");
            sb.AppendLine($"Tie Rate: {(double)ties / successfulBattles:P2}");
        }

        sb.AppendLine();
        sb.AppendLine("Performance Metrics:");
        sb.AppendLine($"Number of threads: {NumThreads}");
        sb.AppendLine($@"Total Execution Time: {stopwatch.Elapsed:hh\:mm\:ss\.fff}");
        sb.AppendLine($"Total Execution Time (seconds): {totalSeconds:F3}");
        sb.AppendLine($"Time per Simulation: {timePerSimulation * 1000:F3} ms");
        sb.AppendLine($"Simulations per Second: {simulationsPerSecond:F0}");
        sb.AppendLine();
        sb.AppendLine("Turn Statistics:");
        sb.AppendLine($"Mean Turns: {meanTurns:F2}");
        sb.AppendLine($"Standard Deviation of Turns: {stdDevTurns:F2}");
        sb.AppendLine($"Median Turns: {medianTurns:F2}");
        sb.AppendLine($"Minimum Turns: {minTurns}");
        sb.AppendLine($"Maximum Turns: {maxTurns}");
        Console.WriteLine(sb.ToString());

        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
    }

    /// <summary>
    /// Thread-local accumulator for parallel battle evaluation.
    /// Eliminates contention on shared ConcurrentBag/ConcurrentDictionary by collecting
    /// results per-thread and merging once after the parallel loop completes.
    /// </summary>
    private sealed class EvaluationLocalState
    {
        public readonly List<SimulatorResult> Results = [];
        public readonly List<int> Turns = [];
        public readonly List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)> Exceptions = [];
        public readonly Dictionary<SpecieId, int> SpeciesCoverage = [];
        public readonly Dictionary<MoveId, int> MoveCoverage = [];
        public readonly Dictionary<AbilityId, int> AbilityCoverage = [];
        public readonly Dictionary<ItemId, int> ItemCoverage = [];
    }

    private void RunRndVsRndVgcRegIEvaluation()
    {
        Console.WriteLine("[Driver] Starting Random Team vs Random Team VGC Reg I Evaluation");
        Console.WriteLine($"[Driver] Running {RandomEvaluationNumTest} battles with {NumThreads} threads");

        const bool debug = false;

        var stopwatch = Stopwatch.StartNew();

        var seedCounter = 0;
        var completedBattles = 0;

        // Merged results collected from thread-local state after parallel loop
        var allResults = new List<SimulatorResult>();
        var allTurns = new List<int>();
        var allExceptions = new List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)>();
        var speciesCoverage = new ConcurrentDictionary<SpecieId, int>();
        var moveCoverage = new ConcurrentDictionary<MoveId, int>();
        var abilityCoverage = new ConcurrentDictionary<AbilityId, int>();
        var itemCoverage = new ConcurrentDictionary<ItemId, int>();

        // Run simulations in parallel with thread-local state to eliminate contention
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = NumThreads,
        };

        Parallel.For(0, RandomEvaluationNumTest, parallelOptions,
            // localInit: create a fresh accumulator per thread
            static () => new EvaluationLocalState(),
            // body: run battle and accumulate into thread-local state (no shared writes)
            (i, _, localState) =>
            {
                // Batch seed allocation: single atomic add instead of 5 separate increments
                int baseOffset = Interlocked.Add(ref seedCounter, 5) - 4;

                int localTeam1Seed = Team1EvalSeed + baseOffset;
                int localTeam2Seed = Team2EvalSeed + baseOffset + 1;
                int localPlayer1Seed = PlayerRandom1EvalSeed + baseOffset + 2;
                int localPlayer2Seed = PlayerRandom2EvalSeed + baseOffset + 3;
                int localBattleSeed = BattleEvalSeed + baseOffset + 4;

                try
                {
                    // Generate teams externally so we can track coverage
                    var team1Generator = new RandomTeamGenerator(Library, FormatId.Gen9VgcRegulationI, localTeam1Seed);
                    var team2Generator = new RandomTeamGenerator(Library, FormatId.Gen9VgcRegulationI, localTeam2Seed);

                    var team1 = team1Generator.GenerateTeam();
                    var team2 = team2Generator.GenerateTeam();

                    // Track coverage into thread-local dictionaries (zero contention)
                    TrackTeamCoverage(team1, localState);
                    TrackTeamCoverage(team2, localState);

                    // Run directly on Parallel.For thread — no Task.Run + Wait overhead
                    (SimulatorResult result, int turn) = RunBattleWithPrebuiltTeamsDirect(
                        team1,
                        team2,
                        localTeam1Seed,
                        localTeam2Seed,
                        localPlayer1Seed,
                        localPlayer2Seed,
                        localBattleSeed,
                        debug);

                    localState.Results.Add(result);
                    localState.Turns.Add(turn);

                    int completed = Interlocked.Increment(ref completedBattles);
                    if (completed % 100 == 0)
                    {
                        Console.WriteLine($"[Driver] Completed {completed}/{RandomEvaluationNumTest} battles");
                    }
                }
                catch (Exception ex)
                {
                    // Store exception with all seed information for debugging
                    localState.Exceptions.Add(
                        (localTeam1Seed, localTeam2Seed, localPlayer1Seed, localPlayer2Seed, localBattleSeed, ex));

                    // Log immediately to console
                    LogExceptionWithAllSeeds(
                        localTeam1Seed,
                        localTeam2Seed,
                        localPlayer1Seed,
                        localPlayer2Seed,
                        localBattleSeed,
                        ex,
                        "RndVsRndVgcRegIEvaluation");
                }

                return localState;
            },
            // localFinally: merge thread-local state into shared collections (runs once per thread)
            localState =>
            {
                lock (allResults)
                {
                    allResults.AddRange(localState.Results);
                    allTurns.AddRange(localState.Turns);
                    allExceptions.AddRange(localState.Exceptions);
                }

                // Merge coverage dictionaries into the shared ConcurrentDictionary
                foreach (var (key, count) in localState.SpeciesCoverage)
                    speciesCoverage.AddOrUpdate(key, static (_, arg) => arg, static (_, existing, arg) => existing + arg, count);
                foreach (var (key, count) in localState.MoveCoverage)
                    moveCoverage.AddOrUpdate(key, static (_, arg) => arg, static (_, existing, arg) => existing + arg, count);
                foreach (var (key, count) in localState.AbilityCoverage)
                    abilityCoverage.AddOrUpdate(key, static (_, arg) => arg, static (_, existing, arg) => existing + arg, count);
                foreach (var (key, count) in localState.ItemCoverage)
                    itemCoverage.AddOrUpdate(key, static (_, arg) => arg, static (_, existing, arg) => existing + arg, count);
            });

        stopwatch.Stop();

        var resultsList = allResults;
        var turnsList = allTurns;
        var exceptionsList = allExceptions;

        int successfulBattles = resultsList.Count;
        int failedBattles = exceptionsList.Count;
        int player1Wins = resultsList.Count(result => result == SimulatorResult.Player1Win);
        int player2Wins = resultsList.Count(result => result == SimulatorResult.Player2Win);
        int ties = resultsList.Count(result => result == SimulatorResult.Tie);

        // Calculate timing metrics
        double totalSeconds = stopwatch.Elapsed.TotalSeconds;
        double timePerSimulation = totalSeconds / RandomEvaluationNumTest;
        double simulationsPerSecond = RandomEvaluationNumTest / totalSeconds;

        // Calculate turn statistics (only if there are successful battles)
        double meanTurns = 0;
        double stdDevTurns = 0;
        double medianTurns = 0;
        var minTurns = 0;
        var maxTurns = 0;

        if (turnsList.Count > 0)
        {
            meanTurns = turnsList.Mean();
            stdDevTurns = turnsList.StandardDeviation();
            medianTurns = turnsList.Median();
            minTurns = turnsList.Minimum();
            maxTurns = turnsList.Maximum();
        }

        // Print results
        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine($"Random Team vs Random Team VGC Reg I Evaluation Results ({RandomEvaluationNumTest} battles):");
        sb.AppendLine("Format: VGC 2025 Regulation I (Random Teams)");
        sb.AppendLine($"Git Commit: {GetGitCommitId()}");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {RandomEvaluationNumTest}");
        sb.AppendLine();

        if (failedBattles > 0)
        {
            sb.AppendLine("??  EXCEPTION SUMMARY:");
            sb.AppendLine("-----------------------------------------------------------");
            for (var i = 0; i < exceptionsList.Count; i++)
            {
                (int t1Seed, int t2Seed, int p1Seed, int p2Seed, int bSeed, Exception ex) = exceptionsList[i];
                sb.AppendLine($"Exception #{i + 1}:");
                sb.AppendLine($"  Team 1 Seed:   {t1Seed}");
                sb.AppendLine($"  Team 2 Seed:   {t2Seed}");
                sb.AppendLine($"  Player 1 Seed: {p1Seed}");
                sb.AppendLine($"  Player 2 Seed: {p2Seed}");
                sb.AppendLine($"  Battle Seed:   {bSeed}");
                sb.AppendLine($"  Exception:     {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine();
            }

            sb.AppendLine("-----------------------------------------------------------");
            sb.AppendLine();
        }

        sb.AppendLine("Battle Results (Successful Battles Only):");
        sb.AppendLine($"Player 1 Wins: {player1Wins}");
        sb.AppendLine($"Player 2 Wins: {player2Wins}");
        sb.AppendLine($"Ties: {ties}");
        if (successfulBattles > 0)
        {
            sb.AppendLine($"Win Rate for Player 1: {(double)player1Wins / successfulBattles:P2}");
            sb.AppendLine($"Win Rate for Player 2: {(double)player2Wins / successfulBattles:P2}");
            sb.AppendLine($"Tie Rate: {(double)ties / successfulBattles:P2}");
        }

        sb.AppendLine();
        sb.AppendLine("Performance Metrics:");
        sb.AppendLine($"Number of threads: {NumThreads}");
        sb.AppendLine($@"Total Execution Time: {stopwatch.Elapsed:hh\:mm\:ss\.fff}");
        sb.AppendLine($"Total Execution Time (seconds): {totalSeconds:F3}");
        sb.AppendLine($"Time per Simulation: {timePerSimulation * 1000:F3} ms");
        sb.AppendLine($"Simulations per Second: {simulationsPerSecond:F0}");
        sb.AppendLine();
        sb.AppendLine("Turn Statistics:");
        sb.AppendLine($"Mean Turns: {meanTurns:F2}");
        sb.AppendLine($"Standard Deviation of Turns: {stdDevTurns:F2}");
        sb.AppendLine($"Median Turns: {medianTurns:F2}");
        sb.AppendLine($"Minimum Turns: {minTurns}");
        sb.AppendLine($"Maximum Turns: {maxTurns}");
        Console.WriteLine(sb.ToString());

        // --- Coverage Report ---
        // Build the legal pools from a reference generator (seed is irrelevant for pool queries)
        var refGenerator = new RandomTeamGenerator(Library, FormatId.Gen9VgcRegulationI, seed: 0);

        // Derive reachable abilities and moves from legal species
        var legalAbilities = new HashSet<AbilityId>();
        var legalMoves = new HashSet<MoveId>();
        foreach (var specieId in refGenerator.LegalSpecies)
        {
            var species = Library.Species[specieId];
            if (species.Abilities.Slot0 != AbilityId.None) legalAbilities.Add(species.Abilities.Slot0);
            if (species.Abilities.Slot1 is { } s1 && s1 != AbilityId.None) legalAbilities.Add(s1);
            if (species.Abilities.Hidden is { } h && h != AbilityId.None) legalAbilities.Add(h);

            if (Library.Learnsets.TryGetValue(specieId, out var learnset) && learnset.LearnsetData != null)
            {
                foreach (var (moveId, sources) in learnset.LearnsetData)
                {
                    if (sources.Any(s => s.Generation == 9))
                    {
                        legalMoves.Add(moveId);
                    }
                }
            }
        }

        var coverageSb = new StringBuilder();
        coverageSb.AppendLine("===========================================================");
        coverageSb.AppendLine("COVERAGE REPORT");
        coverageSb.AppendLine("===========================================================");
        coverageSb.AppendLine();

        AppendCoverageSection(coverageSb, "Species", speciesCoverage, refGenerator.LegalSpecies);
        AppendCoverageSection(coverageSb, "Items", itemCoverage, refGenerator.UsableItems);
        AppendCoverageSection(coverageSb, "Abilities", abilityCoverage, legalAbilities);
        AppendCoverageSection(coverageSb, "Moves", moveCoverage, legalMoves);

        Console.WriteLine(coverageSb.ToString());

        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
    }

    /// <summary>
    /// Tracks species, ability, item, and move coverage from a team into thread-local dictionaries.
    /// No contention — only the owning thread writes to these dictionaries.
    /// </summary>
    private static void TrackTeamCoverage(List<PokemonSet> team, EvaluationLocalState state)
    {
        foreach (var set in team)
        {
            IncrementCount(state.SpeciesCoverage, set.Species);
            IncrementCount(state.AbilityCoverage, set.Ability);
            IncrementCount(state.ItemCoverage, set.Item);
            foreach (var move in set.Moves)
            {
                IncrementCount(state.MoveCoverage, move);
            }
        }

        static void IncrementCount<TKey>(Dictionary<TKey, int> dict, TKey key) where TKey : notnull
        {
            ref int slot = ref System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out _);
            slot++;
        }
    }

    /// <summary>
    /// Appends a coverage section for a single category to the report.
    /// Lists overall stats and any entries from the legal pool that were never selected.
    /// </summary>
    private static void AppendCoverageSection<TId>(
        StringBuilder sb,
        string label,
        ConcurrentDictionary<TId, int> observed,
        IEnumerable<TId> legalPool) where TId : notnull
    {
        var legalSet = legalPool as IReadOnlyCollection<TId> ?? legalPool.ToList();
        int totalLegal = legalSet.Count;
        var neverSelected = legalSet.Where(id => !observed.ContainsKey(id)).OrderBy(id => id).ToList();
        int selectedCount = totalLegal - neverSelected.Count;

        var counts = observed.Values.ToList();
        int minCount = counts.Count > 0 ? counts.Min() : 0;
        int maxCount = counts.Count > 0 ? counts.Max() : 0;
        double meanCount = counts.Count > 0 ? counts.Average() : 0;

        sb.AppendLine($"{label} Coverage: {selectedCount}/{totalLegal} ({(double)selectedCount / totalLegal:P2})");
        sb.AppendLine($"  Selection counts — Min: {minCount}, Max: {maxCount}, Mean: {meanCount:F1}");

        if (neverSelected.Count > 0)
        {
            sb.AppendLine($"  Never selected ({neverSelected.Count}):");
            foreach (var id in neverSelected)
            {
                sb.AppendLine($"    - {id}");
            }
        }

        sb.AppendLine();
    }
}
