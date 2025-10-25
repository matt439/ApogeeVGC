using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public interface IPriorityComparison
{
    IntFalseUnion Order { get; }
    int Priority { get; }
    int Speed { get; }
    int SubOrder { get; }
    int EffectOrder { get; }
    //EffectHolder? EffectHolder { get; }
}