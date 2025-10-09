using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.SideClasses;

public class Side
{
    public IBattle Battle { get; }
    public SideId Id { get; }
    //public int N { get; }

    public required string Name { get; set; }
    public required string Avatar { get; set; }
    //public Side Foe { get; init; } = null!; // set in battle.start()
    //public Side? AllySide { get; init; } = null; // set in battle.start()
    public required List<PokemonSet> Team { get; set; }
    public required List<Pokemon> Pokemon { get; set; }
    public required List<Pokemon> Active { get; set; }
    public Side Foe { get; set; } = null!; // set in battle.start()

    public int PokemonLeft { get; set; }

    public Pokemon? FaintedLastTurn { get; set; }
    public Pokemon? FaintedThisTurn { get; set; }
    public int TotalFainted { get; set; }

    public required Dictionary<ConditionId, EffectState> SideConditions { get; set; }
    public required List<Dictionary<ConditionId, EffectState>> SlotConditions { get; set; }

    public IChoiceRequest? ActiveRequest { get; set; }
    public required Choice Choice { get; set; }

    public Side(string name, IBattle battle, SideId sideNum, PokemonSet[] team)
    {
        // Copy side scripts from battle if needed

        Battle = battle;
        Id = sideNum;
        //N = sideNum;

        Name = name;
        Avatar = string.Empty;

        Team = team.ToList();
        Pokemon = [];
        foreach (PokemonSet set in Team)
        {
            AddPokemon(set);
        }

        Active = battle.GameType switch
        {
            GameType.Doubles => [null!, null!],
            _ => [null!],
        };

        PokemonLeft = Pokemon.Count;

        SideConditions = [];
        SlotConditions = [];

        // Initialize slot conditions for each active slot
        for (int i = 0; i < Active.Count; i++)
        {
            SlotConditions.Add(new Dictionary<ConditionId, EffectState>());
        }

        Choice = new Choice
        {
            CantUndo = false,
            Actions = [],
            ForcedSwitchesLeft = 0,
            ForcedPassesLeft = 0,
            SwitchIns = [],
            Terastallize = false,
        };
    }


    private Pokemon AddPokemon(PokemonSet set)
    {
        throw new NotImplementedException();
    }

    public bool HasAlly(Pokemon pokemon)
    {
        throw new NotImplementedException();
    }
}


//public class Side
//{
//    public required Team Team { get; init; }
//    public required PlayerId PlayerId { get; init; }
//    public required SideId SideId { get; init; }
//    public required GameType GameType { get; init; }
//    public bool PrintDebug { get; init; }

//    // Track whether team preview choice has been made
//    private bool _hasTeamPreviewChoiceBeenMade = false;

//    public required Pokemon Slot1
//    {
//        get;
//        set
//        {
//            value.PokemonSlotId = PokemonSlotId.Slot1;
//            field = value;
//        }
//    }

//    public required Pokemon Slot2
//    {
//        get;
//        set
//        {
//            value.PokemonSlotId = PokemonSlotId.Slot2;
//            field = value;
//        }
//    }

//    public required Pokemon Slot3
//    {
//        get;
//        set
//        {
//            value.PokemonSlotId = PokemonSlotId.Slot3;
//            field = value;
//        }
//    }

//    public required Pokemon Slot4
//    {
//        get;
//        set
//        {
//            value.PokemonSlotId = PokemonSlotId.Slot4;
//            field = value;
//        }
//    }
//    public required Pokemon Slot5
//    {
//        get;
//        set
//        {
//            if (GameType == GameType.Doubles)
//            {
//                throw new InvalidOperationException("Cannot set Slot5 in Doubles format.");
//            }
//            value.PokemonSlotId = PokemonSlotId.Slot5;
//            field = value;
//        }
//    }
//    public required Pokemon Slot6
//    {
//        get;
//        set
//        {
//            if (GameType == GameType.Doubles)
//            {
//                throw new InvalidOperationException("Cannot set Slot6 in Doubles format.");
//            }
//            value.PokemonSlotId = PokemonSlotId.Slot6;
//            field = value;
//        }
//    }

//    public IEnumerable<Pokemon> AllSlots
//    {
//        get
//        {
//            yield return Slot1;
//            yield return Slot2;
//            yield return Slot3;
//            yield return Slot4;
//            if (GameType == GameType.Singles)
//            {
//                yield return Slot5;
//                yield return Slot6;
//            }
//        }
//    }

//    public IEnumerable<Pokemon> ActivePokemon
//    {
//        get
//        {
//            switch (GameType)
//            {
//                case GameType.Singles:
//                    yield return Slot1;
//                    break;
//                case GameType.Doubles:
//                    yield return Slot1;
//                    yield return Slot2;
//                    break;
//                default:
//                    throw new InvalidOperationException("Invalid battle format.");
//            }
//        }
//    }

//    public IEnumerable<Pokemon> AliveActivePokemon => ActivePokemon.Where(p => !p.IsFainted);

//    public IEnumerable<Pokemon> FaintedActivePokemon => ActivePokemon.Where(p => p.IsFainted);

//    public IEnumerable<PokemonSlotId> FaintedActiveSlots => ActivePokemon.Where(p => p.IsFainted).
//        Select(p => p.PokemonSlotId);

//    public bool IsTurnStartSideValid
//    {
//        get
//        {
//            if (AliveActivePokemonCount == 0)
//            {
//                return false; // No active Pokémon alive
//            }
//            if (GameType == GameType.Singles && AliveActivePokemonCount > 1)
//            {
//                return false; // More than one active Pokémon in Singles
//            }
//            if (GameType == GameType.Doubles && AliveActivePokemonCount > 2)
//            {
//                return false; // More than two active Pokémon in Doubles
//            }
//            return true; // Valid state
//        }
//    }

//    //public IEnumerable<(PokemonSlotId, Pokemon)> AllSlotsWithIds
//    //{
//    //    get
//    //    {
//    //        yield return (PokemonSlotId.Slot1, Slot1);
//    //        yield return (PokemonSlotId.Slot2, Slot2);
//    //        yield return (PokemonSlotId.Slot3, Slot3);
//    //        yield return (PokemonSlotId.Slot4, Slot4);
//    //        if (GameType == GameType.Singles)
//    //        {
//    //            yield return (PokemonSlotId.Slot5, Slot5);
//    //            yield return (PokemonSlotId.Slot6, Slot6);
//    //        }
//    //    }
//    //}

//    public IEnumerable<Pokemon> SwitchOptionSlots
//    {
//        get
//        {
//            return GameType switch
//            {
//                GameType.Singles => new[] { Slot2, Slot3, Slot4, Slot5, Slot6 }.Where(p => !p.IsFainted),
//                GameType.Doubles => new[] { Slot3, Slot4 }.Where(p => !p.IsFainted),
//                _ => throw new InvalidOperationException("Invalid battle format."),
//            };
//        }
//    }

//    public int SwitchOptionsCount => SwitchOptionSlots.Count();

//    public bool IsDefeated
//    {
//        get
//        {
//            var allSlots = AllSlots;
//            return allSlots.All(p => p.IsFainted);
//        }
//    }

//    public int HealthTeamTotal
//    {
//        get
//        {
//            var allSlots = AllSlots;
//            return allSlots.Sum(p => p.CurrentHp);
//        }
//    }

//    public bool AnyTeraUsed
//    {
//        get
//        {
//            var allSlots = AllSlots;
//            return allSlots.Any(p => p.IsTeraUsed);
//        }
//    }

//    public int AlivePokemonCount => AllSlots.Count(p => !p.IsFainted);

//    public int AliveActivePokemonCount => ActivePokemon.Count(p => !p.IsFainted);

//    /// <summary>
//    /// Returns true if this side has made their team preview choice.
//    /// Used by MCTS to determine when to advance from team preview to gameplay.
//    /// </summary>
//    public bool HasMadeTeamPreviewChoice() => _hasTeamPreviewChoiceBeenMade;

//    public Pokemon GetSlot(PokemonSlotId pokemonSlotId) => pokemonSlotId switch
//    {
//        PokemonSlotId.Slot1 => Slot1,
//        PokemonSlotId.Slot2 => Slot2,
//        PokemonSlotId.Slot3 => Slot3,
//        PokemonSlotId.Slot4 => Slot4,
//        PokemonSlotId.Slot5 => Slot5,
//        PokemonSlotId.Slot6 => Slot6,
//        _ => null,
//    } ?? throw new InvalidOperationException($"PokemonSlot {pokemonSlotId} is null.");

//    public void SetSlot(PokemonSlotId pokemonSlotId, Pokemon pokemon)
//    {
//        switch (pokemonSlotId)
//        {
//            case PokemonSlotId.Slot1: Slot1 = pokemon; break;
//            case PokemonSlotId.Slot2: Slot2 = pokemon; break;
//            case PokemonSlotId.Slot3: Slot3 = pokemon; break;
//            case PokemonSlotId.Slot4: Slot4 = pokemon; break;
//            case PokemonSlotId.Slot5: Slot5 = pokemon; break;
//            case PokemonSlotId.Slot6: Slot6 = pokemon; break;
//            default:
//                throw new ArgumentOutOfRangeException(nameof(pokemonSlotId), pokemonSlotId, null);
//        }
//    }

//    public void SetSlotsWithCopies(IReadOnlyList<Pokemon> pokemons)
//    {
//        switch (GameType)
//        {
//            case GameType.Singles:
//                if (pokemons.Count != 6)
//                {
//                    throw new ArgumentException("Must provide exactly 6 Pokémon for Singles format.");
//                }
//                Slot1 = pokemons[0].Copy();
//                Slot2 = pokemons[1].Copy();
//                Slot3 = pokemons[2].Copy();
//                Slot4 = pokemons[3].Copy();
//                Slot5 = pokemons[4].Copy();
//                Slot6 = pokemons[5].Copy();
//                break;
//            case GameType.Doubles:

//                if (pokemons.Count != 4)
//                {
//                    throw new ArgumentException("Must provide exactly 4 Pokémon for Doubles format.");
//                }
//                Slot1 = pokemons[0].Copy();
//                Slot2 = pokemons[1].Copy();
//                Slot3 = pokemons[2].Copy();
//                Slot4 = pokemons[3].Copy();
//                break;
//            default:
//                throw new InvalidOperationException("Invalid battle format.");
//        }

//        // Mark that team preview choice has been made
//        _hasTeamPreviewChoiceBeenMade = true;
//    }

//    public void SwitchSlots(PokemonSlotId activeSlot, PokemonSlotId benchSlot)
//    {
//        if (!IsValidActiveSlot(activeSlot))
//        {
//            throw new ArgumentException($"PokemonSlot {activeSlot} is not a valid active slot for {GameType} format.");
//        }
//        if (!IsValidBenchSlot(benchSlot))
//        {
//            throw new ArgumentException($"PokemonSlot {benchSlot} is not a valid bench slot for {GameType} format.");
//        }
//        Pokemon activePokemon = GetSlot(activeSlot);
//        Pokemon benchPokemon = GetSlot(benchSlot);
//        if (benchPokemon.IsFainted)
//        {
//            throw new InvalidOperationException($"Cannot switch in fainted Pokemon in slot {benchSlot}.");
//        }
//        // Perform the switch
//        SetSlot(activeSlot, benchPokemon);
//        SetSlot(benchSlot, activePokemon);
//        // Update their PokemonSlotId properties
//        activePokemon.PokemonSlotId = benchSlot;
//        benchPokemon.PokemonSlotId = activeSlot;
//    }

//    public bool IsActivePokemon(Pokemon pokemon)
//    {
//        return ActivePokemon.Contains(pokemon);
//    }

//    public Pokemon? GetAlly(PokemonSlotId pokemonSlotId)
//    {
//        return GameType switch
//        {
//            GameType.Singles => null, // No allies in Singles,
//            GameType.Doubles => pokemonSlotId switch
//            {
//                PokemonSlotId.Slot1 => Slot2,
//                PokemonSlotId.Slot2 => Slot1,
//                _ => throw new ArgumentException("Only active slots can have allies in Doubles."),
//            },
//            _ => throw new InvalidOperationException("Invalid battle format."),
//        };
//    }

//    public Pokemon? GetAliveAlly(PokemonSlotId pokemonSlotId)
//    {
//        Pokemon? ally = GetAlly(pokemonSlotId);
//        return ally is { IsFainted: false } ? ally : null;
//    }

//    private bool IsValidActiveSlot(PokemonSlotId slot)
//    {
//        return GameType switch
//        {
//            GameType.Singles => slot is PokemonSlotId.Slot1,
//            GameType.Doubles => slot is PokemonSlotId.Slot1 or PokemonSlotId.Slot2,
//            _ => throw new InvalidOperationException("Invalid battle format."),
//        };
//    }

//    private bool IsValidBenchSlot(PokemonSlotId slot)
//    {
//        return GameType switch
//        {
//            GameType.Singles => slot is PokemonSlotId.Slot2 or PokemonSlotId.Slot3 or PokemonSlotId.Slot4 or
//                PokemonSlotId.Slot5 or PokemonSlotId.Slot6,
//            GameType.Doubles => slot is PokemonSlotId.Slot3 or PokemonSlotId.Slot4,
//            _ => throw new InvalidOperationException("Invalid battle format."),
//        };
//    }

//    /// <summary>
//    /// Creates a deep copy of this Side for MCTS simulation purposes.
//    /// </summary>
//    /// <returns>A new Side instance with copied state</returns>
//    public Side Copy()
//    {
//        return new Side
//        {
//            PlayerId = PlayerId, // Value type, safe to copy
//            Team = Team.Copy(),
//            PrintDebug = PrintDebug, // Added missing PrintDebug
//            SideId = SideId, // Value type, safe to copy
//            Slot1 = Slot1.Copy(),
//            Slot2 = Slot2.Copy(),
//            Slot3 = Slot3.Copy(),
//            Slot4 = Slot4.Copy(),
//            Slot5 = Slot5.Copy(),
//            Slot6 = Slot6.Copy(),
//            GameType = GameType,
//            _hasTeamPreviewChoiceBeenMade = _hasTeamPreviewChoiceBeenMade, // Copy the team preview state
//        };
//    }
//}