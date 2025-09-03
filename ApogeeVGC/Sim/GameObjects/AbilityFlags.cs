namespace ApogeeVGC.Sim.GameObjects;

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