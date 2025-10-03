using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.PokemonClasses;

public record PokemonTemplate
{
    public string Name
    {
        get => field == string.Empty ? Specie.Name : field;
        init;
    } = string.Empty;
    public required Specie Specie { get; init; }
    public Item? Item { get; init; }
    public required Ability Ability { get; init; }
    public required IReadOnlyList<Move> Moves
    {
        get;
        init
        {
            if (value == null || value.Count == 0)
            {
                throw new ArgumentException("Pokemon must have at least one move.");
            }
            if (value.Count > 4)
            {
                throw new ArgumentException("Pokemon cannot have more than 4 moves.");
            }
            // Check that all moves are unique
            if (value.Distinct().Count() != value.Count)
            {
                throw new ArgumentException("Pokemon cannot have duplicate moves.");
            }
            field = value;
        }
    }
    public required Nature Nature { get; init; }
    public GenderId Gender { get; init; }
    public required StatsTable Evs { get; init; }
    public StatsTable Ivs { get; init; } = StatsTable.PerfectIvs;
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
    } = 50;
    public bool Shiny { get; init; }
    public int Happiness
    {
        get;
        init
        {
            if (value is < 0 or > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Happiness must be between 0 and 255.");
            }
            field = value;
        }
    } 
    public PokeballId Pokeball { get; init; }
    public PokemonType HiddenPowerType { get; init; }
    public MoveType TeraType { get; init; }
}