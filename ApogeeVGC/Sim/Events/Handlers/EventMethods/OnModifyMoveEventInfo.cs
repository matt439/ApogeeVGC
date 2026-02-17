using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyMove event.
/// Modifies properties of a move before it executes.
/// 
/// Supports three handler patterns:
/// 1. Action-based: (Battle, ActiveMove, Pokemon, Pokemon?) => void (for simple modifications)
/// 2. Func-based: (Battle, ActiveMove, Pokemon, Pokemon?) => VoidFalseUnion? (when return false is needed)
/// 3. Context-based: (EventContext) => RelayVar?
/// 
/// Return values:
/// - VoidFalseUnion.FromVoid() or null: Continue without modification to flow
/// - VoidFalseUnion.FromFalse(): The move should fail/be prevented
/// </summary>
public sealed record OnModifyMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, Move, SourcePokemon, TargetPokemon
    /// </summary>
    public OnModifyMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyMove;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnModifyMoveEventInfo Create(
        Func<Battle, ActiveMove, Pokemon, Pokemon?, VoidFalseUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnModifyMoveEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetMove(),
                    context.GetSourceOrTargetPokemon(),
                    context.TargetPokemon // Direct access, can be null
                );

                // Convert VoidFalseUnion to RelayVar using implicit operator
                if (result is FalseVoidFalseUnion)
                {
                    return false; // Uses implicit operator RelayVar(bool)
                }

                return null;
            },
            priority,
            usesSpeed
        );
    }

    /// <summary>
    /// Creates strongly-typed context-based handler for simple modifications (void return).
    /// </summary>
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
