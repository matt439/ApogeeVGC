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
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID except HP."),
        };
    }

    public static StatId ConvertToStatId(this BoostId stat)
    {
        return stat switch
        {
            BoostId.Atk => StatId.Atk,
            BoostId.Def => StatId.Def,
            BoostId.SpA => StatId.SpA,
            BoostId.SpD => StatId.SpD,
            BoostId.Spe => StatId.Spe,
            BoostId.Accuracy => throw new ArgumentOutOfRangeException(nameof(stat),
                "Cannot convert Accuracy to StatId."),
            BoostId.Evasion => throw new ArgumentOutOfRangeException(nameof(stat),
                "Cannot convert Evasion to StatId."),
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid boost ID."),
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
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
        };
    }

    public static StatIdExceptHp ConvertToStatIdExceptId(this BoostId stat)
    {
        return stat switch
        {
            BoostId.Atk => StatIdExceptHp.Atk,
            BoostId.Def => StatIdExceptHp.Def,
            BoostId.SpA => StatIdExceptHp.SpA,
            BoostId.SpD => StatIdExceptHp.SpD,
            BoostId.Spe => StatIdExceptHp.Spe,
            BoostId.Accuracy => throw new ArgumentOutOfRangeException(nameof(stat),
                "Cannot convert Accuracy to StatId."),
            BoostId.Evasion => throw new ArgumentOutOfRangeException(nameof(stat),
                "Cannot convert Evasion to StatId."),
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid boost ID."),
        };
    }

    public static BoostId ConvertToBoostId(this StatId stat)
    {
        return stat switch
        {
            StatId.Atk => BoostId.Atk,
            StatId.Def => BoostId.Def,
            StatId.SpA => BoostId.SpA,
            StatId.SpD => BoostId.SpD,
            StatId.Spe => BoostId.Spe,
            StatId.Hp => throw new ArgumentOutOfRangeException(nameof(stat),
                "Cannot convert HP to BoostId."),
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
        };
    }

    public static BoostId ConvertToBoostId(this StatIdExceptHp stat)
    {
        return stat switch
        {
            StatIdExceptHp.Atk => BoostId.Atk,
            StatIdExceptHp.Def => BoostId.Def,
            StatIdExceptHp.SpA => BoostId.SpA,
            StatIdExceptHp.SpD => BoostId.SpD,
            StatIdExceptHp.Spe => BoostId.Spe,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
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
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
        };
    }

    public static string ConvertToString(this StatIdExceptHp stat, bool leadingCapital = false)
    {
        return stat.ConvertToStatId().ConvertToString(leadingCapital);
    }

    public static string ConvertToString(this BoostId boost, bool leadingCapital = false)
    {
        return (boost, leadingCapital) switch
        {
            (BoostId.Atk, true) => "Attack",
            (BoostId.Atk, false) => "attack",
            (BoostId.Def, true) => "Defense",
            (BoostId.Def, false) => "defense",
            (BoostId.SpA, true) => "Special Attack",
            (BoostId.SpA, false) => "special attack",
            (BoostId.SpD, true) => "Special Defense",
            (BoostId.SpD, false) => "special defense",
            (BoostId.Spe, true) => "Speed",
            (BoostId.Spe, false) => "speed",
            (BoostId.Accuracy, true) => "Accuracy",
            (BoostId.Accuracy, false) => "accuracy",
            (BoostId.Evasion, true) => "Evasion",
            (BoostId.Evasion, false) => "evasion",
            _ => throw new ArgumentOutOfRangeException(nameof(boost), "Invalid boost ID."),
        };
    }
}