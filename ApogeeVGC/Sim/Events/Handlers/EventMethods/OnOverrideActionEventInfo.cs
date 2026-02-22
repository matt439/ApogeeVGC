using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnOverrideAction event.
/// Overrides the action a Pokemon will take.
/// TS signature: (this: Battle, pokemon: Pokemon, target: Pokemon, move: ActiveMove) => string | void
/// </summary>
public sealed record OnOverrideActionEventInfo : EventHandlerInfo
{
    public OnOverrideActionEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.OverrideAction;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Returns MoveIdVoidUnion: MoveId to override the move, void to keep current move.
    /// </summary>
    public static OnOverrideActionEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, MoveIdVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnOverrideActionEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetSourceOrTargetPokemon(),
                    context.GetTargetOrSourcePokemon(),
                    context.GetMove()
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
