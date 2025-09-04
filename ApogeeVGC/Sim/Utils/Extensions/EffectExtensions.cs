using ApogeeVGC.Sim.Effects;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class EffectExtensions
{
    /// <summary>
    /// Determines if this effect is of a specific type
    /// </summary>
    public static bool IsOfType(this IEffect effect, EffectType effectType)
    {
        return effect.EffectType == effectType;
    }

    /// <summary>
    /// Safely casts an IEffect to a specific type if possible
    /// </summary>
    public static T? As<T>(this IEffect effect) where T : class, IEffect
    {
        return effect as T;
    }
}