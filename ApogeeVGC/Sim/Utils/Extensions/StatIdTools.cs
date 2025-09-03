using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Utils.Extensions;

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
        return stat.ConvertToStatId().ConvertToString(leadingCapital);
    }
}