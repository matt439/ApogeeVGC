namespace ApogeeVGC.Sim.Stats;

public class SparseBoostsTable
{
    public int? Atk { get; set; }
    public int? Def { get; set; }
    public int? SpA { get; set; }
    public int? SpD { get; set; }
    public int? Spe { get; set; }
    public int? Accuracy { get; set; }
    public int? Evasion { get; set; }

    public int? GetBoost(BoostId stat)
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

    public void SetBoost(BoostId boostId, int boost)
    {
        switch (boostId)
        {
            case BoostId.Atk:
                Atk = boost;
                break;
            case BoostId.Def:
                Def = boost;
                break;
            case BoostId.SpA:
                SpA = boost;
                break;
            case BoostId.SpD:
                SpD = boost;
                break;
            case BoostId.Spe:
                Spe = boost;
                break;
            case BoostId.Accuracy:
                Accuracy = boost;
                break;
            case BoostId.Evasion:
                Evasion = boost;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(boostId), "Invalid stat ID.");
        }
    }

    public double? GetBoostMultiplier(BoostId stat)
    {
        int? boost = GetBoost(stat);
        if (!boost.HasValue)
            return null;

        return stat switch
        {
            BoostId.Atk or BoostId.Def or BoostId.SpA or BoostId.SpD or BoostId.Spe 
                => BoostsTable.CalculateRegularStatMultiplier(boost.Value),
            BoostId.Accuracy => BoostsTable.CalculateAccuracyStatMultiplier(boost.Value),
            BoostId.Evasion => BoostsTable.CalculateEvasionStatMultiplier(boost.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
        };
    }

    public BoostsTable ToBoostsTable()
    {
        return new BoostsTable
        {
            Atk = Atk ?? 0,
            Def = Def ?? 0,
            SpA = SpA ?? 0,
            SpD = SpD ?? 0,
            Spe = Spe ?? 0,
            Accuracy = Accuracy ?? 0,
            Evasion = Evasion ?? 0,
        };
    }

    public static SparseBoostsTable FromBoostsTable(BoostsTable boosts)
    {
        return new SparseBoostsTable
        {
            Atk = boosts.Atk,
            Def = boosts.Def,
            SpA = boosts.SpA,
            SpD = boosts.SpD,
            Spe = boosts.Spe,
            Accuracy = boosts.Accuracy,
            Evasion = boosts.Evasion,
        };
    }

    /// <summary>
    /// Enumerates all non-null boost values in this table.
    /// Only yields boosts that have been explicitly set (non-null).
    /// </summary>
    /// <returns>An enumerable of (BoostId, value) pairs for all non-null boosts</returns>
    public IEnumerable<(BoostId BoostId, int Value)> GetNonNullBoosts()
    {
        if (Atk.HasValue)
            yield return (BoostId.Atk, Atk.Value);
        
        if (Def.HasValue)
            yield return (BoostId.Def, Def.Value);
        
        if (SpA.HasValue)
            yield return (BoostId.SpA, SpA.Value);
        
        if (SpD.HasValue)
            yield return (BoostId.SpD, SpD.Value);
        
        if (Spe.HasValue)
            yield return (BoostId.Spe, Spe.Value);
        
        if (Accuracy.HasValue)
            yield return (BoostId.Accuracy, Accuracy.Value);
        
        if (Evasion.HasValue)
            yield return (BoostId.Evasion, Evasion.Value);
    }
}