namespace ApogeeVGC.Sim;

public enum AbilityId
{
    AsOneGlastrier,
    HadronEngine,
    Guts,
    FlameBody,
    Prankster,
    QuarkDrive,

    ChillingNeigh,
    Unnerve,
    QuickFeet,
}

public record AbilityFlags
{
    // Can be suppressed by Mold Breaker and related effects
    public bool? Breakable { get; init; }
    // Ability can't be suppressed by e.g. Gastro Acid or Neutralizing Gas
    public bool? CantSuppress { get; init; }
    // Role Play fails if target has this Ability
    public bool? FailRolePlay { get; init; }
    // Skill Swap fails if either the user or target has this Ability
    public bool? FailSkillSwap { get; init; }
    // Entrainment fails if user has this Ability
    public bool? NoEntrain { get; init; }
    // Receiver and Power of Alchemy will not activate if an ally faints with this Ability
    public bool? NoReceiver { get; init; }
    // Trace cannot copy this Ability
    public bool? NoTrace { get; init; }
    // Disables the Ability if the user is Transformed
    public bool? NoTransform { get; init; }
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
                _ => field,
            };
        }
        init;
    }

    public string Name { get; init; } = string.Empty;
    public int Num { get; init; } = 0;
    public AbilityFlags Flags { get; init; } = new();
    public int OnSwitchInPriority { get; init; } = 0;
    /// <summary>
    /// length, target, source, effect
    /// </summary>
    public Action<int, Pokemon, Pokemon, IEffect, BattleContext>? OnSourceAfterFaint { get; init; }
    public Action<Pokemon, Field, Pokemon[], IEffect, BattleContext>? OnStart { get; init; }
    public Func<int, Move, int>? OnModifyPriority { get; init; }
    public Action<Pokemon, Field, BattleContext>? OnTerrainChange { get; init; }

}