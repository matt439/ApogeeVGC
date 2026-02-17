using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeModifyMove event.
/// Signature: Action<Battle, ActiveMove, Pokemon, Pokemon?>
/// </summary>
public sealed record OnFoeModifyMoveEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnFoeModifyMoveEventInfo(
        Action<Battle, ActiveMove, Pokemon, Pokemon?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyMove;
        Prefix = EventPrefix.Foe;
   #pragma warning disable CS0618
   Handler = handler;
   #pragma warning restore CS0618
        Priority = priority;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(ActiveMove), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeModifyMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyMove;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeModifyMoveEventInfo Create(
        Action<Battle, ActiveMove, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeModifyMoveEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetMove(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
