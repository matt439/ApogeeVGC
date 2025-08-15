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

        Pokemon pokemon = new()
        {
            Specie = library.Species[specie] ?? throw new ArgumentException($"Specie {specie} not found in library."),
            Moves = movesList,
            Item = library.Items[item] ?? throw new ArgumentException($"Item {item} not found in library."),
            Ability = library.Abilities[ability] ?? throw new ArgumentException($"Ability {ability} not found in library."),
            Evs = evs,
            Nature = library.Natures[nature] ?? throw new ArgumentException($"Nature {nature} not found in library."),
            Ivs = ivs ?? StatsTable.PerfectIvs,
            Name = nickname ?? library.Species[specie].Name,
            Shiny = shiny,
            Level = level,
        };

        return PokemonValidator.IsValid(library, pokemon) ? pokemon
            : throw new ArgumentException("Invalid Pokemon configuration.");
    }

    public static PokemonTeam BuildTestTeam(Library library)
    {
        return new PokemonTeam()
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
            ]
        };
    }
}

public class PokemonTeam
{
    public List<Pokemon> Pokemons { get; init; } = [];
}

public class Pokemon
{
    public required Specie Specie { get; init; }
    public required IEnumerable<Move> Moves { get; init; }
    public required Item Item { get; init; }
    public required Ability Ability { get; init; }
    public required StatsTable Evs { get; init; }
    public required Nature Nature { get; init; }
    public required StatsTable Ivs { get; init; }
    public required string Name { get; init; }
    public int Level { get; init; }
    public bool Shiny { get; init; }
    public MoveType TerraType { get; init; }

    public StatsTable UnmodifiedStats { get; }
    public StatsTable CurrentStats { get; }

    public bool IsFainted => CurrentStats.Hp <= 0;

    public Pokemon()
    {
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