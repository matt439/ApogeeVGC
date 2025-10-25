using ApogeeVGC.Data;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public class ModdedDex(Library library)
{
    private readonly TypeChart _typeChart = library.TypeChart;


    /// <summary>
    /// Calculates type effectiveness for a move against a target.
    /// Returns a MoveEffectiveness enum value representing the combined effectiveness.
    /// </summary>
    /// <param name="source">Move to check effectiveness for</param>
    /// <param name="target">Pokemon target</param>
    /// <returns>MoveEffectiveness enum value</returns>
    public MoveEffectiveness GetEffectiveness(ActiveMove source, Pokemon target)
    {
        return GetEffectiveness(source.Type, target.GetTypes());
    }

    /// <summary>
    /// Calculates type effectiveness for a move type against a Pokemon.
    /// </summary>
    public MoveEffectiveness GetEffectiveness(MoveType sourceType, Pokemon target)
    {
        return GetEffectiveness(sourceType, target.GetTypes());
    }

    /// <summary>
    /// Calculates type effectiveness for a move type against an array of types.
    /// Delegates to TypeChart which already handles combining effectiveness.
    /// </summary>
    public MoveEffectiveness GetEffectiveness(MoveType sourceType, PokemonType[] targetTypes)
    {
        // TypeChart already has this exact logic!
        return _typeChart.GetMoveEffectiveness(targetTypes, sourceType);
    }

    /// <summary>
    /// Calculates type effectiveness for a move type against target types.
    /// Delegates to TypeChart which already handles combining effectiveness.
    /// </summary>
    public MoveEffectiveness GetEffectiveness(MoveType sourceType, IReadOnlyList<PokemonType> targetTypes)
    {
        // TypeChart already has this exact logic!
        return _typeChart.GetMoveEffectiveness(targetTypes, sourceType);
    }

    /// <summary>
    /// Calculates type effectiveness for a move type against a single target type.
    /// Delegates to TypeChart for the base lookup.
    /// </summary>
    /// <param name="sourceType">The attacking move's type</param>
    /// <param name="targetType">The defending Pokemon's type</param>
    /// <returns>MoveEffectiveness enum value for single-type matchup</returns>
    public MoveEffectiveness GetEffectiveness(MoveType sourceType, PokemonType targetType)
    {
        // TypeChart already has this exact logic!
        return _typeChart.GetMoveEffectiveness(targetType, sourceType);
    }


    /// <summary>
    /// Returns false if the target is immune; true otherwise.
    /// Also checks immunity to some statuses.
    /// </summary>
    public bool GetImmunity(MoveType sourceType, Pokemon target)
    {
        return GetImmunity(sourceType, target.Types);
    }

    /// <summary>
    /// Returns false if the target is immune; true otherwise.
    /// Also checks immunity to some statuses.
    /// </summary>
    public bool GetImmunity(MoveType sourceType, IReadOnlyList<PokemonType> targetTypes)
    {
        // For multiple types, if ANY type is immune, the Pokemon is immune
        // This matches the TypeScript logic: if (!this.getImmunity(sourceType, type)) return false;
        return targetTypes.All(targetType => GetImmunity(sourceType, targetType));
    }

    /// <summary>
    /// Returns false if the target is immune; true otherwise.
    /// Also checks immunity to some statuses.
    /// </summary>
    public bool GetImmunity(MoveType sourceType, PokemonType targetType)
    {
        MoveEffectiveness effectiveness = _typeChart.GetMoveEffectiveness(targetType, sourceType);
        return effectiveness != MoveEffectiveness.Immune;
    }

    public bool GetImmunity(ConditionId condition, PokemonType targetType)
    {
        return GetImmunity(library.Conditions[condition], targetType);
    }

    public bool GetImmunity(ConditionId condition, IReadOnlyList<PokemonType> targetTypes)
    {
        return GetImmunity(library.Conditions[condition], targetTypes);
    }

    public bool GetImmunity(Condition condition, PokemonType targetType)
    {
        var immuneTypes = condition.ImmuneTypes;
        return !(immuneTypes?.Contains(targetType) ?? false);
    }

    public static bool GetImmunity(Condition condition, IReadOnlyList<PokemonType> targetTypes)
    {
        var immuneTypes = condition.ImmuneTypes;
        if (immuneTypes == null || immuneTypes.Count == 0)
        {
            return true;
        }
        // For multiple types, if ANY type is immune, the Pokemon is immune
        return targetTypes.All(targetType => !immuneTypes.Contains(targetType));
    }

    public bool GetImmunity(AbilityId ability, PokemonType targetType)
    {
        return GetImmunity(library.Abilities[ability], targetType);
    }

    public bool GetImmunity(AbilityId ability, IReadOnlyList<PokemonType> targetTypes)
    {
        return GetImmunity(library.Abilities[ability], targetTypes);
    }

    public bool GetImmunity(Ability ability, PokemonType targetType)
    {
        var immuneTypes = ability.ImmuneTypes;
        return !(immuneTypes?.Contains(targetType) ?? false);
    }

    public bool GetImmunity(Ability ability, IReadOnlyList<PokemonType> targetTypes)
    {
        var immuneTypes = ability.ImmuneTypes;
        if (immuneTypes == null || immuneTypes.Count == 0)
        {
            return true;
        }
        // For multiple types, if ANY type is immune, the Pokemon is immune
        return targetTypes.All(targetType => !immuneTypes.Contains(targetType));
    }
}