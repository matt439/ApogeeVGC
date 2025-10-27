using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Effects;

public abstract class BasicEffect : IEffect
{
    public abstract EffectType EffectType { get; }
    public abstract string Name { get; }
    public abstract string FullName { get; }
    public abstract EffectStateId EffectStateId { get; }
    public abstract EffectDelegate? GetDelegate(EventId id);

    public abstract IntFalseUnion? GetOrder(EventId id);

    public abstract int? GetPriority(EventId id);

    public abstract int? GetSubOrder(EventId id);

    public override string ToString()
    {
        return Name;
    }
}