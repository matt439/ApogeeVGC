using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.SideClasses;

public partial class Side
{
    public Battle Battle { get; }
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

    /// <summary>
    /// The last move selected by this side (used by Copycat, Mirror Move, etc.)
    /// </summary>
    public MoveId? LastSelectedMove { get; set; }

    public Side(Battle battle)
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

    public Side(string name, Battle battle, SideId sideNum, PokemonSet[] team)
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

    public SideRequestData GetRequestData(bool forAlly = false)
    {
        var pokemonData = new List<PokemonSwitchRequestData>(Pokemon.Count);
        foreach (var p in Pokemon)
        {
            pokemonData.Add(p.GetSwitchRequestData(forAlly));
        }

        SideRequestData data = new()
        {
            Name = Name,
            Id = Id,
            Pokemon = pokemonData,
        };
        return data;
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

    /// <summary>
    /// Internal copy constructor for deep copy. Creates a copy of this Side with all
    /// Pokemon deep-copied and registered in pokemonMap. Pokemon references in EffectStates
    /// are deferred to Pass 2 via <see cref="RemapPokemonReferences"/>.
    /// </summary>
    internal Side(Battle newBattle, Side source, Dictionary<Pokemon, Pokemon> pokemonMap)
        : this(newBattle)
    {
        // Override get-only properties set by base constructor
        Id = source.Id;
        N = source.N;
        Name = source.Name;
        Avatar = source.Avatar;

        // Share team data (immutable PokemonSets)
        Team = new List<PokemonSet>(source.Team);

        // Deep copy all Pokemon (Pass 1)
        Pokemon = new List<Pokemon>(source.Pokemon.Count);
        foreach (var pokemon in source.Pokemon)
        {
            Pokemon.Add(pokemon.Copy(this, pokemonMap));
        }

        // Set Active list by remapping from pokemonMap
        Active = source.Active
            .Select(p => p is not null ? pokemonMap[p] : null)
            .ToList();

        // Copy battle state
        PokemonLeft = source.PokemonLeft;
        TotalFainted = source.TotalFainted;
        Initialised = source.Initialised;
        RequestState = source.RequestState;
        LastSelectedMove = source.LastSelectedMove;

        // FaintedLastTurn/ThisTurn are remapped in Pass 2
        FaintedLastTurn = null;
        FaintedThisTurn = null;

        // Deep clone side conditions
        SideConditions = source.SideConditions.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.DeepClone());

        SlotConditions = source.SlotConditions
            .Select(dict => dict.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.DeepClone()))
            .ToList();

        // Fresh choice state
        Choice = CreateDefaultChoice();
        ActiveRequest = null;
    }

    public Side Copy(Battle newBattle, Dictionary<Pokemon, Pokemon> pokemonMap)
    {
        return new Side(newBattle, this, pokemonMap);
    }

    /// <summary>
    /// Pass 2: Remaps all Pokemon references in this Side's conditions and Pokemon.
    /// </summary>
    internal void RemapPokemonReferences(Dictionary<Pokemon, Pokemon> pokemonMap, Side source)
    {
        FaintedLastTurn = EffectState.RemapPokemon(source.FaintedLastTurn, pokemonMap);
        FaintedThisTurn = EffectState.RemapPokemon(source.FaintedThisTurn, pokemonMap);

        // Remap EffectStates in SideConditions and SlotConditions
        foreach (var es in SideConditions.Values)
            es.RemapPokemonReferences(pokemonMap);
        foreach (var dict in SlotConditions)
            foreach (var es in dict.Values)
                es.RemapPokemonReferences(pokemonMap);

        // Remap each Pokemon's references
        for (int i = 0; i < Pokemon.Count; i++)
            Pokemon[i].RemapPokemonReferences(pokemonMap, source.Pokemon[i]);
    }

    public override string ToString() => $"{Id}: {Name}";

    public SidePlayerPerspective GetPlayerPerspective()
    {
        return new SidePlayerPerspective
        {
            Team = Team.AsReadOnly(),
            Pokemon = Pokemon.Select(p => new PokemonPerspective
            {
                Name = p.Name,
                Species = p.Species.Id,
                Level = p.Level,
                Gender = p.Gender,
                Shiny = p.Set.Shiny,
                Hp = p.Hp,
                MaxHp = p.MaxHp,
                Fainted = p.Fainted,
                Status = p.Status,
                MoveSlots = p.MoveSlots.AsReadOnly(),
                Boosts = p.Boosts,
                StoredStats = p.StoredStats,
                Ability = p.Ability,
                Item = p.Item,
                Types = p.Types,
                Terastallized = p.Terastallized,
                TeraType = p.TeraType,
                CanTerastallize = p.CanTerastallize,
                Volatiles = p.Volatiles.Keys.ToList().AsReadOnly(),
                VolatilesWithDuration = p.Volatiles.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Duration
                 ).AsReadOnly(),
                Position = p.Position,
                IsActive = p.IsActive,
            }).ToList().AsReadOnly(),
            Active = Active.Select(p => p == null
                ? null
                : new PokemonPerspective
                {
                    Name = p.Name,
                    Species = p.Species.Id,
                    Level = p.Level,
                    Gender = p.Gender,
                    Shiny = p.Set.Shiny,
                    Hp = p.Hp,
                    MaxHp = p.MaxHp,
                    Fainted = p.Fainted,
                    Status = p.Status,
                    MoveSlots = p.MoveSlots.AsReadOnly(),
                    Boosts = p.Boosts,
                    StoredStats = p.StoredStats,
                    Ability = p.Ability,
                    Item = p.Item,
                    Types = p.Types,
                    Terastallized = p.Terastallized,
                    TeraType = p.TeraType,
                    CanTerastallize = p.CanTerastallize,
                    CanMegaEvo = p.CanMegaEvo,
                    Volatiles = p.Volatiles.Keys.ToList().AsReadOnly(),
                    VolatilesWithDuration = p.Volatiles.ToDictionary(
                     kvp => kvp.Key,
                     kvp => kvp.Value.Duration
                   ).AsReadOnly(),
                    Position = p.Position,
                    IsActive = p.IsActive,
                }).ToList().AsReadOnly(),
            SideConditionsWithDuration = SideConditions.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Duration
            ).AsReadOnly(),
        };
    }

    public SideOpponentPerspective GetOpponentPerspective()
    {
        return new SideOpponentPerspective
        {
            Pokemon = Pokemon.Select(p => new PokemonPerspective
            {
                Name = p.Name,
                Species = p.Species.Id,
                Level = p.Level,
                Gender = p.Gender,
                Shiny = p.Set.Shiny,
                Hp = p.Hp,  // Full observability - exact HP
                MaxHp = p.MaxHp,  // Full observability - exact MaxHp
                Fainted = p.Fainted,
                Status = p.Status,
                MoveSlots = p.MoveSlots.AsReadOnly(),  // Full observability - all moves
                Boosts = p.Boosts,  // Full observability - all boosts
                StoredStats = p.StoredStats,  // Full observability - all stats
                Ability = p.Ability,  // Full observability - ability always visible
                Item = p.Item,  // Full observability - item always visible
                Types = p.Types,
                Terastallized = p.Terastallized,
                TeraType = p.TeraType,
                CanTerastallize = p.CanTerastallize,
                Volatiles = p.Volatiles.Keys.ToList().AsReadOnly(),
                VolatilesWithDuration = p.Volatiles.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Duration
                 ).AsReadOnly(),
                Position = p.Position,
                IsActive = p.IsActive,
            }).ToList().AsReadOnly(),
            Active = Active.Select(p =>
            {
                if (p == null) return null;
                return new PokemonPerspective
                {
                    Name = p.Name,
                    Species = p.Species.Id,
                    Level = p.Level,
                    Gender = p.Gender,
                    Shiny = p.Set.Shiny,
                    Hp = p.Hp,  // Full observability - exact HP
                    MaxHp = p.MaxHp,  // Full observability - exact MaxHp
                    Fainted = p.Fainted,
                    Status = p.Status,
                    MoveSlots = p.MoveSlots.AsReadOnly(),  // Full observability - all moves
                    Boosts = p.Boosts,  // Full observability - all boosts
                    StoredStats = p.StoredStats,  // Full observability - all stats
                    Ability = p.Ability,  // Full observability - ability always visible
                    Item = p.Item,  // Full observability - item always visible
                    Types = p.Types,
                    Terastallized = p.Terastallized,
                    TeraType = p.TeraType,
                    CanTerastallize = p.CanTerastallize,
                    CanMegaEvo = p.CanMegaEvo,
                    Volatiles = p.Volatiles.Keys.ToList().AsReadOnly(),
                    VolatilesWithDuration = p.Volatiles.ToDictionary(
                     kvp => kvp.Key,
                     kvp => kvp.Value.Duration
                   ).AsReadOnly(),
                    Position = p.Position,
                    IsActive = p.IsActive,
                };
            }).ToList().AsReadOnly(),
            SideConditionsWithDuration = SideConditions.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Duration
            ).AsReadOnly(),
        };
    }
}