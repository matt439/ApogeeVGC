using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using System.Diagnostics;
using System.Text;
using System.Text.Json;


namespace ApogeeVGC.Sim.Core;

public partial class Driver
{
    // Don't change these — used for evaluation tests to reproduce errors
    private const int PlayerRandom1EvalSeed = 12345;
    private const int PlayerRandom2EvalSeed = 1818;
    private const int BattleEvalSeed = 9876;

    // Random vs Random evaluation settings
    private const int RandomEvaluationNumTest = 100000;
    private const int NumThreads = CpuCoreCount;

    // Greedy evaluation settings
    private const int GreedyEvaluationNumTest = 200000;
    private const int GreedyNumThreads = CpuThreadCount;

    // MCTS evaluation settings
    private const int MctsEvaluationNumTest = 1000;
    private const int MctsNumThreads = CpuThreadCount;

    // Standalone MCTS evaluation settings
    private const int MctsStandaloneEvaluationNumTest = 100;
    private const int MctsStandaloneNumThreads = CpuThreadCount;
    private const int MctsStandaloneIterations = 1000;

    // MCTS-DL evaluation settings (DL priors + value, no info tracking)
    private const int MctsDlEvaluationNumTest = 20;
    private const int MctsDlNumThreads = CpuThreadCount;
    private const int MctsDlIterations = 10000;

    // DL-Greedy evaluation settings (argmax policy, no search)
    private const int DlGreedyEvaluationNumTest = 30000;
    private const int DlGreedyNumThreads = CpuThreadCount;

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

    /// <summary>
    /// Runs a parallel random-team evaluation for the given format.
    /// Pre-generates teams, runs battles in parallel with thread-local state,
    /// and prints comprehensive statistics.
    /// </summary>
    private void RunRandomTeamEvaluation(FormatId formatId)
    {
        string formatLabel = Library.Formats[formatId].Name;
        Console.WriteLine($"[Driver] Starting Random Team vs Random Team {formatLabel} Evaluation");
        Console.WriteLine($"[Driver] Running {RandomEvaluationNumTest} battles with {NumThreads} threads");

        const bool debug = false;

        // Pre-generate all teams before timing so the benchmark measures only battle simulation
        Console.WriteLine("[Driver] Pre-generating teams...");
        EvaluationBattleInput[] battles = new EvaluationBattleInput[RandomEvaluationNumTest];
        for (int i = 0; i < RandomEvaluationNumTest; i++)
        {
            int baseOffset = i * 5 + 1;
            int team1Seed = Team1EvalSeed + baseOffset;
            int team2Seed = Team2EvalSeed + baseOffset + 1;

            List<PokemonSet> team1 = new RandomTeamGenerator(Library, formatId, team1Seed).GenerateTeam();
            List<PokemonSet> team2 = new RandomTeamGenerator(Library, formatId, team2Seed).GenerateTeam();

            battles[i] = new EvaluationBattleInput(
                team1, team2,
                team1Seed, team2Seed,
                PlayerRandom1EvalSeed + baseOffset + 2,
                PlayerRandom2EvalSeed + baseOffset + 3,
                BattleEvalSeed + baseOffset + 4);
        }

        Console.WriteLine("[Driver] Team pre-generation complete. Starting battles...");

        Stopwatch stopwatch = Stopwatch.StartNew();

        int completedBattles = 0;

        // Milestone timestamps for throughput analysis (one entry per 100 battles)
        const int numMilestones = RandomEvaluationNumTest / 100;
        long[] milestoneTicks = new long[numMilestones];

        // Merged results collected from thread-local state after parallel loop
        List<SimulatorResult> allResults = [];
        List<int> allTurns = [];
        List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)> allExceptions =
            [];

        // Run simulations in parallel with thread-local state to eliminate contention
        ParallelOptions parallelOptions = new()
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
                        nameof(DriverMode.SingleBattleDebug));
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

        List<SimulatorResult> resultsList = allResults;
        List<int> turnsList = allTurns;
        List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)> exceptionsList = allExceptions;

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
        int minTurns = 0;
        int maxTurns = 0;

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
            for (int i = 0; i < exceptionsList.Count; i++)
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
        if (WaitForInput) Console.ReadLine();
    }

    #region Greedy vs Random Evaluation

    /// <summary>
    /// Runs Greedy (Player 1) vs Random (Player 2) evaluation for the given format.
    /// Uses the same high-throughput parallel pattern as RunRandomTeamEvaluation.
    /// </summary>
    private void RunGreedyVsRandomEvaluation(FormatId formatId)
    {
        string formatLabel = Library.Formats[formatId].Name;
        Console.WriteLine($"[Driver] Starting Greedy vs Random {formatLabel} Evaluation");
        Console.WriteLine($"[Driver] Running {GreedyEvaluationNumTest} battles with {GreedyNumThreads} threads");

        const bool debug = false;

        // Pre-generate all teams before timing
        Console.WriteLine("[Driver] Pre-generating teams...");
        EvaluationBattleInput[] battles = new EvaluationBattleInput[GreedyEvaluationNumTest];
        for (int i = 0; i < GreedyEvaluationNumTest; i++)
        {
            int baseOffset = i * 5 + 1;
            int team1Seed = Team1EvalSeed + baseOffset;
            int team2Seed = Team2EvalSeed + baseOffset + 1;

            List<PokemonSet> team1 = new RandomTeamGenerator(Library, formatId, team1Seed).GenerateTeam();
            List<PokemonSet> team2 = new RandomTeamGenerator(Library, formatId, team2Seed).GenerateTeam();

            battles[i] = new EvaluationBattleInput(
                team1, team2,
                team1Seed, team2Seed,
                PlayerRandom1EvalSeed + baseOffset + 2,
                PlayerRandom2EvalSeed + baseOffset + 3,
                BattleEvalSeed + baseOffset + 4);
        }

        Console.WriteLine("[Driver] Team pre-generation complete. Starting battles...");

        Stopwatch stopwatch = Stopwatch.StartNew();
        int completedBattles = 0;

        List<SimulatorResult> allResults = [];
        List<int> allTurns = [];
        List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)> allExceptions =
            [];

        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = GreedyNumThreads,
        };

        Parallel.For(0, GreedyEvaluationNumTest, parallelOptions,
            static () => new EvaluationLocalState(),
            (i, _, localState) =>
            {
                EvaluationBattleInput battle = battles[i];

                try
                {
                    (SimulatorResult result, int turn) = RunGreedyBattleDirect(
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
                        Console.WriteLine($"[Driver] Completed {completed}/{GreedyEvaluationNumTest} battles");
                    }
                }
                catch (Exception ex)
                {
                    localState.Exceptions.Add(
                        (battle.Team1Seed, battle.Team2Seed, battle.Player1Seed, battle.Player2Seed,
                            battle.BattleRandSeed, ex));

                    LogExceptionWithAllSeeds(
                        battle.Team1Seed,
                        battle.Team2Seed,
                        battle.Player1Seed,
                        battle.Player2Seed,
                        battle.BattleRandSeed,
                        ex,
                        nameof(DriverMode.SingleBattleDebug));
                }

                return localState;
            },
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

        int successfulBattles = allResults.Count;
        int failedBattles = allExceptions.Count;
        int greedyWins = allResults.Count(r => r == SimulatorResult.Player1Win);
        int randomWins = allResults.Count(r => r == SimulatorResult.Player2Win);
        int ties = allResults.Count(r => r == SimulatorResult.Tie);

        double totalSeconds = stopwatch.Elapsed.TotalSeconds;

        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine($"Greedy vs Random {formatLabel} Evaluation Results ({GreedyEvaluationNumTest} battles):");
        sb.AppendLine($"Format: {formatLabel}");
        sb.AppendLine($"Git Commit: {GetGitCommitId()}");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {GreedyEvaluationNumTest}");
        sb.AppendLine();

        if (failedBattles > 0)
        {
            sb.AppendLine("EXCEPTION SUMMARY:");
            sb.AppendLine("-----------------------------------------------------------");
            for (int i = 0; i < allExceptions.Count; i++)
            {
                (int t1, int t2, int p1, int p2, int b, Exception ex) = allExceptions[i];
                sb.AppendLine($"Exception #{i + 1}: {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine($"  Seeds: T1={t1} T2={t2} P1={p1} P2={p2} B={b}");
            }

            sb.AppendLine("-----------------------------------------------------------");
            sb.AppendLine();
        }

        sb.AppendLine("Battle Results:");
        sb.AppendLine($"Greedy Wins: {greedyWins}");
        sb.AppendLine($"Random Wins: {randomWins}");
        sb.AppendLine($"Ties:        {ties}");
        if (successfulBattles > 0)
        {
            sb.AppendLine($"Greedy Win Rate: {(double)greedyWins / successfulBattles:P2}");
            sb.AppendLine($"Random Win Rate: {(double)randomWins / successfulBattles:P2}");
        }

        sb.AppendLine();
        sb.AppendLine("Performance:");
        sb.AppendLine($"Threads: {GreedyNumThreads}");
        sb.AppendLine($@"Total Time: {stopwatch.Elapsed:hh\:mm\:ss\.fff}");
        sb.AppendLine($"Time per Battle: {totalSeconds / GreedyEvaluationNumTest * 1000:F3} ms");
        sb.AppendLine($"Simulations per Second: {GreedyEvaluationNumTest / totalSeconds:F0}");

        if (allTurns.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Turn Statistics:");
            sb.AppendLine($"Mean: {allTurns.Mean():F2}, Median: {allTurns.Median():F2}");
            sb.AppendLine($"Std Dev: {allTurns.StandardDeviation():F2}");
            sb.AppendLine($"Min: {allTurns.Minimum()}, Max: {allTurns.Maximum()}");
        }

        Console.WriteLine(sb.ToString());

        Console.WriteLine("Press Enter key to exit...");
        if (WaitForInput) Console.ReadLine();
    }

    #endregion

    #region MCTS Evaluation

    /// <summary>
    /// Runs MCTS (Player 1) vs Random (Player 2) evaluation for the given format.
    /// Requires ONNX model and vocab files — fails gracefully if missing.
    /// </summary>
    private void RunMctsVsRandomEvaluation(FormatId formatId)
    {
        string formatLabel = Library.Formats[formatId].Name;
        Console.WriteLine($"[Driver] Starting MCTS vs Random {formatLabel} Evaluation");
        Console.WriteLine($"[Driver] Running {MctsEvaluationNumTest} battles with {MctsNumThreads} threads");

        // Initialize MCTS resources — fail gracefully if model files are missing
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

        MctsResources.Initialize(MctsModelPath, MctsVocabPath, Library,
            teamPreviewModelPath: MctsTeamPreviewModelPath);
        Console.WriteLine("[Driver] MCTS resources initialized");

        const bool debug = false;

        // Pre-generate teams
        Console.WriteLine("[Driver] Pre-generating teams...");
        EvaluationBattleInput[] battles = new EvaluationBattleInput[MctsEvaluationNumTest];
        for (int i = 0; i < MctsEvaluationNumTest; i++)
        {
            int baseOffset = i * 5 + 1;
            int team1Seed = Team1EvalSeed + baseOffset;
            int team2Seed = Team2EvalSeed + baseOffset + 1;

            List<PokemonSet> team1 = new RandomTeamGenerator(Library, formatId, team1Seed).GenerateTeam();
            List<PokemonSet> team2 = new RandomTeamGenerator(Library, formatId, team2Seed).GenerateTeam();

            battles[i] = new EvaluationBattleInput(
                team1, team2,
                team1Seed, team2Seed,
                PlayerRandom1EvalSeed + baseOffset + 2,
                PlayerRandom2EvalSeed + baseOffset + 3,
                BattleEvalSeed + baseOffset + 4);
        }

        Console.WriteLine("[Driver] Team pre-generation complete. Starting battles...");

        Stopwatch stopwatch = Stopwatch.StartNew();
        int completedBattles = 0;

        List<SimulatorResult> allResults = [];
        List<int> allTurns = [];
        List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)> allExceptions =
            [];

        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = MctsNumThreads,
        };

        Parallel.For(0, MctsEvaluationNumTest, parallelOptions,
            static () => new EvaluationLocalState(),
            (i, _, localState) =>
            {
                EvaluationBattleInput battle = battles[i];

                try
                {
                    (SimulatorResult result, int turn) = RunMctsBattleDirect(
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
                    if (completed % 10 == 0)
                    {
                        Console.WriteLine($"[Driver] Completed {completed}/{MctsEvaluationNumTest} battles");
                    }
                }
                catch (Exception ex)
                {
                    localState.Exceptions.Add(
                        (battle.Team1Seed, battle.Team2Seed, battle.Player1Seed, battle.Player2Seed,
                            battle.BattleRandSeed, ex));

                    LogExceptionWithAllSeeds(
                        battle.Team1Seed,
                        battle.Team2Seed,
                        battle.Player1Seed,
                        battle.Player2Seed,
                        battle.BattleRandSeed,
                        ex,
                        nameof(DriverMode.SingleBattleDebug));
                }

                return localState;
            },
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
        MctsResources.Shutdown();

        int successfulBattles = allResults.Count;
        int failedBattles = allExceptions.Count;
        int mctsWins = allResults.Count(r => r == SimulatorResult.Player1Win);
        int randomWins = allResults.Count(r => r == SimulatorResult.Player2Win);
        int ties = allResults.Count(r => r == SimulatorResult.Tie);

        double totalSeconds = stopwatch.Elapsed.TotalSeconds;

        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine($"MCTS vs Random {formatLabel} Evaluation Results ({MctsEvaluationNumTest} battles):");
        sb.AppendLine($"Format: {formatLabel}");
        sb.AppendLine($"Git Commit: {GetGitCommitId()}");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {MctsEvaluationNumTest}");
        sb.AppendLine();

        if (failedBattles > 0)
        {
            sb.AppendLine("EXCEPTION SUMMARY:");
            sb.AppendLine("-----------------------------------------------------------");
            for (int i = 0; i < allExceptions.Count; i++)
            {
                (int t1, int t2, int p1, int p2, int b, Exception ex) = allExceptions[i];
                sb.AppendLine($"Exception #{i + 1}: {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine($"  Seeds: T1={t1} T2={t2} P1={p1} P2={p2} B={b}");
            }

            sb.AppendLine("-----------------------------------------------------------");
            sb.AppendLine();
        }

        sb.AppendLine("Battle Results:");
        sb.AppendLine($"MCTS Wins:   {mctsWins}");
        sb.AppendLine($"Random Wins: {randomWins}");
        sb.AppendLine($"Ties:        {ties}");
        if (successfulBattles > 0)
        {
            sb.AppendLine($"MCTS Win Rate:   {(double)mctsWins / successfulBattles:P2}");
            sb.AppendLine($"Random Win Rate: {(double)randomWins / successfulBattles:P2}");
        }

        sb.AppendLine();
        sb.AppendLine("Performance:");
        sb.AppendLine($"Threads: {MctsNumThreads}");
        sb.AppendLine($@"Total Time: {stopwatch.Elapsed:hh\:mm\:ss\.fff}");
        sb.AppendLine($"Time per Battle: {totalSeconds / MctsEvaluationNumTest * 1000:F1} ms");

        if (allTurns.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Turn Statistics:");
            sb.AppendLine($"Mean: {allTurns.Mean():F2}, Median: {allTurns.Median():F2}");
            sb.AppendLine($"Min: {allTurns.Minimum()}, Max: {allTurns.Maximum()}");
        }

        Console.WriteLine(sb.ToString());

        Console.WriteLine("Press Enter key to exit...");
        if (WaitForInput) Console.ReadLine();
    }

    #endregion

    #region Standalone MCTS Evaluation

    /// <summary>
    /// Runs standalone MCTS (heuristic eval, uniform priors) vs Random evaluation.
    /// No DL model dependencies.
    /// </summary>
    private void RunMctsStandaloneVsRandomEvaluation(FormatId formatId)
    {
        string formatLabel = Library.Formats[formatId].Name;
        Console.WriteLine($"[Driver] Starting MCTS-Standalone vs Random {formatLabel} Evaluation");
        Console.WriteLine($"[Driver] Running {MctsStandaloneEvaluationNumTest} battles with {MctsStandaloneNumThreads} threads");
        Console.WriteLine($"[Driver] MCTS iterations per search: {MctsStandaloneIterations}");

        MctsConfig mctsConfig = new()
        {
            NumIterations = MctsStandaloneIterations,
            MaxDegreeOfParallelism = MctsStandaloneNumThreads,
        };
        const bool debug = false;

        // Pre-generate teams
        Console.WriteLine("[Driver] Pre-generating teams...");
        EvaluationBattleInput[] battles = new EvaluationBattleInput[MctsStandaloneEvaluationNumTest];
        for (int i = 0; i < MctsStandaloneEvaluationNumTest; i++)
        {
            int baseOffset = i * 5 + 1;
            int team1Seed = Team1EvalSeed + baseOffset;
            int team2Seed = Team2EvalSeed + baseOffset + 1;

            List<PokemonSet> team1 = new RandomTeamGenerator(Library, formatId, team1Seed).GenerateTeam();
            List<PokemonSet> team2 = new RandomTeamGenerator(Library, formatId, team2Seed).GenerateTeam();

            battles[i] = new EvaluationBattleInput(
                team1, team2,
                team1Seed, team2Seed,
                PlayerRandom1EvalSeed + baseOffset + 2,
                PlayerRandom2EvalSeed + baseOffset + 3,
                BattleEvalSeed + baseOffset + 4);
        }

        Console.WriteLine("[Driver] Team pre-generation complete. Starting battles...");

        Stopwatch stopwatch = Stopwatch.StartNew();
        int completedBattles = 0;

        List<SimulatorResult> allResults = [];
        List<int> allTurns = [];
        List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)> allExceptions =
            [];

        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = MctsStandaloneNumThreads,
        };

        Parallel.For(0, MctsStandaloneEvaluationNumTest, parallelOptions,
            static () => new EvaluationLocalState(),
            (i, _, localState) =>
            {
                EvaluationBattleInput battle = battles[i];

                try
                {
                    (SimulatorResult result, int turn) = RunMctsStandaloneBattleDirect(
                        battle.Team1,
                        battle.Team2,
                        battle.Team1Seed,
                        battle.Team2Seed,
                        battle.Player1Seed,
                        battle.Player2Seed,
                        battle.BattleRandSeed,
                        formatId,
                        debug,
                        mctsConfig);

                    localState.Results.Add(result);
                    localState.Turns.Add(turn);

                    int completed = Interlocked.Increment(ref completedBattles);
                    if (completed % 100 == 0)
                    {
                        Console.WriteLine($"[Driver] Completed {completed}/{MctsStandaloneEvaluationNumTest} battles");
                    }
                }
                catch (Exception ex)
                {
                    localState.Exceptions.Add(
                        (battle.Team1Seed, battle.Team2Seed, battle.Player1Seed, battle.Player2Seed,
                            battle.BattleRandSeed, ex));

                    LogExceptionWithAllSeeds(
                        battle.Team1Seed,
                        battle.Team2Seed,
                        battle.Player1Seed,
                        battle.Player2Seed,
                        battle.BattleRandSeed,
                        ex,
                        nameof(DriverMode.SingleBattleDebug));
                }

                return localState;
            },
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

        int successfulBattles = allResults.Count;
        int failedBattles = allExceptions.Count;
        int mctsWins = allResults.Count(r => r == SimulatorResult.Player1Win);
        int randomWins = allResults.Count(r => r == SimulatorResult.Player2Win);
        int ties = allResults.Count(r => r == SimulatorResult.Tie);

        double totalSeconds = stopwatch.Elapsed.TotalSeconds;

        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine($"MCTS-Standalone vs Random {formatLabel} Evaluation Results ({MctsStandaloneEvaluationNumTest} battles):");
        sb.AppendLine($"Format: {formatLabel}");
        sb.AppendLine($"MCTS Iterations: {MctsStandaloneIterations}");
        sb.AppendLine($"Git Commit: {GetGitCommitId()}");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {MctsStandaloneEvaluationNumTest}");
        sb.AppendLine();

        if (failedBattles > 0)
        {
            sb.AppendLine("EXCEPTION SUMMARY:");
            sb.AppendLine("-----------------------------------------------------------");
            for (int i = 0; i < allExceptions.Count; i++)
            {
                (int t1, int t2, int p1, int p2, int b, Exception ex) = allExceptions[i];
                sb.AppendLine($"Exception #{i + 1}: {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine($"  Seeds: T1={t1} T2={t2} P1={p1} P2={p2} B={b}");
            }

            sb.AppendLine("-----------------------------------------------------------");
            sb.AppendLine();
        }

        sb.AppendLine("Battle Results:");
        sb.AppendLine($"MCTS-Standalone Wins: {mctsWins}");
        sb.AppendLine($"Random Wins:          {randomWins}");
        sb.AppendLine($"Ties:                 {ties}");
        if (successfulBattles > 0)
        {
            sb.AppendLine($"MCTS-Standalone Win Rate: {(double)mctsWins / successfulBattles:P2}");
            sb.AppendLine($"Random Win Rate:          {(double)randomWins / successfulBattles:P2}");
        }

        sb.AppendLine();
        sb.AppendLine("Performance:");
        sb.AppendLine($"Threads: {MctsStandaloneNumThreads}");
        sb.AppendLine($@"Total Time: {stopwatch.Elapsed:hh\:mm\:ss\.fff}");
        sb.AppendLine($"Time per Battle: {totalSeconds / MctsStandaloneEvaluationNumTest * 1000:F1} ms");

        if (allTurns.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Turn Statistics:");
            sb.AppendLine($"Mean: {allTurns.Mean():F2}, Median: {allTurns.Median():F2}");
            sb.AppendLine($"Min: {allTurns.Minimum()}, Max: {allTurns.Maximum()}");
        }

        Console.WriteLine(sb.ToString());

        Console.WriteLine("Press Enter key to exit...");
        if (WaitForInput) Console.ReadLine();
    }

    #endregion

    #region MCTS-DL Evaluation

    /// <summary>
    /// Runs MCTS with DL models (policy priors + value eval, no info tracking) vs Random evaluation.
    /// </summary>
    private void RunMctsDlVsRandomEvaluation(FormatId formatId)
    {
        string formatLabel = Library.Formats[formatId].Name;
        Console.WriteLine($"[Driver] Starting MCTS-DL vs Random {formatLabel} Evaluation");
        Console.WriteLine($"[Driver] Running {MctsDlEvaluationNumTest} battles with {MctsDlNumThreads} threads");
        Console.WriteLine($"[Driver] MCTS iterations per search: {MctsDlIterations}");

        // Initialize MCTS resources — fail gracefully if model files are missing
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
            NumIterations = MctsDlIterations,
        };

        MctsResources.Initialize(MctsModelPath, MctsVocabPath, Library, mctsConfig,
            teamPreviewModelPath: MctsTeamPreviewModelPath);
        Console.WriteLine("[Driver] MCTS resources initialized");

        // Enable value head diagnostics
        MctsLogger.Reset();
        MctsLogger.Enabled = true;

        const bool debug = false;

        // Pre-generate teams
        Console.WriteLine("[Driver] Pre-generating teams...");
        EvaluationBattleInput[] battles = new EvaluationBattleInput[MctsDlEvaluationNumTest];
        for (int i = 0; i < MctsDlEvaluationNumTest; i++)
        {
            int baseOffset = i * 5 + 1;
            int team1Seed = Team1EvalSeed + baseOffset;
            int team2Seed = Team2EvalSeed + baseOffset + 1;

            List<PokemonSet> team1 = new RandomTeamGenerator(Library, formatId, team1Seed).GenerateTeam();
            List<PokemonSet> team2 = new RandomTeamGenerator(Library, formatId, team2Seed).GenerateTeam();

            battles[i] = new EvaluationBattleInput(
                team1, team2,
                team1Seed, team2Seed,
                PlayerRandom1EvalSeed + baseOffset + 2,
                PlayerRandom2EvalSeed + baseOffset + 3,
                BattleEvalSeed + baseOffset + 4);
        }

        Console.WriteLine("[Driver] Team pre-generation complete. Starting battles...");

        Stopwatch stopwatch = Stopwatch.StartNew();
        int completedBattles = 0;

        List<SimulatorResult> allResults = [];
        List<int> allTurns = [];
        List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)> allExceptions =
            [];

        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = MctsDlNumThreads,
        };

        Parallel.For(0, MctsDlEvaluationNumTest, parallelOptions,
            static () => new EvaluationLocalState(),
            (i, _, localState) =>
            {
                EvaluationBattleInput battle = battles[i];

                try
                {
                    (SimulatorResult result, int turn) = RunMctsDLBattleDirect(
                        battle.Team1,
                        battle.Team2,
                        battle.Team1Seed,
                        battle.Team2Seed,
                        battle.Player1Seed,
                        battle.Player2Seed,
                        battle.BattleRandSeed,
                        formatId,
                        debug,
                        mctsConfig);

                    localState.Results.Add(result);
                    localState.Turns.Add(turn);

                    int completed = Interlocked.Increment(ref completedBattles);
                    Console.WriteLine($"[Driver] Completed {completed}/{MctsDlEvaluationNumTest} battles");
                }
                catch (Exception ex)
                {
                    localState.Exceptions.Add(
                        (battle.Team1Seed, battle.Team2Seed, battle.Player1Seed, battle.Player2Seed,
                            battle.BattleRandSeed, ex));

                    LogExceptionWithAllSeeds(
                        battle.Team1Seed,
                        battle.Team2Seed,
                        battle.Player1Seed,
                        battle.Player2Seed,
                        battle.BattleRandSeed,
                        ex,
                        nameof(DriverMode.SingleBattleDebug));
                }

                return localState;
            },
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

        MctsLogger.Enabled = false;

        MctsResources.Shutdown();

        int successfulBattles = allResults.Count;
        int failedBattles = allExceptions.Count;
        int mctsWins = allResults.Count(r => r == SimulatorResult.Player1Win);
        int randomWins = allResults.Count(r => r == SimulatorResult.Player2Win);
        int ties = allResults.Count(r => r == SimulatorResult.Tie);

        double totalSeconds = stopwatch.Elapsed.TotalSeconds;

        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine($"MCTS-DL vs Random {formatLabel} Evaluation Results ({MctsDlEvaluationNumTest} battles):");
        sb.AppendLine($"Format: {formatLabel}");
        sb.AppendLine($"MCTS Iterations: {MctsDlIterations}");
        sb.AppendLine($"Git Commit: {GetGitCommitId()}");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {MctsDlEvaluationNumTest}");
        sb.AppendLine();

        if (failedBattles > 0)
        {
            sb.AppendLine("EXCEPTION SUMMARY:");
            sb.AppendLine("-----------------------------------------------------------");
            for (int i = 0; i < allExceptions.Count; i++)
            {
                (int t1, int t2, int p1, int p2, int b, Exception ex) = allExceptions[i];
                sb.AppendLine($"Exception #{i + 1}: {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine($"  Seeds: T1={t1} T2={t2} P1={p1} P2={p2} B={b}");
            }

            sb.AppendLine("-----------------------------------------------------------");
            sb.AppendLine();
        }

        sb.AppendLine("Battle Results:");
        sb.AppendLine($"MCTS-DL Wins: {mctsWins}");
        sb.AppendLine($"Random Wins:  {randomWins}");
        sb.AppendLine($"Ties:         {ties}");
        if (successfulBattles > 0)
        {
            sb.AppendLine($"MCTS-DL Win Rate: {(double)mctsWins / successfulBattles:P2}");
            sb.AppendLine($"Random Win Rate:  {(double)randomWins / successfulBattles:P2}");
        }

        sb.AppendLine();
        sb.AppendLine("Performance:");
        sb.AppendLine($"Threads: {MctsDlNumThreads}");
        sb.AppendLine($@"Total Time: {stopwatch.Elapsed:hh\:mm\:ss\.fff}");
        sb.AppendLine($"Time per Battle: {totalSeconds / MctsDlEvaluationNumTest * 1000:F1} ms");

        if (allTurns.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Turn Statistics:");
            sb.AppendLine($"Mean: {allTurns.Mean():F2}, Median: {allTurns.Median():F2}");
            sb.AppendLine($"Min: {allTurns.Minimum()}, Max: {allTurns.Maximum()}");
        }

        sb.AppendLine();
        sb.AppendLine(MctsLogger.FormatSummary());

        Console.WriteLine(sb.ToString());

        Console.WriteLine("Press Enter key to exit...");
        if (WaitForInput) Console.ReadLine();
    }

    #endregion

    #region DL-Greedy Evaluation

    /// <summary>
    /// Runs DL-Greedy (argmax policy, no search) vs Random evaluation.
    /// Isolates DL model quality from MCTS search performance.
    /// Includes value head diagnostics via MctsLogger.
    /// </summary>
    private void RunDlGreedyVsRandomEvaluation(FormatId formatId)
    {
        string formatLabel = Library.Formats[formatId].Name;
        Console.WriteLine($"[Driver] Starting DL-Greedy vs Random {formatLabel} Evaluation");
        Console.WriteLine($"[Driver] Running {DlGreedyEvaluationNumTest} battles with {DlGreedyNumThreads} threads");

        if (!File.Exists(MctsModelPath))
        {
            Console.WriteLine($"[Driver] ERROR: Model not found at: {MctsModelPath}");
            Console.WriteLine("[Driver] Export the model to ONNX format using Tools/DLModel/export_onnx.py");
            return;
        }

        if (!File.Exists(MctsVocabPath))
        {
            Console.WriteLine($"[Driver] ERROR: Vocab not found at: {MctsVocabPath}");
            return;
        }

        MctsResources.Initialize(MctsModelPath, MctsVocabPath, Library,
            teamPreviewModelPath: MctsTeamPreviewModelPath);
        Console.WriteLine("[Driver] DL resources initialized");

        // Enable value head diagnostics
        MctsLogger.Reset();
        MctsLogger.Enabled = true;

        const bool debug = false;

        Console.WriteLine("[Driver] Pre-generating teams...");
        EvaluationBattleInput[] battles = new EvaluationBattleInput[DlGreedyEvaluationNumTest];
        for (int i = 0; i < DlGreedyEvaluationNumTest; i++)
        {
            int baseOffset = i * 5 + 1;
            int team1Seed = Team1EvalSeed + baseOffset;
            int team2Seed = Team2EvalSeed + baseOffset + 1;

            List<PokemonSet> team1 = new RandomTeamGenerator(Library, formatId, team1Seed).GenerateTeam();
            List<PokemonSet> team2 = new RandomTeamGenerator(Library, formatId, team2Seed).GenerateTeam();

            battles[i] = new EvaluationBattleInput(
                team1, team2,
                team1Seed, team2Seed,
                PlayerRandom1EvalSeed + baseOffset + 2,
                PlayerRandom2EvalSeed + baseOffset + 3,
                BattleEvalSeed + baseOffset + 4);
        }

        Console.WriteLine("[Driver] Team pre-generation complete. Starting battles...");

        Stopwatch stopwatch = Stopwatch.StartNew();
        int completedBattles = 0;

        List<SimulatorResult> allResults = [];
        List<int> allTurns = [];
        List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)> allExceptions =
            [];

        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = DlGreedyNumThreads,
        };

        Parallel.For(0, DlGreedyEvaluationNumTest, parallelOptions,
            static () => new EvaluationLocalState(),
            (i, _, localState) =>
            {
                EvaluationBattleInput battle = battles[i];

                try
                {
                    (SimulatorResult result, int turn) = RunDLGreedyBattleDirect(
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
                        Console.WriteLine($"[Driver] Completed {completed}/{DlGreedyEvaluationNumTest} battles");
                    }
                }
                catch (Exception ex)
                {
                    localState.Exceptions.Add(
                        (battle.Team1Seed, battle.Team2Seed, battle.Player1Seed, battle.Player2Seed,
                            battle.BattleRandSeed, ex));

                    LogExceptionWithAllSeeds(
                        battle.Team1Seed,
                        battle.Team2Seed,
                        battle.Player1Seed,
                        battle.Player2Seed,
                        battle.BattleRandSeed,
                        ex,
                        nameof(DriverMode.SingleBattleDebug));
                }

                return localState;
            },
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

        MctsLogger.Enabled = false;

        MctsResources.Shutdown();

        int successfulBattles = allResults.Count;
        int failedBattles = allExceptions.Count;
        int dlGreedyWins = allResults.Count(r => r == SimulatorResult.Player1Win);
        int randomWins = allResults.Count(r => r == SimulatorResult.Player2Win);
        int ties = allResults.Count(r => r == SimulatorResult.Tie);

        double totalSeconds = stopwatch.Elapsed.TotalSeconds;

        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine($"DL-Greedy vs Random {formatLabel} Evaluation Results ({DlGreedyEvaluationNumTest} battles):");
        sb.AppendLine($"Format: {formatLabel}");
        sb.AppendLine($"Git Commit: {GetGitCommitId()}");
        sb.AppendLine();
        sb.AppendLine("Execution Summary:");
        sb.AppendLine($"Successful Battles: {successfulBattles}");
        sb.AppendLine($"Failed Battles (Exceptions): {failedBattles}");
        sb.AppendLine($"Total Battles Attempted: {DlGreedyEvaluationNumTest}");
        sb.AppendLine();

        if (failedBattles > 0)
        {
            sb.AppendLine("EXCEPTION SUMMARY:");
            sb.AppendLine("-----------------------------------------------------------");
            for (int i = 0; i < allExceptions.Count; i++)
            {
                (int t1, int t2, int p1, int p2, int b, Exception ex) = allExceptions[i];
                sb.AppendLine($"Exception #{i + 1}: {ex.GetType().Name}: {ex.Message}");
                sb.AppendLine($"  Seeds: T1={t1} T2={t2} P1={p1} P2={p2} B={b}");
            }

            sb.AppendLine("-----------------------------------------------------------");
            sb.AppendLine();
        }

        sb.AppendLine("Battle Results:");
        sb.AppendLine($"DL-Greedy Wins: {dlGreedyWins}");
        sb.AppendLine($"Random Wins:    {randomWins}");
        sb.AppendLine($"Ties:           {ties}");
        if (successfulBattles > 0)
        {
            sb.AppendLine($"DL-Greedy Win Rate: {(double)dlGreedyWins / successfulBattles:P2}");
            sb.AppendLine($"Random Win Rate:    {(double)randomWins / successfulBattles:P2}");
        }

        sb.AppendLine();
        sb.AppendLine("Performance:");
        sb.AppendLine($"Threads: {DlGreedyNumThreads}");
        sb.AppendLine($@"Total Time: {stopwatch.Elapsed:hh\:mm\:ss\.fff}");
        sb.AppendLine($"Time per Battle: {totalSeconds / DlGreedyEvaluationNumTest * 1000:F1} ms");

        if (allTurns.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Turn Statistics:");
            sb.AppendLine($"Mean: {allTurns.Mean():F2}, Median: {allTurns.Median():F2}");
            sb.AppendLine($"Min: {allTurns.Minimum()}, Max: {allTurns.Maximum()}");
        }

        sb.AppendLine();
        sb.AppendLine(MctsLogger.FormatSummary());

        Console.WriteLine(sb.ToString());

        Console.WriteLine("Press Enter key to exit...");
        if (WaitForInput) Console.ReadLine();
    }

    #endregion

    #region Bot-vs-Bot Evaluation

    /// <summary>
    /// Maps a CLI player type string to a <see cref="Player.PlayerType"/> enum.
    /// </summary>
    private static Player.PlayerType ParsePlayerType(string name) => name.ToLowerInvariant() switch
    {
        "random" => Player.PlayerType.Random,
        "greedy" => Player.PlayerType.Greedy,
        "mcts_standalone" or "mctsstandalone" => Player.PlayerType.MctsStandalone,
        "dlgreedy" or "dl_greedy" => Player.PlayerType.DLGreedy,
        "mctsdl" or "mcts_dl" => Player.PlayerType.MctsDL,
        "mcts" => Player.PlayerType.Mcts,
        _ => throw new ArgumentException($"Unknown player type: {name}"),
    };

    /// <summary>
    /// Returns true if the given player type requires DL model resources.
    /// </summary>
    private static bool NeedsDlModel(Player.PlayerType type) =>
        type is Player.PlayerType.DLGreedy or Player.PlayerType.MctsDL or Player.PlayerType.Mcts;

    /// <summary>
    /// Per-battle result for JSON output.
    /// </summary>
    private sealed class BattleResult
    {
        public int Index { get; init; }
        public int P1Side { get; init; }
        public string Result { get; init; } = "";
        public int Turns { get; init; }
    }

    /// <summary>
    /// Runs a configurable bot-vs-bot evaluation between any two player types.
    /// Supports side-swapping: runs half the battles with P1 as side 1, half as side 2.
    /// Outputs structured JSON results for the pipeline orchestrator.
    /// </summary>
    private void RunBotVsBotEvaluation(
        FormatId formatId,
        string player1Name,
        string player2Name,
        int numBattles,
        int mctsIterations,
        int numThreads,
        string outputPath)
    {
        Player.PlayerType p1Type = ParsePlayerType(player1Name);
        Player.PlayerType p2Type = ParsePlayerType(player2Name);
        string formatLabel = Library.Formats[formatId].Name;
        string regulation = GetRegulationName(formatId);

        Console.WriteLine($"[Evaluate] {player1Name} vs {player2Name} — {formatLabel}");
        Console.WriteLine($"[Evaluate] {numBattles} battles, {numThreads} threads, MCTS iterations: {mctsIterations}");

        // Initialize DL resources if needed
        bool needsDl = NeedsDlModel(p1Type) || NeedsDlModel(p2Type);
        if (needsDl)
        {
            string modelPath = Environment.GetEnvironmentVariable("APOGEE_BATTLE_MODEL")
                ?? $"Tools/DLModel/models/{regulation}/battle_model.onnx";
            string vocabPath = Environment.GetEnvironmentVariable("APOGEE_BATTLE_VOCAB")
                ?? $"Tools/DLModel/models/{regulation}/battle_model_vocab.json";
            string previewPath = Environment.GetEnvironmentVariable("APOGEE_PREVIEW_MODEL")
                ?? $"Tools/DLModel/models/{regulation}/team_preview_model.onnx";

            if (!File.Exists(modelPath))
            {
                Console.WriteLine($"[Evaluate] ERROR: Model not found at: {modelPath}");
                return;
            }

            if (!File.Exists(vocabPath))
            {
                Console.WriteLine($"[Evaluate] ERROR: Vocab not found at: {vocabPath}");
                return;
            }

            MctsConfig mctsConfig = new() { NumIterations = mctsIterations };
            MctsResources.Initialize(modelPath, vocabPath, Library, mctsConfig,
                teamPreviewModelPath: File.Exists(previewPath) ? previewPath : null);
            Console.WriteLine("[Evaluate] DL resources initialized");
        }

        // Pre-generate teams
        Console.WriteLine("[Evaluate] Pre-generating teams...");
        EvaluationBattleInput[] battles = new EvaluationBattleInput[numBattles];
        for (int i = 0; i < numBattles; i++)
        {
            int baseOffset = i * 5 + 1;
            int team1Seed = Team1EvalSeed + baseOffset;
            int team2Seed = Team2EvalSeed + baseOffset + 1;

            List<PokemonSet> team1 = new RandomTeamGenerator(Library, formatId, team1Seed).GenerateTeam();
            List<PokemonSet> team2 = new RandomTeamGenerator(Library, formatId, team2Seed).GenerateTeam();

            battles[i] = new EvaluationBattleInput(
                team1, team2,
                team1Seed, team2Seed,
                PlayerRandom1EvalSeed + baseOffset + 2,
                PlayerRandom2EvalSeed + baseOffset + 3,
                BattleEvalSeed + baseOffset + 4);
        }

        Console.WriteLine("[Evaluate] Starting battles...");
        Stopwatch stopwatch = Stopwatch.StartNew();
        int completedBattles = 0;

        MctsConfig? sharedMctsConfig = new() { NumIterations = mctsIterations };
        ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = numThreads };

        // Indexed arrays so results stay ordered despite parallel execution
        SimulatorResult[] orderedResults = new SimulatorResult[numBattles];
        int[] orderedTurns = new int[numBattles];
        bool[] orderedCompleted = new bool[numBattles];
        List<(int Team1Seed, int Team2Seed, int Player1Seed, int Player2Seed, int BattleSeed, Exception Exception)>
            allExceptions = [];

        Parallel.For(0, numBattles, parallelOptions,
            () => new List<(int T1, int T2, int P1, int P2, int B, Exception Error)>(),
            (i, _, localExceptions) =>
            {
                EvaluationBattleInput battle = battles[i];

                // Side swap: first half P1 is side1, second half P1 is side2
                bool swapSides = i >= numBattles / 2;

                Player.PlayerType side1Type = swapSides ? p2Type : p1Type;
                Player.PlayerType side2Type = swapSides ? p1Type : p2Type;
                string side1Name = swapSides ? player2Name : player1Name;
                string side2Name = swapSides ? player1Name : player2Name;

                try
                {
                    PlayerOptions side1Options = new()
                    {
                        Type = side1Type,
                        Name = side1Name,
                        Team = battle.Team1,
                        Seed = new PrngSeed(battle.Player1Seed),
                        PrintDebug = false,
                        MctsConfig = sharedMctsConfig,
                    };

                    PlayerOptions side2Options = new()
                    {
                        Type = side2Type,
                        Name = side2Name,
                        Team = battle.Team2,
                        Seed = new PrngSeed(battle.Player2Seed),
                        PrintDebug = false,
                        MctsConfig = sharedMctsConfig,
                    };

                    BattleOptions battleOptions = new()
                    {
                        Id = formatId,
                        Player1Options = side1Options,
                        Player2Options = side2Options,
                        Debug = false,
                        Sync = true,
                        Seed = new PrngSeed(battle.BattleRandSeed),
                        MaxTurns = 5000,
                    };

                    SimulatorSync simulator = new();
                    SimulatorResult result = simulator.Run(Library, battleOptions, printDebug: false);
                    int turn = simulator.Battle?.Turn ?? 0;

                    orderedResults[i] = result;
                    orderedTurns[i] = turn;
                    orderedCompleted[i] = true;

                    int completed = Interlocked.Increment(ref completedBattles);
                    if (completed % Math.Max(1, numBattles / 20) == 0)
                    {
                        Console.WriteLine($"[Evaluate] Completed {completed}/{numBattles} battles");
                    }
                }
                catch (Exception ex)
                {
                    localExceptions.Add((battle.Team1Seed, battle.Team2Seed,
                        battle.Player1Seed, battle.Player2Seed, battle.BattleRandSeed, ex));
                }

                return localExceptions;
            },
            localExceptions =>
            {
                lock (allExceptions)
                {
                    allExceptions.AddRange(localExceptions);
                }
            });

        stopwatch.Stop();

        if (needsDl)
            MctsResources.Shutdown();

        // Compute stats split by side assignment
        int p1AsSide1Wins = 0, p1AsSide1Losses = 0, p1AsSide1Ties = 0;
        int p1AsSide2Wins = 0, p1AsSide2Losses = 0, p1AsSide2Ties = 0;
        List<BattleResult> perBattle = [];
        List<int> turnsList = [];

        for (int i = 0; i < numBattles; i++)
        {
            if (!orderedCompleted[i]) continue;

            bool swapSides = i >= numBattles / 2;
            SimulatorResult simResult = orderedResults[i];
            int turns = orderedTurns[i];
            turnsList.Add(turns);

            // Determine result from P1's perspective
            string p1Result;
            if (swapSides)
            {
                // P1 is side2
                p1Result = simResult switch
                {
                    SimulatorResult.Player2Win => "win",
                    SimulatorResult.Player1Win => "loss",
                    _ => "tie",
                };
                if (p1Result == "win") p1AsSide2Wins++;
                else if (p1Result == "loss") p1AsSide2Losses++;
                else p1AsSide2Ties++;
            }
            else
            {
                // P1 is side1
                p1Result = simResult switch
                {
                    SimulatorResult.Player1Win => "win",
                    SimulatorResult.Player2Win => "loss",
                    _ => "tie",
                };
                if (p1Result == "win") p1AsSide1Wins++;
                else if (p1Result == "loss") p1AsSide1Losses++;
                else p1AsSide1Ties++;
            }

            perBattle.Add(new BattleResult
            {
                Index = i,
                P1Side = swapSides ? 2 : 1,
                Result = p1Result,
                Turns = turns,
            });
        }

        int totalP1Wins = p1AsSide1Wins + p1AsSide2Wins;
        int totalP2Wins = p1AsSide1Losses + p1AsSide2Losses;
        int totalTies = p1AsSide1Ties + p1AsSide2Ties;
        int successfulBattles = totalP1Wins + totalP2Wins + totalTies;
        double p1WinRate = successfulBattles > 0 ? (double)totalP1Wins / successfulBattles : 0;
        double avgTurns = turnsList.Count > 0 ? turnsList.Average() : 0;

        // Build JSON output
        int halfBattles = numBattles / 2;
        var jsonOutput = new
        {
            player1 = player1Name,
            player2 = player2Name,
            format = regulation,
            mcts_iterations = mctsIterations,
            total_battles = numBattles,
            successful_battles = successfulBattles,
            failed_battles = allExceptions.Count,
            p1_as_side1 = new
            {
                wins = p1AsSide1Wins,
                losses = p1AsSide1Losses,
                ties = p1AsSide1Ties,
                battles = halfBattles,
            },
            p1_as_side2 = new
            {
                wins = p1AsSide2Wins,
                losses = p1AsSide2Losses,
                ties = p1AsSide2Ties,
                battles = numBattles - halfBattles,
            },
            combined = new
            {
                p1_wins = totalP1Wins,
                p2_wins = totalP2Wins,
                ties = totalTies,
                p1_win_rate = Math.Round(p1WinRate, 4),
            },
            avg_turns = Math.Round(avgTurns, 1),
            elapsed_seconds = Math.Round(stopwatch.Elapsed.TotalSeconds, 1),
            per_battle = perBattle.Select(b => new
            {
                index = b.Index,
                p1_side = b.P1Side,
                result = b.Result,
                turns = b.Turns,
            }),
        };

        string json = JsonSerializer.Serialize(jsonOutput, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        });

        // Ensure output directory exists
        string? outputDir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outputDir))
            Directory.CreateDirectory(outputDir);

        File.WriteAllText(outputPath, json);

        // Console summary
        Console.WriteLine();
        Console.WriteLine($"[Evaluate] {player1Name} vs {player2Name} — {successfulBattles} battles completed");
        Console.WriteLine($"[Evaluate] {player1Name} wins: {totalP1Wins} ({p1WinRate:P1})");
        Console.WriteLine($"[Evaluate] {player2Name} wins: {totalP2Wins} ({1 - p1WinRate - (double)totalTies / Math.Max(1, successfulBattles):P1})");
        Console.WriteLine($"[Evaluate] Ties: {totalTies}");
        Console.WriteLine($"[Evaluate] Avg turns: {avgTurns:F1}");
        Console.WriteLine($"[Evaluate] Time: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}");
        Console.WriteLine($"[Evaluate] Results written to: {outputPath}");

        if (allExceptions.Count > 0)
        {
            Console.WriteLine($"[Evaluate] WARNING: {allExceptions.Count} battles failed with exceptions");
        }
    }

    #endregion
}
