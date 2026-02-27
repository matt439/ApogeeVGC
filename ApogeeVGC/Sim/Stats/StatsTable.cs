namespace ApogeeVGC.Sim.Stats;

public class StatsTable
{
    public int Hp { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    public int SpA { get; set; }
    public int SpD { get; set; }
    public int Spe { get; set; }

    /// <summary>
    /// All stat IDs for iteration, replacing Dictionary.Keys enumeration.
    /// </summary>
    public static ReadOnlySpan<StatId> AllStatIds =>
        [StatId.Hp, StatId.Atk, StatId.Def, StatId.SpA, StatId.SpD, StatId.Spe];

    public int this[StatId stat]
    {
        get => GetStat(stat);
        set => SetStat(stat, value);
    }

    public int GetStat(StatId stat)
    {
        return stat switch
        {
            StatId.Hp => Hp,
            StatId.Atk => Atk,
            StatId.Def => Def,
            StatId.SpA => SpA,
            StatId.SpD => SpD,
            StatId.Spe => Spe,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
        };
    }

    public void SetStat(StatId stat, int value)
    {
        switch (stat)
        {
            case StatId.Hp:
                Hp = value;
                break;
            case StatId.Atk:
                Atk = value;
                break;
            case StatId.Def:
                Def = value;
                break;
            case StatId.SpA:
                SpA = value;
                break;
            case StatId.SpD:
                SpD = value;
                break;
            case StatId.Spe:
                Spe = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.");
        }
    }

    public int BaseStatTotal => Hp + Atk + Def + SpA + SpD + Spe;

    public static bool IsValidIv(int stat)
    {
        return stat is >= 0 and <= 31;
    }

    public static bool IsValidEv(int stat)
    {
        return stat is >= 0 and <= 255;
    }

    public bool IsValidIvs()
    {
        return IsValidIv(Hp) && IsValidIv(Atk) && IsValidIv(Def) &&
               IsValidIv(SpA) && IsValidIv(SpD) && IsValidIv(Spe);
    }

    public bool IsValidEvs()
    {
        return IsValidEv(Hp) && IsValidEv(Atk) && IsValidEv(Def) &&
               IsValidEv(SpA) && IsValidEv(SpD) && IsValidEv(Spe) &&
               BaseStatTotal <= 510;
    }

    public static StatsTable PerfectIvs => new()
    {
        Hp = 31,
        Atk = 31,
        Def = 31,
        SpA = 31,
        SpD = 31,
        Spe = 31,
    };

    public StatsTable Copy()
    {
        return new StatsTable
        {
            Hp = Hp,
            Atk = Atk,
            Def = Def,
            SpA = SpA,
            SpD = SpD,
            Spe = Spe,
        };
    }
}