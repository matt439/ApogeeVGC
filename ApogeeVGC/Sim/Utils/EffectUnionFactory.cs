using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Utils;

public static class EffectUnionFactory
{
    public static SingleEventSource ToSingleEventSource(IEffect effect) =>
        SingleEventSource.FromEffect(effect);

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