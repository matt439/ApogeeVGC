using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnModifyMove event (move-specific).
/// Triggered to modify a move.
/// Signature: Action&lt;Battle, ActiveMove, Pokemon, Pokemon?&gt;
/// </summary>
public sealed record OnModifyMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnModifyMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyMove;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static OnModifyMoveEventInfo Create(
        Action<Battle, ActiveMove, Pokemon, Pokemon?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnModifyMoveEventInfo(
            context =>
            {
                handler(
                    context.Battle,
                    context.GetMove(),
                    context.GetSourceOrTargetPokemon(),
                    context.TargetPokemon
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
