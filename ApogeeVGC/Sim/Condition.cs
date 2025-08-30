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
    Tailwind,
    Reflect,
    LightScreen,
    Leftovers,
    ChoiceSpecs,
    FlameOrb,
    AssaultVest,
}

public enum ConditionEffectType
{
    Condition,
    Weather,
    Status,
    Terrain,
    PseudoWeather,
    SideCondition,
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

    public Func<Pokemon, Pokemon?, IEffect?, BattleContext, bool>? OnStart;
    //public int EffectOrder { get; init; }

    public int? OnResidualOrder { get; init; }
    public int? OnResidualSubOrder { get; init; }

    public Action<Pokemon, Side?, IEffect?, BattleContext>? OnResidual { get; init; }
    public int? Duration { get; set; }
    //public Func<Pokemon, Pokemon?, int>? DurationCallback { get; init; }
    //public Action<Field?, Pokemon, IEffect?>? OnFieldStart { get; init; }
    //public Action<Field, Pokemon?, IEffect?>? OnFieldRestart { get; init; }
    //public int? OnFieldResidualOrder { get; init; }
    //public int? OnFieldResidualSubOrder { get; init; }
    //public Action<Field?>? OnFieldEnd { get; init; }
    public int? CounterMax { get; init; }
    public int? Counter { get; set; }
    public Func<Pokemon, BattleContext, bool>? OnStallMove { get; init; }
    /// <summary>
    /// target, source, sourceEffect, context
    /// </summary>
    public Func<Pokemon, Pokemon?, IEffect?, BattleContext, bool>? OnRestart { get; init; }
    public int? OnTryHitPriority { get; init; }
    /// <summary>
    /// source, target, move, context
    /// </summary>
    public Func<Pokemon, Pokemon, Move, BattleContext, bool>? OnTryHit { get; init; }
    public Action<Pokemon, BattleContext>? OnTurnEnd { get; init; }
    //public Action<Field>? OnPseudoWeatherStart { get; init; }
    public Func<Pokemon, double>? OnModifyAtk { get; init; }
    public Func<Pokemon, double>? OnModifyDef { get; init; }
    public Func<Pokemon, double>? OnModifySpA { get; init; }
    public Func<Pokemon, double>? OnModifySpD { get; init; }
    public Func<Pokemon, double>? OnModifySpe { get; init; }

    public int? OnModifyAtkPriority { get; init; }
    public int? OnModifyDefPriority { get; init; }
    public int? OnModifySpAPriority { get; init; }
    public int? OnModifySpDPriority { get; init; }
    public int? OnModifySpePriority { get; init; }

    /// <summary>
    /// source, target, move, context
    /// </summary>
    public Func<Pokemon, Pokemon, Move, BattleContext, bool>? OnBeforeMove { get; init; }
    public int? OnBeforeMovePriority { get; init; }

    /// <summary>
    /// moveDamage, source, target, move, isCrit, numActivePokemonSide
    /// </summary>
    public Func<int, Pokemon, Pokemon, Move, bool, int, double>? OnAnyModifyDamage { get; init; }
    /// <summary>
    /// attacker, move, context
    /// </summary>
    public Action<Pokemon, Move, BattleContext>? OnDisableMove { get; init; }
    public bool NoCopy { get; init; }

    public Condition Copy()
    {
        return this with 
        { 
            // Records have built-in copy semantics with 'with' expression
            // This creates a shallow copy which is appropriate since most properties
            // are either value types, immutable references (strings), or function delegates
            // The only mutable properties (Duration, Counter) are copied correctly
        };
    }

    //// Add copy constructor for explicit copying when needed
    //public Condition(Condition original)
    //{
    //    Id = original.Id;
    //    Name = original.Name;
    //    ConditionEffectType = original.ConditionEffectType;
    //    ConditionVolatility = original.ConditionVolatility;
    //    OnStart = original.OnStart;
    //    OnResidualOrder = original.OnResidualOrder;
    //    OnResidual = original.OnResidual;
    //    Duration = original.Duration;
    //    CounterMax = original.CounterMax;
    //    Counter = original.Counter;
    //    OnStallMove = original.OnStallMove;
    //    OnRestart = original.OnRestart;
    //    OnTryHitPriority = original.OnTryHitPriority;
    //    OnTryHit = original.OnTryHit;
    //    OnTurnEnd = original.OnTurnEnd;
    //}
}