namespace ApogeeVGC.Sim.Stats;

public record BoostsTable
{
    public int Atk 
    { 
        get;
        init => field = ValidateBoost(value, nameof(Atk));
    }
    
    public int Def 
    { 
        get;
        init => field = ValidateBoost(value, nameof(Def));
    }
    
    public int SpA 
    { 
        get;
        init => field = ValidateBoost(value, nameof(SpA));
    }
    
    public int SpD 
    { 
        get;
        init => field = ValidateBoost(value, nameof(SpD));
    }
    
    public int Spe 
    { 
        get;
        init => field = ValidateBoost(value, nameof(Spe));
    }
    
    public int Evasion 
    { 
        get;
        init => field = ValidateBoost(value, nameof(Evasion));
    }
    
    public int Accuracy
    { 
        get;
        init => field = ValidateBoost(value, nameof(Accuracy));
    }

    private static int ValidateBoost(int value, string propertyName)
    {
        if (value is < -12 or > 12)
        {
            throw new ArgumentOutOfRangeException(
                propertyName, 
                value, 
                $"Boost value must be between -12 and +12 inclusive. Got: {value}");
        }
        return value;
    }
}