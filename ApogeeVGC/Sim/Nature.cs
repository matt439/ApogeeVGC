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

public record Nature
{
    public required NatureType Type { get; init; }
    public StatIdExceptHp? Plus { get; init; }
    public StatIdExceptHp? Minus { get; init; }

    public double GetStatModifier(StatIdExceptHp stat)
    {
        if (Plus == stat)
        {
            return 1.1; // 10% increase
        }
        if (Minus == stat)
        {
            return 0.9; // 10% decrease
        }
        return 1.0; // no change
    }
}