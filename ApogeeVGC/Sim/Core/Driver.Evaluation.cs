using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using System.Diagnostics;
using System.Text;

namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    /// <summary>
    /// Thread-local accumulator for parallel battle evaluation.
    /// Eliminates contention on shared collections by collecting
    /// results per-thread and merging once after the parallel loop completes.
    /// </summary>
    private sealed class EvaluationLocalState
    {
        public readonly List<SimulatorResult> Results = [];
        public readonly List<int> Turns = [];

        public readonly
            List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)>
            Exceptions = [];
    }

    /// <summary>
    /// Pre-generated battle input containing teams and seeds for a single evaluation battle.
    /// Teams are generated before timing begins so the benchmark measures only battle simulation.
    /// </summary>
    private readonly record struct EvaluationBattleInput(
        List<PokemonSet> Team1,
        List<PokemonSet> Team2,
        int Team1Seed,
        int Team2Seed,
        int Player1Seed,
        int Player2Seed,
        int BattleRandSeed);

    private void RunRndVsRndVgcRegIEvaluation()
    {
        RunRandomTeamEvaluation(
            formatId: FormatId.Gen9VgcRegulationI,
            formatLabel: "VGC 2025 Regulation I",
            debugModeName: "RndVsRndVgcRegIEvaluation");
    }

    private void RunRndVsRndMegaEvaluation()
    {
        RunRandomTeamEvaluation(
            formatId: FormatId.Gen9VgcMega,
            formatLabel: "VGC Mega Evolution",
            debugModeName: "RndVsRndMegaEvaluation");
    }

    /// <summary>
    /// Runs a parallel random-team evaluation for the given format.
    /// Pre-generates teams, runs battles in parallel with thread-local state,
    /// and prints comprehensive statistics.
    /// </summary>
    private void RunRandomTeamEvaluation(FormatId formatId, string formatLabel, string debugModeName)
    {
        Console.WriteLine($"[Driver] Starting Random Team vs Random Team {formatLabel} Evaluation");
        Console.WriteLine($"[Driver] Running {RandomEvaluationNumTest} battles with {NumThreads} threads");

        const bool debug = false;

        // Pre-generate all teams before timing so the benchmark measures only battle simulation
        Console.WriteLine("[Driver] Pre-generating teams...");
        var battles = new EvaluationBattleInput[RandomEvaluationNumTest];
        for (var i = 0; i < RandomEvaluationNumTest; i++)
        {
            int baseOffset = i * 5 + 1;
            int team1Seed = Team1EvalSeed + baseOffset;
            int team2Seed = Team2EvalSeed + baseOffset + 1;

            var team1 = new RandomTeamGenerator(Library, formatId, team1Seed).GenerateTeam();
            var team2 = new RandomTeamGenerator(Library, formatId, team2Seed).GenerateTeam();

            battles[i] = new EvaluationBattleInput(
                team1, team2,
                team1Seed, team2Seed,
                PlayerRandom1EvalSeed + baseOffset + 2,
                PlayerRandom2EvalSeed + baseOffset + 3,
                BattleEvalSeed + baseOffset + 4);
        }

        Console.WriteLine("[Driver] Team pre-generation complete. Starting battles...");

        var stopwatch = Stopwatch.StartNew();

        var completedBattles = 0;

        // Milestone timestamps for throughput analysis (one entry per 100 battles)
        int numMilestones = RandomEvaluationNumTest / 100;
        var milestoneTicks = new long[numMilestones];

        // Merged results collected from thread-local state after parallel loop
        var allResults = new List<SimulatorResult>();
        var allTurns = new List<int>();
        var allExceptions =
            new List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception
                Exception)>();

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
                EvaluationBattleInput battle = battles[i];

                try
                {
                    // Run directly on Parallel.For thread — no Task.Run + Wait overhead
                    (SimulatorResult result, int turn) = RunBattleWithPrebuiltTeamsDirect(
                        battle.Team1,
                        battle.Team2,
                        battle.Team1Seed,
                        battle.Team2Seed,
                        battle.Player1Seed,
                        battle.Player2Seed,
                        battle.BattleRandSeed,
                        formatId,
                        debug);

                    localState.Results.Add(result);
                    localState.Turns.Add(turn);

                    int completed = Interlocked.Increment(ref completedBattles);
                    if (completed % 100 == 0)
                    {
                        int milestoneIndex = completed / 100 - 1;
                        if (milestoneIndex < milestoneTicks.Length)
                        {
                            milestoneTicks[milestoneIndex] = stopwatch.ElapsedTicks;
                        }

                        Console.WriteLine($"[Driver] Completed {completed}/{RandomEvaluationNumTest} battles");
                    }
                }
                catch (Exception ex)
                {
                    // Store exception with all seed information for debugging
                    localState.Exceptions.Add(
                        (battle.Team1Seed, battle.Team2Seed, battle.Player1Seed, battle.Player2Seed,
                            battle.BattleRandSeed, ex));

                    // Log immediately to console
                    LogExceptionWithAllSeeds(
                        battle.Team1Seed,
                        battle.Team2Seed,
                        battle.Player1Seed,
                        battle.Player2Seed,
                        battle.BattleRandSeed,
                        ex,
                        debugModeName);
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
        sb.AppendLine($"Random Team vs Random Team {formatLabel} Evaluation Results ({RandomEvaluationNumTest} battles):");
        sb.AppendLine($"Format: {formatLabel} (Random Teams)");
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

        // Compute first-half and second-half (steady-state) throughput from milestones
        if (numMilestones >= 2)
        {
            int halfIndex = numMilestones / 2;
            int firstHalfBattles = (halfIndex + 1) * 100;
            int secondHalfBattles = numMilestones * 100 - firstHalfBattles;

            double firstHalfSeconds = milestoneTicks[halfIndex] / (double)Stopwatch.Frequency;
            double secondHalfSeconds = (milestoneTicks[numMilestones - 1] - milestoneTicks[halfIndex])
                                       / (double)Stopwatch.Frequency;

            if (firstHalfSeconds > 0 && secondHalfSeconds > 0)
            {
                double firstHalfRate = firstHalfBattles / firstHalfSeconds;
                double secondHalfRate = secondHalfBattles / secondHalfSeconds;
                double speedup = secondHalfRate / firstHalfRate;

                sb.AppendLine();
                sb.AppendLine("Throughput Breakdown (showing JIT warm-up effect):");
                sb.AppendLine($"Warm-up Phase    (battles 1-{firstHalfBattles}): {firstHalfRate:F0} sims/sec");
                sb.AppendLine($"Steady-State     (battles {firstHalfBattles + 1}-{numMilestones * 100}): {secondHalfRate:F0} sims/sec ({speedup:F2}x faster)");

                // If we have enough milestones, show the last quarter as "peak steady-state"
                if (numMilestones >= 4)
                {
                    int threeQuarterIndex = (numMilestones * 3) / 4;
                    int lastQuarterBattles = numMilestones * 100 - (threeQuarterIndex + 1) * 100;
                    double lastQuarterSeconds = (milestoneTicks[numMilestones - 1] - milestoneTicks[threeQuarterIndex])
                                                / (double)Stopwatch.Frequency;

                    if (lastQuarterSeconds > 0)
                    {
                        double lastQuarterRate = lastQuarterBattles / lastQuarterSeconds;
                        sb.AppendLine($"Peak Steady-State (battles {(threeQuarterIndex + 1) * 100 + 1}-{numMilestones * 100}): {lastQuarterRate:F0} sims/sec");
                    }
                }
            }
        }

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
}
