using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    public Specie Specie { get; init; }
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
            field = value;
        }
    }
    public Item? Item { get; init; }
    public required Ability Ability { get; set; }
    //public Ability BaseAbility { get; }
    //public EffectState 
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
    public MoveType TeraType { get; init; }
    public GenderId Gender { get; init; }
    public bool PrintDebug { get; init; }
    public Trainer Trainer { get; init; }
    public SideId SideId { get; init; }
    public SlotId SlotId { get; set; }

    // Core constructor
    public Pokemon(Specie specie, StatsTable evs, StatsTable ivs, Nature nature, int level, Trainer trainer,
        SideId sideId)
    {
        Specie = specie;
        Evs = evs;
        Ivs = ivs;
        Nature = nature;
        Level = level;
        UnmodifiedStats = CalculateUnmodifiedStats();
        CurrentHp = UnmodifiedStats.Hp;
        Trainer = trainer;
        SideId = sideId;
    }

    //public override string ToString()
    //{
    //    string line1 = $"{Name} ({Specie.Name}) - Lv. {Level}";
    //    string line2 = $"{CurrentHp}/{UnmodifiedStats.Hp} HP";
    //    string line3 = $"Ability: {Ability.Name} - Item: {Item.Name}";
    //    string movesLine = string.Join(", ", Moves.Select(m => m.Name));
    //    return $"{line1}\n{line2}\n{line3}\nMoves: {movesLine}\n";
    //}
}