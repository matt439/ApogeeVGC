using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryMove event.
/// Triggered when attempting to use a move.
/// Signature: (Battle battle, Pokemon source, Pokemon target, ActiveMove move) => BoolEmptyVoidUnion?
/// </summary>
public sealed record OnTryMoveEventInfo : EventHandlerInfo
{
    public OnTryMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryMove;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnTryMoveEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, RelayVar?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnTryMoveEventInfo(
            context => handler(
                context.Battle,
                context.GetSourceOrTargetPokemon(),
                context.GetTargetOrSourcePokemon(),
                context.GetMove()
            ),
            priority,
            usesSpeed
        );
    }
}
