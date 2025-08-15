namespace ApogeeVGC.Sim;

public enum PokemonType
{
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy,
    /// <summary>
    /// Represents the "???" type
    /// </summary>
    Unknown,
}

public enum MoveType
{
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy,
    Stellar,
}

public enum Nonstandard
{
    Past, Future, Unobtainable, Cap, Lgpe, Custom, Gigantamax
}

public enum BoostId
{
    Atk, Def, SpA, SpD, Spe, Accuracy, Evasion
}

public enum StatType
{
    Hp,
    Atk,
    Def,
    SpA,
    SpD,
    Spe,
}

public enum MoveEffectiveness
{
    Normal = 0,
    SuperEffective = 1,
    NotVeryEffective = 2,
    Immune = 3,
}

public class TypeData
{
    public required Dictionary<MoveType, MoveEffectiveness> DamageTaken { get; init; }
    public Dictionary<StatType, int>? HpDvs { get; init; }
    public Dictionary<StatType, int>? HpIvs { get; init; }
    public Nonstandard? IsNonstandard { get; init; }
}