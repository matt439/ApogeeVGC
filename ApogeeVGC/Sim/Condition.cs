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
    TrickRoom,
    Stall,
    Protect,
}

public enum ConditionEffectType
{
    Condition,
    Weather,
    Status,
    Terrain,
}

public enum ConditionVolatility
{
    Volatile,
    NonVolatile,
}

public record Condition : IEffect
{
    public required ConditionId Id { get; init; }
    public EffectType EffectType => EffectType.Condition;
    public string Name { get; init; } = string.Empty;
    public ConditionEffectType ConditionEffectType { get; init; }
    public ConditionVolatility ConditionVolatility { get; init; }

    public Func<Pokemon, Pokemon?, IEffect?, bool, bool>? OnStart;
    //public int EffectOrder { get; init; }

    public int? OnResidualOrder { get; init; }
    public Action<Pokemon, Side?, IEffect?, bool>? OnResidual { get; init; }
    public int? Duration { get; set; }
    public Func<Pokemon, Pokemon?, int>? DurationCallback { get; init; }
    public Action<Field?, Pokemon, IEffect?>? OnFieldStart { get; init; }
    public Action<Field, Pokemon?, IEffect?>? OnFieldRestart { get; init; }
    public int? OnFieldResidualOrder { get; init; }
    public int? OnFieldResidualSubOrder { get; init; }
    public Action<Field?>? OnFieldEnd { get; init; }
    public int? CounterMax { get; init; }
    public int? Counter { get; set; }
    public Func<Random, Pokemon, bool>? OnStallMove { get; init; }
    public Func<Pokemon, Pokemon?, IEffect?, bool>? OnRestart { get; init; }
    public int? OnTryHitPriority { get; init; }
    public Func<Pokemon, Pokemon, Move, bool>? OnTryHit { get; init; }
}