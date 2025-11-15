using ApogeeVGC.Sim.Abilities;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// AbilityId | false
/// </summary>
public abstract record AbilityIdFalseUnion
{
    public static implicit operator AbilityIdFalseUnion(AbilityId abilityId) =>
        new AbilityIdAbilityIdFalseUnion(abilityId);
    public static AbilityIdFalseUnion FromFalse() => new FalseAbilityIdFalseUnion();
}

public record AbilityIdAbilityIdFalseUnion(AbilityId AbilityId) : AbilityIdFalseUnion;
public record FalseAbilityIdFalseUnion : AbilityIdFalseUnion;
