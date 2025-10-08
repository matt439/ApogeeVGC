namespace ApogeeVGC.Sim.Stats;

public class StatsExceptHpTable : Dictionary<StatIdExceptHp, int>
{
    public StatsExceptHpTable()
    {
        this[StatIdExceptHp.Atk] = 0;
        this[StatIdExceptHp.Def] = 0;
        this[StatIdExceptHp.SpA] = 0;
        this[StatIdExceptHp.SpD] = 0;
        this[StatIdExceptHp.Spe] = 0;
    }

    public StatsExceptHpTable(StatsExceptHpTable other) : this()
    {
        foreach (var kvp in other)
        {
            this[kvp.Key] = kvp.Value;
        }
    }

    public int Atk
    {
        get => this[StatIdExceptHp.Atk];
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            this[StatIdExceptHp.Atk] = value;
        }
    }

    public int Def
    {
        get => this[StatIdExceptHp.Def];
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            this[StatIdExceptHp.Def] = value;
        }
    }

    public int SpA
    {
        get => this[StatIdExceptHp.SpA];
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            this[StatIdExceptHp.SpA] = value;
        }
    }

    public int SpD
    {
        get => this[StatIdExceptHp.SpD];
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            this[StatIdExceptHp.SpD] = value;
        }
    }

    public int Spe
    {
        get => this[StatIdExceptHp.Spe];
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("value must be positive.");
            }
            this[StatIdExceptHp.Spe] = value;
        }
    }

    public int GetStat(StatIdExceptHp stat)
    {
        return this[stat];
    }

    public StatsExceptHpTable Copy()
    {
        return new StatsExceptHpTable(this);
    }
}