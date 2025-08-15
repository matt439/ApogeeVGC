namespace ApogeeVGC.Sim;

public enum NatureType
{
    Adamant,
    Bashful,
    Bold,
    Brave,
    Calm,
    Careful,
    Docile,
    Gentle,
    Hardy,
    Hasty,
    Impish,
    Jolly,
    Lax,
    Lonely,
    Mild,
    Modest,
    Naive,
    Naughty,
    Quiet,
    Quirky,
    Rash,
    Relaxed,
    Sassy,
    Serious,
    Timid,
}

public record NatureData
{
    public StatTypeExceptHp? Plus { get; init; }
    public StatTypeExceptHp? Minus { get; init; }
}