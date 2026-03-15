using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Effects;

/// <summary>
/// A lightweight IEffect with EffectType.Move, used when Showdown creates
/// a fake ActiveMove object for damage attribution (e.g. confusion self-hit:
/// <c>{ id: 'confused', effectType: 'Move', type: '???' }</c>).
/// This ensures abilities like Magic Guard (which only block non-Move damage)
/// don't interfere.
/// </summary>
public sealed record PseudoMoveEffect(string Id, string Name) : IEffect
{
    public static readonly PseudoMoveEffect Confused = new("confused", "confused");

    public EffectType EffectType => EffectType.Move;
    public string FullName => Name;
    public EffectStateId EffectStateId => EffectStateId.FromEmpty();
    public bool HasAnyEventHandlers => false;
    public bool HasPrefixedHandlers => false;

    public EventHandlerInfo? GetEventHandlerInfo(EventId id,
        EventPrefix prefix = EventPrefix.None, EventSuffix suffix = EventSuffix.None) => null;
}
