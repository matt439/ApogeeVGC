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

    BoosterEnergy,
}

public record FlingData
{
    public int BasePower { get; init; }
    public ConditionId? Status { get; init; }
}

public record Item : IEffect
{
    public required ItemId Id { get; init; }
    public EffectType EffectType => EffectType.Item;
    public string Name { get; init; } = string.Empty;
    public int SpriteNum { get; init; }
    public FlingData Fling { get; init; } = new();
    public int Num { get; init; }
    public int Gen { get; init; }
    public Condition? Condition { get; init; }


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
        };
    }
}