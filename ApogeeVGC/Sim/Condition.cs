namespace ApogeeVGC.Sim;

public enum ConditionId
{
    Burn,
    Paralysis,
    Sleep,
    Freeze,
    Poison,
    Toxic,
    Confusion,
    Flinch,
    ChoiceLock,
    Stall,
}

public record Condition
{
    public string Name { get; init; } = string.Empty;
}