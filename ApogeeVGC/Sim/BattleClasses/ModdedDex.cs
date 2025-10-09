using ApogeeVGC.Data;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public class ModdedDex(TypeChart typeChart, Library library)
{
    //    getEffectiveness(
    //        source: { type: string } | string,
    //target: { getTypes: () => string[]
    //} | { types: string[] } | string[] | string
    //    ): number {
    //    const sourceType: string = typeof source !== 'string' ? source.type : source;
    //// @ts-expect-error really wish TS would support this
    //    const targetTyping: string[] | string = target.getTypes?.() || target.types || target;
    //    let totalTypeMod = 0;
    //    if (Array.isArray(targetTyping))
    //    {
    //        for (const type of targetTyping) {
    //            totalTypeMod += this.getEffectiveness(sourceType, type);
    //        }
    //        return totalTypeMod;
    //    }
    //    const typeData = this.types.get(targetTyping);
    //    if (!typeData) return 0;
    //    switch (typeData.damageTaken[sourceType])
    //    {
    //        case 1: return 1; // super-effective
    //        case 2: return -1; // resist
    //        // in case of weird situations like Gravity, immunity is handled elsewhere
    //        default: return 0;
    //    }
    //}
    
    
    
    
    
    
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
        MoveEffectiveness effectiveness = typeChart.GetMoveEffectiveness(targetType, sourceType);
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

    public static bool GetImmunity(Condition condition, PokemonType targetType)
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
}