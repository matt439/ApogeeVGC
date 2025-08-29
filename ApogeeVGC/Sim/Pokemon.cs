using ApogeeVGC.Data;

namespace ApogeeVGC.Sim;


public static class PokemonValidator
{
    public static bool IsValid(Library library, Pokemon pokemon)
    {
        // Implement validation logic for the Pokemon object
        // For example, check if the species, moves, item, ability, etc. are valid
        // This is a placeholder implementation
        
        if (!pokemon.Evs.IsValidEvs())
        {
            throw new ArgumentException("Invalid EVs in Pokemon.");
        }
        if (!pokemon.Ivs.IsValidIvs())
        {
            throw new ArgumentException("Invalid IVs in Pokemon.");
        }

        return true;
    }
}

public struct MoveSetup
{
    public MoveId Id { get; init; }
    public int PpUp
    {
        get;
        init
        {
            if (value is < 0 or > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "PP Up must be between 0 and 3.");
            }
            field = value;
        }
    } = 0;
    public MoveSetup(MoveId id, int ppUp = 0)
    {
        Id = id;
        PpUp = ppUp;
    }
}

public static class PokemonBuilder
{
    public static Pokemon Build(
        Library library,
        SpecieId specie,
        MoveSetup[] moves,
        ItemId item,
        AbilityId ability,
        StatsTable evs,
        NatureType nature,
        bool pringDebug = false,
        StatsTable? ivs = null,
        string? nickname = null,
        bool shiny = false,
        int level = 50)
    {
        List<Move> movesList = [];

        foreach (MoveSetup moveSetup in moves)
        {
            if (!library.Moves.TryGetValue(moveSetup.Id, out Move? move))
            {
                throw new ArgumentException($"Move {moveSetup.Id} not found in library.");
            }
            Move moveCopy = move.Copy(); // Use Copy() to ensure we get a deep copy of the move
            moveCopy.PpUp = moveSetup.PpUp; // Set PP Up
            movesList.Add(moveCopy);
        }

        Specie spec = library.Species[specie] ??
                        throw new ArgumentException($"Specie {specie} not found in library.");
        Nature nat = library.Natures[nature] ??
                        throw new ArgumentException($"Nature {nature} not found in library.");

        Pokemon pokemon = new(spec, evs, ivs ?? StatsTable.PerfectIvs, nat, level)
        {
            Moves = movesList.ToArray(),
            Item = library.Items[item] ?? throw new ArgumentException($"Item {item} not found in library."),
            Ability = library.Abilities[ability] ??
                      throw new ArgumentException($"Ability {ability} not found in library."),
            Evs = evs,
            Name = nickname ?? library.Species[specie].Name,
            Shiny = shiny,
            PrintDebug = pringDebug
        };

        return PokemonValidator.IsValid(library, pokemon) ? pokemon
            : throw new ArgumentException("Invalid Pokemon configuration.");
    }

    public static PokemonSet BuildTestSet(Library library, bool printDebug = false)
    {
        return new PokemonSet()
        {
            Pokemons =
            [
                Build(
                    library,
                    SpecieId.CalyrexIce,
                    [new MoveSetup(MoveId.GlacialLance, 3),
                        new MoveSetup(MoveId.LeechSeed, 1),
                            new MoveSetup(MoveId.TrickRoom, 2),
                                new MoveSetup(MoveId.Protect, 3)],
                    ItemId.Leftovers,
                    AbilityId.AsOneGlastrier,
                    new StatsTable { Hp = 236, Atk = 36, SpD = 236 },
                    NatureType.Adamant,
                    printDebug
                ),
                Build(
                    library,
                    SpecieId.Miraidon,
                    [new MoveSetup(MoveId.VoltSwitch),
                        new MoveSetup(MoveId.DazzlingGleam),
                            new MoveSetup(MoveId.ElectroDrift),
                                new MoveSetup(MoveId.PsychicBasic)],
                    ItemId.ChoiceSpecs,
                    AbilityId.HadronEngine,
                    new StatsTable { Hp = 236, Def = 52, SpA = 124, SpD = 68, Spe = 28 },
                    NatureType.Modest,
                    printDebug
                ),
                Build(
                    library,
                    SpecieId.Ursaluna,
                    [new MoveSetup(MoveId.NormalBasic),
                        new MoveSetup(MoveId.FightingBasic),
                            new MoveSetup(MoveId.GroundBasic),
                                new MoveSetup(MoveId.RockBasic)],
                    ItemId.FlameOrb,
                    AbilityId.Guts,
                    new StatsTable { Hp = 108, Atk = 156, Def = 4, SpD = 116, Spe = 124 },
                    NatureType.Adamant,
                    printDebug
                ),
                Build(
                    library,
                    SpecieId.Volcarona,
                    [new MoveSetup(MoveId.FireBasic),
                        new MoveSetup(MoveId.BugBasic),
                            new MoveSetup(MoveId.FlyingBasic),
                                new MoveSetup(MoveId.PsychicBasic)],
                    ItemId.RockyHelmet,
                    AbilityId.FlameBody,
                    new StatsTable { Hp = 252, Def = 196, SpD = 60 },
                    NatureType.Bold,
                    printDebug
                ),
                Build(
                    library,
                    SpecieId.Grimmsnarl,
                    [new MoveSetup(MoveId.DarkBasic),
                        new MoveSetup(MoveId.FairyBasic),
                            new MoveSetup(MoveId.PsychicBasic),
                                new MoveSetup(MoveId.FightingBasic)],
                    ItemId.LightClay,
                    AbilityId.Prankster,
                    new StatsTable { Hp = 236, Atk = 4, Def = 140, SpD = 116, Spe = 12 },
                    NatureType.Careful,
                    printDebug
                ),
                Build(
                    library,
                    SpecieId.IronHands,
                    [new MoveSetup(MoveId.FightingBasic),
                        new MoveSetup(MoveId.ElectricBasic),
                            new MoveSetup(MoveId.GroundBasic),
                                new MoveSetup(MoveId.PsychicBasic)],
                    ItemId.AssaultVest,
                    AbilityId.QuarkDrive,
                    new StatsTable { Atk = 236, SpD = 236, Spe = 36 },
                    NatureType.Adamant,
                    printDebug
                )
            ]
        };
    }
}

public class PokemonSet
{
    public required Pokemon[] Pokemons
    {
        get;
        init
        {
            if (value == null || value.Length == 0)
            {
                throw new ArgumentException("Pokemon set must contain at least one Pokemon.");
            }
            if (value.Length > 6)
            {
                throw new ArgumentException("Pokemon set cannot contain more than 6 Pokemon.");
            }
            field = value;
        }
    }
    public int PokemonCount => Pokemons.Length;
    public Pokemon[] AlivePokemon => Pokemons.Where(pokemon => !pokemon.IsFainted).ToArray();
    public int AlivePokemonCount => AlivePokemon.Length;
    public int FaintedCount => PokemonCount - AlivePokemonCount;
    public bool AllFainted => AlivePokemonCount == 0;

    /// <summary>
    /// Creates a deep copy of this PokemonSet for MCTS simulation purposes.
    /// </summary>
    /// <returns>A new PokemonSet instance with copied Pokemon</returns>
    public PokemonSet Copy()
    {
        return new PokemonSet
        {
            Pokemons = Pokemons.Select(pokemon => pokemon.Copy()).ToArray()
        };
    }
}

public class Pokemon
{
    public Specie Specie { get; init; }
    public required Move[] Moves
    {
        get;
        init
        {
            if (value == null || value.Length == 0)
            {
                throw new ArgumentException("Pokemon must have at least one move.");
            }
            if (value.Length > 4)
            {
                throw new ArgumentException("Pokemon cannot have more than 4 moves.");
            }
            field = value;
        }
    }
    public Item? Item { get; init; }
    public required Ability Ability { get; init; }
    public StatsTable Evs { get; init; }
    public Nature Nature { get; init; }
    public StatsTable Ivs { get; init; }
    public required string Name { get; init; }
    public int Level
    {
        get;
        init
        {
            if (value is < 1 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Level must be between 1 and 100.");
            }
            field = value;
        }
    }

    public List<Condition> Conditions { get; init; } = [];
    public bool Shiny { get; init; }
    public MoveType TerraType { get; init; }
    public GenderId Gender { get; init; }
    private StatsTable UnmodifiedStats { get; }
    //private StatsTable CurrentStats => new(CalculateModifiedStats());
    public StatModifiers StatModifiers { get; private set; } = new();
    public int UnmodifiedHp => UnmodifiedStats.Hp;
    public int UnmodifiedAtk => UnmodifiedStats.Atk;
    public int UnmodifiedDef => UnmodifiedStats.Def;
    public int UnmodifiedSpA => UnmodifiedStats.SpA;
    public int UnmodifiedSpD => UnmodifiedStats.SpD;
    public int UnmodifiedSpe => UnmodifiedStats.Spe;
    public int CurrentHp { get; private set; } = 0;
    public int CurrentAtk => CalculateModifiedStat(StatId.Atk);
    public int CurrentDef => CalculateModifiedStat(StatId.Def);
    public int CurrentSpA => CalculateModifiedStat(StatId.SpA);
    public int CurrentSpD => CalculateModifiedStat(StatId.SpD);
    public int CurrentSpe => CalculateModifiedStat(StatId.Spe);
    public double CurrentHpRatio => (double)CurrentHp / UnmodifiedHp;
    public int CurrentHpPercentage => (int)Math.Ceiling(CurrentHpRatio * 100);
    public bool IsFainted => CurrentHp <= 0;
    public bool PrintDebug { get; init; }

    // Need to have these parameters to calculate the stats correctly
    public Pokemon(Specie specie, StatsTable evs, StatsTable ivs, Nature nature, int level)
    {
        Specie = specie;
        Evs = evs;
        Ivs = ivs;
        Nature = nature;
        Level = level;
        UnmodifiedStats = CalculateUnmodifiedStats();
        CurrentHp = UnmodifiedStats.Hp; // Start with full HP
    }

    /// <summary>
    /// Creates a deep copy of this Pokemon for MCTS simulation purposes.
    /// This method creates an independent copy with the same state while sharing immutable references.
    /// </summary>
    /// <returns>A new Pokemon instance with copied state</returns>
    public Pokemon Copy()
    {
        // Create a new Pokemon with the same base data
        Pokemon copy = new(Specie, Evs, Ivs, Nature, Level)
        {
            Moves = Moves.Select(m => m.Copy()).ToArray(), // Deep copy of moves
            Conditions = Conditions.Select(c => c.Copy()).ToList(), // Deep copy of conditions
            Item = Item,          // Immutable, safe to share
            Ability = Ability,    // Immutable, safe to share
            Name = Name,
            Shiny = Shiny,
            TerraType = TerraType,
            Gender = Gender,
        };

        // Copy the current HP state (most important for battle simulation)
        // Calculate the HP difference and apply it to the copy
        int hpDifference = UnmodifiedHp - CurrentHp;
        if (hpDifference > 0)
        {
            copy.Damage(hpDifference);
        }

        // Copy stat modifiers using record copy semantics
        copy.StatModifiers = StatModifiers with { };

        // TODO: When status effects, stat boosts, etc. are implemented,
        // they will need to be copied here as well

        return copy;
    }

    /// <summary>
    /// Returns the actual amount healed (could be less than amount if at max HP).
    /// </summary>
    /// <param name="amount">The amount of HP to heal</param>
    /// <returns>The actual amount healed</returns>
    public int Heal(int amount)
    {
        int previousHp = CurrentHp;
        CurrentHp = Math.Min(CurrentHp + amount, UnmodifiedStats.Hp);
        return CurrentHp - previousHp; // Return actual amount healed
    }

    /// <summary>
    /// Returns the actual amount of damage taken (could be less than amount if at 0 HP).
    /// </summary>
    /// <param name="amount">The amount of damage to deal</param>
    /// <returns>The acutual damage dealt</returns>
    public int Damage(int amount)
    {
        int previousHp = CurrentHp;
        CurrentHp = Math.Max(CurrentHp - amount, 0);
        return previousHp - CurrentHp; // Return actual damage taken
    }

    public int GetAttackStat(Move move)
    {
        switch (move.Category)
        {
            case MoveCategory.Physical:
                return CurrentAtk;
            case MoveCategory.Special:
                return CurrentSpA;
            case MoveCategory.Status:
            default:
                throw new ArgumentException("Invalid move category.");
        }
    }

    public int GetDefenseStat(Move move)
    {
        switch (move.Category)
        {
            case MoveCategory.Physical:
                return CurrentDef;
            case MoveCategory.Special:
                return CurrentSpD;
            case MoveCategory.Status:
            default:
                throw new ArgumentException("Invalid move category.");
        }
    }

    public bool IsStab(Move move)
    {
        PokemonType moveType = move.Type.ConvertToPokemonType();
        return moveType == Specie.Types[0] || moveType == Specie.Types[1];
    }

    public bool HasType(PokemonType type)
    {
        return Specie.Types.Contains(type);
    }

    public bool HasType(MoveType type)
    {
        return Specie.Types.Contains(type.ConvertToPokemonType());
    }

    public bool HasCondition(ConditionId conditionId)
    {
        return Conditions.Any(c => c.Id == conditionId);
    }

    public Condition? GetCondition(ConditionId conditionId)
    {
        return Conditions.FirstOrDefault(c => c.Id == conditionId);
    }

    public void AddCondition(Condition condition, BattleContext context, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        Condition? existingCondition = GetCondition(condition.Id);

        if (existingCondition is null)
        {
            Conditions.Add(condition);
            condition.OnStart?.Invoke(this, source, sourceEffect, context);
        }
        else
        {
            existingCondition.OnRestart?.Invoke(this, source, sourceEffect, context);
        }
    }

    public bool RemoveCondition(ConditionId conditionId)
    {
        Condition? condition = GetCondition(conditionId);
        return condition != null && Conditions.Remove(condition);
    }

    public Condition[] GetAllResidualConditions()
    {
        return Conditions.Where(c => c.OnResidual != null).ToArray();
    }

    //public Condition[] GetAllConditionsWithTurnEnd()
    //{
    //    return Conditions.Where(c => c.OnTurnEnd != null).ToArray();
    //}

    public void OnSwitchOut()
    {
        // Clear volatile conditions
        Conditions.RemoveAll(c => c.ConditionVolatility == ConditionVolatility.Volatile);

        // Reset stat modifiers
        StatModifiers = new StatModifiers();
    }

    public void OnSwitchIn()
    {
        //TODO: Trigger any switch-in effects from conditions, abilities, items, etc.
    }

    private int CalculateModifiedStat(StatId stat)
    {
        if (stat == StatId.Hp)
        {
            return UnmodifiedStats.Hp; // HP is not modified by stat stages
        }

        // apply stat modifiers
        double modifier;
        switch (stat)
        {
            case StatId.Atk:
                modifier = StatModifiers.AtkMultiplier;
                break;
            case StatId.Def:
                modifier = StatModifiers.DefMultiplier;
                break;
            case StatId.SpA:
                modifier = StatModifiers.SpAMultiplier;
                break;
            case StatId.SpD:
                modifier = StatModifiers.SpDMultiplier;
                break;
            case StatId.Spe:
                modifier = StatModifiers.SpeMultiplier;
                break;
            case StatId.Hp:
            default:
                throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.");
        }

        // TODO: Implement ability and status condition effects on stats

        //// if speed, check for paralysis
        //if (stat == StatId.Spe && Specie.IsParalyzed)
        //{
        //    modifier *= 0.5;
        //}

        //// if attack, check for burn and if ability is not Guts
        //if (stat == StatId.Atk && Specie.IsBurned && Ability.Id != AbilityId.Guts)
        //{
        //    modifier *= 0.5;
        //}

        //// check for any status and guts ability
        //if (stat == StatId.Atk && Specie.IsStatused && Ability.Id == AbilityId.Guts)
        //{
        //    modifier *= 1.5;
        //}


        return (int)Math.Floor(UnmodifiedStats.GetStat(stat) * modifier);
    }

    private int CalculateUnmodifiedStat(StatId stat)
    {
        int baseStat = Specie.BaseStats.GetStat(stat);
        int iv = Ivs.GetStat(stat);
        int ev = Evs.GetStat(stat);

        if (stat == StatId.Hp)
        {
            
            return (int)Math.Floor((2 * baseStat + iv + Math.Floor(ev / 4.0)) * Level / 100.0) + Level + 10;
        }
        int preNature = (int)Math.Floor((2 * baseStat + iv + Math.Floor(ev / 4.0)) * Level / 100.0) + 5;
        double natureModifier = Nature.GetStatModifier(stat.ConvertToStatIdExceptId());
        return (int)Math.Floor(preNature * natureModifier);
    }

    private StatsTable CalculateUnmodifiedStats()
    {
        return new StatsTable
        {
            Hp = CalculateUnmodifiedStat(StatId.Hp),
            Atk = CalculateUnmodifiedStat(StatId.Atk),
            Def = CalculateUnmodifiedStat(StatId.Def),
            SpA = CalculateUnmodifiedStat(StatId.SpA),
            SpD = CalculateUnmodifiedStat(StatId.SpD),
            Spe = CalculateUnmodifiedStat(StatId.Spe)
        };
    }

    public override string ToString()
    {
        string line1 = $"{Name} ({Specie.Name}) - Lv. {Level}";
        string line2 = $"{CurrentHp}/{UnmodifiedStats.Hp} HP";
        string line3 = $"Ability: {Ability.Name} - Item: {Item.Name}";
        string movesLine = string.Join(", ", Moves.Select(m => m.Name));
        return $"{line1}\n{line2}\n{line3}\nMoves: {movesLine}\n";
    }
}

public record StatModifiers
{
    public int Atk
    {
        get;
        init
        {
            if (!IsValidStage(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
            }

            field = value;
        }
    } = 0;

    public int Def
    {
        get;
        init
        {
            if (!IsValidStage(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
            }

            field = value;
        }
    } = 0;

    public int SpA
    {
        get;
        init
        {
            if (!IsValidStage(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
            }

            field = value;
        }
    } = 0;

    public int SpD
    {
        get;
        init
        {
            if (!IsValidStage(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
            }

            field = value;
        }
    } = 0;

    public int Spe
    {
        get;
        init
        {
            if (!IsValidStage(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
            }

            field = value;
        }
    } = 0;

    public int Accuracy
    {
        get;
        init
        {
            if (!IsValidStage(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
            }

            field = value;
        }
    } = 0;

    public int Evasion
    {
        get;
        init
        {
            if (!IsValidStage(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Stat stage must be between -6 and +6.");
            }

            field = value;
        }
    } = 0;

    public double AtkMultiplier => CalculateRegularStatMultiplier(Atk);
    public double DefMultiplier => CalculateRegularStatMultiplier(Def);
    public double SpAMultiplier => CalculateRegularStatMultiplier(SpA);
    public double SpDMultiplier => CalculateRegularStatMultiplier(SpD);
    public double SpeMultiplier => CalculateRegularStatMultiplier(Spe);
    public double AccuracyMultiplier => CalculateAccuracyStatMultiplier(Accuracy);
    public double EvasionMultiplier => CalculateEvasionStatMultiplier(Evasion);

    private static bool IsValidStage(int stage) => stage is >= -6 and <= 6;

    private static double CalculateRegularStatMultiplier(int stage)
    {
        if (!IsValidStage(stage))
        {
            throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.");
        }

        return stage switch
        {
            -6 => 2.0 / 8.0,
            -5 => 2.0 / 7.0,
            -4 => 2.0 / 6.0,
            -3 => 2.0 / 5.0,
            -2 => 2.0 / 4.0,
            -1 => 2.0 / 3.0,
            0 => 2.0 / 2.0,
            1 => 3.0 / 2.0,
            2 => 4.0 / 2.0,
            3 => 5.0 / 2.0,
            4 => 6.0 / 2.0,
            5 => 7.0 / 2.0,
            6 => 8.0 / 2.0,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.")
        };
    }

    private static double CalculateAccuracyStatMultiplier(int stage)
    {
        if (!IsValidStage(stage))
        {
            throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.");
        }

        return stage switch
        {
            -6 => 3.0 / 9.0,
            -5 => 3.0 / 8.0,
            -4 => 3.0 / 7.0,
            -3 => 3.0 / 6.0,
            -2 => 3.0 / 5.0,
            -1 => 3.0 / 4.0,
            0 => 3.0 / 3.0,
            1 => 4.0 / 3.0,
            2 => 5.0 / 3.0,
            3 => 6.0 / 3.0,
            4 => 7.0 / 3.0,
            5 => 8.0 / 3.0,
            6 => 9.0 / 3.0,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.")
        };
    }

    private static double CalculateEvasionStatMultiplier(int stage)
    {
        if (!IsValidStage(stage))
        {
            throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.");
        }

        return stage switch
        {
            -6 => 9.0 / 3.0,
            -5 => 8.0 / 3.0,
            -4 => 7.0 / 3.0,
            -3 => 6.0 / 3.0,
            -2 => 5.0 / 3.0,
            -1 => 4.0 / 3.0,
            0 => 3.0 / 3.0,
            1 => 3.0 / 4.0,
            2 => 3.0 / 5.0,
            3 => 3.0 / 6.0,
            4 => 3.0 / 7.0,
            5 => 3.0 / 8.0,
            6 => 3.0 / 9.0,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.")
        };
    }
}

public record StatsTable
{
    public int Hp
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }

            field = value;
        }
    }
    public int Atk
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int Def
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int SpA
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int SpD
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int Spe
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;

    public int BaseStatTotal => Hp + Atk + Def + SpA + SpD + Spe;

    public int GetStat(StatId stat)
    {
        return stat switch
        {
            StatId.Hp => Hp,
            StatId.Atk => Atk,
            StatId.Def => Def,
            StatId.SpA => SpA,
            StatId.SpD => SpD,
            StatId.Spe => Spe,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.")
        };
    }

    public static bool IsValidIv(int stat)
    {
        return stat is >= 0 and <= 31;
    }

    public static bool IsValidEv(int stat)
    {
        return stat is >= 0 and <= 255;
    }

    public bool IsValidIvs()
    {
        return IsValidIv(Hp) && IsValidIv(Atk) && IsValidIv(Def) &&
               IsValidIv(SpA) && IsValidIv(SpD) && IsValidIv(Spe);
    }

    public bool IsValidEvs()
    {
        return IsValidEv(Hp) && IsValidEv(Atk) && IsValidEv(Def) &&
               IsValidEv(SpA) && IsValidEv(SpD) && IsValidEv(Spe) &&
               BaseStatTotal <= 510;
    }

    public static StatsTable PerfectIvs => new()
    {
        Hp = 31,
        Atk = 31,
        Def = 31,
        SpA = 31,
        SpD = 31,
        Spe = 31,
    };

    public StatsTable(StatsTable other)
    {
        Hp = other.Hp;
        Atk = other.Atk;
        Def = other.Def;
        SpA = other.SpA;
        SpD = other.SpD;
        Spe = other.Spe;
    }

    public StatsTable()
    {
    }
}