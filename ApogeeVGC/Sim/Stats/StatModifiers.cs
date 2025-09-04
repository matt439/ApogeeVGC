namespace ApogeeVGC.Sim.Stats;

public class StatModifiers
{
    private readonly Dictionary<StatModifierId, int> _modifiers;

    public StatModifiers()
    {
        _modifiers = new Dictionary<StatModifierId, int>();
        // Initialize all stats to 0
        foreach (StatModifierId stat in Enum.GetValues<StatModifierId>())
        {
            _modifiers[stat] = 0;
        }
    }

    public StatModifiers(Dictionary<StatModifierId, int> modifiers)
    {
        _modifiers = new Dictionary<StatModifierId, int>();
        
        // Initialize all stats to 0 first
        foreach (StatModifierId stat in Enum.GetValues<StatModifierId>())
        {
            _modifiers[stat] = 0;
        }
        
        // Then set provided values with validation
        foreach (var kvp in modifiers)
        {
            if (!IsValidStage(kvp.Value))
            {
                throw new ArgumentOutOfRangeException(nameof(modifiers), 
                    $"Stat stage for {kvp.Key} must be between -6 and +6.");
            }
            _modifiers[kvp.Key] = kvp.Value;
        }
    }

    public int Atk => _modifiers[StatModifierId.Atk];
    public int Def => _modifiers[StatModifierId.Def];
    public int SpA => _modifiers[StatModifierId.SpA];
    public int SpD => _modifiers[StatModifierId.SpD];
    public int Spe => _modifiers[StatModifierId.Spe];
    public int Accuracy => _modifiers[StatModifierId.Accuracy];
    public int Evasion => _modifiers[StatModifierId.Evasion];

    public int this[StatModifierId stat]
    {
        get => _modifiers[stat];
        private set
        {
            if (!IsValidStage(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), 
                    $"Stat stage for {stat} must be between -6 and +6.");
            }
            _modifiers[stat] = value;
        }
    }

    public double AtkMultiplier => CalculateRegularStatMultiplier(Atk);
    public double DefMultiplier => CalculateRegularStatMultiplier(Def);
    public double SpAMultiplier => CalculateRegularStatMultiplier(SpA);
    public double SpDMultiplier => CalculateRegularStatMultiplier(SpD);
    public double SpeMultiplier => CalculateRegularStatMultiplier(Spe);
    public double AccuracyMultiplier => CalculateAccuracyStatMultiplier(Accuracy);
    public double EvasionMultiplier => CalculateEvasionStatMultiplier(Evasion);

    public BoostsTable GetCappedBoost(BoostsTable boosts)
    {
        var cappedBoosts = new Dictionary<StatModifierId, int>();
        foreach (var kvp in boosts.Boosts)
        {
            int currentStage = _modifiers[kvp.Key];
            int newStage = Math.Clamp(currentStage + kvp.Value, -6, 6);
            cappedBoosts[kvp.Key] = newStage - currentStage;
        }
        return new BoostsTable { Boosts = cappedBoosts };
    }

    /// <summary>Updates the current StatModifiers by applying the given BoostsTable</summary>
    /// <returns>A delta of summed changes</returns>
    public int BoostBy(BoostsTable boosts)
    {
        int totalChange = 0;
        var newModifiers = new Dictionary<StatModifierId, int>(_modifiers);
        
        foreach (var kvp in boosts.Boosts)
        {
            int currentStage = newModifiers[kvp.Key];
            int newStage = Math.Clamp(currentStage + kvp.Value, -6, 6);
            newModifiers[kvp.Key] = newStage;
            totalChange += newStage - currentStage;
        }
        return totalChange;
    }

    public int BoostBy(StatModifierId stat, int change)
    {
        if (change == 0) return 0;
        
        int currentStage = _modifiers[stat];
        int newStage = Math.Clamp(currentStage + change, -6, 6);
        _modifiers[stat] = newStage;
        return newStage - currentStage;
    }

    public StatModifiers WithStat(StatModifierId stat, int value)
    {
        if (!IsValidStage(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), 
                $"Stat stage for {stat} must be between -6 and +6.");
        }
        
        var newModifiers = new Dictionary<StatModifierId, int>(_modifiers)
        {
            [stat] = value,
        };
        
        return new StatModifiers(newModifiers);
    }

    public StatModifiers Copy()
    {
        return new StatModifiers(new Dictionary<StatModifierId, int>(_modifiers));
    }

    private static bool IsValidStage(int stage) => stage is >= -6 and <= 6;

    private static double CalculateRegularStatMultiplier(int stage)
    {
        if (!IsValidStage(stage))
        {
            throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.");
        }

        return stage switch
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
            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.")
        };
    }

    private static double CalculateAccuracyStatMultiplier(int stage)
    {
        if (!IsValidStage(stage))
        {
            throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.");
        }

        return stage switch
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
            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.")
        };
    }

    private static double CalculateEvasionStatMultiplier(int stage)
    {
        if (!IsValidStage(stage))
        {
            throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.");
        }

        return stage switch
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
            _ => throw new ArgumentOutOfRangeException(nameof(stage), "Stat stage must be between -6 and +6.")
        };
    }
}