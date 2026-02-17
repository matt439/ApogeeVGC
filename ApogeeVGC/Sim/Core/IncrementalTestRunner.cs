using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace ApogeeVGC.Sim.Core;

/// <summary>
/// Orchestrates incremental testing of Pokemon battle elements.
/// Slowly expands the pool of allowed elements to identify which cause bugs.
/// </summary>
public class IncrementalTestRunner
{
    private readonly Library _library;
    private readonly DebugElementPools _pools;
    private readonly FormatId _formatId;
    private readonly int _battlesPerVerification;
    private readonly int _battleTimeoutMs;
    private readonly bool _printDebug;
    private readonly string _stateDirectory;

    private int _seedCounter;

    /// <summary>
    /// The current seed counter value. Each battle consumes 3 consecutive seeds.
    /// </summary>
    public int SeedCounter => _seedCounter;

    /// <summary>
    /// Category of element being tested.
    /// Order matters: Items first (fewest dependencies), then Moves, Abilities, Species.
    /// </summary>
    public enum ElementCategory
    {
        Item,
        Move,
        Ability,
        Species,
    }

    /// <summary>
    /// Creates a new incremental test runner.
    /// </summary>
    /// <param name="library">The game data library.</param>
    /// <param name="formatId">The battle format to use.</param>
    /// <param name="battlesPerVerification">Number of successful battles needed to verify elements.</param>
    /// <param name="battleTimeoutMs">Timeout in milliseconds per battle.</param>
    /// <param name="printDebug">Whether to print debug output.</param>
    /// <param name="stateDirectory">Directory to save/load state.</param>
    public IncrementalTestRunner(
        Library library,
        FormatId formatId = FormatId.CustomDoubles,
        int battlesPerVerification = 100,
        int battleTimeoutMs = 3000,
        bool printDebug = false,
        string stateDirectory = ".")
    {
        ArgumentNullException.ThrowIfNull(library);

        _library = library;
        _formatId = formatId;
        _battlesPerVerification = battlesPerVerification;
        _battleTimeoutMs = battleTimeoutMs;
        _printDebug = printDebug;
        _stateDirectory = stateDirectory;

        _pools = new DebugElementPools { VerificationThreshold = battlesPerVerification };
    }

    /// <summary>
    /// Runs the incremental testing loop.
    /// </summary>
    /// <param name="maxIterations">Maximum number of elements to test (0 = unlimited).</param>
    /// <returns>Summary of testing results.</returns>
    public TestingSummary Run(int maxIterations = 0)
    {
        Console.WriteLine("=== Incremental Debug Test Runner ===");
        Console.WriteLine();

        // Try to load existing state
        if (_pools.LoadState(_stateDirectory))
        {
            Console.WriteLine("Loaded existing state from file.");
            // Override the verification threshold with our configured value
            _pools.VerificationThreshold = _battlesPerVerification;
        }
        else
        {
            Console.WriteLine("No existing state found. Initializing baseline...");
            _pools.InitializeBaseline();
            _pools.SaveState(_stateDirectory);
        }

        Console.WriteLine(_pools.GetSummary());
        Console.WriteLine();

        // Validate that the pools can support team generation
        Console.WriteLine(_pools.ValidatePools(_library));
        Console.WriteLine();

        var iterationCount = 0;
        var stopwatch = Stopwatch.StartNew();

        while (maxIterations == 0 || iterationCount < maxIterations)
        {
            // Get untested elements
            var (untestedSpecies, untestedMoves, untestedAbilities, untestedItems) =
                _pools.GetUntestedElements(_library);

            // Check if there are any untested elements left
            if (untestedItems.Count == 0 &&
                untestedMoves.Count == 0 &&
                untestedAbilities.Count == 0 &&
                untestedSpecies.Count == 0)
            {
                Console.WriteLine("All elements have been tested!");
                break;
            }

            // Pick the next element to test based on priority order
            var category = GetNextCategory(untestedItems, untestedMoves, untestedAbilities, untestedSpecies);

            if (category == null)
            {
                // No testable elements available - need more base elements
                Console.WriteLine("No testable elements available. Waiting for dependencies...");
                break;
            }

            // Add one element to testing
            var elementAdded = AddElementToTesting(category.Value,
                untestedItems, untestedMoves, untestedAbilities, untestedSpecies);

            if (!elementAdded)
            {
                Console.WriteLine($"Could not add element for category {category.Value}. Skipping...");
                continue;
            }

            iterationCount++;
            Console.WriteLine($"--- Iteration {iterationCount} ---");
            Console.WriteLine(_pools.GetTestingDetails());

            // Run verification battles
            var result = RunVerificationBattles();

            if (result.Success)
            {
                Console.WriteLine($"? Verified! ({result.BattlesRun} battles successful)");
                _pools.PromoteTestingToAllowed();
            }
            else
            {
                Console.WriteLine($"? FAILED after {result.BattlesRun} battles!");
                Console.WriteLine($"  Exception: {result.Exception?.GetType().Name}: {result.Exception?.Message}");
                Console.WriteLine($"  Seeds: P1={result.Player1Seed}, P2={result.Player2Seed}, Battle={result.BattleSeed}");

                // Save a snapshot of the pools BEFORE marking as failed so
                // RunSingleBattleDebug can reproduce the exact same battle.
                _pools.SaveFailureSnapshot(_stateDirectory);

                _pools.MarkTestingAsFailed();
            }

            // Save state after each iteration
            _pools.SaveState(_stateDirectory);

            Console.WriteLine();
        }

        stopwatch.Stop();

        var summary = new TestingSummary
        {
            IterationsCompleted = iterationCount,
            TotalTime = stopwatch.Elapsed,
            AllowedSpecies = _pools.AllowedSpecies.Count,
            AllowedMoves = _pools.AllowedMoves.Count,
            AllowedAbilities = _pools.AllowedAbilities.Count,
            AllowedItems = _pools.AllowedItems.Count,
            FailedSpecies = _pools.FailedSpecies.ToList(),
            FailedMoves = _pools.FailedMoves.ToList(),
            FailedAbilities = _pools.FailedAbilities.ToList(),
            FailedItems = _pools.FailedItems.ToList(),
        };

        PrintSummary(summary);
        return summary;
    }

    /// <summary>
    /// Gets the next category to test, prioritizing based on dependencies.
    /// </summary>
    private ElementCategory? GetNextCategory(
        List<ItemId> untestedItems,
        List<MoveId> untestedMoves,
        List<AbilityId> untestedAbilities,
        List<SpecieId> untestedSpecies)
    {
        // Priority order: Items -> Moves -> Abilities -> Species
        // Items have fewest dependencies
        // Species require moves and abilities to be available

        if (untestedItems.Count > 0)
        {
            return ElementCategory.Item;
        }

        if (untestedMoves.Count > 0)
        {
            return ElementCategory.Move;
        }

        if (untestedAbilities.Count > 0)
        {
            return ElementCategory.Ability;
        }

        if (untestedSpecies.Count > 0 && CanAddSpecies(untestedSpecies))
        {
            return ElementCategory.Species;
        }

        return null;
    }

    /// <summary>
    /// Checks if any untested species can be added (has enough allowed moves and abilities).
    /// </summary>
    private bool CanAddSpecies(List<SpecieId> untestedSpecies)
    {
        foreach (var speciesId in untestedSpecies)
        {
            if (CanAddSpecificSpecies(speciesId))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if a specific species can be added based on available moves and abilities.
    /// </summary>
    private bool CanAddSpecificSpecies(SpecieId speciesId)
    {
        if (!_library.Species.TryGetValue(speciesId, out var species))
        {
            return false;
        }

        // Skip battle-only formes
        if (species.BattleOnly.HasValue)
        {
            return false;
        }

        // Check if species has at least one allowed ability
        var hasAbility = HasAllowedAbility(species);
        if (!hasAbility)
        {
            return false;
        }

        // Check if species has at least 4 allowed moves
        var hasEnoughMoves = HasEnoughAllowedMoves(speciesId);
        return hasEnoughMoves;
    }

    private bool HasAllowedAbility(Species species)
    {
        var allowed = _pools.AllowedAbilities;

        if (species.Abilities.Slot0 != AbilityId.None && allowed.Contains(species.Abilities.Slot0))
        {
            return true;
        }

        if (species.Abilities.Slot1 is { } slot1 && slot1 != AbilityId.None && allowed.Contains(slot1))
        {
            return true;
        }

        if (species.Abilities.Hidden is { } hidden && hidden != AbilityId.None && allowed.Contains(hidden))
        {
            return true;
        }

        return false;
    }

    private bool HasEnoughAllowedMoves(SpecieId speciesId)
    {
        if (!_library.Learnsets.TryGetValue(speciesId, out var learnset) ||
            learnset.LearnsetData == null)
        {
            return false;
        }

        var allowed = _pools.AllowedMoves;
        var count = learnset.LearnsetData
            .Count(kvp => allowed.Contains(kvp.Key) &&
                          kvp.Value.Any(source => source.Generation == 9));

        return count >= 4;
    }

    /// <summary>
    /// Adds one element from the specified category to testing.
    /// </summary>
    private bool AddElementToTesting(
        ElementCategory category,
        List<ItemId> untestedItems,
        List<MoveId> untestedMoves,
        List<AbilityId> untestedAbilities,
        List<SpecieId> untestedSpecies)
    {
        switch (category)
        {
            case ElementCategory.Item:
                if (untestedItems.Count > 0)
                {
                    _pools.AddToTesting(untestedItems[0]);
                    return true;
                }
                break;

            case ElementCategory.Move:
                if (untestedMoves.Count > 0)
                {
                    _pools.AddToTesting(untestedMoves[0]);
                    return true;
                }
                break;

            case ElementCategory.Ability:
                if (untestedAbilities.Count > 0)
                {
                    _pools.AddToTesting(untestedAbilities[0]);
                    return true;
                }
                break;

            case ElementCategory.Species:
                // Find the first species that can be added
                foreach (var speciesId in untestedSpecies)
                {
                    if (CanAddSpecificSpecies(speciesId))
                    {
                        _pools.AddToTesting(speciesId);
                        return true;
                    }
                }
                break;
        }

        return false;
    }

    /// <summary>
    /// Runs verification battles for the current testing elements.
    /// </summary>
    private VerificationResult RunVerificationBattles()
    {
        var battlesRun = 0;
        var successfulBattles = 0;
        var exceptions = new ConcurrentBag<(int P1Seed, int P2Seed, int BSeed, Exception Ex)>();
        
        Console.WriteLine($"  Running {_battlesPerVerification} verification battles (parallel with {Environment.ProcessorCount} threads)...");

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        Parallel.For(0, _battlesPerVerification, parallelOptions, i =>
        {
            // Generate unique seeds using thread-safe increment
            var p1Seed = Interlocked.Increment(ref _seedCounter);
            var p2Seed = Interlocked.Increment(ref _seedCounter);
            var battleSeed = Interlocked.Increment(ref _seedCounter);

            try
            {
                RunSingleBattle(p1Seed, p2Seed, battleSeed);
                var completed = Interlocked.Increment(ref battlesRun);
                Interlocked.Increment(ref successfulBattles);

                // Print progress every 50 battles
                if ((completed % 50) == 0)
                {
                    Console.WriteLine($"  Progress: {completed}/{_battlesPerVerification} battles");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"  [BATTLE FAILED] Seeds: P1={p1Seed}, P2={p2Seed}, Battle={battleSeed} | {ex.GetType().Name}: {ex.Message}");
                exceptions.Add((p1Seed, p2Seed, battleSeed, ex));
            }
        });

        // Update the pools with successful battle count
        _pools.SuccessfulTestBattles += successfulBattles;

        // Check if any battles failed
        if (!exceptions.IsEmpty)
        {
            var (p1Seed, p2Seed, bSeed, ex) = exceptions.First();
            Console.WriteLine($"  Battle failed: {ex.GetType().Name}");
            return new VerificationResult
            {
                Success = false,
                BattlesRun = battlesRun,
                Player1Seed = p1Seed,
                Player2Seed = p2Seed,
                BattleSeed = bSeed,
                Exception = ex,
            };
        }

        return new VerificationResult
        {
            Success = true,
            BattlesRun = battlesRun,
        };
    }

    /// <summary>
    /// Runs a single battle with the specified seeds.
    /// </summary>
    private void RunSingleBattle(int p1Seed, int p2Seed, int battleSeed)
    {
        Exception? taskException = null;

        var simulationTask = Task.Run(() =>
        {
            try
            {
                // Create teams using debug generator
                var team1Gen = new DebugTeamGenerator(_library, _pools, p1Seed);
                var team2Gen = new DebugTeamGenerator(_library, _pools, p2Seed);

                var team1 = team1Gen.GenerateTeam();
                var team2 = team2Gen.GenerateTeam();

                PlayerOptions player1Options = new()
                {
                    Type = Player.PlayerType.Random,
                    Name = "Debug1",
                    Team = team1,
                    Seed = new PrngSeed(p1Seed),
                    PrintDebug = _printDebug,
                };

                PlayerOptions player2Options = new()
                {
                    Type = Player.PlayerType.Random,
                    Name = "Debug2",
                    Team = team2,
                    Seed = new PrngSeed(p2Seed),
                    PrintDebug = _printDebug,
                };

                BattleOptions battleOptions = new()
                {
                    Id = _formatId,
                    Player1Options = player1Options,
                    Player2Options = player2Options,
                    Debug = _printDebug,
                    Sync = true,
                    Seed = new PrngSeed(battleSeed),
                    MaxTurns = 1000,
                };

                var simulator = new SyncSimulator();
                SimulatorResult result = simulator.Run(_library, battleOptions, printDebug: _printDebug);
                return result;
            }
            catch (Exception ex)
            {
                // Attach seeds to the exception so they are visible in the
                // debugger's exception dialog before any outer catch runs.
                ex.Data["P1Seed"] = p1Seed;
                ex.Data["P2Seed"] = p2Seed;
                ex.Data["BattleSeed"] = battleSeed;
                taskException = ex;
                throw;
            }
        });

        // Wait for simulation with timeout.
        // Task.Wait(int) throws AggregateException when the task faults before
        // the timeout, so we must catch it here to unwrap the real exception.
        bool completedInTime;
        try
        {
            completedInTime = simulationTask.Wait(_battleTimeoutMs);
        }
        catch (AggregateException ae)
        {
            throw taskException
                ?? ae.InnerException
                ?? ae;
        }

        if (!completedInTime)
        {
            throw new BattleTimeoutException(p1Seed, p2Seed, battleSeed, _battleTimeoutMs);
        }

        // Check if task completed with exception (defensive; normally caught above)
        if (simulationTask.IsFaulted)
        {
            throw taskException
                ?? simulationTask.Exception?.InnerException
                ?? (Exception?)simulationTask.Exception
                ?? new InvalidOperationException("Task faulted but no exception captured");
        }

        // Get the result (may throw if task faulted)
        _ = simulationTask.Result;
    }

    private void PrintSummary(TestingSummary summary)
    {
        Console.WriteLine();
        Console.WriteLine("=== Testing Summary ===");
        Console.WriteLine($"Iterations completed: {summary.IterationsCompleted}");
        Console.WriteLine($"Total time: {summary.TotalTime}");
        Console.WriteLine();
        Console.WriteLine("Verified elements:");
        Console.WriteLine($"  Species:   {summary.AllowedSpecies}");
        Console.WriteLine($"  Moves:     {summary.AllowedMoves}");
        Console.WriteLine($"  Abilities: {summary.AllowedAbilities}");
        Console.WriteLine($"  Items:     {summary.AllowedItems}");

        if (summary.FailedSpecies.Count > 0 ||
            summary.FailedMoves.Count > 0 ||
            summary.FailedAbilities.Count > 0 ||
            summary.FailedItems.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Failed elements:");

            if (summary.FailedSpecies.Count > 0)
            {
                Console.WriteLine($"  Species:   {string.Join(", ", summary.FailedSpecies)}");
            }

            if (summary.FailedMoves.Count > 0)
            {
                Console.WriteLine($"  Moves:     {string.Join(", ", summary.FailedMoves)}");
            }

            if (summary.FailedAbilities.Count > 0)
            {
                Console.WriteLine($"  Abilities: {string.Join(", ", summary.FailedAbilities)}");
            }

            if (summary.FailedItems.Count > 0)
            {
                Console.WriteLine($"  Items:     {string.Join(", ", summary.FailedItems)}");
            }
        }

        Console.WriteLine("========================");
    }

    /// <summary>
    /// Result of a verification batch.
    /// </summary>
    private record VerificationResult
    {
        public bool Success { get; init; }
        public int BattlesRun { get; init; }
        public int Player1Seed { get; init; }
        public int Player2Seed { get; init; }
        public int BattleSeed { get; init; }
        public Exception? Exception { get; init; }
    }

    /// <summary>
    /// Summary of testing results.
    /// </summary>
    public record TestingSummary
    {
        public int IterationsCompleted { get; init; }
        public TimeSpan TotalTime { get; init; }
        public int AllowedSpecies { get; init; }
        public int AllowedMoves { get; init; }
        public int AllowedAbilities { get; init; }
        public int AllowedItems { get; init; }
        public List<SpecieId> FailedSpecies { get; init; } = [];
        public List<MoveId> FailedMoves { get; init; } = [];
        public List<AbilityId> FailedAbilities { get; init; } = [];
        public List<ItemId> FailedItems { get; init; } = [];
    }
}
