namespace ApogeeVGC.Sim.Stats;

public class StatsExceptHpTable
{
    public int Atk { get; set; }
    public int Def { get; set; }
    public int SpA { get; set; }
    public int SpD { get; set; }
    public int Spe { get; set; }

    /// <summary>
    /// All stat IDs for iteration, replacing Dictionary.Keys enumeration.
    /// </summary>
    public static ReadOnlySpan<StatIdExceptHp> AllStatIds =>
        [StatIdExceptHp.Atk, StatIdExceptHp.Def, StatIdExceptHp.SpA, StatIdExceptHp.SpD, StatIdExceptHp.Spe];

    public int this[StatIdExceptHp stat]
    {
        get => GetStat(stat);
        set => SetStat(stat, value);
    }

    public int GetStat(StatIdExceptHp stat)
    {
        return stat switch
        {
            StatIdExceptHp.Atk => Atk,
            StatIdExceptHp.Def => Def,
            StatIdExceptHp.SpA => SpA,
            StatIdExceptHp.SpD => SpD,
            StatIdExceptHp.Spe => Spe,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
        };
    }

    public void SetStat(StatIdExceptHp stat, int value)
    {
        switch (stat)
        {
            case StatIdExceptHp.Atk:
                Atk = value;
                break;
            case StatIdExceptHp.Def:
                Def = value;
                break;
            case StatIdExceptHp.SpA:
                SpA = value;
                break;
            case StatIdExceptHp.SpD:
                SpD = value;
                break;
            case StatIdExceptHp.Spe:
                Spe = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.");
        }
    }

    public StatsExceptHpTable Copy()
    {
        return new StatsExceptHpTable
        {
            Atk = Atk,
            Def = Def,
            SpA = SpA,
            SpD = SpD,
            Spe = Spe,
        };
    }
}