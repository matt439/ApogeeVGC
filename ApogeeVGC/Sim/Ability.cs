namespace ApogeeVGC.Sim;

public enum AbilityId
{
    AsOneGlastrier,
    HadronEngine,
    Guts,
    FlameBody,
    Prankster,
    QuarkDrive,
}

public record Ability : IEffect
{
    public EffectType EffectType => EffectType.Ability;
    public required AbilityId Id { get; init; }

    /// <summary>
    /// Rating from -1 Detrimental to +5 Essential
    /// </summary>
    public double Rating
    {
        get;
        init
        {
            if (value is < -1.0 or > 5.0)
                throw new ArgumentOutOfRangeException(nameof(value), "Rating must be between -1 and 5.");
            field = value;
        }
    }

    public string Fullname => $"ability: {Name}";

    public int Gen
    {
        get
        {
            if (field is >= 1 and <= 9) return field;
            return Num switch
            {
                >= 268 => 9,
                >= 234 => 8,
                >= 192 => 7,
                >= 165 => 6,
                >= 124 => 5,
                >= 77 => 4,
                >= 1 => 3,
                _ => field
            };
        }
        init;
    }

    public string Name { get; init; } = string.Empty;
    public int Num { get; init; } = 0;
    public int OnSwitchInPriority { get; init; } = 0;

}