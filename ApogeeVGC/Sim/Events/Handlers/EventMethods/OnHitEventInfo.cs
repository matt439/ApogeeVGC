using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnHit event.
/// Triggered when a move hits a target.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => BoolEmptyVoidUnion?
/// </summary>
public sealed record OnHitEventInfo : EventHandlerInfo
{
    public OnHitEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Hit;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnHitEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, RelayVar?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnHitEventInfo(
            context => handler(
                context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetMove()
            ),
            priority,
            usesSpeed
        );
    }
}
