using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using System.Text.Json;

namespace ApogeeVGC.Sim.Generators;

/// <summary>
/// Manages whitelisted elements for incremental debug testing.
/// Allows controlled expansion of species, moves, abilities, and items
/// to isolate which newly added element causes bugs.
/// </summary>
public class DebugElementPools
{
    private const string StateFileName = "debug-element-pools-state.json";

    /// <summary>
    /// Species that have been verified to work correctly.
    /// </summary>
    public HashSet<SpecieId> AllowedSpecies { get; private set; } = [];

    /// <summary>
    /// Moves that have been verified to work correctly.
    /// </summary>
    public HashSet<MoveId> AllowedMoves { get; private set; } = [];

    /// <summary>
    /// Abilities that have been verified to work correctly.
    /// </summary>
    public HashSet<AbilityId> AllowedAbilities { get; private set; } = [];

    /// <summary>
    /// Items that have been verified to work correctly.
    /// </summary>
    public HashSet<ItemId> AllowedItems { get; private set; } = [];

    /// <summary>
    /// Elements currently being tested (not yet verified).
    /// </summary>
    public HashSet<SpecieId> TestingSpecies { get; private set; } = [];
    public HashSet<MoveId> TestingMoves { get; private set; } = [];
    public HashSet<AbilityId> TestingAbilities { get; private set; } = [];
    public HashSet<ItemId> TestingItems { get; private set; } = [];

    /// <summary>
    /// Elements that have been identified as problematic.
    /// </summary>
    public HashSet<SpecieId> FailedSpecies { get; private set; } = [];
    public HashSet<MoveId> FailedMoves { get; private set; } = [];
    public HashSet<AbilityId> FailedAbilities { get; private set; } = [];
    public HashSet<ItemId> FailedItems { get; private set; } = [];

    /// <summary>
    /// Number of successful battles run with current testing elements.
    /// </summary>
    public int SuccessfulTestBattles { get; set; }

    /// <summary>
    /// Threshold of successful battles before promoting testing elements to allowed.
    /// </summary>
    public int VerificationThreshold { get; set; } = 100;

    /// <summary>
    /// Gets all species available for team generation (allowed + testing).
    /// </summary>
    public IReadOnlySet<SpecieId> AvailableSpecies =>
        AllowedSpecies.Union(TestingSpecies).ToHashSet();

    /// <summary>
    /// Gets all moves available for team generation (allowed + testing).
    /// </summary>
    public IReadOnlySet<MoveId> AvailableMoves =>
        AllowedMoves.Union(TestingMoves).ToHashSet();

    /// <summary>
    /// Gets all abilities available for team generation (allowed + testing).
    /// </summary>
    public IReadOnlySet<AbilityId> AvailableAbilities =>
        AllowedAbilities.Union(TestingAbilities).ToHashSet();

    /// <summary>
    /// Gets all items available for team generation (allowed + testing).
    /// </summary>
    public IReadOnlySet<ItemId> AvailableItems =>
        AllowedItems.Union(TestingItems).ToHashSet();

    /// <summary>
    /// Initializes pools with the baseline elements from GenerateTestTeam.
    /// These are confirmed to work over 100,000+ battles.
    /// </summary>
    public void InitializeBaseline()
    {
        // Baseline species from GenerateTestTeam
        AllowedSpecies =
        [
            SpecieId.CalyrexIce,
            SpecieId.Miraidon,
            SpecieId.Ursaluna,
            SpecieId.Volcarona,
            SpecieId.Grimmsnarl,
            SpecieId.IronHands,
        ];

        // Baseline moves from GenerateTestTeam
        AllowedMoves =
        [
            MoveId.GlacialLance,
            MoveId.LeechSeed,
            MoveId.TrickRoom,
            MoveId.Protect,
            MoveId.VoltSwitch,
            MoveId.DazzlingGleam,
            MoveId.ElectroDrift,
            MoveId.DracoMeteor,
            MoveId.Facade,
            MoveId.Crunch,
            MoveId.HeadlongRush,
            MoveId.StruggleBug,
            MoveId.Overheat,
            MoveId.Tailwind,
            MoveId.SpiritBreak,
            MoveId.ThunderWave,
            MoveId.Reflect,
            MoveId.LightScreen,
            MoveId.FakeOut,
            MoveId.HeavySlam,
            MoveId.LowKick,
            MoveId.WildCharge,
        ];

        // Baseline abilities from GenerateTestTeam
        AllowedAbilities =
        [
            AbilityId.AsOneGlastrier,
            AbilityId.HadronEngine,
            AbilityId.Guts,
            AbilityId.FlameBody,
            AbilityId.Prankster,
            AbilityId.QuarkDrive,
        ];

        // Baseline items from GenerateTestTeam
        AllowedItems =
        [
            ItemId.Leftovers,
            ItemId.ChoiceSpecs,
            ItemId.FlameOrb,
            ItemId.RockyHelmet,
            ItemId.LightClay,
            ItemId.AssaultVest,
        ];

        // Clear testing and failed sets
        TestingSpecies.Clear();
        TestingMoves.Clear();
        TestingAbilities.Clear();
        TestingItems.Clear();
        FailedSpecies.Clear();
        FailedMoves.Clear();
        FailedAbilities.Clear();
        FailedItems.Clear();
        SuccessfulTestBattles = 0;
    }

    /// <summary>
    /// Adds a new element to the testing pool.
    /// </summary>
    public void AddToTesting(SpecieId species)
    {
        if (!AllowedSpecies.Contains(species) && !FailedSpecies.Contains(species))
        {
            TestingSpecies.Add(species);
            SuccessfulTestBattles = 0;
        }
    }

    public void AddToTesting(MoveId move)
    {
        if (!AllowedMoves.Contains(move) && !FailedMoves.Contains(move))
        {
            TestingMoves.Add(move);
            SuccessfulTestBattles = 0;
        }
    }

    public void AddToTesting(AbilityId ability)
    {
        if (!AllowedAbilities.Contains(ability) && !FailedAbilities.Contains(ability))
        {
            TestingAbilities.Add(ability);
            SuccessfulTestBattles = 0;
        }
    }

    public void AddToTesting(ItemId item)
    {
        if (!AllowedItems.Contains(item) && !FailedItems.Contains(item))
        {
            TestingItems.Add(item);
            SuccessfulTestBattles = 0;
        }
    }

    /// <summary>
    /// Promotes all testing elements to allowed after successful verification.
    /// </summary>
    public void PromoteTestingToAllowed()
    {
        foreach (var species in TestingSpecies)
        {
            AllowedSpecies.Add(species);
        }
        foreach (var move in TestingMoves)
        {
            AllowedMoves.Add(move);
        }
        foreach (var ability in TestingAbilities)
        {
            AllowedAbilities.Add(ability);
        }
        foreach (var item in TestingItems)
        {
            AllowedItems.Add(item);
        }

        TestingSpecies.Clear();
        TestingMoves.Clear();
        TestingAbilities.Clear();
        TestingItems.Clear();
        SuccessfulTestBattles = 0;
    }

    /// <summary>
    /// Marks all testing elements as failed after a bug is detected.
    /// </summary>
    public void MarkTestingAsFailed()
    {
        foreach (var species in TestingSpecies)
        {
            FailedSpecies.Add(species);
        }
        foreach (var move in TestingMoves)
        {
            FailedMoves.Add(move);
        }
        foreach (var ability in TestingAbilities)
        {
            FailedAbilities.Add(ability);
        }
        foreach (var item in TestingItems)
        {
            FailedItems.Add(item);
        }

        TestingSpecies.Clear();
        TestingMoves.Clear();
        TestingAbilities.Clear();
        TestingItems.Clear();
        SuccessfulTestBattles = 0;
    }

    /// <summary>
    /// Gets all untested elements from the library.
    /// </summary>
    public (List<SpecieId> Species, List<MoveId> Moves, List<AbilityId> Abilities, List<ItemId> Items)
        GetUntestedElements(Library library)
    {
        var untestedSpecies = library.Species.Keys
            .Where(s => !AllowedSpecies.Contains(s) &&
                        !TestingSpecies.Contains(s) &&
                        !FailedSpecies.Contains(s))
            .ToList();

        var untestedMoves = library.Moves.Keys
            .Where(m => !AllowedMoves.Contains(m) &&
                        !TestingMoves.Contains(m) &&
                        !FailedMoves.Contains(m))
            .ToList();

        var untestedAbilities = library.Abilities.Keys
            .Where(a => !AllowedAbilities.Contains(a) &&
                        !TestingAbilities.Contains(a) &&
                        !FailedAbilities.Contains(a))
            .ToList();

        var untestedItems = library.Items.Keys
            .Where(i => !AllowedItems.Contains(i) &&
                        !TestingItems.Contains(i) &&
                        !FailedItems.Contains(i))
            .ToList();

        return (untestedSpecies, untestedMoves, untestedAbilities, untestedItems);
    }

    /// <summary>
    /// Gets a summary of the current pool state.
    /// </summary>
    public string GetSummary()
    {
        return $"""
            Debug Element Pools Summary:
            ----------------------------
            Allowed:  {AllowedSpecies.Count} species, {AllowedMoves.Count} moves, {AllowedAbilities.Count} abilities, {AllowedItems.Count} items
            Testing:  {TestingSpecies.Count} species, {TestingMoves.Count} moves, {TestingAbilities.Count} abilities, {TestingItems.Count} items
            Failed:   {FailedSpecies.Count} species, {FailedMoves.Count} moves, {FailedAbilities.Count} abilities, {FailedItems.Count} items
            Successful test battles: {SuccessfulTestBattles}/{VerificationThreshold}
            """;
    }

    /// <summary>
    /// Gets details about testing elements (for debugging when failure occurs).
    /// </summary>
    public string GetTestingDetails()
    {
        var details = new List<string>();

        if (TestingSpecies.Count > 0)
            details.Add($"Testing Species: {string.Join(", ", TestingSpecies)}");
        if (TestingMoves.Count > 0)
            details.Add($"Testing Moves: {string.Join(", ", TestingMoves)}");
        if (TestingAbilities.Count > 0)
            details.Add($"Testing Abilities: {string.Join(", ", TestingAbilities)}");
        if (TestingItems.Count > 0)
            details.Add($"Testing Items: {string.Join(", ", TestingItems)}");

        return details.Count > 0
            ? string.Join(Environment.NewLine, details)
            : "No elements currently being tested.";
    }

    /// <summary>
    /// Validates that the current pools can support team generation.
    /// Returns diagnostic info about which species are usable.
    /// </summary>
    public string ValidatePools(Library library)
    {
        var lines = new List<string> { "Pool Validation:" };
        var usableSpecies = new List<SpecieId>();

        foreach (var speciesId in AvailableSpecies)
        {
            if (!library.Species.TryGetValue(speciesId, out var species))
            {
                lines.Add($"  {speciesId}: NOT FOUND in library");
                continue;
            }

            // Check abilities
            var hasAbility = false;
            var abilityInfo = new List<string>();
            if (species.Abilities.Slot0 != AbilityId.None)
            {
                var inPool = AvailableAbilities.Contains(species.Abilities.Slot0);
                abilityInfo.Add($"{species.Abilities.Slot0}:{(inPool ? "?" : "?")}");
                hasAbility |= inPool;
            }
            if (species.Abilities.Slot1 is { } slot1 && slot1 != AbilityId.None)
            {
                var inPool = AvailableAbilities.Contains(slot1);
                abilityInfo.Add($"{slot1}:{(inPool ? "?" : "?")}");
                hasAbility |= inPool;
            }
            if (species.Abilities.Hidden is { } hidden && hidden != AbilityId.None)
            {
                var inPool = AvailableAbilities.Contains(hidden);
                abilityInfo.Add($"{hidden}:{(inPool ? "?" : "?")}");
                hasAbility |= inPool;
            }

            // Check moves
            var moveCount = 0;
            if (library.Learnsets.TryGetValue(speciesId, out var learnset) && learnset.LearnsetData != null)
            {
                moveCount = learnset.LearnsetData
                    .Count(kvp => AvailableMoves.Contains(kvp.Key) &&
                                  kvp.Value.Any(source => source.Generation == 9));
            }

            var usable = hasAbility && moveCount >= 4;
            if (usable) usableSpecies.Add(speciesId);

            lines.Add($"  {speciesId}: Abilities[{string.Join(",", abilityInfo)}] Moves:{moveCount}/4 => {(usable ? "USABLE" : "NOT USABLE")}");
        }

        lines.Add($"Total usable species: {usableSpecies.Count}/6 required");
        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Saves the current state to a JSON file.
    /// </summary>
    public void SaveState(string directory = ".")
    {
        var state = new DebugElementPoolsState
        {
            AllowedSpecies = [.. AllowedSpecies],
            AllowedMoves = [.. AllowedMoves],
            AllowedAbilities = [.. AllowedAbilities],
            AllowedItems = [.. AllowedItems],
            TestingSpecies = [.. TestingSpecies],
            TestingMoves = [.. TestingMoves],
            TestingAbilities = [.. TestingAbilities],
            TestingItems = [.. TestingItems],
            FailedSpecies = [.. FailedSpecies],
            FailedMoves = [.. FailedMoves],
            FailedAbilities = [.. FailedAbilities],
            FailedItems = [.. FailedItems],
            SuccessfulTestBattles = SuccessfulTestBattles,
            VerificationThreshold = VerificationThreshold,
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(state, options);
        var filePath = Path.Combine(directory, StateFileName);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Loads state from a JSON file if it exists.
    /// Returns true if state was loaded, false if starting fresh.
    /// </summary>
    public bool LoadState(string directory = ".")
    {
        var filePath = Path.Combine(directory, StateFileName);
        if (!File.Exists(filePath))
        {
            return false;
        }

        var json = File.ReadAllText(filePath);
        var state = JsonSerializer.Deserialize<DebugElementPoolsState>(json);
        if (state == null)
        {
            return false;
        }

        AllowedSpecies = [.. state.AllowedSpecies];
        AllowedMoves = [.. state.AllowedMoves];
        AllowedAbilities = [.. state.AllowedAbilities];
        AllowedItems = [.. state.AllowedItems];
        TestingSpecies = [.. state.TestingSpecies];
        TestingMoves = [.. state.TestingMoves];
        TestingAbilities = [.. state.TestingAbilities];
        TestingItems = [.. state.TestingItems];
        FailedSpecies = [.. state.FailedSpecies];
        FailedMoves = [.. state.FailedMoves];
        FailedAbilities = [.. state.FailedAbilities];
        FailedItems = [.. state.FailedItems];
        SuccessfulTestBattles = state.SuccessfulTestBattles;
        VerificationThreshold = state.VerificationThreshold;

        return true;
    }

    /// <summary>
    /// Serializable state for JSON persistence.
    /// </summary>
    private record DebugElementPoolsState
    {
        public List<SpecieId> AllowedSpecies { get; init; } = [];
        public List<MoveId> AllowedMoves { get; init; } = [];
        public List<AbilityId> AllowedAbilities { get; init; } = [];
        public List<ItemId> AllowedItems { get; init; } = [];
        public List<SpecieId> TestingSpecies { get; init; } = [];
        public List<MoveId> TestingMoves { get; init; } = [];
        public List<AbilityId> TestingAbilities { get; init; } = [];
        public List<ItemId> TestingItems { get; init; } = [];
        public List<SpecieId> FailedSpecies { get; init; } = [];
        public List<MoveId> FailedMoves { get; init; } = [];
        public List<AbilityId> FailedAbilities { get; init; } = [];
        public List<ItemId> FailedItems { get; init; } = [];
        public int SuccessfulTestBattles { get; init; }
        public int VerificationThreshold { get; init; }
    }
}
