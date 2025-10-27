using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Effects;

public interface IEffect
{
    EffectType EffectType { get; }
    string Name { get; }
    string FullName { get; }

    /// <summary>
    /// This is the ID used in Pokémon Showdown's codebase to identify this effect.
    /// It is typically a lowercase version of the effect's name with spaces and special characters removed.
    /// </summary>
    string ShowdownId { get; }
    EffectStateId EffectStateId { get; }
    EffectDelegate? GetDelegate(EventId id);

    IntFalseUnion? GetOrder(EventId id);
    int? GetPriority(EventId id);
    int? GetSubOrder(EventId id);
}