using System.Dynamic;

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
    LeechSeed,
    //Stall,
}

public enum EffectType
{
    Condition,
    Weather,
    Status,
    Terrain,
}

public record Condition
{
    public string Name { get; init; } = string.Empty;
    public EffectType EffectType { get; init; } = EffectType.Condition;

    public Func<Pokemon, Pokemon, bool>? OnStart;
    //public int EffectOrder { get; init; }

    public int? OnResidualOrder { get; init; }
}