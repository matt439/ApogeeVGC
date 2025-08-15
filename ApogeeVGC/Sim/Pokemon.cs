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

        return pokemon != null;
    }
}

public static class PokemonBuilder
{
    public static Pokemon Build(
        Library library,
        SpecieId specie,
        IEnumerable<MoveId> moves,
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
        foreach (MoveId moveId in moves)
        {
            if (!library.Moves.TryGetValue(moveId, out Move? move))
            {
                throw new ArgumentException($"Move {moveId} not found in library.");
            }
            movesList.Add(move);
        }

        Specie spec = library.Species[specie] ??
                        throw new ArgumentException($"Specie {specie} not found in library.");
        Nature nat = library.Natures[nature] ??
                        throw new ArgumentException($"Nature {nature} not found in library.");

        Pokemon pokemon = new(spec, evs, ivs ?? StatsTable.PerfectIvs, nat)
        {
            Moves = movesList,
            Item = library.Items[item] ?? throw new ArgumentException($"Item {item} not found in library."),
            Ability = library.Abilities[ability] ??
                      throw new ArgumentException($"Ability {ability} not found in library."),
            Evs = evs,
            Name = nickname ?? library.Species[specie].Name,
            Shiny = shiny,
            Level = level,
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
                    [MoveId.IceBasic, MoveId.PsychicBasic, MoveId.GrassBasic, MoveId.DarkBasic],
                    ItemId.Leftovers,
                    AbilityId.AsOneGlastrier,
                    new StatsTable { Hp = 236, Atk = 36, SpD = 236 },
                    NatureType.Adamant
                ),
                Build(
                    library,
                    SpecieId.Miraidon,
                    [MoveId.DragonBasic, MoveId.ElectricBasic, MoveId.FlyingBasic, MoveId.PsychicBasic],
                    ItemId.ChoiceSpecs,
                    AbilityId.HadronEngine,
                    new StatsTable { Hp = 236, Def = 52, SpA = 124, SpD = 68, Spe = 28 },
                    NatureType.Modest
                ),
                Build(
                    library,
                    SpecieId.Ursaluna,
                    [MoveId.NormalBasic, MoveId.FightingBasic, MoveId.GroundBasic, MoveId.RockBasic],
                    ItemId.FlameOrb,
                    AbilityId.Guts,
                    new StatsTable { Hp = 108, Atk = 156, Def = 4, SpD = 116, Spe = 124 },
                    NatureType.Adamant
                ),
                Build(
                    library,
                    SpecieId.Volcarona,
                    [MoveId.FireBasic, MoveId.BugBasic, MoveId.FlyingBasic, MoveId.PsychicBasic],
                    ItemId.RockyHelmet,
                    AbilityId.FlameBody,
                    new StatsTable { Hp = 252, Def = 196, SpD = 60 },
                    NatureType.Bold
                ),
                Build(
                    library,
                    SpecieId.Grimmsnarl,
                    [MoveId.DarkBasic, MoveId.FairyBasic, MoveId.PsychicBasic, MoveId.FightingBasic],
                    ItemId.LightClay,
                    AbilityId.Prankster,
                    new StatsTable { Hp = 236, Atk = 4, Def = 140, SpD = 116, Spe = 12 },
                    NatureType.Careful
                ),
                Build(
                    library,
                    SpecieId.IronHands,
                    [MoveId.FightingBasic, MoveId.ElectricBasic, MoveId.GroundBasic, MoveId.PsychicBasic],
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
    public List<Pokemon> Pokemons { get; init; } = [];
}

public class Pokemon
{
    public Specie Specie { get; init; }
    public required IEnumerable<Move> Moves { get; init; }
    public required Item Item { get; init; }
    public required Ability Ability { get; init; }
    public StatsTable Evs { get; init; }
    public Nature Nature { get; init; }
    public StatsTable Ivs { get; init; }
    public required string Name { get; init; }
    public int Level { get; init; }
    public bool Shiny { get; init; }
    public MoveType TerraType { get; init; }
    public StatsTable UnmodifiedStats { get; }
    public StatsTable CurrentStats { get; }
    public bool IsFainted => CurrentStats.Hp <= 0;

    public Pokemon(Specie specie, StatsTable evs, StatsTable ivs, Nature nature)
    {
        Specie = specie;
        Evs = evs;
        Ivs = ivs;
        Nature = nature;
        UnmodifiedStats = CalculateUnmodifiedStats();
        CurrentStats = UnmodifiedStats;
    }

    public void Heal(int amount)
    {
        CurrentStats.Hp = Math.Min(CurrentStats.Hp + amount, UnmodifiedStats.Hp);
    }

    public void Damage(int amount)
    {
        CurrentStats.Hp = Math.Max(CurrentStats.Hp - amount, 0);
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
        double natureModifier = Nature.GetStatModifier(StatIdTools.ConvertToStatIdExceptId(stat));
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
}