using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class ShowdownIdTools
{

    public static string ToShowdownId(this string name)
    {
        return name.ToLower().Replace(" ", "").Replace("-", "").Replace("'", "").Replace(".", "").Replace("é", "e");
    }

    public static string ToShowdownId(this AbilityId abilityId)
    {
        return abilityId.ToString().ToShowdownId();
    }

    public static string ToShowdownId(this ConditionId conditionId)
    {
        return conditionId.ToString().ToShowdownId();
    }

    public static string ToShowdownId(this SpecieId specieId)
    {
        return specieId.ToString().ToShowdownId();
    }

    public static string ToShowdownId(this MoveId moveId)
    {
        return moveId.ToString().ToShowdownId();
    }

    public static string ToShowdownId(this FormatId formatId)
    {
        return formatId.ToString().ToShowdownId();
    }

    public static string ToShowdownId(this ItemId itemId)
    {
        return itemId.ToString().ToShowdownId();
    }

    public static string ToShowdownId(this NatureId natureId)
    {
        return natureId.ToString().ToShowdownId();
    }
}