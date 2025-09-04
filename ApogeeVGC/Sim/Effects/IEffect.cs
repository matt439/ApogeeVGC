using ApogeeVGC.Sim.Methods;

namespace ApogeeVGC.Sim.Effects;

/// <summary>
/// Ability, Item, Move, Specie, Condition, and Format all implement this interface
/// </summary>
public interface IEffect //: ITestMethods
{
    EffectType EffectType { get; }
}