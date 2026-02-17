using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnCriticalHit event.
/// Determines if a move should critically hit.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => BoolVoidUnion | bool
/// </summary>
public sealed record OnCriticalHitEventInfo : UnionEventHandlerInfo<OnCriticalHit>
{
    /// <summary>
    /// Creates a constant-value or delegate-based handler via union type.
    /// </summary>
    public OnCriticalHitEventInfo(
        OnCriticalHit unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.CriticalHit;
        UnionValue = unionValue;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public OnCriticalHitEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.CriticalHit;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnCriticalHitEventInfo Create(
        Func<Battle, Pokemon, Pokemon?, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnCriticalHitEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.SourcePokemon,
                context.GetMove()
                );
                return result switch
                {
                    BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidBoolVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
