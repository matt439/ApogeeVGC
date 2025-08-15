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
    Past,
    Future,
    Unobtainable,
    Cap,
    Lgpe,
    Custom,
    Gigantamax
}

public enum BoostId
{
    Atk,
    Def,
    SpA,
    SpD,
    Spe,
    Accuracy,
    Evasion
}

public enum StatId
{
    Hp,
    Atk,
    Def,
    SpA,
    SpD,
    Spe,
}

public enum StatIdExceptHp
{
    Atk,
    Def,
    SpA,
    SpD,
    Spe,
}

public static class StatIdTools
{
    public static StatId ConvertToStatId(StatIdExceptHp stat)
    {
        return stat switch
        {
            StatIdExceptHp.Atk => StatId.Atk,
            StatIdExceptHp.Def => StatId.Def,
            StatIdExceptHp.SpA => StatId.SpA,
            StatIdExceptHp.SpD => StatId.SpD,
            StatIdExceptHp.Spe => StatId.Spe,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID except HP.")
        };
    }

    public static StatIdExceptHp ConvertToStatIdExceptId(StatId stat)
    {
        return stat switch
        {
            StatId.Atk => StatIdExceptHp.Atk,
            StatId.Def => StatIdExceptHp.Def,
            StatId.SpA => StatIdExceptHp.SpA,
            StatId.SpD => StatIdExceptHp.SpD,
            StatId.Spe => StatIdExceptHp.Spe,
            StatId.Hp => throw new ArgumentOutOfRangeException(nameof(stat),
                "Cannot convert HP to StatIdExceptHp."),
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.")
        };
    }
}


public enum MoveEffectiveness
{
    Normal = 0,
    SuperEffective = 1,
    NotVeryEffective = 2,
    Immune = 3,
}

public record TypeData
{
    public required IReadOnlyDictionary<MoveType, MoveEffectiveness> DamageTaken { get; init; }
    public IReadOnlyDictionary<StatId, int>? HpDvs { get; init; }
    public IReadOnlyDictionary<StatId, int>? HpIvs { get; init; }
    public Nonstandard? IsNonstandard { get; init; }
}