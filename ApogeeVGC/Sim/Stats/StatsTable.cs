namespace ApogeeVGC.Sim.Stats;

public class StatsTable : Dictionary<StatId, int>
{
    public StatsTable()
    {
        this[StatId.Hp] = 0;
        this[StatId.Atk] = 0;
        this[StatId.Def] = 0;
        this[StatId.SpA] = 0;
        this[StatId.SpD] = 0;
        this[StatId.Spe] = 0;
    }

    public StatsTable(StatsTable other) : this()
    {
        foreach (var kvp in other)
        {
            this[kvp.Key] = kvp.Value;
        }
    }

    public int Hp
    {
        get => this[StatId.Hp];
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            this[StatId.Hp] = value;
        }
    }

    public int Atk
    {
        get => this[StatId.Atk];
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            this[StatId.Atk] = value;
        }
    }

    public int Def
    {
        get => this[StatId.Def];
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            this[StatId.Def] = value;
        }
    }

    public int SpA
    {
        get => this[StatId.SpA];
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            this[StatId.SpA] = value;
        }
    }

    public int SpD
    {
        get => this[StatId.SpD];
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            this[StatId.SpD] = value;
        }
    }

    public int Spe
    {
        get => this[StatId.Spe];
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            this[StatId.Spe] = value;
        }
    }

    public int BaseStatTotal => Hp + Atk + Def + SpA + SpD + Spe;

    public int GetStat(StatId stat)
    {
        return this[stat];
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

    public StatsTable Copy()
    {
        return new StatsTable(this);
    }
}