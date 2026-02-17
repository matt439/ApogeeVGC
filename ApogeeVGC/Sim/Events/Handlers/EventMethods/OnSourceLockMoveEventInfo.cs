using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceLockMove event.
/// Forces a Pokémon to use a specific move.
/// Signature: (Battle battle, Pokemon pokemon) => MoveIdVoidUnion | MoveId?
/// </summary>
public sealed record OnSourceLockMoveEventInfo : UnionEventHandlerInfo<OnLockMove>
{
    public OnSourceLockMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.LockMove;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceLockMoveEventInfo Create(
        Func<Battle, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceLockMoveEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon()
                );
                return result switch
                {
                    null => null,
                    _ => new EffectRelayVar(result)
                };
            },
            priority,
            usesSpeed
        );
    }
}
