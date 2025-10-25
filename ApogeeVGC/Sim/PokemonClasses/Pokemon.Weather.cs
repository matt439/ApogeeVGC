using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.GameObjects;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    /// <summary>
    /// Gets the effective weather for this Pokemon.
    /// Like Field.EffectiveWeather(), but ignores sun and rain if
    /// the Utility Umbrella item is being held by the Pokemon.
    /// Returns ConditionId.None if weather is negated by Utility Umbrella.
    /// </summary>
    /// <returns>The effective weather condition, or ConditionId.None if negated</returns>
    public ConditionId EffectiveWeather()
    {
        // Get the current effective weather from the field
        ConditionId weather = Battle.Field.EffectiveWeather();

        // Check if Utility Umbrella negates sun/rain effects
        switch (weather)
        {
            case ConditionId.SunnyDay:
            case ConditionId.RainDance:
            case ConditionId.DesolateLand:
            case ConditionId.PrimordialSea:
                // Utility Umbrella negates these weather conditions
                if (HasItem(ItemId.UtilityUmbrella))
                {
                    return ConditionId.None;
                }
                break;
        }

        // Return the weather as-is (not affected by Utility Umbrella)
        return weather;
    }
}