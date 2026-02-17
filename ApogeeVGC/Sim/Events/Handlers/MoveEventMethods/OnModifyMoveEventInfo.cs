using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnModifyMove event (move-specific).
/// Triggered to modify a move.
/// Signature: Action<Battle, ActiveMove, Pokemon, Pokemon?>
/// </summary>
public sealed record OnModifyMoveEventInfo : EventHandlerInfo
{
[Obsolete("Use Create factory method instead.")]
public OnModifyMoveEventInfo(
        Action<Battle, ActiveMove, Pokemon, Pokemon?> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
    Id = EventId.ModifyMove;
        Prefix = EventPrefix.None;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
      UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(ActiveMove), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

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
