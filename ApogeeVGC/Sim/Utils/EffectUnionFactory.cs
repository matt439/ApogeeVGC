using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Utils;

public static class EffectUnionFactory
{
    public static SingleEventSource ToSingleEventSource(IEffect effect) => effect switch
    {
        Ability ability => new EffectSingleEventSource(ability),
        Item item => new EffectSingleEventSource(item),
        ActiveMove activeMove => new EffectSingleEventSource(activeMove),
        Species specie => new EffectSingleEventSource(specie),
        Condition condition => new EffectSingleEventSource(condition),
        Format format => new EffectSingleEventSource(format),
        _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to SingleEventSource"),
    };

    public static RelayVar ToRelayVar(IEffect effect) => effect switch
    {
        Ability ability => new EffectRelayVar(ability),
        Item item => new EffectRelayVar(item),
        ActiveMove activeMove => new EffectRelayVar(activeMove),
        Species specie => new SpecieRelayVar(specie),
        Condition condition => new EffectRelayVar(condition),
        Format format => new EffectRelayVar(format),
        _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to RelayVar"),
    };

    public static Part ToPart(IEffect effect) => effect switch
    {
        Ability ability => new EffectPart(ability),
        Item item => new EffectPart(item),
        ActiveMove activeMove => new EffectPart(activeMove),
        Species specie => new EffectPart(specie),
        Condition condition => new EffectPart(condition),
        Format format => new EffectPart(format),
        _ => throw new InvalidOperationException($"Cannot convert {effect.GetType()} to Part"),
    };
}