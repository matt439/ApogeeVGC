using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.GameObjects;

public enum ItemId
{
    Leftovers,
    ChoiceSpecs,
    FlameOrb,
    RockyHelmet,
    LightClay,
    AssaultVest,
}

public record FlingData
{
    public int BasePower { get; init; }
    public ConditionId? Status { get; init; }
}

public record Item : EffectMethods, IEffect
{
    public required ItemId Id { get; init; }
    public EffectType EffectType => EffectType.Item;
    public string Name { get; init; } = string.Empty;
    public int SpriteNum { get; init; }
    public FlingData Fling { get; init; } = new();
    public int Num { get; init; }
    public int Gen { get; init; }
    public Condition? Condition { get; init; }

    /// <summary>
    /// itemHolder, context
    /// </summary>
    public Action<Pokemon, BattleContext>? OnBeforeResiduals { get; init; }
    public bool IsChoice { get; init; }

    ///// <summary>
    ///// user, context
    ///// </summary>
    //public Action<Pokemon, BattleContext>? OnStart { get; init; }
    ///// <summary>
    ///// move, user, target, context
    ///// </summary>
    //public Action<Move, Pokemon, Pokemon?, BattleContext>? OnModifyMove { get; init; }

    //public int OnDamagingHitOrder { get; init; }

    ///// <summary>
    ///// damage, target, source, move, context
    ///// </summary>
    //public Action<int, Pokemon, Pokemon, Move, BattleContext>? OnDamagingHit { get; init; }

    //public int? OnResidualOrder { get; init; }
    //public int? OnResidualSubOrder { get; init; }

    public Item Copy()
    {
        return new Item
        {
            Id = Id,
            Name = Name,
            SpriteNum = SpriteNum,
            Fling = Fling with { },
            Num = Num,
            Gen = Gen,
            Condition = Condition?.Copy(),
            OnBeforeResiduals = OnBeforeResiduals,
            IsChoice = IsChoice,
            OnStart = OnStart,
            OnModifyMove = OnModifyMove,
            OnDamagingHitOrder = OnDamagingHitOrder,
            OnDamagingHit = OnDamagingHit,
        };
    }
}