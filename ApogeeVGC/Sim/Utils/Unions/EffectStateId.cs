using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// SpecieId | AbilityId | ItemId | ConditionId | MoveId | FormatId | Empty
/// </summary>
public abstract record EffectStateId
{
    public static implicit operator EffectStateId(SpecieId specieId) => new SpecieEffectStateId(specieId);
    public static implicit operator EffectStateId(AbilityId abilityId) => new AbilityEffectStateId(abilityId);
    public static implicit operator EffectStateId(ItemId itemId) => new ItemEffectStateId(itemId);
    public static implicit operator EffectStateId(ConditionId conditionId) => new ConditionEffectStateId(conditionId);
    public static implicit operator EffectStateId(MoveId moveId) => new MoveEffectStateId(moveId);
    public static implicit operator EffectStateId(FormatId formatId) => new FormatEffectStateId(formatId);
public static EffectStateId FromEmpty() => new EmptyEffectStateId();
}

public record SpecieEffectStateId(SpecieId SpecieId) : EffectStateId;
public record AbilityEffectStateId(AbilityId AbilityId) : EffectStateId;
public record ItemEffectStateId(ItemId ItemId) : EffectStateId;
public record ConditionEffectStateId(ConditionId ConditionId) : EffectStateId;
public record MoveEffectStateId(MoveId MoveId) : EffectStateId;
public record FormatEffectStateId(FormatId FormatId) : EffectStateId;
public record EmptyEffectStateId : EffectStateId;
