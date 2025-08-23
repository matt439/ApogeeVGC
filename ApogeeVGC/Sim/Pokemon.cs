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
        };

        return PokemonValidator.IsValid(library, pokemon) ? pokemon
            : throw new ArgumentException("Invalid Pokemon configuration.");
    }

    public static PokemonSet BuildTestSet(Library library)
    {
        return new PokemonSet()
        {
            Pokemons =
            [
                Build(
                    library,
                    SpecieId.CalyrexIce,
                    [new MoveSetup(MoveId.IceBasic),
                        new MoveSetup(MoveId.PsychicBasic),
                            new MoveSetup(MoveId.GrassBasic),
                                new MoveSetup(MoveId.DarkBasic)],
                    ItemId.Leftovers,
                    AbilityId.AsOneGlastrier,
                    new StatsTable { Hp = 236, Atk = 36, SpD = 236 },
                    NatureType.Adamant
                ),
                Build(
                    library,
                    SpecieId.Miraidon,
                    [new MoveSetup(MoveId.DragonBasic),
                        new MoveSetup(MoveId.ElectricBasic),
                            new MoveSetup(MoveId.FlyingBasic),
                                new MoveSetup(MoveId.PsychicBasic)],
                    ItemId.ChoiceSpecs,
                    AbilityId.HadronEngine,
                    new StatsTable { Hp = 236, Def = 52, SpA = 124, SpD = 68, Spe = 28 },
                    NatureType.Modest
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
                    NatureType.Adamant
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
                    NatureType.Bold
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
                    NatureType.Careful
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
                    NatureType.Adamant
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
    public required Item Item { get; init; }
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
    public bool Shiny { get; init; }
    public MoveType TerraType { get; init; }
    public GenderId Gender { get; init; }
    private StatsTable UnmodifiedStats { get; }
    private StatsTable CurrentStats { get; }
    public int UnmodifiedHp => UnmodifiedStats.Hp;
    public int UnmodifiedAtk => UnmodifiedStats.Atk;
    public int UnmodifiedDef => UnmodifiedStats.Def;
    public int UnmodifiedSpA => UnmodifiedStats.SpA;
    public int UnmodifiedSpD => UnmodifiedStats.SpD;
    public int UnmodifiedSpe => UnmodifiedStats.Spe;
    public int CurrentHp => CurrentStats.Hp;
    public int CurrentAtk => CurrentStats.Atk;
    public int CurrentDef => CurrentStats.Def;
    public int CurrentSpA => CurrentStats.SpA;
    public int CurrentSpD => CurrentStats.SpD;
    public int CurrentSpe => CurrentStats.Spe;
    public double CurrentHpRatio => (double)CurrentHp / UnmodifiedStats.Hp;
    public int CurrentHpPercentage => (int)(CurrentHpRatio * 100);
    public bool IsFainted => CurrentStats.Hp <= 0;

    // Need to have these parameters to calculate the stats correctly
    public Pokemon(Specie specie, StatsTable evs, StatsTable ivs, Nature nature, int level)
    {
        Specie = specie;
        Evs = evs;
        Ivs = ivs;
        Nature = nature;
        Level = level;
        UnmodifiedStats = CalculateUnmodifiedStats();
        CurrentStats = CalculateUnmodifiedStats();
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
            Moves = Moves,        // Immutable, safe to share
            Item = Item,          // Immutable, safe to share
            Ability = Ability,    // Immutable, safe to share
            Name = Name,
            Shiny = Shiny,
            TerraType = TerraType,
            Gender = Gender
        };

        // Copy the current HP state (most important for battle simulation)
        // Calculate the HP difference and apply it to the copy
        int hpDifference = UnmodifiedHp - CurrentHp;
        if (hpDifference > 0)
        {
            copy.Damage(hpDifference);
        }

        // TODO: When status effects, stat boosts, etc. are implemented,
        // they will need to be copied here as well

        return copy;
    }

    public void Heal(int amount)
    {
        CurrentStats.Hp = Math.Min(CurrentStats.Hp + amount, UnmodifiedStats.Hp);
    }

    public void Damage(int amount)
    {
        CurrentStats.Hp = Math.Max(CurrentStats.Hp - amount, 0);
    }

    public int GetAttackStat(Move move)
    {
        return move.Category switch
        {
            MoveCategory.Physical => CurrentAtk,
            MoveCategory.Special => CurrentSpA,
            _ => throw new ArgumentException("Invalid move category.")
        };
    }

    public int GetDefenseStat(Move move)
    {
        return move.Category switch
        {
            MoveCategory.Physical => CurrentDef,
            MoveCategory.Special => CurrentSpD,
            _ => throw new ArgumentException("Invalid move category.")
        };
    }

    public bool IsStab(Move move)
    {
        PokemonType moveType = move.Type.ConvertToPokemonType();
        return moveType == Specie.Types[0] || moveType == Specie.Types[1];
    }

    private int CalculateModifiedStat(StatId stat)
    {
        // TODO: add logic for stat boosts, items, etc.
        return CalculateUnmodifiedStat(stat);
    }

    private StatsTable CalculateModifiedStats()
    {
        return new StatsTable
        {
            Hp = CalculateModifiedStat(StatId.Hp),
            Atk = CalculateModifiedStat(StatId.Atk),
            Def = CalculateModifiedStat(StatId.Def),
            SpA = CalculateModifiedStat(StatId.SpA),
            SpD = CalculateModifiedStat(StatId.SpD),
            Spe = CalculateModifiedStat(StatId.Spe)
        };
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
        string line2 = $"{CurrentStats.Hp}/{UnmodifiedStats.Hp} HP";
        string line3 = $"Ability: {Ability.Name} - Item: {Item.Name}";
        string movesLine = string.Join(", ", Moves.Select(m => m.Name));
        return $"{line1}\n{line2}\n{line3}\nMoves: {movesLine}\n";
    }
}