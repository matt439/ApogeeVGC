namespace ApogeeVGC.Sim.Stats;

public class SparseBoostsTable
{
    // Track insertion order to match Showdown's JS property iteration order
    private readonly List<BoostId> _insertionOrder = new();

    private int? _atk;
    private int? _def;
    private int? _spa;
    private int? _spd;
    private int? _spe;
    private int? _accuracy;
    private int? _evasion;

    public int? Atk
    {
        get => _atk;
        set { _atk = value; TrackOrder(BoostId.Atk, value); }
    }

    public int? Def
    {
        get => _def;
        set { _def = value; TrackOrder(BoostId.Def, value); }
    }

    public int? SpA
    {
        get => _spa;
        set { _spa = value; TrackOrder(BoostId.SpA, value); }
    }

    public int? SpD
    {
        get => _spd;
        set { _spd = value; TrackOrder(BoostId.SpD, value); }
    }

    public int? Spe
    {
        get => _spe;
        set { _spe = value; TrackOrder(BoostId.Spe, value); }
    }

    public int? Accuracy
    {
        get => _accuracy;
        set { _accuracy = value; TrackOrder(BoostId.Accuracy, value); }
    }

    public int? Evasion
    {
        get => _evasion;
        set { _evasion = value; TrackOrder(BoostId.Evasion, value); }
    }

    private void TrackOrder(BoostId id, int? value)
    {
        if (value.HasValue)
        {
            if (!_insertionOrder.Contains(id))
                _insertionOrder.Add(id);
        }
        else
        {
            _insertionOrder.Remove(id);
        }
    }

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

    /// <summary>
    /// Clears (sets to null) a specific boost stat, effectively removing it from the table.
    /// This is equivalent to 'delete boost[stat]' in TypeScript.
    /// </summary>
    public void ClearBoost(BoostId boostId)
    {
        switch (boostId)
        {
            case BoostId.Atk:
                Atk = null;
                break;
            case BoostId.Def:
                Def = null;
                break;
            case BoostId.SpA:
                SpA = null;
                break;
            case BoostId.SpD:
                SpD = null;
                break;
            case BoostId.Spe:
                Spe = null;
                break;
            case BoostId.Accuracy:
                Accuracy = null;
                break;
            case BoostId.Evasion:
                Evasion = null;
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
    public SparseBoostsTable Copy()
    {
        // Must preserve insertion order to match Showdown's JS spread ({...boost}) behavior.
        // Setting properties in _insertionOrder sequence ensures the copy iterates identically.
        var copy = new SparseBoostsTable();
        foreach (var boostId in _insertionOrder)
        {
            int? val = GetBoost(boostId);
            if (val.HasValue)
                copy.SetBoost(boostId, val.Value);
        }
        return copy;
    }

    /// <summary>
    /// Enumerates all non-null boost values in insertion order.
    /// Matches Showdown's JS property iteration order (for..in).
    /// </summary>
    public IEnumerable<(BoostId BoostId, int Value)> GetNonNullBoosts()
    {
        foreach (var boostId in _insertionOrder)
        {
            int? val = GetBoost(boostId);
            if (val.HasValue)
                yield return (boostId, val.Value);
        }
    }
}