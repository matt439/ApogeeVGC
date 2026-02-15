using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyModifyMove event.
/// Signature: Action<Battle, ActiveMove, Pokemon, Pokemon?>
/// </summary>
public sealed record OnAnyModifyMoveEventInfo : EventHandlerInfo
{
    public OnAnyModifyMoveEventInfo(
        Action<Battle, ActiveMove, Pokemon, Pokemon?> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.ModifyMove;
Prefix = EventPrefix.Any;
        Handler = handler;
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
    public OnAnyModifyMoveEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyMove;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyModifyMoveEventInfo Create(
        Action<Battle, ActiveMove, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyModifyMoveEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetMove(),
                context.GetTargetPokemon(),
                context.GetSourcePokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}