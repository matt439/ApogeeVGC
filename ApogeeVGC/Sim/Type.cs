using ApogeeVGC.Data;

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

public static class PokemonTypeTools
{
    public static MoveType ConvertToMoveType(this PokemonType type)
    {
        return type switch
        {
            PokemonType.Normal => MoveType.Normal,
            PokemonType.Fire => MoveType.Fire,
            PokemonType.Water => MoveType.Water,
            PokemonType.Electric => MoveType.Electric,
            PokemonType.Grass => MoveType.Grass,
            PokemonType.Ice => MoveType.Ice,
            PokemonType.Fighting => MoveType.Fighting,
            PokemonType.Poison => MoveType.Poison,
            PokemonType.Ground => MoveType.Ground,
            PokemonType.Flying => MoveType.Flying,
            PokemonType.Psychic => MoveType.Psychic,
            PokemonType.Bug => MoveType.Bug,
            PokemonType.Rock => MoveType.Rock,
            PokemonType.Ghost => MoveType.Ghost,
            PokemonType.Dragon => MoveType.Dragon,
            PokemonType.Dark => MoveType.Dark,
            PokemonType.Steel => MoveType.Steel,
            PokemonType.Fairy => MoveType.Fairy,
            PokemonType.Unknown => throw new ArgumentOutOfRangeException(nameof(type),
                "Unknown type cannot be converted to MoveType."),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid Pokemon type.")
        };
    }
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

public static class MoveTypeTools
{
    public static PokemonType ConvertToPokemonType(this MoveType type)
    {
        return type switch
        {
            MoveType.Normal => PokemonType.Normal,
            MoveType.Fire => PokemonType.Fire,
            MoveType.Water => PokemonType.Water,
            MoveType.Electric => PokemonType.Electric,
            MoveType.Grass => PokemonType.Grass,
            MoveType.Ice => PokemonType.Ice,
            MoveType.Fighting => PokemonType.Fighting,
            MoveType.Poison => PokemonType.Poison,
            MoveType.Ground => PokemonType.Ground,
            MoveType.Flying => PokemonType.Flying,
            MoveType.Psychic => PokemonType.Psychic,
            MoveType.Bug => PokemonType.Bug,
            MoveType.Rock => PokemonType.Rock,
            MoveType.Ghost => PokemonType.Ghost,
            MoveType.Dragon => PokemonType.Dragon,
            MoveType.Dark => PokemonType.Dark,
            MoveType.Steel => PokemonType.Steel,
            MoveType.Fairy => PokemonType.Fairy,
            MoveType.Stellar => throw new ArgumentOutOfRangeException(nameof(type),
                "Stellar type cannot be converted to Pokemon type."),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid move type.")
        };
    }

    public static string ConvertToString(this MoveType type)
    {
        return type switch
        {
            MoveType.Normal => "Normal",
            MoveType.Fire => "Fire",
            MoveType.Water => "Water",
            MoveType.Electric => "Electric",
            MoveType.Grass => "Grass",
            MoveType.Ice => "Ice",
            MoveType.Fighting => "Fighting",
            MoveType.Poison => "Poison",
            MoveType.Ground => "Ground",
            MoveType.Flying => "Flying",
            MoveType.Psychic => "Psychic",
            MoveType.Bug => "Bug",
            MoveType.Rock => "Rock",
            MoveType.Ghost => "Ghost",
            MoveType.Dragon => "Dragon",
            MoveType.Dark => "Dark",
            MoveType.Steel => "Steel",
            MoveType.Fairy => "Fairy",
            MoveType.Stellar => "Stellar",
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid move type.")
        };
    }
}

public enum Nonstandard
{
    Past,
    Future,
    Unobtainable,
    Cap,
    Lgpe,
    Custom,
    Gigantamax,
}

public enum BoostId
{
    Atk,
    Def,
    SpA,
    SpD,
    Spe,
    Accuracy,
    Evasion,
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
    public static StatId ConvertToStatId(this StatIdExceptHp stat)
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

    public static StatIdExceptHp ConvertToStatIdExceptId(this StatId stat)
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

    public static string ConvertToString(this StatId stat, bool leadingCapital = false)
    {
        return (stat, leadingCapital) switch
        {
            (StatId.Hp, true) => "HP",
            (StatId.Hp, false) => "hp",
            (StatId.Atk, true) => "Attack",
            (StatId.Atk, false) => "attack",
            (StatId.Def, true) => "Defense",
            (StatId.Def, false) => "defense",
            (StatId.SpA, true) => "Special Attack",
            (StatId.SpA, false) => "special attack",
            (StatId.SpD, true) => "Special Defense",
            (StatId.SpD, false) => "special defense",
            (StatId.Spe, true) => "Speed",
            (StatId.Spe, false) => "speed",
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.")
        };
    }

    public static string ConvertToString(this StatIdExceptHp stat, bool leadingCapital = false)
    {
        return ConvertToString(stat.ConvertToStatId(), leadingCapital);
    }
}

public enum TypeEffectiveness
{
    Normal,
    SuperEffective,
    NotVeryEffective,
    Immune,
}

public static class TypeEffectivenessTools
{
    public static MoveEffectiveness ConvertToMoveEffectiveness(this TypeEffectiveness effectiveness)
    {
        return effectiveness switch
        {
            TypeEffectiveness.Normal => MoveEffectiveness.Normal,
            TypeEffectiveness.SuperEffective => MoveEffectiveness.SuperEffective2X,
            TypeEffectiveness.NotVeryEffective => MoveEffectiveness.NotVeryEffective05X,
            TypeEffectiveness.Immune => MoveEffectiveness.Immune,
            _ => throw new ArgumentException("Invalid type effectiveness value.", nameof(effectiveness))
        };
    }
}

public record TypeData
{
    public required IReadOnlyDictionary<MoveType, TypeEffectiveness> DamageTaken { get; init; }
    public IReadOnlyDictionary<StatId, int>? HpDvs { get; init; }
    public IReadOnlyDictionary<StatId, int>? HpIvs { get; init; }
    public Nonstandard? IsNonstandard { get; init; }
}