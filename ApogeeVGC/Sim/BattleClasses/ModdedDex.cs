using ApogeeVGC.Data;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public class ModdedDex(TypeChart typeChart)
{
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

    /// <summary>
    /// Checks immunity for special effects (status conditions, weather, etc.)
    /// </summary>
    public bool GetSpecialImmunity(SpecialImmunityId immunityId, Pokemon target)
    {
        return GetSpecialImmunity(immunityId, target.Types);
    }

    /// <summary>
    /// Checks immunity for special effects against multiple types
    /// </summary>
    public bool GetSpecialImmunity(SpecialImmunityId immunityId, IReadOnlyList<PokemonType> targetTypes)
    {
        // For multiple types, if ANY type is immune, the Pokemon is immune
        return targetTypes.All(targetType => GetSpecialImmunity(immunityId, targetType));
    }

    /// <summary>
    /// Checks immunity for special effects against a single type
    /// </summary>
    public bool GetSpecialImmunity(SpecialImmunityId immunityId, PokemonType targetType)
    {
        MoveEffectiveness effectiveness = typeChart.GetSpecialEffectiveness(targetType, immunityId);
        return effectiveness != MoveEffectiveness.Immune;
    }

    // Move overloads
    public bool GetImmunity(Move source, Pokemon target)
    {
        return GetImmunity(source.Type, target);
    }

    public bool GetImmunity(Move source, IReadOnlyList<PokemonType> targetTypes)
    {
        return GetImmunity(source.Type, targetTypes);
    }

    public bool GetImmunity(Move source, PokemonType targetType)
    {
        return GetImmunity(source.Type, targetType);
    }

    /// <summary>
    /// Combined immunity check for both regular type effectiveness and special immunities
    /// Useful for status moves that need to check both
    /// </summary>
    public bool GetCombinedImmunity(MoveType sourceType, SpecialImmunityId? specialImmunity, Pokemon target)
    {
        // Check regular type immunity
        if (!GetImmunity(sourceType, target))
            return false;

        // Check special immunity if provided
        return !specialImmunity.HasValue || GetSpecialImmunity(specialImmunity.Value, target);
    }
}