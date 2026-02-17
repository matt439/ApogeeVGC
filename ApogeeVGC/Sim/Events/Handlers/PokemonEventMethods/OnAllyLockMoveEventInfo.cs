using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyLockMove event (pokemon/ally-specific).
/// Forces a Pokémon to use a specific move.
/// Signature: (Battle battle, Pokemon pokemon) => MoveIdVoidUnion | MoveId?
/// </summary>
public sealed record OnAllyLockMoveEventInfo : UnionEventHandlerInfo<OnLockMove>
{
    public OnAllyLockMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.LockMove;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyLockMoveEventInfo Create(
        Func<Battle, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyLockMoveEventInfo(
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
