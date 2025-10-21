namespace ApogeeVGC.Sim.Stats;

public class BoostsTable
{
    public int Atk { get; set; }
    public int Def { get; set; }
    public int SpA { get; set; }
    public int SpD { get; set; }
    public int Spe { get; set; }
    public int Accuracy { get; set; }
    public int Evasion { get; set; }

    public double AtkMultiplier => CalculateRegularStatMultiplier(Atk);
    public double DefMultiplier => CalculateRegularStatMultiplier(Def);
    public double SpAMultiplier => CalculateRegularStatMultiplier(SpA);
    public double SpDMultiplier => CalculateRegularStatMultiplier(SpD);
    public double SpeMultiplier => CalculateRegularStatMultiplier(Spe);
    public double AccuracyMultiplier => CalculateAccuracyStatMultiplier(Accuracy);
    public double EvasionMultiplier => CalculateEvasionStatMultiplier(Evasion);

    public int GetBoost(BoostId stat)
    {
        return stat switch
        {
            BoostId.Atk => Atk,
            BoostId.Def => Def,
            BoostId.SpA => SpA,
            BoostId.SpD => SpD,
            BoostId.Spe => Spe,
            BoostId.Accuracy => Accuracy,
            BoostId.Evasion => Evasion,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
        };
    }

    public void SetBoost(BoostId stat, int value)
    {
        value = ClampBoost(value);
        switch (stat)
        {
            case BoostId.Atk:
                Atk = value;
                break;
            case BoostId.Def:
                Def = value;
                break;
            case BoostId.SpA:
                SpA = value;
                break;
            case BoostId.SpD:
                SpD = value;
                break;
            case BoostId.Spe:
                Spe = value;
                break;
            case BoostId.Accuracy:
                Accuracy = value;
                break;
            case BoostId.Evasion:
                Evasion = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.");
        }
    }

    public double GetBoostMultiplier(BoostId stat)
    {
        return stat switch
        {
            BoostId.Atk => AtkMultiplier,
            BoostId.Def => DefMultiplier,
            BoostId.SpA => SpAMultiplier,
            BoostId.SpD => SpDMultiplier,
            BoostId.Spe => SpeMultiplier,
            BoostId.Accuracy => AccuracyMultiplier,
            BoostId.Evasion => EvasionMultiplier,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
        };
    }

    public BoostsTable Copy()
    {
        return new BoostsTable
        {
            Atk = Atk,
            Def = Def,
            SpA = SpA,
            SpD = SpD,
            Spe = Spe,
            Accuracy = Accuracy,
            Evasion = Evasion,
        };
    }

    public static int ClampBoost(int boost)
    {
        return boost switch
        {
            < -6 => -6,
            > 6 => 6,
            _ => boost,
        };
    }

    public static double CalculateRegularStatMultiplier(int stage)
    {
        return ClampBoost(stage) switch
        {
            -6 => 2.0 / 8.0,
            -5 => 2.0 / 7.0,
            -4 => 2.0 / 6.0,
            -3 => 2.0 / 5.0,
            -2 => 2.0 / 4.0,
            -1 => 2.0 / 3.0,
            0 => 2.0 / 2.0,
            1 => 3.0 / 2.0,
            2 => 4.0 / 2.0,
            3 => 5.0 / 2.0,
            4 => 6.0 / 2.0,
            5 => 7.0 / 2.0,
            6 => 8.0 / 2.0,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6."),
        };
    }

    public static double CalculateAccuracyStatMultiplier(int stage)
    {
        return ClampBoost(stage) switch
        {
            -6 => 3.0 / 9.0,
            -5 => 3.0 / 8.0,
            -4 => 3.0 / 7.0,
            -3 => 3.0 / 6.0,
            -2 => 3.0 / 5.0,
            -1 => 3.0 / 4.0,
            0 => 3.0 / 3.0,
            1 => 4.0 / 3.0,
            2 => 5.0 / 3.0,
            3 => 6.0 / 3.0,
            4 => 7.0 / 3.0,
            5 => 8.0 / 3.0,
            6 => 9.0 / 3.0,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6."),
        };
    }

    public static double CalculateEvasionStatMultiplier(int stage)
    {
        return ClampBoost(stage) switch
        {
            -6 => 9.0 / 3.0,
            -5 => 8.0 / 3.0,
            -4 => 7.0 / 3.0,
            -3 => 6.0 / 3.0,
            -2 => 5.0 / 3.0,
            -1 => 4.0 / 3.0,
            0 => 3.0 / 3.0,
            1 => 3.0 / 4.0,
            2 => 3.0 / 5.0,
            3 => 3.0 / 6.0,
            4 => 3.0 / 7.0,
            5 => 3.0 / 8.0,
            6 => 3.0 / 9.0,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6."),
        };
    }

    public IEnumerable<(BoostId BoostId, int Value)> GetBoosts()
    {
        yield return (BoostId.Atk, Atk);
        yield return (BoostId.Def, Def);
        yield return (BoostId.SpA, SpA);
        yield return (BoostId.SpD, SpD);
        yield return (BoostId.Spe, Spe);
        yield return (BoostId.Accuracy, Accuracy);
        yield return (BoostId.Evasion, Evasion);
    }
}