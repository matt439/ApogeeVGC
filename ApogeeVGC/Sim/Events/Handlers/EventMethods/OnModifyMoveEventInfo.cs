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
    /// Creates a new OnModifyMove event handler for simple modifications that don't need to return false.
    /// </summary>
    /// <param name="handler">The event handler delegate (void return)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyMoveEventInfo(
        Action<Battle, ActiveMove, Pokemon, Pokemon?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyMove;
        // Wrap Action in Func that returns null
        Func<Battle, ActiveMove, Pokemon, Pokemon?, VoidFalseUnion?> wrappedHandler = (b, m, p, t) =>
        {
            handler(b, m, p, t);
            return null;
        };
        Handler = wrappedHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(ActiveMove),
            typeof(Pokemon),
            typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(VoidFalseUnion);
        
        // Nullability: All parameters non-nullable by default, target can be null
        ParameterNullability = [false, false, false, true];
        ReturnTypeNullable = true;
    
        // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates a new OnModifyMove event handler that can return false to prevent the move.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyMoveEventInfo(
        Func<Battle, ActiveMove, Pokemon, Pokemon?, VoidFalseUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyMove;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(ActiveMove),
            typeof(Pokemon),
            typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(VoidFalseUnion);
        
        // Nullability: All parameters non-nullable by default, target can be null
        ParameterNullability = [false, false, false, true];
        ReturnTypeNullable = true;
    
        // Validate configuration
        ValidateConfiguration();
    }
    
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
                    context.GetSourcePokemon(),
                    context.TargetPokemon  // Direct access, can be null
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
}
