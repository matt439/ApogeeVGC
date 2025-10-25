using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using System.Text.Json.Nodes;

namespace ApogeeVGC.Sim.SideClasses;

public partial class Side : IDisposable
{
    private bool _disposed;
    public IBattle Battle { get; }
    public SideId Id { get; }
    public int N { get; set; }

    public string Name { get; set; }
    public string Avatar { get; set; }

    // Only used in multi battles so not implemented in this program
    // public Side? AllySide { get; init; } = null;
    public List<PokemonSet> Team { get; set; }
    public List<Pokemon> Pokemon { get; set; }
    public List<Pokemon?> Active { get; set; }
    
    // Foe is set during Battle.Start(), not during construction
    // This is nullable until Start() is called, but should never be accessed before then
    public Side Foe { get; set; } = null!;

    public int PokemonLeft { get; set; }

    public Pokemon? FaintedLastTurn { get; set; }
    public Pokemon? FaintedThisTurn { get; set; }
    public int TotalFainted { get; set; }

    public Dictionary<ConditionId, EffectState> SideConditions { get; set; }
    public List<Dictionary<ConditionId, EffectState>> SlotConditions { get; set; }

    public IChoiceRequest? ActiveRequest { get; set; }
    public Choice Choice { get; set; }
    public bool Initialised { get; init; }

    public RequestState RequestState { get; set; }

    public Side(IBattle battle)
    {
        Battle = battle;
        Id = SideId.P1;
        Name = string.Empty;
        Avatar = string.Empty;
        Team = [];
        Pokemon = [];

        // Initialize Active list with proper size based on game type
        // Start with null entries that will be filled when Pokemon are switched in
        Active = battle.GameType switch
        {
            GameType.Doubles => [null, null],
            _ => [null],
        };

        PokemonLeft = 0;
        SideConditions = [];
        SlotConditions = [];

        // Initialize slot conditions for each active slot
        for (int i = 0; i < Active.Count; i++)
        {
            SlotConditions.Add(new Dictionary<ConditionId, EffectState>());
        }

        Choice = CreateDefaultChoice();
        Initialised = false;
    }

    public Side(string name, IBattle battle, SideId sideNum, PokemonSet[] team)
        : this(battle)
    {
        // Override values set by base constructor
        Id = sideNum;
        Name = name;

        // Add team-specific initialization
        Team = team.ToList();
        foreach (PokemonSet set in Team)
        {
            AddPokemon(set);
        }

        PokemonLeft = Pokemon.Count;
        Initialised = true;
    }

    private Choice CreateDefaultChoice()
    {
        return new Choice
        {
            CantUndo = false,
            Actions = [],
            ForcedSwitchesLeft = 0,
            ForcedPassesLeft = 0,
            SwitchIns = [],
            Terastallize = false,
        };
    }

    public JsonObject ToJson()
    {
        throw new NotImplementedException();
    }

    private Pokemon? AddPokemon(PokemonSet set)
    {
        if (Pokemon.Count >= 24) return null;
        var newPokemon = new Pokemon(Battle, set, this)
        {
            Position = Pokemon.Count,
        };
        Pokemon.Add(newPokemon);
        PokemonLeft++;
        return newPokemon;
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }

    public SideRequestData GetRequestData(bool forAlly = false)
    {
        SideRequestData data = new()
        {
            Name = Name,
            Id = Id,
            Pokemon = Pokemon.Select(p => p.GetSwitchRequestData(forAlly)).ToList(),
        };
        return data;
    }

    /// <summary>
    /// Disposes all resources held by this Side instance.
    /// Cleans up Pokemon references and choice actions to break circular references.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Dispose children and clear references to them
            foreach (Pokemon pokemon in Pokemon)
            {
                if (pokemon is IDisposable disposablePokemon)
                {
                    disposablePokemon.Dispose();
                }
            }

            // Clear choice actions by replacing with an empty list
            // Note: ChosenAction is a record with init-only properties,
            // so we can't modify individual instances. We replace the entire list instead.
            Choice.Actions = [];

            // Clear circular/large references
            Pokemon.Clear();
            Active.Clear();
        }

        _disposed = true;
    }

    /// <summary>
    /// Legacy method for compatibility. Calls Dispose().
    /// </summary>
    public void Destroy()
    {
        Dispose();
    }

    /// <summary>
    /// Gets a Pokemon from the Active list at the specified index.
    /// Throws an exception if the slot is null, as this indicates an invalid battle state.
    /// </summary>
    /// <param name="index">The active slot index (0-based)</param>
    /// <returns>The Pokemon in the specified slot</returns>
    /// <exception cref="InvalidOperationException">Thrown if the slot is empty (null)</exception>
    public Pokemon GetActiveAt(int index)
    {
        if (index < 0 || index >= Active.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index),
                $"Active slot index {index} is out of range. Valid range: 0-{Active.Count - 1}");
        }

        return Active[index] ?? throw new InvalidOperationException(
            $"Active slot {index} for {Name} (Side {Id}) is empty. " +
            "This indicates an invalid battle state where a Pokemon should be present.");
    }
}