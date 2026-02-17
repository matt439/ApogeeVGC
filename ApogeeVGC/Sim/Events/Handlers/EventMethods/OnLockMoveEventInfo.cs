using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnLockMove event.
/// Returns which move a Pokemon is locked into.
/// Signature: (Battle, Pokemon) => MoveIdVoidUnion | MoveId constant
/// </summary>
public sealed record OnLockMoveEventInfo : UnionEventHandlerInfo<OnLockMove>
{
    /// <summary>
    /// Creates a constant-value or delegate-based handler via union type.
    /// </summary>
    public OnLockMoveEventInfo(
        OnLockMove unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.LockMove;
        UnionValue = unionValue;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public OnLockMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.LockMove;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnLockMoveEventInfo Create(
        Func<Battle, Pokemon, MoveIdVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnLockMoveEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon()
                );
                return result switch
                {
                    MoveIdMoveIdVoidUnion m => new MoveIdRelayVar(m.MoveId),
                    VoidMoveIdVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
