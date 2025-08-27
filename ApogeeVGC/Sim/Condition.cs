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

public enum ConditionEffectType
{
    Condition,
    Weather,
    Status,
    Terrain,
}

public record Condition : IEffect
{
    public EffectType EffectType => EffectType.Condition;
    public string Name { get; init; } = string.Empty;
    public ConditionEffectType ConditionEffectType { get; init; }

    public Func<Pokemon, Pokemon, IEffect, bool>? OnStart;
    //public int EffectOrder { get; init; }

    public int? OnResidualOrder { get; init; }
    public Action<Pokemon, Pokemon?, IEffect?>? OnResidual;
}