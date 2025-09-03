namespace ApogeeVGC.Sim;

public record StatsTable
{
    public int Hp
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }

            field = value;
        }
    }
    public int Atk
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int Def
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int SpA
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int SpD
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;
    public int Spe
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            field = value;
        }
    } = 0;

    public int BaseStatTotal => Hp + Atk + Def + SpA + SpD + Spe;

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
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.")
        };
    }

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

    public StatsTable(StatsTable other)
    {
        Hp = other.Hp;
        Atk = other.Atk;
        Def = other.Def;
        SpA = other.SpA;
        SpD = other.SpD;
        Spe = other.Spe;
    }

    public StatsTable()
    {
    }

    public StatsTable Copy()
    {
        return new StatsTable(this);
    }
}