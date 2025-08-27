namespace ApogeeVGC.Sim;

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

public record Item : IEffect
{
    public required ItemId Id { get; init; }
    public EffectType EffectType => EffectType.Item;
    public string Name { get; init; } = string.Empty;
    public int SpriteNum { get; init; }
    public FlingData Fling { get; init; } = new();
    public int Num { get; init; }
    public int Gen { get; init; }
    public int? OnResidualOrder { get; init; }
    public int? OnResidualSubOrder { get; init; }
}